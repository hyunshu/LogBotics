using Xunit;

namespace FRC_APP.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test_OneEqualsOne()
        {
            // Arrange
            int expected = 1;
            int actual = 1;

            // Act & Assert
            Assert.Equal(expected, actual);
        }
    }
}