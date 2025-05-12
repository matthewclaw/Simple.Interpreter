using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.TestCases.Models
{
    internal class ScopedExpressionTest
    {
        public readonly string Expression;
        public readonly Dictionary<string, object> Variables;
        public readonly bool ErrorExpected;
        public readonly object? ExpectedValue;

        public ScopedExpressionTest(string expression, Dictionary<string, object> variables, bool errorExpected) : this(expression, variables, errorExpected, null) { }
        public ScopedExpressionTest(string expression, Dictionary<string, object> variables, bool errorExpected, object? expectedValue)
        {
            Expression = expression;
            Variables = variables;
            ErrorExpected = errorExpected;
            ExpectedValue = expectedValue;
        }

        public static implicit operator object[](ScopedExpressionTest obj)
        {
            return new object[] { obj.Expression, obj.Variables, obj.ErrorExpected, obj.ExpectedValue };
        }
    }
}
