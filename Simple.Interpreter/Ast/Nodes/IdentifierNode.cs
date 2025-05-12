using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Ast.Nodes
{
    /// <summary>
    /// Represents an identifier (variable name) in the abstract syntax tree.
    /// </summary>
    public class IdentifierNode : ExpressionNode
    {
        #region Public Fields

        public string Name;

        #endregion Public Fields

        #region Public Methods

        public override string GetCSharp() => Name;

        public override string ToString()
        {
            return Name;
        }

        #endregion Public Methods
    }
}
