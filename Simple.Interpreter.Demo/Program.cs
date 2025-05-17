using Simple.Interpreter.Ast;
using Simple.Interpreter.Demo.Examples;
using System;
using System.Linq.Expressions;

namespace Simple.Interpreter.Demo
{

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(ConditionExpressionDemo.DESCRIPTION);
            Console.WriteLine("==============");
            ConditionExpressionDemo.Run();
            Console.WriteLine("==============");
            Console.WriteLine(CustomFunctionsDemo.DESCRIPTION);
            Console.WriteLine("==============");
            CustomFunctionsDemo.Run();
            Console.WriteLine("==============");
        }
    }
}
