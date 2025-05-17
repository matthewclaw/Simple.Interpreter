using Simple.Interpreter.Demo.Examples;

namespace Simple.Interpreter.Demo
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            string choice = string.Empty;
            while (choice != "x")
            {
                Console.WriteLine("Select which demo would you like to run:");
                Console.WriteLine("c - An example of a simple condition expression.");
                Console.WriteLine("f - An example of a custom function usage.");
                Console.WriteLine("v - An example of the validation functionality (you can write an expression).");
                Console.WriteLine("x - Stop the demo");
                choice = Console.ReadLine()?.ToLower().Trim() ?? string.Empty;
                Console.Clear();
                switch (choice)
                {
                    case "c":
                        ConditionExpressionDemo.Run();
                        break;

                    case "f":
                        CustomFunctionsDemo.Run();
                        break;

                    case "v":
                        ValidationDemo.Run();
                        break;
                }
                if (choice != "x")
                {
                    Console.Clear();
                }
            }
        }

        #endregion Private Methods
    }
}