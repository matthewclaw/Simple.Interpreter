using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.TestCases.Models
{
    internal class ExpressionValidationTest
    {
        public readonly string Expression;
        public readonly Dictionary<string, Type> ValidVariables;
        public readonly List<string> ExpectedErrorParts;
        public ExpressionValidationTest(string expression, List<string> expectedErrorParts) : this(expression, null, expectedErrorParts) { }
        public ExpressionValidationTest(string expression) : this(expression, new Dictionary<string, Type>()) { }
        public ExpressionValidationTest(string expression, Dictionary<string, Type>? validVariables) : this(expression, validVariables, null)
        {

        }
        public ExpressionValidationTest(string expression, Dictionary<string, Type>? validVariables, List<string>? expectedErrorParts)
        {
            Expression = expression;
            ValidVariables = validVariables ?? new Dictionary<string, Type>();
            ExpectedErrorParts = expectedErrorParts ?? new List<string>();
        }

        public static implicit operator object[](ExpressionValidationTest obj)
        {
            return new object[] { obj.Expression, obj.ValidVariables, obj.ExpectedErrorParts };
        }
    }
}
