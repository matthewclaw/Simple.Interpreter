using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Ast.Nodes
{
    /// <summary>
    /// Represents a call to a function.
    /// within the AST.
    /// </summary>
    public class FunctionCallNode : MemberCallNode
    {
        #region Public Properties

        public List<ExpressionNode> Arguments { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override List<ExpressionNode> GetChildren(bool recursive)
        {
            var argumentChildren = new List<ExpressionNode>();
            foreach (var arg in Arguments)
            {
                argumentChildren.Add(arg);
                if (recursive)
                {
                    argumentChildren.AddRange(GetChildrenFor(arg, true));
                }
            }
            return argumentChildren;
        }

        public override string GetCSharp()
        {
            return $"{base.ToString()}({string.Join(", ", Arguments.Select(x => x.GetCSharp()))})";
        }

        public override string ToString()
        {
            return $"{base.ToString()}({string.Join(", ", Arguments.Select(x => x.ToString()))})";
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Represents a call to a member (property or field) of an object.
    /// within the AST.
    /// </summary>
    public class MemberCallNode : ExpressionNode
    {
        #region Public Properties

        public string Name { get; set; }
        public string Parent { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string GetCSharp() => ToString();

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Parent)) return Name;
            return $"{Parent}.{Name}";
        }

        #endregion Public Methods
    }
}
