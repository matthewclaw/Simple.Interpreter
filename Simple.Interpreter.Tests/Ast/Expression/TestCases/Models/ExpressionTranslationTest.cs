using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.TestCases.Models
{
    [ExcludeFromCodeCoverage]
    internal class ExpressionTranslationTest
    {
        public readonly string Expression;
        public readonly string CSharpTranslation;
        public ExpressionTranslationTest(string expression, string cSharpTranslation)
        {
            this.Expression = expression;
            this.CSharpTranslation = cSharpTranslation;
        }
        public static implicit operator object[](ExpressionTranslationTest obj)
        {
            return new object[] { obj.Expression, obj.CSharpTranslation };
        }
    }
}
