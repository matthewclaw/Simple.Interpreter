
using Simple.Interpreter.Tests.Ast.Expression.Models;
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
    public class ExpressionScopedTestCases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new ExpressionScopeTest(expression: "min(600,42)", variables: new Dictionary<string, object>(), errorExpected: false, expectedValue: 42);
            yield return new ExpressionScopeTest(expression: "fax(600,42)", variables: new Dictionary<string, object>(), errorExpected: true);
            yield return new ExpressionScopeTest(expression: "value in []", variables: new Dictionary<string, object> { { "value", 50.2 } }, errorExpected: true, expectedValue: true);
            yield return new ExpressionScopeTest(expression: "foo+bar", variables: new Dictionary<string, object> { { "foo", 5 }, { "bar", 4 } }, errorExpected: false, expectedValue: 9);
            yield return new ExpressionScopeTest(expression: "first+' '+last", variables: new Dictionary<string, object> { { "first", "Test" }, { "last", "Expression Value" } }, errorExpected: false, expectedValue: "Test Expression Value");
            yield return new ExpressionScopeTest(expression: "fake+invalid",variables: null,errorExpected: true);
            yield return new ExpressionScopeTest(expression: "value in ['foo','bar']", variables: new Dictionary<string, object> { { "value", "test" } }, errorExpected: false, expectedValue: false);
            yield return new ExpressionScopeTest(expression: "value in ['foo','bar']", variables: new Dictionary<string, object> { { "value", "foo" } }, errorExpected: false, expectedValue: true);
            yield return new ExpressionScopeTest(expression: "value in [42,5]", variables: new Dictionary<string, object> { { "value", 42 } }, errorExpected: false, expectedValue: true);
            yield return new ExpressionScopeTest(expression: "value is 42", variables: new Dictionary<string, object> { { "value", 42 } }, errorExpected: false, expectedValue: true);
            yield return new ExpressionScopeTest(expression: "value in [42,50.2]", variables: new Dictionary<string, object> { { "value", 50.2 } }, errorExpected: false, expectedValue: true);
            yield return new ExpressionScopeTest(expression: "value in [42.1,50.2]", variables: new Dictionary<string, object> { { "value", 50.2 } }, errorExpected: false, expectedValue: true);
            yield return new ExpressionScopeTest(expression: "value not in [42.1,50.2]", variables: new Dictionary<string, object> { { "value", 60 } }, errorExpected: false, expectedValue: true);
            yield return new ExpressionScopeTest(expression: "value not in [42.1,50.2]", variables: new Dictionary<string, object> { { "value", "foo" } }, errorExpected: true);
            yield return new ExpressionScopeTest(expression: "context.MyMethod()", variables: new Dictionary<string, object> { { "context", new ExpressionTestObject() } }, errorExpected: false, expectedValue: "foo");
            yield return new ExpressionScopeTest(expression: "context.MyField", variables: new Dictionary<string, object> { { "context", new ExpressionTestObject() } }, errorExpected: false, expectedValue: "foo");
            yield return new ExpressionScopeTest(expression: "context.MyProperty2", variables: new Dictionary<string, object>(), errorExpected: true);
            yield return new ExpressionScopeTest(expression: "context.MyProperty33", variables: new Dictionary<string, object> { { "context", new ExpressionTestObject() } }, errorExpected: true);
            yield return new ExpressionScopeTest(expression: "context.MyProperty", variables: new Dictionary<string, object> { { "context", new ExpressionTestObject() } }, errorExpected: false, expectedValue: 42);
            yield return new ExpressionScopeTest(expression: "context.MyProperty is greater than 20", variables: new Dictionary<string, object> { { "context", new ExpressionTestObject() } }, errorExpected: false, expectedValue: true);
            yield return new ExpressionScopeTest(expression: "context.MyMethod2('test')", variables: new Dictionary<string, object> { { "context", new ExpressionTestObject() } }, errorExpected: false, expectedValue: "TEST");
            yield return new ExpressionScopeTest(expression: "context.MyMethod7()", variables: new Dictionary<string, object> { { "context", new ExpressionTestObject() } }, errorExpected: true);
            yield return new ExpressionScopeTest(expression: "context.MyMethod2('test')", variables: new Dictionary<string, object>(), errorExpected: true);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
