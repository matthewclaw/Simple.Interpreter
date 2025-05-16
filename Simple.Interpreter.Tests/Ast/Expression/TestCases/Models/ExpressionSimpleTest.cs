using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.TestCases.Models
{
    public class ExpressionSimpleTest
    {
        public readonly string Expression;
        public readonly object ExpectedValue;
        public ExpressionSimpleTest(string expression, object expectedValue)
        {
            Expression = expression;
            ExpectedValue = expectedValue;
        }
        public static implicit operator object[](ExpressionSimpleTest obj)
        {
            return new object[] { obj.Expression, obj.ExpectedValue };
        }
    }
}
