using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter
{
    public static class BasicExpressionFunctions
    {
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
    }
}
