using Microservices;
using Microservices.ExternalServices.Authorization;
using Microservices.ExternalServices.Billing;
using Microservices.ExternalServices.CatDb;
using Microservices.ExternalServices.CatExchange;
using Microservices.ExternalServices.Database;

using NSubstitute;

using NUnit.Framework;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microservices.Types;

namespace Microservices.Tests
{
    [TestFixture]
    public class CatShelterServiceTests
    {
        private IDatabase subDatabase;
        private IAuthorizationService subAuthorizationService;
        private IBillingService subBillingService;
        private ICatInfoService subCatInfoService;
        private ICatExchangeService subCatExchangeService;

        [SetUp]
        public void SetUp()
        {
            subDatabase = Substitute.For<IDatabase>();
            subAuthorizationService = Substitute.For<IAuthorizationService>();
            subBillingService = Substitute.For<IBillingService>();
            subCatInfoService = Substitute.For<ICatInfoService>();
            subCatExchangeService = Substitute.For<ICatExchangeService>();
        }

        private CatShelterService CreateService()
        {
            return new CatShelterService(
                subDatabase,
                subAuthorizationService,
                subBillingService,
                subCatInfoService,
                subCatExchangeService);
        }

        [Test]
        public async Task GetCatsAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = CreateService();
            string sessionId = null;
            int skip = 0;
            int limit = 0;
            CancellationToken cancellationToken = default(CancellationToken);

            // Act
            var result = await service.GetCatsAsync(
                sessionId,
                skip,
                limit,
                cancellationToken);

            // Assert
            Assert.Fail();
        }

        [Test]
        public async Task AddCatToFavouritesAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = CreateService();
            string sessionId = null;
            Guid catId = default(Guid);
            CancellationToken cancellationToken = default(CancellationToken);

            // Act
            await service.AddCatToFavouritesAsync(
                sessionId,
                catId,
                cancellationToken);

            // Assert
            Assert.Fail();
        }

        [Test]
        public async Task GetFavouriteCatsAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = CreateService();
            string sessionId = null;
            CancellationToken cancellationToken = default(CancellationToken);

            // Act
            var result = await service.GetFavouriteCatsAsync(
                sessionId,
                cancellationToken);

            // Assert
            Assert.Fail();
        }

        [Test]
        public async Task DeleteCatFromFavouritesAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = CreateService();
            string sessionId = null;
            Guid catId = default(Guid);
            CancellationToken cancellationToken = default(CancellationToken);

            // Act
            await service.DeleteCatFromFavouritesAsync(
                sessionId,
                catId,
                cancellationToken);

            // Assert
            Assert.Fail();
        }

        [Test]
        public async Task BuyCatAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = CreateService();
            string sessionId = null;
            Guid catId = default(Guid);
            CancellationToken cancellationToken = default(CancellationToken);

            // Act
            var result = await service.BuyCatAsync(
                sessionId,
                catId,
                cancellationToken);

            // Assert
            Assert.Fail();
        }

        [Test]
        public async Task AddCatAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = CreateService();
            string sessionId = null;
            AddCatRequest request = null;
            CancellationToken cancellationToken = default(CancellationToken);

            // Act
            var result = await service.AddCatAsync(
                sessionId,
                request,
                cancellationToken);

            // Assert
            Assert.Fail();
        }
    }
}
