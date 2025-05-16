using Simple.Interpreter.Tests.Ast.Expression.Models;
using Simple.Interpreter.Tests.Ast.Expression.TestCases.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.TestCases
{
    internal class ExpressionValidationTestCases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new ExpressionValidationTest(expression: "2+1 if(5==5) else 4");
            yield return new ExpressionValidationTest("2+1 if(5+5) else 4", new List<string> { "" });
            yield return new ExpressionValidationTest("2+1 if(context==true) else 4", new Dictionary<string, Type>() { { "context", typeof(bool) } });
            yield return new ExpressionValidationTest("foo+' test'", new Dictionary<string, Type> { { "foo", typeof(string) } });
            yield return new ExpressionValidationTest("bar+' test'", new Dictionary<string, Type> { { "foo", typeof(string) } }, new List<string> { "" });
            yield return new ExpressionValidationTest("context.MyProperty+1", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } });
            yield return new ExpressionValidationTest("context.MyProperty4", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, new List<string> { ""});
            yield return new ExpressionValidationTest("context.MyMethod()", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } });
            yield return new ExpressionValidationTest("context.MyMethod(()", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, new List<string> { ""});
            yield return new ExpressionValidationTest("context.MyMethod2('test')", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } });
            yield return new ExpressionValidationTest("context.MyMethod2('test','bar')", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, new List<string> { ""});
            yield return new ExpressionValidationTest("context.MyMethod3('test','bar')", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } });
            yield return new ExpressionValidationTest("1+(2*3)");
            yield return new ExpressionValidationTest("1+((2*3)/2)");
            yield return new ExpressionValidationTest("1)+((2*3)/2)", new List<string> { ""});
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
