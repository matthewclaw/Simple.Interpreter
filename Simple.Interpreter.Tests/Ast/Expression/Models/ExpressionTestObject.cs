using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Ast.Expression.Models
{
    public class ExpressionTestObject
    {
        public int MyProperty { get; set; }
        public string MyField;

        public ExpressionTestObject()
        {
            MyProperty = 42;
            MyField = "foo";
        }
        public string MyMethod()
        {
            return "foo";
        }
        public string MyMethod2(string argument)
        {
            return argument.ToUpper();
        }
        public string MyMethod3(string argument, string argument2)
        {
            return argument.ToUpper() + argument2.ToLower();
        }
    }
}
