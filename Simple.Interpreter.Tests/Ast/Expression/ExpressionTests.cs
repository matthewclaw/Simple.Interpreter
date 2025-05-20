using Simple.Interpreter.Ast;
using Simple.Interpreter.Tests.Ast.Expression.TestCases;

namespace Simple.Interpreter.Tests.Ast.Expression
{
    public class ExpressionTests
    {
        private ExpressionInterpreter _expressionInterpreter;
        public ExpressionTests()
        {
            _expressionInterpreter = new ExpressionInterpreter();
        }
        [Theory]
        [ClassData(typeof(ExpressionScopedTestCases))]
        public void Expression_EvaluateWithScopeExpression_ReturnsCorrectly(string expression, Dictionary<string, object>? variables, bool errorExpected, object? expectedValue)
        {
            // Arrange
            var exceptionThrown = false;
            object actualValue = null;

            // Act
            try
            {
                var expressionObj = _expressionInterpreter.GetExpression(expression);
                if (variables != null)
                {
                    expressionObj.SetScope(variables);
                }
                actualValue = expressionObj.Evaluate();
            }
            catch
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.Equal(errorExpected, exceptionThrown);
            if (!errorExpected)
            {
                Assert.Equal(expectedValue, actualValue);
            }
        }
        [Theory]
        [ClassData(typeof(ExpressionSimpleTestCases))]
        public void Expression_EvaluateWithSimpleExpression_ReturnsCorrectly(string expression, object expectedValue)
        {
            // Arrange

            //Act
            var expressionObj = _expressionInterpreter.GetExpression(expression);
            var actualValue = expressionObj.Evaluate();
            var type = actualValue.GetType();
            var expectedType = expectedValue.GetType();

            //Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Theory]
        [ClassData(typeof(ExpressionTranslationTestCases))]
        public void Expression_GetCSharp_TranslatesCorrectly(string expression, string expectedValue)
        {
            // Arrange
            var expressionObj = _expressionInterpreter.GetExpression(expression);

            // Act
            var actualCSharp = expressionObj.GetCSharp();

            // Assert
            Assert.Equal(expectedValue, actualCSharp);
        }
        [Theory]
        [ClassData(typeof(ExpressionValidationTestCases))]
        public void Expression_Validate_ValidatesCorrectly(string expression, Dictionary<string, Type> variableTypes, List<string> expectedErrorParts, bool useRegister)
        {
            // Arrange
            var expectedErrorCount = expectedErrorParts.Count;
            var expectedResult = expectedErrorCount == 0;
            bool actualResult = false;
            List<Exception>? actualErrors = null;

            // Act
            if (variableTypes is not null && variableTypes.Count > 0)
            {
                if (useRegister)
                {
                    _expressionInterpreter.GlobalScope.RegisterVariableTypes(variableTypes);
                    actualResult = _expressionInterpreter.Validate(expression, out actualErrors);
                }
                else
                {
                    actualResult = _expressionInterpreter.Validate(expression, variableTypes, out actualErrors);
                }
            }
            else
            {
                actualResult = _expressionInterpreter.Validate(expression, out actualErrors);
            }

            // Assert
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedErrorCount, actualErrors.Count);
            if (actualErrors.Count > 0)
            {
                for (var i = 0; i < expectedErrorCount; i++)
                {
                    Assert.Contains(expectedErrorParts[i], actualErrors[i].Message);
                }
            }
        }
    }
}
