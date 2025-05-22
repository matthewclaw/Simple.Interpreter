using Microsoft.Extensions.Logging;
using Simple.Interpreter.Ast.Nodes;
using Simple.Interpreter.Enums;
using Simple.Interpreter.Extensions;
using Simple.Interpreter.Scoping;
using System.Reflection;

namespace Simple.Interpreter.Ast
{
    /// <summary>
    /// Represents an expression that can be parsed and evaluated within the pseudo-code interpreter.
    /// It handles tokenization, AST construction, scope management, and evaluation of the expression.
    /// </summary>
    public class Expression
    {
        #region Public Fields

        public readonly ExpressionNode Tree;

        #endregion Public Fields

        #region Private Fields

        private readonly ExpressionInterpreter _interpreter;
        private ILogger? _logger;
        private bool _loggingEnable = true;
        private Scope _scope;

        #endregion Private Fields

        #region Public Constructors

        public Expression(ExpressionNode tree, ExpressionInterpreter interpreter, ILoggerFactory? loggerFactory) : this(tree, interpreter)
        {
            _logger = loggerFactory?.CreateLogger<Expression>();
        }

        public Expression(ExpressionNode tree, ExpressionInterpreter interpreter)
        {
            _scope = new Scope(interpreter.GlobalScope);
            _interpreter = interpreter;
            Tree = tree;
        }

        public Expression WithDebugging(bool enable)
        {
            if (_logger is null)
            {
                throw new InvalidOperationException($"No Logger was found, please use {nameof(WithDebugging)}(ILoggerFactory, bool)");
            }
            _loggingEnable = enable;
            return this;
        }

        public Expression WithDebugging(ILoggerFactory loggerFactory, bool enable)
        {
            _logger = loggerFactory.CreateLogger<Expression>();
            return WithDebugging(enable);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Evaluates the expression tree and returns the result.
        /// </summary>
        /// <returns>The result of evaluating the expression tree.</returns>
        public object Evaluate()
        {
            Log("Evaluating Expression: {expression}...", Tree);
            var result = EvaluateExpressionNode(Tree);
            Log("Expression Evaluated: {result}", result);
            return result;
        }

        /// <summary>
        /// Retrieves a list of child nodes of the expression tree of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of ExpressionNode to filter for.</typeparam>
        /// <param name="recursive">Whether to recursively search through all descendants or only direct children.</param>
        /// <returns>A list of ExpressionNodes of type T found within the expression tree.</returns>
        public List<T> GetChildren<T>(bool recursive) where T : ExpressionNode
        {
            List<T> results = new List<T>();
            if (Tree is T cast)
            {
                results.Add(cast);
            }
            if (recursive)
            {
                results.AddRange(Tree.GetChildren<T>(recursive));
            }
            return results;
        }

        public string GetCSharp()
        {
            return Tree.GetCSharp();
        }

        /// <summary>
        /// Registers variable names and their respective types without assigning a value. This is useful for declaring variables before assigning values, or for expression validation
        /// </summary>
        /// <param name="validVariables">A dictionary of valid variables and their corresponding types to register in the scope before validation.</param>
        public void RegisterScopedVariableTypes(Dictionary<string, Type> validVariables)
        {
            foreach (var variable in validVariables)
            {
                _scope.RegisterVariableType(variable.Key, variable.Value);
            }
        }

        /// <summary>
        /// Sets the current scope of the expression evaluator using a dictionary of variable names and their corresponding values.
        /// </summary>
        /// <param name="scope">A dictionary representing the new scope, where keys are variable names and values are the variable values.</param>
        public void SetScope(Dictionary<string, object> scope)
        {
            _scope.SetVariables(scope);
        }

        /// <summary>
        /// Sets the value of a variable in the current scope.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value to assign to the variable.</param>
        public void SetScopedVariable(string name, object value)
        {
            _scope.SetVariable(name, value);
        }

        public override string ToString()
        {
            return Tree.ToString();
        }

        /// <summary>
        /// Validates the variable reference, member calls and function calls within the expression tree, using a dictionary of valid variables and their types for context.
        /// It registers the valid variables in the scope and then attempts to evaluate each member and function call node, catching any exceptions that occur during evaluation.
        /// If any exceptions are caught, they are added to the errors list.
        /// </summary>
        /// <param name="validVariables">A dictionary of valid variables and their corresponding types to register in the scope before validation.</param>
        /// <param name="errors">An output parameter that will contain a list of exceptions encountered during validation.</param>
        /// <returns>True if all member and function calls are valid (no exceptions were thrown), false otherwise.</returns>
        public bool ValidateVariables(Dictionary<string, Type> validVariables, out List<Exception> errors)
        {
            RegisterScopedVariableTypes(validVariables);
            return ValidateVariables(out errors);
        }

        private void Log(string message, params object[] args)
        {
            if (!_loggingEnable || _scope.IsValidationScope)
            {
                return;
            }
            _logger?.LogDebug(message, args);
        }

        /// <summary>
        /// Validates the variable reference, member calls and function calls within the expression tree.
        /// It attempts to evaluate each member and function call node and catches any exceptions that occur during evaluation.
        /// If any exceptions are caught, they are added to the errors list.
        /// </summary>
        /// <param name="errors">An output parameter that will contain a list of exceptions encountered during validation.</param>
        /// <returns>True if all member and function calls are valid (no exceptions were thrown), false otherwise.</returns>
        public bool ValidateVariables(out List<Exception> errors)
        {
            var previousLoggingEnable = _loggingEnable;
            _loggingEnable = false;
            errors = new List<Exception>();
            var nodes = Tree.GetChildren(true);
            nodes.Insert(0, Tree); // This is to ensure the top level node is also tested
            var memberAndFunctionNodes = nodes
                .Where(child => child is MemberCallNode || child is FunctionCallNode || child is IdentifierNode);
            foreach (var member in memberAndFunctionNodes)
            {
                try
                {
                    _ = EvaluateExpressionNode(member);
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }
            _loggingEnable = previousLoggingEnable;
            return errors.Count == 0;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Evaluates a binary operation based on the operator and the left and right operands.
        /// Supports arithmetic, comparison, and logical operations on doubles, booleans, and strings.
        /// Also supports the 'in' operator for checking if a string is present in a list of strings.
        /// Returns the result of the operation as an object.
        /// </summary>
        /// <param name="op">The operator to evaluate.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result of the binary operation, or null if the operation is not supported.</returns>
        private object EvaluateBinaryOperation(string op, object left, object right)
        {
            Log("Evaluating: {left} {operator} {right}...", left, op, right);
            if (left is bool leftBool && right is bool rightBool)
            {
                switch (op)
                {
                    case BinaryOperators.And: return leftBool && rightBool;
                    case BinaryOperators.Or: return leftBool || rightBool;
                    default: return null;
                }
            }
            else if (left is string leftString && right is string rightString)
            {
                switch (op)
                {
                    case BinaryOperators.EqualTo: return leftString == rightString;
                    case BinaryOperators.NotEqualTo: return leftString != rightString;
                    case BinaryOperators.Add: return leftString + rightString;
                    default: return null;
                }
            }
            else if ((left is double || left is int) || (right is double || right is int))
            {
                var leftDouble = Convert.ToDouble(left);
                var rightDouble = Convert.ToDouble(right);
                double arithmeticResult = 0.0;
                bool bothInts = left is int && right is int;
                switch (op)
                {
                    case BinaryOperators.Add: arithmeticResult = leftDouble + rightDouble; break;
                    case BinaryOperators.Minus: arithmeticResult = leftDouble - rightDouble; break;
                    case BinaryOperators.Multiply: arithmeticResult = leftDouble * rightDouble; break;
                    case BinaryOperators.Divide: arithmeticResult = leftDouble / rightDouble; break;
                    case BinaryOperators.EqualTo: return leftDouble == rightDouble;
                    case BinaryOperators.NotEqualTo: return leftDouble != rightDouble;
                    case BinaryOperators.GreaterThan: return leftDouble > rightDouble;
                    case BinaryOperators.LessThan: return leftDouble < rightDouble;
                    case BinaryOperators.GreaterThanOrEqualTo: return leftDouble >= rightDouble;
                    case BinaryOperators.LessThanOrEqualTo: return leftDouble <= rightDouble;
                    default: return null;
                }
                if (bothInts)
                {
                    return Convert.ToInt32(arithmeticResult);
                }
                return arithmeticResult;
            }
            return null;
        }

        /// <summary>
        /// Evaluates an expression node based on its type and returns the result.
        /// Supports binary operations, string literals, number literals, identifiers, string lists, function calls, and member calls.
        /// </summary>
        /// <param name="node">The expression node to evaluate.</param>
        /// <returns>The result of evaluating the expression node, or null if the node type is not supported.</returns>
        private object EvaluateExpressionNode(ExpressionNode node)
        {
            try
            {
                if (node is BinaryOperationNode binaryNode)
                {
                    Log("Evaluating Binary Node: {node}...", node);
                    object left = EvaluateExpressionNode(binaryNode.Left);
                    if (binaryNode.Right is ListNodeBase rightList)
                    {
                        return EvaluationBinaryListOperation(left, binaryNode.Operator, rightList);
                    }
                    object right = EvaluateExpressionNode(binaryNode.Right);
                    var result = EvaluateBinaryOperation(binaryNode.Operator, left, right);
                    Log("Evaluated: {result}", result);
                    return result;
                }
                else if (node is ValueLiteralBase valueNode)
                {
                    return valueNode.Value;
                }
                else if (node is IdentifierNode identifierNode)
                {
                    Log("Evaluating Identifier Node: {node}...", node);
                    if (_scope.TryGetVariable(identifierNode.Name, out var variable))
                    {
                        var result = variable.Value;
                        if (result is not null)
                        {
                            Log("Evaluated: {result}", result);
                        }
                        return result;
                    }
                    else
                    {
                        throw new ArgumentException($"'{identifierNode.Name}' is not a valid variable in scope");
                    }
                }
                else if (node is StringListNode listNode)
                {
                    Log("Evaluating String List Node: {node}...", node);
                    return listNode.Values;
                }
                else if (node is FunctionCallNode functionCallNode)
                {
                    Log("Evaluating Function Call Node: {node}...", node);
                    var result = EvaluateFunctionCallNode(functionCallNode);
                    if (result is not null)
                    {
                        Log("Function Call Evaluated: {result}", result);
                    }
                    return result;
                }
                else if (node is MemberCallNode memberCallNode)
                {
                    Log("Evaluating Member Node: {node}...", node);
                    var result = EvaluateMemberCallNode(memberCallNode);
                    if (result is not null)
                    {
                        Log("Member Evaluated: {result}", result);
                    }
                    return result;
                }
                else if (node is TernaryNode ternaryNode)
                {
                    Log("Evaluating Ternary Node: {node}...", node);
                    var result = EvaluateTernaryNode(ternaryNode);
                    if (result is not null)
                    {
                        Log("Ternary Evaluated: {result}", result);
                    }
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Exception thrown for expression part `{node}`. Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Evaluates a function call node, which can represent either a global function call or a method call on an object.
        /// If the function is a global function registered with the interpreter, it retrieves the corresponding delegate and invokes it with the evaluated arguments.
        /// If the function is a method call, it retrieves the context object from the scope and invokes the method on that object with the evaluated arguments.
        /// </summary>
        /// <param name="node">The FunctionCallNode representing the function call to evaluate.</param>
        /// <returns>The result of the function call.</returns>
        private object EvaluateFunctionCallNode(FunctionCallNode node)
        {
            object[] arguments = node.Arguments.ConvertAll(EvaluateExpressionNode).ToArray();
            if (string.IsNullOrEmpty(node.Parent))
            {
                if (!_interpreter.RegisteredFunctions.TryGetValue(node.Name, out var funcDelegate))
                {
                    if (node.Name == "if")
                    {
                        throw new ArgumentException("Invalid format for tunary statement. Expecting: `<Truthy Expression> if(<Condition>) else <Falsy Expression>`");
                    }
                    throw new ArgumentException($"'{node.Name}' is not a registered function");
                }
                Log("Evaluating Registered Function: {node}...", node);
                return funcDelegate(arguments);
            }
            if (!_scope.TryGetVariable(node.Parent, out var contextObject))
            {
                throw new ArgumentException($"'{node.Parent}' is not a valid variable in scope");
            }
            if (!contextObject.MemberMap.TryGetMethodMember(node.Name, arguments, out var contextMember))
            {
                throw new ArgumentException($"Function/Method '{node.Name}' accepting [{String.Join(", ", arguments.Select(a => a.GetType()))}] was not found in {node.Parent}.");
            }
            if ((contextObject.Value?.Equals(null) ?? true) || contextObject.Value.Equals(default))
            {
                return null;
            }
            Log("Attempting to invoke {method}...", node.ToString(arguments));
            var callSuccessful = contextMember!.TryInvoke(contextObject.Value, out var callResult, arguments);
            if (!callSuccessful)
            {
                return null;
            }
            return callResult;
        }

        /// <summary>
        /// Evaluates a member call node, which represents accessing a property or field of an object in the scope.
        /// It retrieves the context object from the scope using the Parent property of the node.
        /// Then, it retrieves the member (property or field) using the Name property of the node.
        /// Finally, it returns the value of the property or field from the context object.
        /// </summary>
        /// <param name="node">The MemberCallNode representing the member access to evaluate.</param>
        /// <returns>The value of the accessed member.</returns>
        private object EvaluateMemberCallNode(MemberCallNode node)
        {
            if (!_scope.TryGetVariable(node.Parent, out var contextObject))
            {
                throw new ArgumentException($"Object '{node.Parent}' not found in scope.");
            }
            if (!contextObject.MemberMap.TryGetMember(node.Name, out var contextMember))
            {
                throw new ArgumentException($"Member '{node.Name}' not found in {node.Parent}.");
            }
            if (contextObject.Value?.Equals(null) ?? true)
            {
                return null;
            }
            if (contextMember!.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)contextMember)!.GetValue(contextObject.Value);
            }
            else if (contextMember.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)contextMember).GetValue(contextObject.Value);
            }
            throw new ArgumentException($"Unsupported MemberType: {contextMember.MemberType}");
        }

        private object EvaluateTernaryNode(TernaryNode node)
        {
            Log("Evaluating Ternary Condition: {node}...", node.Condition);
            var conditionResultRaw = EvaluateExpressionNode(node.Condition);
            bool? conditionResult = false;
            if (conditionResultRaw != null && !(conditionResultRaw is bool))
            {
                throw new ArgumentException("If condition must return a boolean value");
            }
            conditionResult = conditionResultRaw as bool?;
            if (conditionResult ?? false)
            {
                Log("Condition met, evaluating Truthy Node: {node}", node.Truthy);
                return EvaluateExpressionNode(node.Truthy);
            }
            Log("Condition not met, evaluating Falsy Node: {node}", node.Falsy);
            return EvaluateExpressionNode(node.Falsy);
        }

        /// <summary>
        /// Evaluates a binary operation where the right operand is a list.
        /// Currently, only supports the 'in' and 'not in' operators for checking if the left operand is present in the list.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="op">The operator ('in' or 'not in').</param>
        /// <param name="rightList">The list node (right operand).</param>
        /// <returns>True if the left operand is in the list (for 'in'), or false if it is not in the list (for 'not in').</returns>
        private bool EvaluationBinaryListOperation(object left, string op, ListNodeBase rightList)
        {
            if (left.GetType() != rightList.ItemType)
            {
                throw new InvalidOperationException($"Left node value type '{left.GetType().FullName}' does not match the Right node value type '{rightList.ItemType}'");
            }
            return op == BinaryOperators.In ? rightList.Values.Contains(left) : !rightList.Values.Contains(left);
        }

        #endregion Private Methods
    }
}