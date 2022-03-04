using NUnit.Framework;
using FluentAssertions;

using System.Collections.Generic;

namespace AutoComplete.Tests
{
    [TestFixture]
    public class AutoCompleterTests
    {
        [Test]
        public void Search_ShouldSearchByName()
        {
            // Arrange
            const int fullNameIndex = 1;
            var autoCompleter = new AutoCompleter();
            const string prefix = "Иван";

            var fullNames = new List<FullName>
            {
                new FullName {Name = "Иван", Surname = "Двитриев", Patronymic = "Сергеевич"},
                new FullName {Name = "Иван", Surname = null, Patronymic = "Сергеевич"},
                new FullName {Name = "Иван", Surname = "Двитриев", Patronymic = "Сергеевич"}
            };

            var expectedResult = new List<string> {fullNames[fullNameIndex].ToString()};

            // Act
            autoCompleter.AddToSearch(fullNames);
            var actualResult = autoCompleter.Search(
                prefix);

            // Assert
            actualResult
                .Should()
                .NotBeNullOrEmpty()
                .And
                .HaveCount(expectedResult.Count)
                .And
                .BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Search_ShouldSearchByFio()
        {
            // Arrange
            const int fullNameIndex = 0;
            var autoCompleter = new AutoCompleter();
            const string prefix = "Двитриев Иван Сергеевич";

            var fullNames = new List<FullName>
            {
                new FullName {Name = "Иван", Surname = "Двитриев", Patronymic = "Сергеевич"},
                new FullName {Name = "Иван", Surname = null, Patronymic = "Сергеевич"},
                new FullName {Name = "Иван", Surname = "Двитриев", Patronymic = "Сергеевич"}
            };

            var expectedResult = new List<string> { fullNames[fullNameIndex].ToString() };

            // Act
            autoCompleter.AddToSearch(fullNames);
            var actualResult = autoCompleter.Search(
                prefix);

            // Assert
            actualResult
                .Should()
                .NotBeNullOrEmpty()
                .And
                .HaveCount(expectedResult.Count)
                .And
                .BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Search_SearchByFioWithSpaces()
        {
            // Arrange
            const int fullNameIndex = 0;
            var autoCompleter = new AutoCompleter();
            const string prefix = "   Двитриев    Иван   Сергеевич   ";

            var fullNames = new List<FullName>
            {
                new FullName {Name = "Иван", Surname = "Двитриев", Patronymic = "Сергеевич"},
                new FullName {Name = "Иван", Surname = null, Patronymic = "Сергеевич"},
                new FullName {Name = "Иван", Surname = "Двитриев", Patronymic = "Сергеевич"}
            };

            var expectedResult = new List<string> { fullNames[fullNameIndex].ToString() };

            // Act
            autoCompleter.AddToSearch(fullNames);
            var actualResult = autoCompleter.Search(
                prefix);

            // Assert
            actualResult
                .Should()
                .NotBeNullOrEmpty()
                .And
                .HaveCount(expectedResult.Count)
                .And
                .BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Search_ShouldReturnOne()
        {
            // Arrange
            const int fullNameIndex = 0;
            var autoCompleter = new AutoCompleter();
            const string prefix = "Двитриев1";

            var fullNames = new List<FullName>
            {
                new FullName {Name = "Иван", Surname = "Двитриев1", Patronymic = "Сергеевич"},
                new FullName {Name = "Иван", Surname = null, Patronymic = "Сергеевич"},
                new FullName {Name = "Иван", Surname = "Двитриев", Patronymic = "Сергеевич"}
            };

            var expectedResult = new List<string> { fullNames[fullNameIndex].ToString() };

            // Act
            autoCompleter.AddToSearch(fullNames);
            var actualResult = autoCompleter.Search(
                prefix);

            // Assert
            actualResult
                .Should()
                .NotBeNullOrEmpty()
                .And
                .HaveCount(1)
                .And
                .BeEquivalentTo(expectedResult);
        }
    }
}
