using NUnit.Framework;

using System.Collections.Generic;


namespace Morphology.Tests
{
    [TestFixture]
    public class SentenceMorpherTests
    {
        private SentenceMorpher _morpher;

        [SetUp]
        public void SetUp()
        {
            _morpher = SentenceMorpher.Create(SentenceMorpherTestData.Default);
        }

        [Test]
        public void Create_ShouldNotThrow_OnEmpty()
        {
            // Arrange
            IEnumerable<string> dictionaryLines = SentenceMorpherTestData.Empty;

            // Assert
            Assert.DoesNotThrow(() => SentenceMorpher.Create(dictionaryLines));
        }

        [Test]
        public void Create_ShouldNotThrow_OnEmptyLines()
        {
            // Arrange
            IEnumerable<string> dictionaryLines = SentenceMorpherTestData.EmptyLines;

            // Assert
            Assert.DoesNotThrow(() => SentenceMorpher.Create(dictionaryLines));
        }

        [Test]
        public void Create_ShouldNotThrow_OnDictionaryWithSameNormalForms()
        {
            // Arrange
            IEnumerable<string> dictionaryLines = SentenceMorpherTestData.WithMultipleSameNormalForms;

            // Assert
            Assert.DoesNotThrow(() => SentenceMorpher.Create(dictionaryLines));
        }

        [Test]
        public void Create_ShouldNotThrow_OnNormalDictionary()
        {
            // Arrange
            IEnumerable<string> dictionaryLines = SentenceMorpherTestData.Default;

            // Assert
            Assert.DoesNotThrow(() => SentenceMorpher.Create(dictionaryLines));
        }

        [Test]
        public void Morph_ShouldReturnEmpty_OnEmptyInput()
        {
            // Arrange
            var input = string.Empty;
            var expectedResult = string.Empty;
            string actualResult;

            // Act
            actualResult = _morpher.Morph(input);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void Morph_ShouldReturnMatched_OnExpectedInput()
        {
            // Arrange
            const string input = "СЛОВО{sp1,tag1,tag2,tag3}";
            const string expectedResult = "СЛОВО1";
            string actualResult;

            // Act
            actualResult = _morpher.Morph(input);

            // Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult).IgnoreCase);
        }

        [Test]
        public void Morph_ShouldReturnFirst_OnSeveralMatched()
        {
            // Arrange
            const string input = "WORD2{sp2,tag4,tag5.2,tag6}";
            const string expectedResult = "WORD2";
            string actualResult;

            // Act
            actualResult = _morpher.Morph(input);

            // Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult).IgnoreCase);
        }

        [Test]
        public void Morph_ShouldReturnInput_OnEmptyTags()
        {
            // Arrange
            const string input = "СЛОВО{}";
            const string expectedResult = "СЛОВО";
            string actualResult;

            // Act
            actualResult = _morpher.Morph(input);

            // Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult).IgnoreCase);
        }

        [Test]
        public void Morph_ShouldReturnBaseForm_OnOnlySpeechPart()
        {
            // Arrange
            const string input = "СЛОВО{sp1}";
            const string expectedResult = "СЛОВО";
            string actualResult;

            // Act
            actualResult = _morpher.Morph(input);

            // Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult).IgnoreCase);
        }

        [Test]
        public void Morph_ShouldHoldIndependentDictionaries_OnMultipleInstancesCreated()
        {
            // Arrange
            const string input = "СЛОВО{sp1,tag1,tag2,tag3}";
            const string expectedResultOfFirstMorpher = "СЛОВО1";
            const string expectedResultOfSecondMorpher = "ДРУГОЕСЛОВО";
            var secondDefaultDictionary = new List<string>
            {
                "1",
                "СЛОВО sp1,tag1,tag2,tag3,tag4",
                "ДРУГОЕСЛОВО sp1,tag1,tag2,tag3",
            };
            var secondMorpher = SentenceMorpher.Create(secondDefaultDictionary);
            string actualResultOfFirstMorpher;
            string actualResultOfSecondMorpher;

            // Act
            actualResultOfFirstMorpher = _morpher.Morph(input);
            actualResultOfSecondMorpher = secondMorpher.Morph(input);

            // Assert
            Assert.That(actualResultOfFirstMorpher, Is.EqualTo(expectedResultOfFirstMorpher).IgnoreCase);
            Assert.That(actualResultOfSecondMorpher, Is.EqualTo(expectedResultOfSecondMorpher).IgnoreCase);
        }
    }
}
