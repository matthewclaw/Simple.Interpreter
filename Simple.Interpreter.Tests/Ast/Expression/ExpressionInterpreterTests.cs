using Simple.Interpreter.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression
{
    public class ExpressionInterpreterTests
    {
        [Fact]
        public void SetGlobalScope_AssignsValueAccessibleToExpressions()
        {
            // Arrange
            var interpreter = new ExpressionInterpreter();
            var expectedValue = "foo";
            var expressionString = "value";
            var scope = new Dictionary<string, object>()
            {
                { "value", expectedValue }
            };

            // Act
            interpreter.SetGlobalScope(scope);
            var expression = interpreter.GetExpression(expressionString);
            var actualValue = expression.Evaluate();

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void RegisterFunction_WithDifferentParams_RegistersAndExecutesCorrectly()
        {
            // Arrange
            var interpreter = new ExpressionInterpreter();

            Func<string> noParamFunc = () => "noParamFunc";
            var noParamFuncName = "noParam";
            var expectedNoParamFuncResult = "noParamFunc";
            var noParamExpressionString = "noParam()";

            Func<int, int> oneParamFunc = (a) => a * 2;
            var oneParamFuncName = "oneParam";
            var expectedOneParamFuncResult = 4;
            var oneParamFuncExpressionString = "oneParam(2)";

            Func<string, string, string> twoParamFunc = (a, b) => a + b;
            var twoParamFuncName = "twoParam";
            var expectedTwoParamFuncResult = "foobar";
            var twoParamFuncExpressionString = "twoParam('foo','bar')";

            Func<string, string, string, int> threeParamFunc = (a, b, c) => (a + b + c).Length;
            var threeParamFuncName = "threeParam";
            var expectedThreeParamFuncResult = 3;
            var threeParamFuncExpressionString = "threeParam('a','b','c')";

            Func<int, int, int, double, double> fourParamFunc = (a, b, c, d) => (a + b + c) * d;
            var fourParamFuncName = "fourParam";
            var expectedFourParamFuncResult = 2.5;
            var fourParamFuncExpressionString = "fourParam(1,2,2,0.5)";

            // Act
            interpreter.RegisterFunction(noParamFuncName, noParamFunc);
            interpreter.RegisterFunction(oneParamFuncName, oneParamFunc);
            interpreter.RegisterFunction(twoParamFuncName, twoParamFunc);
            interpreter.RegisterFunction(threeParamFuncName, threeParamFunc);
            interpreter.RegisterFunction(fourParamFuncName, fourParamFunc);

            var expression = interpreter.GetExpression(noParamExpressionString);
            var actualNoParamFuncResult = expression.Evaluate();

            expression = interpreter.GetExpression(oneParamFuncExpressionString);
            var actualOneParamFuncResult = expression.Evaluate();

            expression = interpreter.GetExpression(twoParamFuncExpressionString);
            var actualTwoParamFuncResult = expression.Evaluate();

            expression = interpreter.GetExpression(threeParamFuncExpressionString);
            var actualThreeParamFuncResult = expression.Evaluate();

            expression = interpreter.GetExpression(fourParamFuncExpressionString);
            var actualFourParamFuncResult = expression.Evaluate();

            // Assert
            Assert.Equal(9, interpreter.RegisteredFunctions.Count); // Includes the "built-in" functions
            Assert.Equal(expectedNoParamFuncResult, actualNoParamFuncResult);
            Assert.Equal(expectedOneParamFuncResult, actualOneParamFuncResult);
            Assert.Equal(expectedTwoParamFuncResult, actualTwoParamFuncResult);
            Assert.Equal(expectedThreeParamFuncResult, actualThreeParamFuncResult);
            Assert.Equal(expectedFourParamFuncResult, actualFourParamFuncResult);

        }
    }
}
