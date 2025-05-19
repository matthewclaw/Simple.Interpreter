using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Extensions
{
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Attempts to invoke the specified method on the given instance with the provided arguments.
        /// </summary>
        /// <param name="methodInfo">The MethodInfo object representing the method to invoke.</param>
        /// <param name="instance">The object instance on which to invoke the method.  Use null for static methods.</param>
        /// <param name="result">When this method returns, contains the return value of the invoked method, or null if the invocation failed.</param>
        /// <param name="args">An optional array of arguments to pass to the method.  Can be null or empty.</param>
        /// <returns>True if the method was successfully invoked; otherwise, false.</returns>
        public static bool TryInvoke(this MethodInfo methodInfo, object instance, out object? result, params object[]? args)
        {
            result = null;
            try
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 0)
                {
                    result = methodInfo.Invoke(instance, null);
                    return true;
                }
                object[]? invokeArgs = null;
                if (parameters.Length != (args?.Length ?? 0))
                {
                    invokeArgs = new object[parameters.Length];
                    if (args is null || args.Length == 0)
                    {
                        args = Array.Empty<object>();
                    }
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (i < args.Length)
                        {
                            // If the current index is within the bounds of the provided arguments array,
                            // assign the argument at that index to the corresponding position in the invokeArgs array.
                            invokeArgs[i] = args[i];
                        }
                        else if (parameters[i].IsOptional)
                        {
                            // If the parameter is optional and no argument was provided,
                            // use Type.Missing as the default value for the optional parameter.
                            invokeArgs[i] = Type.Missing;
                        }
                        else
                        {
                            //Not enough arguments and no optional parameter found.
                            return false;
                        }
                    }
                }
                else
                {
                    invokeArgs = args;
                }
                result = methodInfo.Invoke(instance, invokeArgs);


                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
