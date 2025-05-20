using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter
{
    public static class BasicExpressionFunctions
    {
        /// <summary>
        /// Determines the smallest value between two values (accepts numbers and strings) 
        /// </summary>
        /// <param name="args">Accepts 2 arguments in the array</param>
        /// <returns>If numbers (double or int) are supplied, then the smallest value is return. Else if strings are supplied then the shortest one is returned.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static object Min(object[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Min expects 2 arguments");
            }
            var a = args[0];
            var b = args[1];
            if (a == null || b == null) { return a ?? b; }
            if (a is int ai && b is int bi)
            {
                return Math.Min(ai, bi);
            }
            if (a is double ad && b is double bd)
            {
                return Math.Min(ad, bd);
            }
            if (a is string aStr && b is string bStr)
            {
                return aStr.Length < bStr.Length ? aStr : bStr;
            }
            throw new InvalidOperationException();
        }
        /// <summary>
        /// Determines the largest value between two values (accepts numbers and strings) 
        /// </summary>
        /// <param name="args">Accepts 2 arguments in the array</param>
        /// <returns>If numbers (double or int) are supplied, then the largest value is return. Else if strings are supplied then the longest one is returned.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static object Max(object[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Max expects 2 arguments");
            }
            var a = args[0];
            var b = args[1];
            if (a == null || b == null) { return a ?? b; }
            if (a is int ai && b is int bi)
            {
                return Math.Max(ai, bi);
            }
            if (a is double ad && b is double bd)
            {
                return Math.Max(ad, bd);
            }
            if (a is string aStr && b is string bStr)
            {
                return aStr.Length > bStr.Length ? aStr : bStr;
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Tests whether the supplied string starts with the supplied value
        /// </summary>
        /// <returns>True, if the first string starts with the second</returns>
        public static bool StartsWith(string stringToTest, string value)
        {
           return stringToTest!.StartsWith(value!);
        }

        /// <summary>
        /// Tests whether the supplied string ends with the supplied value
        /// </summary>
        /// <returns>True, if the first string ends with the second</returns>
        public static bool EndsWith(string stringToTest, string value)
        {
           return stringToTest!.EndsWith(value!);
        }
    }
}
