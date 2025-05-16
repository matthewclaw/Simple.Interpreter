using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.TestCases.Models
{
    [ExcludeFromCodeCoverage]
    internal class ExpressionScopeTest
    {
        public readonly string Expression;
        public readonly Dictionary<string, object>? Variables;
        public readonly bool ErrorExpected;
        public readonly object? ExpectedValue;

        public ExpressionScopeTest(string expression, Dictionary<string, object>? variables, bool errorExpected) : this(expression, variables, errorExpected, null) { }
        public ExpressionScopeTest(string expression, Dictionary<string, object>? variables, bool errorExpected, object? expectedValue)
        {
            Expression = expression;
            Variables = variables;
            ErrorExpected = errorExpected;
            ExpectedValue = expectedValue;
        }

        public static implicit operator object[](ExpressionScopeTest obj)
        {
            return new object[] { obj.Expression, obj.Variables, obj.ErrorExpected, obj.ExpectedValue };
        }
    }
}
