using Simple.Interpreter.Tests.Ast.Expression.TestCases.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.TestCases
{
    [ExcludeFromCodeCoverage]
    internal class ExpressionSimpleTestCases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new ExpressionSimpleTest(expression: "1+4 if(5==2) else 4", expectedValue: 4);
            yield return new ExpressionSimpleTest(expression: "1+4 if(5 equals 2) else 4", expectedValue: 4);
            yield return new ExpressionSimpleTest(expression: "2+3", expectedValue: 5);
            yield return new ExpressionSimpleTest(expression: "10/2.5", expectedValue: 4.0);
            yield return new ExpressionSimpleTest(expression: "10+2+3", expectedValue: 15);
            yield return new ExpressionSimpleTest(expression: "100 - (10*8)", expectedValue: 20);
            yield return new ExpressionSimpleTest(expression: "2>3", expectedValue: false);
            yield return new ExpressionSimpleTest(expression: "2 greater than 3", expectedValue: false);
            yield return new ExpressionSimpleTest(expression: "3>=3", expectedValue: true);
            yield return new ExpressionSimpleTest(expression: "3 is greater or equal to 3", expectedValue: true);
            yield return new ExpressionSimpleTest(expression: "2<=3", expectedValue: true);
            yield return new ExpressionSimpleTest(expression: "2 less or equal to 3", expectedValue: true);
            yield return new ExpressionSimpleTest(expression: "2<1", expectedValue: false);
            yield return new ExpressionSimpleTest(expression: "2 is less than 1", expectedValue: false);
            yield return new ExpressionSimpleTest(expression: "'foo'=='foo'", expectedValue: true);
            yield return new ExpressionSimpleTest(expression: "'foo' is equal to 'foo'", expectedValue: true);
            yield return new ExpressionSimpleTest(expression: "4==5", expectedValue: false);
            yield return new ExpressionSimpleTest(expression: "4 equals 5", expectedValue: false);
            yield return new ExpressionSimpleTest(expression: "4 is 5", expectedValue: false);
            yield return new ExpressionSimpleTest(expression: "4 equal to 5", expectedValue: false);
            yield return new ExpressionSimpleTest(expression: "6!=4", expectedValue: true);
            yield return new ExpressionSimpleTest(expression: "6 not equal to 4", expectedValue: true);
            yield return new ExpressionSimpleTest(expression: "6 is not equal to 4", expectedValue: true);
            yield return new ExpressionSimpleTest(expression: "true and false", expectedValue: false);
            yield return new ExpressionSimpleTest(expression: "true or false", expectedValue: true);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
