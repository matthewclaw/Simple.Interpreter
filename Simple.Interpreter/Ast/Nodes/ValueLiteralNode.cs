using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Ast.Nodes
{
    public class BoolLiteralNode : ValueLiteralNode<bool>
    {
        #region Public Methods

        public override string GetCSharp() => $"{Value}".ToLower();

        #endregion Public Methods
    }

    public class DoubleLiteralNode : ValueLiteralNode<double>
    {
        #region Public Methods

        public override string GetCSharp() => $"{Value}f".Replace(',', '.');

        #endregion Public Methods
    }

    public class IntLiteralNode : ValueLiteralNode<int>
    {
        #region Public Methods

        public override string GetCSharp() => $"{Value}";

        #endregion Public Methods
    }

    public class StringLiteralNode : ValueLiteralNode<string>
    {
        #region Public Methods

        public override string GetCSharp() => $"\"{Value}\"";
        public override string ToString() => $"\"{Value}\"";

        #endregion Public Methods
    }

    public abstract class ValueLiteralBase : ExpressionNode
    {
        #region Public Properties

        public abstract object Value { get; set; }
        public abstract Type ValueType { get; }

        #endregion Public Properties
    }

    [ExcludeFromCodeCoverage]
    public abstract class ValueLiteralNode<T> : ValueLiteralBase
    {
        #region Private Fields

        private T _value;

        #endregion Private Fields

        #region Public Properties

        public override object Value { get => _value; set => _value = (T)value; }
        public override Type ValueType => typeof(T);

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $"{Value}";
        }

        #endregion Public Methods
    }
}
