using Simple.Interpreter.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Extensions
{
    public class CollectionExtensionTests
    {
        #region Public Methods

        [Fact]
        public void Select_WithSimpleExpression_ReturnsTransformedSequence()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3 };
            string variableName = "x";
            string expressionStr = "string(x) + '!'";

            // Act
            var result = source.Select<int, string>(variableName, expressionStr);

            // Assert
            Assert.Equal(new List<string> { "1!", "2!", "3!" }, result.ToList());
        }

        [Fact]
        public void Where_EmptySource_ReturnsEmptySequence()
        {
            // Arrange
            var source = new List<int>();
            string variableName = "x";
            string expressionStr = "x > 2";

            // Act
            var result = source.Where(variableName, expressionStr);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Where_WithLengthGreaterThan5_ReturnsFilteredSequence()
        {
            // Arrange
            var source = new List<string> { "foo", "bar", "equivalent", "Hello Unit Test!", "xyz" };
            string variableName = "x";
            string expressionStr = "length(x) is greater than 5";

            // Act
            var result = source.Where(variableName, expressionStr);

            // Assert
            Assert.Equal(new List<string> { "equivalent", "Hello Unit Test!" }, result.ToList());
        }

        [Fact]
        public void Where_WithNonBoolExpression_ThrowsException()
        {
            // Arrange
            var source = new List<string> { "foo", "bar", "equivalent", "Hello Unit Test!", "xyz" };
            string variableName = "x";
            string expressionStr = "length(x)+4";
            bool exceptionWasThrown = false;

            // Act
            try
            {
                _ = source.Where(variableName, expressionStr).ToArray();
            }
            catch (Exception ex)
            {
                exceptionWasThrown = true;
            }
            // Assert
            Assert.True(exceptionWasThrown);
        }

        [Fact]
        public void Where_WithStartsWith_ReturnsFilteredSequence()
        {
            // Arrange
            var source = new List<string> { "apple", "banana", "apricot" };
            string variableName = "x";
            string expressionStr = "startsWith(x,'a')";

            // Act
            var result = source.Where(variableName, expressionStr);

            // Assert
            Assert.Equal(new List<string> { "apple", "apricot" }, result.ToList());
        }

        #endregion Public Methods
    }
}