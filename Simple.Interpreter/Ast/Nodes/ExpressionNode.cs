using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Ast.Nodes
{
    public abstract class ExpressionNode
    {
        #region Public Methods
        /// <summary>
        /// Retrieves a list of child nodes.
        /// </summary>
        /// <param name="recursive">A boolean value indicating whether to recursively search for child nodes.</param>
        /// <returns>A list of child nodes.</returns>
        public virtual List<ExpressionNode> GetChildren(bool recursive)
        {
            return new List<ExpressionNode>();
        }

        /// <summary>
        /// Retrieves a list of child nodes of a specific type.
        /// </summary>
        /// <typeparam name="TChild">The type of child nodes to retrieve.</typeparam>
        /// <param name="recursive">A boolean value indicating whether to recursively search for child nodes.</param>
        /// <returns>A list of child nodes of the specified type.</returns>
        public virtual List<TChild> GetChildren<TChild>(bool recursive) where TChild : ExpressionNode
        {
            var children = GetChildren(recursive);
            return children.Where(child => child is TChild).Cast<TChild>().ToList();
        }

        /// <summary>
        /// Generates a C# representation of the node.
        /// </summary>
        /// <returns>A string containing C# code.</returns>
        public abstract string GetCSharp();

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Retrieves a list of child nodes for a given expression node, optionally recursively.
        /// </summary>
        /// <typeparam name="T">The type of the expression node.</typeparam>
        /// <param name="node">The expression node to retrieve children for.</param>
        /// <param name="recursive">True to recursively retrieve children; otherwise, false.</param>
        /// <returns>A list of child expression nodes.</returns>
        protected virtual List<ExpressionNode> GetChildrenFor<T>(T node, bool recursive) where T : ExpressionNode
        {
            var firstLayer = node.GetChildren(false);
            if (!recursive)
            {
                return firstLayer;
            }
            var fullList = firstLayer.Select(x => x).ToList();
            foreach (var child in firstLayer)
            {
                var children = GetChildrenFor(child, true);
                fullList.AddRange(children);
            }
            return fullList;
        }

        #endregion Protected Methods
    }
}
