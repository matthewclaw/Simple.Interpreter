using Simple.Interpreter.Ast;
using Simple.Interpreter.Demo.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Demo.Examples
{
    internal class ValidationDemo
    {
        public static void Run()
        {
            var alice = new User
            {
                Name = "Alice",
                Age = 25,
                City = "Pretoria"
            };
            var bob = new User
            {
                Name = "Bob",
                Age = 19,
                City = "Johannesburg"
            };
            var frank = new User
            {
                Name = "Frank",
                Age = 40,
                City = "Cape Town"
            };
            var foo = "demo";
            var bar = 42;
            var demoScope = new Dictionary<string, object>
            {
                { "alice", alice },
                { "bob", bob },
                { "frank", frank },
                { "foo", foo },
                { "bar", bar },
            };
            ExpressionInterpreter interpreter = new ExpressionInterpreter();
            interpreter.SetGlobalScope(demoScope);
            Console.WriteLine("Try input an expression and hit enter, lets see if it validates. You have the following scope at your disposal:");
            Console.WriteLine(interpreter.GlobalScope);
            Console.WriteLine("AND the following built-in functions:");
            foreach (var item in interpreter.RegisteredFunctions)
            {
                Console.WriteLine($"- {item.Key}");
            }
            var userExpression = Console.ReadLine() ?? "";
            var isValid = interpreter.Validate(userExpression, out var expression, out var errors);
            if (isValid)
            {
                Console.WriteLine("Nice, looks like it's valid!");
                Console.WriteLine($"And your expression's result is: {expression!.Evaluate()}");
            }
            else
            {
                Console.WriteLine("Oops, looks like it had some issue(s):");
                foreach (var error in errors)
                {
                    Console.WriteLine($"- {error.Message}");
                }
            }
            Console.WriteLine("Want to try another one?(Y/N)");
            switch (Console.ReadLine()?.Trim().ToLower() ?? "n")
            {
                case "y":
                    Run();
                    break;
                default:
                    return;
            }
        }
    }
}
