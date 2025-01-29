using nodes;
using System.Collections;
using static nodes.NodeType;

namespace Interpreting
{
    class Interpreter
    {
        List<Stmt> statements;
        Dictionary<string, object> globals = new Dictionary<string, object> ();
        public Interpreter(List<Stmt> statements)
        {
            this.statements = statements;
        }
        public object Resolve(Expr expr)
        {
            if (expr.ntype == BINARY)
            {
                Binary resolvedExpr = (Binary)expr;
                object leftResult = Resolve(resolvedExpr.left);
                object rightResult = Resolve(resolvedExpr.right);

                return resolvedExpr.operation.DynamicInvoke(leftResult, rightResult);

            } else if (expr.ntype == UNARY) {
                Unary resolvedExpr = (Unary)expr;
                object rightResult = Resolve(resolvedExpr.right);

                return resolvedExpr.operation.DynamicInvoke(rightResult);
            } else if (expr.ntype == GROUPING)
            {
                Grouping resolvedExpr = (Grouping)expr;

                return Resolve(resolvedExpr.expression);
            } else if (expr.ntype == LITERAL)
            {
                Literal resolvedExpr = (Literal)expr;
                return resolvedExpr.value;
            } else
            {
                throw new InvalidOperationException("Unsupported expression type");
            }
        }
    }
}