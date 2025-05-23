﻿using Simple.Interpreter.Tests.Ast.Expression.Models;
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
            yield return new ExpressionValidationTest(expression: "2+1 if(5 equal to 5) else 4");
            yield return new ExpressionValidationTest(expression: "2+1 if(5 is 5) else 4");
            yield return new ExpressionValidationTest("2+1 if(5+5) else 4", new List<string> { "If condition must return a boolean value" });
            yield return new ExpressionValidationTest("2+1 if(context==true) else 4", new Dictionary<string, Type>() { { "context", typeof(bool) } }, true);
            yield return new ExpressionValidationTest("2+1 if(context==false) else 4", new Dictionary<string, Type>() { { "context", typeof(bool) } }, false);
            yield return new ExpressionValidationTest("2+1 if(2+5) else 4", new Dictionary<string, Type>() { { "context", typeof(bool) } }, new List<string> { "If condition must return a boolean value" }, false);
            yield return new ExpressionValidationTest("foo+' test'", new Dictionary<string, Type> { { "foo", typeof(string) } }, false);
            yield return new ExpressionValidationTest("bar+' test'", new Dictionary<string, Type> { { "foo", typeof(string) } }, new List<string> { "'bar' is not a valid variable in scope" }, false);
            yield return new ExpressionValidationTest("context.MyProperty+1", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } },true);
            yield return new ExpressionValidationTest("context.MyProperty.Sub+1", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, new List<string> { "Can only access members one level deep" }, false);
            yield return new ExpressionValidationTest("context.MyProperty.Sub()", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, new List<string> { "Can only access members one level deep" }, true);
            yield return new ExpressionValidationTest("context.MyProperty4+contextOther.MyProp", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, new List<string> { "Member 'MyProperty4' not found in context.", "Object 'contextOther' not found in scope." }, true);
            yield return new ExpressionValidationTest("context.MyMethod()", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, false);
            yield return new ExpressionValidationTest("context.MyMethod(()", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, new List<string> { "near token 3 (\")\")" }, false);
            yield return new ExpressionValidationTest("context.MyMethod2('test')", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, true);
            yield return new ExpressionValidationTest("context.MyMethod2('test','bar')", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, new List<string> { "Error: Function/Method 'MyMethod2' accepting [System.String, System.String] was not found in context." }, false);
            yield return new ExpressionValidationTest("context.MyMethod3('test','bar')", new Dictionary<string, Type> { { "context", typeof(ExpressionTestObject) } }, false);
            yield return new ExpressionValidationTest("1+(2*3)");
            yield return new ExpressionValidationTest("1+((2*3)/2)");
            yield return new ExpressionValidationTest("1)+((2*3)/2)", new List<string> { "near token 2 (\"+\")" });
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
