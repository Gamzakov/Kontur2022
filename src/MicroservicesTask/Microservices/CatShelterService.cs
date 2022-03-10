using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microservices.Common.Exceptions;
using Microservices.ExternalServices.Authorization;
using Microservices.ExternalServices.Billing;
using Microservices.ExternalServices.Billing.Types;
using Microservices.ExternalServices.CatDb;
using Microservices.ExternalServices.CatExchange;
using Microservices.ExternalServices.Database;
using Microservices.Types;

using Polly;

namespace Microservices
{
    public class CatShelterService : ICatShelterService
    {
        private readonly IDatabase _database;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBillingService _billingService;
        private readonly ICatInfoService _catInfoService;
        private readonly ICatExchangeService _catExchangeService;

        private const string UsersFavouritesTableName = "UsersFavourites";
        private const string CatsTableName = "Cats";

        private const int RetryCount = 3;
        private readonly IAsyncPolicy _retryPolicy;

        public CatShelterService(
            IDatabase database,
            IAuthorizationService authorizationService,
            IBillingService billingService,
            ICatInfoService catInfoService,
            ICatExchangeService catExchangeService)
        {
            _database = database;
            _authorizationService = authorizationService;
            _billingService = billingService;
            _catInfoService = catInfoService;
            _catExchangeService = catExchangeService;

            _retryPolicy = Policy
                .Handle<ConnectionException>()
                .RetryAsync(RetryCount, (exception, retry) =>
                {
                    if (retry >= RetryCount)
                        throw new InternalErrorException(exception);
                });
        }

        public async Task<List<Cat>> GetCatsAsync(string sessionId, int skip, int limit,
            CancellationToken cancellationToken)
        {
            await AuthorizeAsync(sessionId, cancellationToken);

            var products =
                await _retryPolicy.ExecuteAsync(token => _billingService.GetProductsAsync(skip, limit, token),
                    cancellationToken);

            var cats = new List<Cat>(products.Count);

            foreach (var product in products)
            {
                var cat = await _database.GetCollection<Cat, Guid>(CatsTableName)
                    .FindAsync(product.Id, cancellationToken);
                cats.Add(cat);
            }

            return cats;
        }

        public async Task AddCatToFavouritesAsync(string sessionId, Guid catId, CancellationToken cancellationToken)
        {
            var userId = await AuthorizeAsync(sessionId, cancellationToken);

            var cat = await _database.GetCollection<Cat, Guid>(CatsTableName).FindAsync(catId, cancellationToken);
            var user = await _database.GetCollection<UserFavourites, Guid>(UsersFavouritesTableName).FindAsync(userId, cancellationToken) ??
                       new UserFavourites {Id = userId};

            if (cat == null)
                return;

            user.CatsIds ??= new List<Guid>();

            if (!user.CatsIds.Contains(cat.Id))
                user.CatsIds.Add(cat.Id);

            await _database.GetCollection<UserFavourites, Guid>(UsersFavouritesTableName).WriteAsync(user, cancellationToken);
        }

        public async Task<List<Cat>> GetFavouriteCatsAsync(string sessionId, CancellationToken cancellationToken)
        {
            var userId = await AuthorizeAsync(sessionId, cancellationToken);

            var user = await _database.GetCollection<UserFavourites, Guid>(UsersFavouritesTableName)
                .FindAsync(userId, cancellationToken);

            var availableProducts = new List<Cat>();

            if (user == null)
                return availableProducts;

            foreach (var catId in user.CatsIds)
            {
                var product = await _retryPolicy.ExecuteAsync(token => _billingService.GetProductAsync(catId, token),
                    cancellationToken);

                if (product == null)
                    continue;

                var cat = await _database.GetCollection<Cat, Guid>(CatsTableName)
                    .FindAsync(product.Id, cancellationToken);
                availableProducts.Add(cat);
            }

            return availableProducts;
        }

        public async Task DeleteCatFromFavouritesAsync(string sessionId, Guid catId,
            CancellationToken cancellationToken)
        {
            var userId = await AuthorizeAsync(sessionId, cancellationToken);

            var cat = await _database.GetCollection<Cat, Guid>(CatsTableName).FindAsync(catId, cancellationToken);
            var user = await _database.GetCollection<UserFavourites, Guid>(UsersFavouritesTableName).FindAsync(userId, cancellationToken);

            if (cat == null || user == null)
                return;

            user.CatsIds?.Remove(cat.Id);
            await _database.GetCollection<UserFavourites, Guid>(UsersFavouritesTableName).WriteAsync(user, cancellationToken);
        }

        public async Task<Bill> BuyCatAsync(string sessionId, Guid catId, CancellationToken cancellationToken)
        {
            await AuthorizeAsync(sessionId, cancellationToken);

            var cat = await _database.GetCollection<Cat, Guid>(CatsTableName).FindAsync(catId, cancellationToken);

            var product = await _retryPolicy.ExecuteAsync(
                token => _billingService.GetProductAsync(cat?.Id ?? Guid.Empty, token),
                cancellationToken);

            var bill = await _retryPolicy.ExecuteAsync(
                token => _billingService.SellProductAsync(product?.Id ?? Guid.Empty, cat?.Price ?? 0, token),
                cancellationToken);

            return bill;
        }

        public async Task<Guid> AddCatAsync(string sessionId, AddCatRequest request,
            CancellationToken cancellationToken)
        {
            var userId = await AuthorizeAsync(sessionId, cancellationToken);

            var catInfo =
                await _retryPolicy.ExecuteAsync(token => _catInfoService.FindByBreedNameAsync(request.Breed, token),
                    cancellationToken);

            var priceInfo =
                await _retryPolicy.ExecuteAsync(token => _catExchangeService.GetPriceInfoAsync(catInfo.BreedId, token),
                    cancellationToken);

            var catId = Guid.NewGuid();
            var price = priceInfo.Prices.Count != 0 ? priceInfo.Prices[^1].Price : 1000;
            var prices = priceInfo.Prices?.ConvertAll(catPriceInfo => (catPriceInfo.Date, catPriceInfo.Price));

            var cat = new Cat
            {
                Id = catId,
                Name = request.Name,
                CatPhoto = request.Photo,
                AddedBy = userId,
                Breed = catInfo.BreedName,
                BreedId = catInfo.BreedId,
                BreedPhoto = catInfo.Photo,
                Price = price,
                Prices = prices
            };

            var product = new Product
            {
                Id = catId,
                BreedId = cat.BreedId
            };

            await _retryPolicy.ExecuteAsync(token => _billingService.AddProductAsync(product, token),
                cancellationToken);

            await _database.GetCollection<Cat, Guid>(CatsTableName).WriteAsync(cat, cancellationToken);

            return cat.Id;
        }

        /// <summary>
        /// јвторизует пользовател€ по Id сессии
        /// </summary>
        /// <param name="sessionId">Id сессии</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Id пользовател€</returns>
        /// <exception cref="AuthorizationException">¬озникает при неуспешной авторизации</exception>
        private async Task<Guid> AuthorizeAsync(string sessionId, CancellationToken cancellationToken)
        {
            var authorizationResult =
                await _retryPolicy
                    .ExecuteAsync(
                        token => _authorizationService.AuthorizeAsync(sessionId, token),
                        cancellationToken
                    );

            if (!authorizationResult.IsSuccess)
                throw new AuthorizationException();

            return authorizationResult.UserId;
        }
    }

    /// <summary>
    /// <inheritdoc cref="Microservices.Types.Cat"/>
    /// </summary>
    public class Cat : Microservices.Types.Cat, IEntityWithId<Guid>
    {
    }

    /// <summary>
    /// »збранные котики пользовател€
    /// </summary>
    public class UserFavourites : IEntityWithId<Guid>
    {
        public Guid Id { get; set; }

        /// <summary>
        /// —одержит список Id котиков, которых пользователь добавил в избранное
        /// </summary>
        public List<Guid>? CatsIds { get; set; }
    }
}