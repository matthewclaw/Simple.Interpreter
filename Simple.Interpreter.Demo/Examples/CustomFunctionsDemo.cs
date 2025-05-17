using Simple.Interpreter.Ast;
using Simple.Interpreter.Demo.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Demo.Examples
{
    /// <summary>
    /// Class to demonstrate registering custom functions in the <seealso cref="ExpressionInterpreter"/>
    /// </summary>
    internal class CustomFunctionsDemo
    {
        public const string EXPRESSION = "isUserOlderThan(user, age)";
        public const string DESCRIPTION = $"Custom Function Registration and Evaluation for: {EXPRESSION}";

        public static void Run()
        {
            ExpressionInterpreter interpreter = new ExpressionInterpreter();
            //Register custom Function
            // PS: Due to limitations, custom functions must accept either an `object` or an `object[]` and return an `object`. Sorry :/
            interpreter.RegisterFunction("isUserOlderThan", IsUserOlderThan);

            var frank = new User
            {
                Name = "Frank",
                Age = 40,
                City = "Cape Town"
            };
            var ageToTest = 30;

            var scope = new Dictionary<string, object>()
            {
                {"user", frank },
                {"age", ageToTest }
            };

            Expression expression = interpreter.GetExpression(EXPRESSION);
            // Set its scope
            expression.SetScope(scope);

            var result = expression.Evaluate();
            if(result is bool isOldEnough && isOldEnough)
            {
                Console.WriteLine($"{frank} is Older than {ageToTest}");
            }
            else
            {
                Console.WriteLine($"{frank} is not older than {ageToTest}");
            }
        }


        private static object IsUserOlderThan(object[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException($"{nameof(IsUserOlderThan)} expects 2 arguments");
            }
            User? user = args[0] as User;
            int? age = args[1] as int?;
            bool result = user?.Age > age;
            return result;
        }
    }
}
