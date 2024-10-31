using NUnit.Framework;

namespace FRC_App.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void AdditionTest()
        {
            // Arrange
            int a = 5;
            int b = 10;
            
            // Act
            int result = a + b;

            // Assert
            Assert.AreEqual(15, result);
        }

        [Test]
        public void StringConcatenationTest()
        {
            // Arrange
            string first = "Hello";
            string second = "World";

            // Act
            string result = first + " " + second;

            // Assert
            Assert.AreEqual("Hello World", result);
        }

        [Test]
        public void ListCountTest()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            int count = items.Count;

            // Assert
            Assert.AreEqual(5, count);
        }
    }
}
