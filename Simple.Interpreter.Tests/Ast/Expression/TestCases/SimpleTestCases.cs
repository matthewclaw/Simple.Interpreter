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
    internal class SimpleTestCases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new SimpleTest(expression: "1+4 if(5==2) else 4", expectedValue: 4);
            yield return new SimpleTest(expression: "2+3", expectedValue: 5);
            yield return new SimpleTest(expression: "10/2.5", expectedValue: 4.0);
            yield return new SimpleTest(expression: "10+2+3", expectedValue: 15);
            yield return new SimpleTest(expression: "100 - (10*8)", expectedValue: 20);
            yield return new SimpleTest(expression: "2>3", expectedValue: false);
            yield return new SimpleTest(expression: "3>=3", expectedValue: true);
            yield return new SimpleTest(expression: "2<=3", expectedValue: true);
            yield return new SimpleTest(expression: "2<1", expectedValue: false);
            yield return new SimpleTest(expression: "'foo'=='foo'", expectedValue: true);
            yield return new SimpleTest(expression: "4==5", expectedValue: false);
            yield return new SimpleTest(expression: "6!=4", expectedValue: true);
            yield return new SimpleTest(expression: "true and false", expectedValue: false);
            yield return new SimpleTest(expression: "true or false", expectedValue: true);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
