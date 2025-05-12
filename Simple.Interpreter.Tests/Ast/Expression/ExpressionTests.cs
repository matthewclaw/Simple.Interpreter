using Simple.Interpreter.Ast;
using Simple.Interpreter.Ast.Interfaces;
using Simple.Interpreter.Tests.Ast.Expression.TestCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        [ClassData(typeof(SimpleTestCases))]
        public void Expression_EvaluateWithScopeExpression_ReturnsCorrectly(string expression, object expectedValue)
        {
            //// Arrange
            //var exceptionThrown = false;
            //object actualValue = null;

            //// Act
            //try
            //{
            //    var expressionObj = _expressionInterpreter.GetExpression(expression);
            //    if (data.Variables != null)
            //    {
            //        expression.SetScope(data.Variables);
            //    }
            //    actualValue = expression.Evaluate();
            //}
            //catch
            //{
            //    exceptionThrown = true;
            //}

            //// Assert
            //Assert.Equal(data.ErrorExpected, exceptionThrown);
            //if (!data.ErrorExpected)
            //{
            //    Assert.Equal(data.ExpectedValue, actualValue);
            //}
        }
        [Theory]
        [ClassData(typeof(SimpleTestCases))]
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
    }
}
