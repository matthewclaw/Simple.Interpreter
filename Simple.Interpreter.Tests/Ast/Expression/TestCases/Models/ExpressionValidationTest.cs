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
        public readonly bool UseRegister;
        public ExpressionValidationTest(string expression, List<string> expectedErrorParts) : this(expression, null, expectedErrorParts, false) { }
        public ExpressionValidationTest(string expression) : this(expression, new Dictionary<string, Type>(), false) { }
        public ExpressionValidationTest(string expression, Dictionary<string, Type>? validVariables, bool useRegister) : this(expression, validVariables, null, useRegister)
        {

        }
        public ExpressionValidationTest(string expression, Dictionary<string, Type>? validVariables, List<string>? expectedErrorParts, bool useRegister)
        {
            Expression = expression;
            ValidVariables = validVariables ?? new Dictionary<string, Type>();
            ExpectedErrorParts = expectedErrorParts ?? new List<string>();
            UseRegister = useRegister;
        }

        public static implicit operator object[](ExpressionValidationTest obj)
        {
            return new object[] { obj.Expression, obj.ValidVariables, obj.ExpectedErrorParts, obj.UseRegister };
        }
    }
}
