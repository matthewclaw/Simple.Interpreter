using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Simple.Interpreter;
using Simple.Interpreter.Ast.Interfaces;
using Simple.Interpreter.Ast.Nodes;
using Simple.Interpreter.Enums;
using Simple.Interpreter.Scoping;

namespace Simple.Interpreter.Ast
{
    public class ExpressionInterpreter : IExpressionInterpreter
    {
        #region Public Fields

        //public const string TokenPattern = @"((not\s)?in\s)|==|!=|>=|<=|>|<|and|or|(""[^""]*""|'[^']*'|\d+\.?\d*|\w+(\.\w+)*|\[|\]|[\,\+\-\*\/\(\)])";
       public const string TokenPattern = TokenRegex.COMPILED;

        #endregion Public Fields

        #region Public Properties

        public Scope GlobalScope { get; private set; }

        public Dictionary<string, Func<object[], object>> RegisteredFunctions { get; private set; }

        #endregion Public Properties

        #region Public Constructors

        public ExpressionInterpreter()
        {
            GlobalScope = new Scope();
            RegisteredFunctions = new Dictionary<string, Func<object[], object>>();
            RegisterFunction<object,object,object>("min", BasicExpressionFunctions.Min);
            RegisterFunction<object,object,object>("max", BasicExpressionFunctions.Max);
            RegisterFunction<string, string, bool>("startsWith", BasicExpressionFunctions.StartsWith);
            RegisterFunction<string, string, bool>("endsWith", BasicExpressionFunctions.EndsWith);
        }

        #endregion Public Constructors

        #region Public Methods

        public Expression GetExpression(string expression)
        {
            int tokenPosition = 0;
            expression = SanitizeExpression(expression);
            var tokens = Tokenize(expression);
            try
            {
                var expressionTree = ParseExpression(tokens, ref tokenPosition);
                if (tokenPosition != tokens.Count)
                {
                    throw new Exception("Mismatched parenthesis");
                }
                return new Expression(expressionTree, this);
            }
            catch (Exception ex)
            {
                if (tokenPosition == tokens.Count)
                {
                    tokenPosition--;
                }
                throw new ArgumentException($"Exception while parsing expression \"{expression}\" near token {tokenPosition} (\"{tokens[tokenPosition]}\") Error: {ex.Message}", ex);
            }
        }

        public void RegisterFunction<TResult>(string functionName, Func<TResult> implementation)
        {
            RegisteredFunctions[functionName] = array => implementation();
        }

        public void RegisterFunction<TParam, TResult>(string functionName, Func<TParam, TResult> implementation)
        {
            RegisteredFunctions[functionName] = array =>
            {
                CheckArgCount(1, array);
                return implementation((TParam)array[0]);
            };
        }

        public void RegisterFunction<TParam, TParam2, TResult>(string functionName, Func<TParam, TParam2, TResult> implementation)
        {
            RegisteredFunctions[functionName] = array =>
            {
                CheckArgCount(2, array);
                return implementation((TParam)array[0], (TParam2)array[1]);
            };
        }

        public void RegisterFunction<TParam, TParam2, TParam3, TResult>(string functionName, Func<TParam, TParam2, TParam3, TResult> implementation)
        {
            RegisteredFunctions[functionName] = array =>
            {
                CheckArgCount(3, array);
                return implementation((TParam)array[0], (TParam2)array[1], (TParam3)array[2]);
            };
        }

        public void RegisterFunction<TParam, TParam2, TParam3, TParam4, TResult>(string functionName, Func<TParam, TParam2, TParam3, TParam4, TResult> implementation)
        {
            RegisteredFunctions[functionName] = array =>
            {
                CheckArgCount(4, array);
                return implementation((TParam)array[0], (TParam2)array[1], (TParam3)array[2], (TParam4)array[3]);
            };
        }

        public void SetGlobalScope(Dictionary<string, object> scope)
        {
            GlobalScope.SetVariables(scope);
        }

        public bool Validate(string expression, out List<Exception> errors)
        {
            return Validate(expression, out _, out errors);
        }

        public bool Validate(string expression, out Expression? validExpression, out List<Exception> errors)
        {
            errors = new List<Exception>();
            if (!ValidateExpressionString(expression, out var parsed, out var error))
            {
                errors.Add(error);
                validExpression = null;
                return false;
            }
            if (!parsed.ValidateVariables(out var memberErrors))
            {
                errors.AddRange(memberErrors);
                validExpression = null;
                return false;
            }
            var ternaryNodes = parsed.GetChildren<TernaryNode>(true);
            if (ternaryNodes == null || ternaryNodes.Count == 0)
            {
                validExpression = parsed;
                return true;
            }
            if (!ValidateTernaryNodes(ternaryNodes, out var ternaryErrors))
            {
                errors.AddRange(ternaryErrors);
                validExpression = null;
                return false;
            }
            validExpression = parsed;
            return true;
        }

        public bool Validate(string expression, Dictionary<string, Type> validVariables, out List<Exception> errors)
        {
            errors = new List<Exception>();
            if (!ValidateExpressionString(expression, out var parsed, out var error))
            {
                errors.Add(error);
                return false;
            }
            if (!parsed.ValidateVariables(validVariables, out var memberErrors))
            {
                errors.AddRange(memberErrors);
                return false;
            }
            var ternaryNodes = parsed.GetChildren<TernaryNode>(true);
            if (ternaryNodes == null || ternaryNodes.Count == 0)
            {
                return true;
            }
            if (!ValidateTernaryNodes(ternaryNodes, validVariables, out var ternaryErrors))
            {
                errors.AddRange(ternaryErrors);
                return false;
            }
            return errors.Count == 0;
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckArgCount(int required, object[] args)
        {
            if (args.Length != required)
            {
                throw new ArgumentException($"Expected {required} arguments/parameters");
            }
        }

        /// <summary>
        /// Returns the precedence of a given operator.  Higher precedence operators are evaluated first.
        /// </summary>
        /// <param name="op">The operator to get the precedence for.</param>
        /// <returns>An integer representing the precedence of the operator. Higher values indicate higher precedence.</returns>
        private int GetPrecedence(string op)
        {
            switch (op)
            {
                case BinaryOperators.Or: return 1;
                case BinaryOperators.And: return 2;
                case BinaryOperators.In:
                case BinaryOperators.NotIn: return 3;
                case BinaryOperators.EqualTo:
                case BinaryOperators.NotEqualTo:
                case BinaryOperators.GreaterThan:
                case BinaryOperators.LessThan:
                case BinaryOperators.LessThanOrEqualTo: return 4;
                case BinaryOperators.Add:
                case BinaryOperators.Minus: return 5;
                case BinaryOperators.Multiply:
                case BinaryOperators.Divide: return 6;
                default: return 0;
            }
        }

        /// <summary>
        /// Parses a list of tokens into an expression tree, handling operator precedence and different token contexts.
        /// It recursively builds the tree based on the order of operations and token types.
        /// </summary>
        /// <param name="tokens">The list of tokens to parse.</param>
        /// <param name="position">A reference to the current position in the token list, updated as tokens are consumed.</param>
        /// <param name="tokenContext">The context in which the tokens are being parsed (e.g., argument of a function). Defaults to None.</param>
        /// <param name="precedence">The precedence of the current operator being parsed.  Defaults to 0.</param>
        /// <returns>The root node of the constructed expression tree, or null if no expression could be parsed.</returns>
        private ExpressionNode ParseExpression(List<string> tokens, ref int position, TokenContext tokenContext = TokenContext.None, int precedence = 0)
        {
            ExpressionNode left = ParsePrimary(tokens, ref position);

            if (left == null)
            {
                return null;
            }

            while (position < tokens.Count)
            {
                if (position < tokens.Count && tokens[position] == ")")
                {
                    if (tokenContext != TokenContext.Argument)
                    {
                        position++; // Consume ')'
                    }
                    return left;
                }
                else if (position < tokens.Count && tokens[position] == ",")
                {
                    return left;
                }

                string op = tokens[position];
                int opPrecedence = GetPrecedence(op);
                if (opPrecedence < precedence)
                {
                    break;
                }
                if (op.ToLower() == "if")
                {
                    left = ParseTunaryNode(left, tokens, ref position);
                    break;
                }
                position++;
                ExpressionNode right = ParseExpression(tokens, ref position, tokenContext, opPrecedence);
                if (right == null)
                {
                    break;
                }
                left = new BinaryOperationNode { Operator = op, Left = left, Right = right };
                if (position < tokens.Count && tokens[position].ToLower() == "if")
                {
                    left = ParseTunaryNode(left, tokens, ref position);
                    break;
                }
            }

            if (position < tokens.Count && tokens[position] == ")")
            {
                position++;
                return left;
            }

            return left;
        }

        /// <summary>
        /// Parses a function call from the token stream.  It expects the current token to be the function name,
        /// and the next token to be an opening parenthesis.  It then parses the arguments to the function, which
        /// are comma-separated expressions, until it encounters a closing parenthesis.
        /// If the function name contains a '.', it is treated as a method call on an object.  Otherwise, it is
        /// treated as a global function call.
        /// </summary>
        /// <param name="currentToken">The current token, which is expected to be the function name.</param>
        /// <param name="tokens">The list of tokens to parse.</param>
        /// <param name="position">A reference to the current position in the token list, updated as tokens are consumed.</param>
        /// <returns>A FunctionCallNode representing the parsed function call.</returns>
        private FunctionCallNode ParseFunctionCall(string currentToken, List<string> tokens, ref int position)
        {
            List<ExpressionNode> arguments = new List<ExpressionNode>();
            position++; // Consume '('

            if (position < tokens.Count && tokens[position] != ")")
            {
                arguments.Add(ParseExpression(tokens, ref position, TokenContext.Argument));
                while (position < tokens.Count && tokens[position] == ",")
                {
                    position++;
                    arguments.Add(ParseExpression(tokens, ref position, TokenContext.Argument));
                }
            }

            if (position >= tokens.Count || tokens[position] != ")")
            {
                throw new Exception("Mismatched parenthesis in function call");
            }
            position++; //Consume the ')'
            var parts = currentToken.Split('.');
            if (parts.Length == 1)
            {
                return new FunctionCallNode { Name = currentToken, Arguments = arguments };
            }
            return new FunctionCallNode { Parent = parts[0], Name = parts[1], Arguments = arguments };
        }

        /// <summary>
        /// Parses a list literal from the token stream. It determines the type of list (string, integer, or double)
        /// based on the types of the elements within the list. It supports comma-separated values within square brackets.
        /// If the list is empty or if the types are mixed and cannot be promoted, an exception is thrown.
        /// </summary>
        /// <param name="tokens">The list of tokens to parse.</param>
        /// <param name="position">A reference to the current position in the token list, updated as tokens are consumed.</param>
        /// <returns>A ListNodeBase representing the parsed list.</returns>
        private ListNodeBase ParseList(List<string> tokens, ref int position)
        {
            ListNodeBase list = null;
            var currentToken = tokens[position];
            if (currentToken == "]")
            {
                throw new ArgumentException($"Could not infer list type from empty list. Position {position}");
            }
            while (position < tokens.Count && tokens[position] != "]")
            {
                if (tokens[position].StartsWith("'") || tokens[position].StartsWith("\""))
                {
                    if (list?.Equals(null) ?? true)
                    {
                        list = new StringListNode();
                    }
                    list.Values.Add(tokens[position].Trim('\'', '"'));
                }
                else if (int.TryParse(tokens[position], out var intVal))
                {
                    if (list?.Equals(null) ?? true)
                    {
                        list = new IntListNode();
                    }
                    list.Values.Add(intVal);
                }
                else if (double.TryParse(tokens[position], out var doubleVal))
                {
                    if (list?.Equals(null) ?? true)
                    {
                        list = new DoubleListNode();
                    }
                    else if (list is IntListNode intList)
                    {
                        list = intList.PromoteToDouble();
                    }
                    list.Values.Add(doubleVal);
                }
                else
                {
                    throw new ArgumentException($"Could not parse token: {tokens[position]}. Position: {position}");
                }
                position++;
                if (position < tokens.Count && tokens[position] == ",")
                {
                    position++;
                }
            }
            position++;
            return list;
        }

        /// <summary>
        /// Parses a primary expression from the token stream. A primary expression can be a parenthesized expression,
        /// a number literal, a string literal, a list of strings, a function call, a member call, or an identifier.
        /// It determines the type of expression based on the current token and calls the appropriate parsing logic.
        /// </summary>
        /// <param name="tokens">The list of tokens to parse.</param>
        /// <param name="position">A reference to the current position in the token list, updated as tokens are consumed.</param>
        /// <returns>An ExpressionNode representing the parsed primary expression, or null if no primary expression could be parsed.</returns>
        private ExpressionNode ParsePrimary(List<string> tokens, ref int position)
        {
            if (position >= tokens.Count)
            {
                return null;
            }

            string currentToken = tokens[position];
            position++;

            if (currentToken == "(")
            {
                ExpressionNode innerExpression = ParseExpression(tokens, ref position);
                if (position > tokens.Count)
                {
                    throw new Exception("Mismatched parenthesis");
                }
                return innerExpression;
            }
            else if (currentToken.StartsWith("'") || currentToken.StartsWith("\""))
            {
                return new StringLiteralNode { Value = currentToken.Trim('\'', '"') };
            }
            else if (currentToken == "[")
            {
                return ParseList(tokens, ref position);
            }
            else if (position < tokens.Count && tokens[position] == "(") // Function Call Check
            {
                var parts = currentToken.Split('.');
                if (parts.Length > 2)
                {
                    position--;
                    throw new ArgumentException("Can only access members one level deep");
                }
                return ParseFunctionCall(currentToken, tokens, ref position);
            }
            else if (currentToken[0] == '-' || int.TryParse($"{currentToken[0]}", out _))
            {
                int modifier = currentToken[0] == '-' ? -1 : 1;
                if (modifier == -1)
                {
                    currentToken = tokens[position]; // Moving to the next token, which should be a number
                    position++;
                }
                if (int.TryParse(currentToken, out int intNumber))
                {
                    return new IntLiteralNode { Value = modifier * intNumber };
                }
                else if (double.TryParse(currentToken, out double number))
                {
                    return new DoubleLiteralNode { Value = modifier * number };
                }
            }
            else if (bool.TryParse(currentToken, out bool boolValue))
            {
                return new BoolLiteralNode { Value = boolValue };
            }
            else if (currentToken.Contains('.'))
            {
                var parts = currentToken.Split('.');
                if (parts.Length > 2)
                {
                    position--;
                    throw new ArgumentException("Can only access members one level deep");
                }
                return new MemberCallNode { Parent = parts[0], Name = parts[1] };
            }
            return new IdentifierNode { Name = currentToken };
        }

        private TernaryNode ParseTunaryNode(ExpressionNode truthyNode, List<string> tokens, ref int position)
        {
            var elseIndex = tokens.IndexOf("else");
            if (elseIndex == -1)
            {
                throw new ArgumentException("No 'else' found in expression");
            }
            var conditionExpressionLength = elseIndex - position - 1;
            var conditionExpressionRange = tokens.GetRange(position + 1, conditionExpressionLength);
            var conditionPosition = 0;
            var conditionNode = ParseExpression(conditionExpressionRange, ref conditionPosition);
            position = elseIndex + 1;
            var falsyNode = ParseExpression(tokens, ref position);
            return new TernaryNode(conditionNode, truthyNode, falsyNode);
        }

        /// <summary>
        /// Sanitizes the input expression string by replacing non-standard quotation marks with standard ones.
        /// This ensures that the expression parser can correctly interpret string literals enclosed in these quotation marks.
        /// </summary>
        /// <param name="expression">The expression string to sanitize.</param>
        /// <returns>The sanitized expression string with standard quotation marks.</returns>
        private string SanitizeExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return expression;
            }

            // Replace unique single quotes (e.g., ‘ and ’) with standard single quotes (')
            expression = Regex.Replace(expression, @"[‘’]", "'");

            // Replace unique double quotes (e.g., “ and ”) with standard double quotes (")
            expression = Regex.Replace(expression, @"[“”]", "\"");

            return expression;
        }

        private List<string> Tokenize(string expression)
        {
            List<string> tokens = new List<string>();
            foreach (Match match in Regex.Matches(expression, TokenPattern))
            {
                tokens.Add(match.Value.Trim());
            }
            return tokens;
        }

        /// <summary>
        /// Validates an expression string to ensure it can be parsed without errors.
        /// </summary>
        /// <param name="expressionString">The expression string to validate.</param>
        /// <param name="parsedExpression">The parsed expression if the expression is valid, else null</param>
        /// <param name="error">If the validation fails, this will contain the exception that occurred during parsing; otherwise, it will be null.</param>
        /// <returns>True if the expression is valid; otherwise, false.</returns>
        private bool ValidateExpressionString(string expressionString, out Expression parsedExpression, out Exception error)
        {
            try
            {
                parsedExpression = GetExpression(expressionString);
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                parsedExpression = null;
                return false;
            }
        }

        private bool ValidateTernaryNodes(List<TernaryNode> nodes, Dictionary<string, Type> validVariables, out List<Exception> errors)
        {
            errors = new List<Exception>();
            foreach (var node in nodes)
            {
                try
                {
                    var expression = new Expression(node, this);
                    expression.RegisterScopedVariableTypes(validVariables);
                    _ = expression.Evaluate();
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }
            return errors.Count == 0;
        }

        private bool ValidateTernaryNodes(List<TernaryNode> nodes, out List<Exception> errors)
        {
            errors = new List<Exception>();
            foreach (var node in nodes)
            {
                try
                {
                    var expression = new Expression(node, this);
                    _ = expression.Evaluate();
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }
            return errors.Count == 0;
        }

        #endregion Private Methods
    }
}