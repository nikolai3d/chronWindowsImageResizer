using System;
using Xunit;

namespace ChronoImageResizer.Tests
{
    public class BasicTests
    {
        [Fact]
        public void Test_Addition()
        {
            // Arrange
            int a = 2;
            int b = 2;
            int expected = 4;
            
            // Act
            int result = a + b;
            
            // Assert
            Assert.Equal(expected, result);
        }
    }
}