using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Ast.Nodes
{
    /// <summary>
    /// Defines constants for binary operators used in the interpreter.
    /// </summary>
    public static class BinaryOperators
    {
        #region Public Fields

        public const string Add = "+";
        public const string And = "and";
        public const string Divide = "/";
        public const string EqualTo = "==";
        public const string GreaterThan = ">";
        public const string GreaterThanOrEqualTo = ">=";
        public const string In = "in";
        public const string LessThan = "<";
        public const string LessThanOrEqualTo = "<=";
        public const string Minus = "-";
        public const string Multiply = "*";
        public const string NotEqualTo = "!=";
        public const string NotIn = "not in";
        public const string Or = "or";

        #endregion Public Fields
    }

    /// <summary>
    /// Represents a binary operation node in the abstract syntax tree.
    /// This node contains the left and right expressions, and the operator to apply.
    /// </summary>
    public class BinaryOperationNode : ExpressionNode
    {
        #region Public Fields

        public ExpressionNode Left;
        public string Operator;
        public ExpressionNode Right;

        #endregion Public Fields

        #region Public Methods

        public override List<ExpressionNode> GetChildren(bool recursive)
        {
            var result = new List<ExpressionNode>();
            result.Add(Left);
            if (recursive)
            {
                result.AddRange(GetChildrenFor(Left, true));
            }
            result.Add(Right);
            if (recursive)
            {
                result.AddRange(GetChildrenFor(Right, true));
            }
            return result;
        }

        public override string GetCSharp()
        {
            if (Right is ListNodeBase)
            {
                return BinaryListOperationCSharp();
            }
            var leftSide = Left.GetCSharp();
            var rightSide = Right.GetCSharp();
            if (Operator == BinaryOperators.And || Operator == BinaryOperators.Or)
            {
                // Wrapping in brackets for readability when dealing with condition expressions
                leftSide = $"({leftSide})";
                rightSide = $"({rightSide})";
            }
            return $"{leftSide} {GetCSharpOperator()} {rightSide}";
        }

        public override string ToString()
        {
            return $"{Left} {Operator} {Right}";
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Generates C# code for binary operations involving lists (e.g., 'in' or 'not in').
        /// Handles the specific case where the right-hand side of the operation is a list.
        /// </summary>
        /// <returns>A string representing the C# code for the binary list operation.</returns>
        private string BinaryListOperationCSharp()
        {
            var baseLine = $"({Right.GetCSharp()}).Contains({Left.GetCSharp()})";
            if (Operator == BinaryOperators.In) return baseLine;
            return $"!{baseLine}";
        }

        private string GetCSharpOperator()
        {
            switch (Operator)
            {
                case BinaryOperators.And:
                    return "&&";

                case BinaryOperators.Or:
                    return "||";
            }
            return Operator;
        }

        #endregion Private Methods
    }
}
