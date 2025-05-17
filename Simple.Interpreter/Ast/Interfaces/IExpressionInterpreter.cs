using Simple.Interpreter.Scoping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Ast.Interfaces
{
    public interface IExpressionInterpreter
    {
        #region Public Properties

        Scope GlobalScope { get; }

        /// <summary>
        /// A dictionary containing registered functions that can be called within pseudo-code expressions.
        /// The keys are function names (strings), and the values are delegates that accept an array of objects as arguments and return an object.
        /// </summary>
        Dictionary<string, Func<object[], object>> RegisteredFunctions { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Parses a string expression into an Expression object, which represents the abstract syntax tree (AST) of the expression.
        /// This method tokenizes the input string, parses the tokens into an expression tree, and then constructs an Expression object
        /// that can be evaluated.  It handles operator precedence, function calls, and variable references.
        /// </summary>
        /// <param name="expression">The string representation of the expression to parse.</param>
        /// <returns>An Expression object representing the parsed expression.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the expression contains syntax errors, mismatched parentheses, or other parsing issues.  The exception message
        /// will provide details about the error and the location of the error within the expression.
        /// </exception>
        Expression GetExpression(string expression);

        /// <summary>
        /// Registers a function that can be called within pseudo-code expressions.
        /// </summary>
        /// <typeparam name="T">The return type of the function</typeparam>
        /// <param name="functionName">The function's name/keyword</param>
        /// <param name="implementation"></param>
        void RegisterFunction<T>(string functionName, Func<object, T> implementation);

        /// <summary>
        /// Registers a function that can be called within pseudo-code expressions.
        /// </summary>
        /// <typeparam name="T">The return type of the function</typeparam>
        /// <param name="functionName">The function's name/keyword</param>
        /// <param name="implementation"></param>
        void RegisterFunction<T>(string functionName, Func<object[], T> implementation);

        /// <summary>
        /// Sets the global scope of the interpreter with the provided dictionary. Each Expression created by this interpreter will have access to the variables in this scope.
        /// </summary>
        /// <param name="scope">A dictionary representing the global scope, where keys are variable names and values are their corresponding values.</param>
        void SetGlobalScope(Dictionary<string, object> scope);

        /// <summary>
        /// Validates an expression.  This method checks if the expression string can be parsed and if all members
        /// are valid (i.e., they exist within the global scope).
        /// </summary>
        /// <param name="expression">The expression string to validate.</param>
        /// <param name="errors">A list of exceptions encountered during validation.  Will be empty if validation succeeds.</param>
        /// <returns>True if the expression is valid; otherwise, false.</returns>
        bool Validate(string expression, out List<Exception> errors);

        /// <summary>
        /// Validates an expression.  This method checks if the expression string can be parsed and if all members
        /// are valid (i.e., they exist within the global scope).
        /// </summary>
        /// <param name="expression">The expression string to validate.</param>
        /// <param name="validExpression">If the expression is valid, an instance of <seealso cref="Expression"/> else null</param>
        /// <param name="errors">A list of exceptions encountered during validation.  Will be empty if validation succeeds.</param>
        /// <returns>True if the expression is valid; otherwise, false.</returns>
        bool Validate(string expression, out Expression? validExpression, out List<Exception> errors);

        /// <summary>
        /// Validates an expression.  This method checks if the expression string can be parsed and if all members
        /// are valid (i.e., they exist within <paramref name="validVariables"/>).
        /// </summary>
        /// <param name="expression">The expression string to validate.</param>
        /// <param name="validVariables">A dictionary of valid variable names and their corresponding types.</param>
        /// <param name="errors">A list of exceptions encountered during validation.  Will be empty if validation succeeds.</param>
        /// <returns>True if the expression is valid with the given variables; otherwise, false.</returns>
        bool Validate(string expression, Dictionary<string, Type> validVariables, out List<Exception> errors);

        #endregion Public Methods
    }
}
