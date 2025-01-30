using nodes;
using System.Collections;
using System.Security.Principal;
using static nodes.NodeType;

namespace Interpreting
{
    class Interpreter
    {
        List<Stmt> statements;
        public static Dictionary<string, object> globals = new Dictionary<string, object>
        {
            {"x", 10}
        };
        public Interpreter(List<Stmt> statements)
        {
            this.statements = statements;
        }

        public object Interpret(Stmt stmt)
        {
            if (stmt.ntype == ASSIGNMENT)
            {
                Assignment resolvedStmt = (Assignment)stmt;
                resolvedStmt.Execute(this);
                return null;
            } else
            {
                return this.Evaluate((Expr)stmt);
            }
        }
        public object Evaluate(Expr expr)
        {
            if (expr.ntype == BINARY)
            {
                Binary resolvedExpr = (Binary)expr;
                object leftResult = Evaluate(resolvedExpr.left);
                object rightResult = Evaluate(resolvedExpr.right);

                return resolvedExpr.operation.DynamicInvoke(leftResult, rightResult);
            }
            else if (expr.ntype == UNARY)
            {
                Unary resolvedExpr = (Unary)expr;
                object rightResult = Evaluate(resolvedExpr.right);

                return resolvedExpr.operation.DynamicInvoke(rightResult);
            }
            else if (expr.ntype == GROUPING)
            {
                Grouping resolvedExpr = (Grouping)expr;

                return Evaluate(resolvedExpr.expression);
            }
            else if (expr.ntype == LITERAL)
            {
                Literal resolvedExpr = (Literal)expr;
                return resolvedExpr.value;
            }
            else if (expr.ntype == IDENTIFIER) 
            {
                Identifier variable = (Identifier)expr;
                object result = variable.operation.DynamicInvoke(globals, variable.name);
                if (result != null) return result;
                throw new IdentityNotMappedException($"The variable: {variable.name} is not defined");
            } else
            {
                throw new InvalidOperationException("Unsupported expression type");
            }
        }
    }
}