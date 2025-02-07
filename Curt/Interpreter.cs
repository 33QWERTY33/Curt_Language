using nodes;
using System.Security.Principal;
using static nodes.NodeType;

namespace Interpreting
{
    class Interpreter
    {
        List<Stmt> statements;
        public static Dictionary<string, object> globals = new Dictionary<string, object> { };
        public Interpreter(List<Stmt> statements)
        {
            this.statements = statements;
        }

        public object? Interpret(List<Stmt> stmts)
        {
            foreach (Stmt stmt in stmts)
            {
                Interpret(stmt);
            }
            return null;
        }
        public object? Interpret(Stmt stmt)
        {
            switch (stmt.ntype)
            {
                case (ASSIGNMENT): return Resolver<Assignment>(stmt);
                case (IF): return Resolver<If>(stmt);
                case (BLOCK): return Resolver<Block>(stmt);
                case (FOR): return Resolver<For>(stmt);
                case (WHILE): return Resolver<While>(stmt);
                case (SHOW): return Resolver<Show>(stmt);
                case (FUNCTION): return Resolver<Function>(stmt);
                case (RETURN): return Resolver<Return>(stmt);
                default: return this.Evaluate((Expr)stmt);
            }
        }
        public object? Evaluate(Expr expr)
        {
            if (expr.ntype == BINARY)
            {
                Binary resolvedExpr = (Binary)expr;
                object leftResult = Evaluate(resolvedExpr.left);
                object rightResult = Evaluate(resolvedExpr.right);

                return resolvedExpr.operation.DynamicInvoke(leftResult, rightResult);
            } else if (expr.ntype == UNARY)
            {
                Unary resolvedExpr = (Unary)expr;
                object rightResult = Evaluate(resolvedExpr.right);

                return resolvedExpr.operation.DynamicInvoke(rightResult);
            } else if (expr.ntype == GROUPING)
            {
                Grouping resolvedExpr = (Grouping)expr;

                return Evaluate(resolvedExpr.expression);
            } else if (expr.ntype == LITERAL)
            {
                Literal resolvedExpr = (Literal)expr;
                return resolvedExpr.value;
            } else if (expr.ntype == IDENTIFIER)
            {
                Identifier variable = (Identifier)expr;
                object result = variable.operation.DynamicInvoke(globals, variable.name);
                if (result != null) return result;
                Curt.error(variable.line, $"The identifier \"{variable.name}\" is not defined");
                return null;
            } else if (expr.ntype == ASK) {
                Ask resolvedExpr = (Ask)expr;
                return resolvedExpr.Execute((string)Evaluate(resolvedExpr.value));
            } else if (expr.ntype == RANDINT)
            {
                Randint resolvedExpr = (Randint)expr;
                return resolvedExpr.Execute(Evaluate(resolvedExpr.value));
            } else if (expr.ntype == CALL)
            {
                Call resolvedExpr = (Call)expr;
                return resolvedExpr.Execute(this);
            }
            else
            {
                throw new InvalidOperationException("Unsupported expression type");
            }
        }

        public object? Resolver<T>(Stmt stmt) where T : Stmt
        {
            T resolvedStmt = (T)stmt;
            resolvedStmt.Execute(this);
            return null;
        }
    }
}