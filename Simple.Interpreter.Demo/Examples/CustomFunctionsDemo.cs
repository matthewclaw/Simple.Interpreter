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
            interpreter.RegisterFunction<User, int, bool>("isUserOlderThan", IsUserOlderThan);

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
            if (result is bool isOldEnough && isOldEnough)
            {
                Console.WriteLine($"{frank} is Older than {ageToTest}");
            }
            else
            {
                Console.WriteLine($"{frank} is not older than {ageToTest}");
            }

            Console.ReadLine();
        }


        private static bool IsUserOlderThan(User user, int age)
        {
            bool result = user?.Age > age;
            return result;
        }
    }
}
