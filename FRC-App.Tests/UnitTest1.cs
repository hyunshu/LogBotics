using NUnit.Framework; // This is necessary to use NUnit attributes like [Test] and [SetUp]

namespace FRC_App.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        [SetUp]
        public void Setup()
        {
            // Code that runs before each test
        }

        [Test]
        public void Test1()
        {
            // Your test code here
            Assert.Pass();
        }
    }
}
