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
    /// Class to demonstrate basic usage of the <seealso cref="ExpressionInterpreter"/> and variables 
    /// </summary>
    public static class ConditionExpressionDemo
    {
        public const string EXPRESSION = "user.Age > 18 and user.City == 'Johannesburg'";
        public const string DESCRIPTION = $"Condition Expression: {EXPRESSION}";

        public static void Run()
        {
            
            // Creating users
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

            // Initializing the interpreter
            ExpressionInterpreter interpreter = new ExpressionInterpreter();

            // Preloads the reflection information for the variable (This is optional)
            interpreter.GlobalScope.RegisterVariableType("user", typeof(User));

            // Get the Expression Object
            var expression = interpreter.GetExpression(EXPRESSION);
            Console.WriteLine($"Parsed Expression Succesfully: {expression}");

            #region Alice
            // Setting the variable to "Alice"
            expression.SetScopedVariable("user", alice);
            Console.WriteLine($"Checking {alice} against expression");
            object aliceResult = expression.Evaluate();

            if (aliceResult is bool aliceMeetsCriteria && aliceMeetsCriteria)
            {
                Console.WriteLine($"{alice.Name} meets the criteria.");
            }
            else
            {
                Console.WriteLine($"{alice.Name} does not meet the criteria.");
            }
            #endregion

            #region Alice
            // Setting the variable to "Bob"
            expression.SetScopedVariable("user", bob);
            Console.WriteLine($"Checking {bob} against expression");
            object bobResult = expression.Evaluate();

            if (bobResult is bool bobMeetsCriteria && bobMeetsCriteria)
            {
                Console.WriteLine($"{bob.Name} meets the criteria.");
            }
            else
            {
                Console.WriteLine($"{bob.Name} does not meet the criteria.");
            }
            #endregion
        }
    }
}
