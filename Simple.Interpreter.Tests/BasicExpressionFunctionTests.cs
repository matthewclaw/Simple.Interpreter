using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests
{
    [ExcludeFromCodeCoverage]
    public class BasicExpressionFunctionTests
    {
        #region Public Methods

        [Fact]
        public void EndsWith_ValidParams_ReturnsCorrectly()
        {
            // Arrange
            var value = "bar";
            var testFor = "r";

            // Act
            var result = BasicExpressionFunctions.EndsWith(value, testFor);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Max_WithDoubles_ReturnsLargest()
        {
            // Arrange
            var value1 = 42.5;
            var value2 = 68.999;

            // Act
            var result = BasicExpressionFunctions.Max(value1, value2);

            // Assert
            Assert.Equal(value2, result);
        }

        [Fact]
        public void Max_WithInts_ReturnsLargest()
        {
            // Arrange
            var value1 = 42;
            var value2 = 69;

            // Act
            var result = BasicExpressionFunctions.Max(value1, value2);

            // Assert
            Assert.Equal(value2, result);
        }

        [Fact]
        public void Max_WithMixTypes_ThrowsArgumentException()
        {
            // Arrange
            int value1 = 1;
            var value2 = "foo";
            bool wasExceptionThrown = false;

            // Act
            try
            {
                _ = BasicExpressionFunctions.Max(value1, value2);
            }
            catch
            {
                wasExceptionThrown = true;
            }
            // Assert
            Assert.True(wasExceptionThrown);
        }

        [Fact]
        public void Max_WithOneNull_ReturnsNotNull()
        {
            // Arrange
            object? value1 = null;
            var value2 = "foo";

            // Act
            var result = BasicExpressionFunctions.Max(value1, value2);

            // Assert
            Assert.Equal(value2, result);
        }

        [Fact]
        public void Max_WithStrings_ReturnsLongest()
        {
            // Arrange
            var value1 = "foo-bar";
            var value2 = "foo";

            // Act
            var result = BasicExpressionFunctions.Max(value1, value2);

            // Assert
            Assert.Equal(value1, result);
        }

        [Fact]
        public void Min_WithDoubles_ReturnsSmallest()
        {
            // Arrange
            var value1 = 42.5;
            var value2 = 68.999;

            // Act
            var result = BasicExpressionFunctions.Min(value1, value2);

            // Assert
            Assert.Equal(value1, result);
        }

        [Fact]
        public void Min_WithInts_ReturnsSmallest()
        {
            // Arrange
            var value1 = 42;
            var value2 = 69;

            // Act
            var result = BasicExpressionFunctions.Min(value1, value2);

            // Assert
            Assert.Equal(value1, result);
        }

        [Fact]
        public void Min_WithMixTypes_ThrowsArgumentException()
        {
            // Arrange
            int value1 = 1;
            var value2 = "foo";
            bool wasExceptionThrown = false;

            // Act
            try
            {
                _ = BasicExpressionFunctions.Min(value1, value2);
            }
            catch
            {
                wasExceptionThrown = true;
            }
            // Assert
            Assert.True(wasExceptionThrown);
        }

        [Fact]
        public void Min_WithOneNull_ReturnsNotNull()
        {
            // Arrange
            object? value1 = null;
            var value2 = "foo";

            // Act
            var result = BasicExpressionFunctions.Min(value1, value2);

            // Assert
            Assert.Equal(value2, result);
        }

        [Fact]
        public void Min_WithStrings_ReturnsShortest()
        {
            // Arrange
            var value1 = "foo-bar";
            var value2 = "foo";

            // Act
            var result = BasicExpressionFunctions.Min(value1, value2);

            // Assert
            Assert.Equal(value2, result);
        }

        [Fact]
        public void StartsWith_ValidParams_ReturnsCorrectly()
        {
            // Arrange
            var value = "foo";
            var testFor = "f";

            // Act
            var result = BasicExpressionFunctions.StartsWith(value, testFor);

            // Assert
            Assert.True(result);
        }

        #endregion Public Methods
    }
}