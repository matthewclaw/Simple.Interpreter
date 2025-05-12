using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Interpreter.Ast.Nodes
{
    public class TurnaryNode : ExpressionNode
    {
        #region Public Properties

        public ExpressionNode Condition { get; set; }
        public ExpressionNode Falsy { get; set; }
        public ExpressionNode Truthy { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public TurnaryNode(ExpressionNode condition, ExpressionNode truthy, ExpressionNode falsy)
        {
            Condition = condition;
            Truthy = truthy;
            Falsy = falsy;
        }

        #endregion Public Constructors

        #region Public Methods

        public override List<ExpressionNode> GetChildren(bool recursive)
        {
            List<ExpressionNode> results = new List<ExpressionNode>();
            results.Add(Condition);
            if (recursive)
                results.AddRange(Condition.GetChildren(recursive));
            results.Add(Truthy);
            if (recursive)
                results.AddRange(Truthy.GetChildren(recursive));
            results.Add(Falsy);
            if (recursive)
                results.AddRange(Falsy.GetChildren(recursive));
            return results;
        }

        public override string GetCSharp()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"({Condition.GetCSharp()})? ");
            builder.Append($"{Truthy.GetCSharp()} : ");
            builder.Append(Falsy.GetCSharp());
            return builder.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Truthy.ToString());
            sb.Append(" if(");
            sb.Append(Condition.ToString());
            sb.Append(") else ");
            sb.Append(Falsy.ToString());
            return sb.ToString();
        }

        #endregion Public Methods
    }
}
