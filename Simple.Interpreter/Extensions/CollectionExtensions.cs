using Simple.Interpreter.Ast;
using Simple.Interpreter.Ast.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Extensions
{
    public static class CollectionExtensions
    {
        #region Public Methods

        /// <summary>
        /// Projects each element of a sequence into a new form using an <see cref="Expression"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the function represented by expression.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="variableName">The name of the variable to be used in the expression, representing each element of the source sequence.</param>
        /// <param name="expression">An Expression representing the transformation to apply to each element.</param>
        /// <returns>An IEnumerable whose elements are the result of invoking the transformation expression on each element of source.</returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, string variableName, Expression expression)
        {
            if (source is null)
            {
                return Enumerable.Empty<TResult>();
            }
            return source.Select(x =>
            {
                expression.SetScopedVariable(variableName, x);
                return expression.Evaluate<TResult>();
            });
        }

        /// <summary>
        /// Projects each element of a sequence into a new form using an expression string.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the function represented by expression.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="variableName">The name of the variable to be used in the expression, representing each element of the source sequence.</param>
        /// <param name="expressionStr">A string representing the <seealso cref="Expression"/> to apply to each element.</param>
        /// <param name="interpreter">An optional <seealso cref="IExpressionInterpreter"/> instance to use for parsing and evaluating the expression string. If null, a default <seealso cref="ExpressionInterpreter"/> will be created.</param>
        /// <returns>An IEnumerable whose elements are the result of invoking the transformation expression on each element of source.</returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, string variableName, string expressionStr, IExpressionInterpreter? interpreter = null)
        {
            if (source == null)
            {
                return Enumerable.Empty<TResult>();
            }
            if (interpreter is null)
            {
                interpreter = new ExpressionInterpreter();
            }
            interpreter.GlobalScope.RegisterVariableType(variableName, typeof(TSource));
            var expression = interpreter.GetExpression(expressionStr);
            return source.Select<TSource, TResult>(variableName, expression);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate represented by an <seealso cref="Expression"/> string.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequence.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
        /// <param name="variableName">The name of the variable to be used in the expression, representing each element of the source sequence.</param>
        /// <param name="expression">An Expression representing the predicate to filter on.</param>
        /// <returns>An IEnumerable that contains elements from the input sequence that satisfy the condition specified by the predicate.</returns>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, string variableName, Expression expression)
        {
            if (source is null)
            {
                return Enumerable.Empty<T>();
            }
            return source.Where(x =>
            {
                expression.SetScopedVariable(variableName, x);
                return expression.Evaluate<bool>();
            });
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate represented by an <seealso cref="Expression"/> string.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequence.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
        /// <param name="variableName">The name of the variable to be used in the expression string, representing each element of the source sequence.</param>
        /// <param name="expressionStr">A string representing the <seealso cref="Expression"/> to filter on.</param>
        /// <param name="interpreter">An optional <seealso cref="IExpressionInterpreter"/> instance to use for parsing and evaluating the expression string. If null, a default <seealso cref="ExpressionInterpreter"/> will be created.</param>
        /// <returns>An IEnumerable that contains elements from the input sequence that satisfy the condition specified by the predicate.</returns>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, string variableName, string expressionStr, IExpressionInterpreter? interpreter = null)
        {
            if (source is null)
            {
                return Enumerable.Empty<T>();
            }
            if (interpreter is null)
            {
                interpreter = new ExpressionInterpreter();
            }
            interpreter.GlobalScope.RegisterVariableType(variableName, typeof(T));
            var expression = interpreter.GetExpression(expressionStr);
            return source.Where(variableName, expression);
        }

        #endregion Public Methods
    }
}