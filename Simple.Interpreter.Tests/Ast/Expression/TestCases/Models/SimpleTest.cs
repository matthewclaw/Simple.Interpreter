using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.TestCases.Models
{
    public class SimpleTest
    {
        public readonly string Expression;
        public readonly object ExpectedValue;
        public SimpleTest(string expression, object expectedValue)
        {
            Expression = expression;
            ExpectedValue = expectedValue;
        }
        public static implicit operator object[](SimpleTest obj)
        {
            return new object[] { obj.Expression, obj.ExpectedValue };
        }
    }
}
