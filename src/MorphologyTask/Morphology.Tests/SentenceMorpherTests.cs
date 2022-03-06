using Morphology;

using NUnit.Framework;

using System;
using System.Collections.Generic;

namespace Morphology.Tests
{
    [TestFixture]
    public class SentenceMorpherTests
    {
        [Test]
        public void Create_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            IEnumerable<string> dictionaryLines = null;
            var sentenceMorpher = SentenceMorpher.Create(dictionaryLines);

            // Act
            

            // Assert
            Assert.Fail();
        }

        [Test]
        public void Morph_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            IEnumerable<string> dictionaryLines = null;
            var sentenceMorpher = SentenceMorpher.Create(dictionaryLines);

            // Act


            // Assert
            Assert.Fail();
        }
    }
}
