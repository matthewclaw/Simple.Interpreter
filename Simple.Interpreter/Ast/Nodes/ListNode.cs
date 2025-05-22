using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Ast.Nodes
{
    public class DoubleListNode : ListNode<double>
    {
        #region Protected Methods

        protected override string CSharpConstructorType() => "double";

        protected override string CSharpItemFormat(double item) => $"{item}f".Replace(',','.');

        #endregion Protected Methods
    }

    public class IntListNode : ListNode<int>
    {
        #region Public Methods

        /// <summary>
        /// Promotes the integer list to a double list by converting each integer to a double.
        /// </summary>
        /// <returns>A new DoubleListNode containing the converted values.</returns>
        public DoubleListNode PromoteToDouble()
        {
            var result = new DoubleListNode();
            foreach (var item in Values)
            {
                result.Values.Add(Convert.ToDouble(item));
            }
            return result;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override string CSharpConstructorType() => "int";

        protected override string CSharpItemFormat(int item) => $"{item}";

        #endregion Protected Methods
    }

    /// <summary>
    /// Represents a list node containing elements of a specific type.
    /// Provides generic implementation for list operations.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the list.</typeparam>
    public abstract class ListNode<T> : ListNodeBase
    {
        #region Private Fields

        private List<T> _values = new List<T>();

        #endregion Private Fields

        #region Public Properties

        public override Type ItemType => typeof(T);
        public override IList Values { get => _values; set => _values = (List<T>)value; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Generates a C# representation of the list.
        /// </summary>
        /// <returns>A string containing C# code that represents the list.</returns>
        public override string GetCSharp()
        {
            return $"new {CSharpConstructorType()}[] {{ {string.Join(", ", _values.Select(CSharpItemFormat))} }}";
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"[{string.Join(", ", _values)}]";
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract string CSharpConstructorType();

        protected abstract string CSharpItemFormat(T item);

        #endregion Protected Methods
    }

    [ExcludeFromCodeCoverage]
    /// <summary>
    /// Abstract base class for list nodes.  Provides common functionality
    /// for handling lists of various data types within the AST.
    /// </summary>
    public abstract class ListNodeBase : ExpressionNode
    {
        #region Public Properties

        public abstract Type ItemType { get; }
        public abstract System.Collections.IList Values { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $"[{string.Join(", ", Values)}]";
        }

        #endregion Public Methods
    }

    public class StringListNode : ListNode<string>
    {
        #region Protected Methods

        protected override string CSharpConstructorType() => "string";

        protected override string CSharpItemFormat(string item) => $"\"{item}\"";

        #endregion Protected Methods
    }
}
