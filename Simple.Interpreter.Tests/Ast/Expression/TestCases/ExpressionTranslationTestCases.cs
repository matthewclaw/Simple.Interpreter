using Simple.Interpreter.Tests.Ast.Expression.TestCases.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.TestCases
{
    internal class ExpressionTranslationTestCases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new ExpressionTranslationTest("3*2", "3 * 2");
            yield return new ExpressionTranslationTest("3.0*2.0", "3f * 2f");
            yield return new ExpressionTranslationTest("value in [4,2]", "(new int[] { 4, 2 }).Contains(value)");
            yield return new ExpressionTranslationTest("value in [4.3,2.5]", "(new double[] { 4.3f, 2.5f }).Contains(value)");
            yield return new ExpressionTranslationTest("value in ['foo', 'bar']", "(new string[] { \"foo\", \"bar\" }).Contains(value)");
            yield return new ExpressionTranslationTest("value not in [4,2]", "!(new int[] { 4, 2 }).Contains(value)");
            yield return new ExpressionTranslationTest("value and test", "(value) && (test)");
            yield return new ExpressionTranslationTest("value or test", "(value) || (test)");
            yield return new ExpressionTranslationTest("'42'", "\"42\"");
            yield return new ExpressionTranslationTest("variable.MyProperty", "variable.MyProperty");
            yield return new ExpressionTranslationTest("variable.MyMethod('foo')", "variable.MyMethod(\"foo\")");
            yield return new ExpressionTranslationTest("'42' if(true) else '69'", "(true)? \"42\" : \"69\"");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
