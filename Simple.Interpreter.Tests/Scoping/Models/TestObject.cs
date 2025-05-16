using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Tests.Scoping.Models
{
    [ExcludeFromCodeCoverage]
    public class TestObject
    {
        public int PublicProperty { get; set; }
        public string PublicField;
        public void PublicMethod() { }
        public int PublicMethodWithReturn() => 1;
        public void PublicMethodWithParams(int a, string b) { }
        public void PublicMethodWithOptionalParams(int a, string b = "default") { }
        public T GenericMethod<T>(T input) => input;
        public T GenericMethodWithConstraint<T>(T input) where T : class => input;
        public int Add(int a, int b) => a + b;
    }
}
