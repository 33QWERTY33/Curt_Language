using nodes;
using System.Security.Principal;
using static nodes.NodeType;
using StdLib;

namespace Interpreting
{
    class RTE : Exception
    {
        public string location;
        public string reason;
        public RTE(string location, string reason)
        {
            this.location = location;
            this.reason = reason;
        }
    }

    class Interpreter
    {
        List<Stmt> statements;
        public static Dictionary<string, object> globals = new Dictionary<string, object> {
            {"ascii",  new Native(Lib.ascii, 1)},
            {"abs",  new Native(Lib.abs, 1)},
            {"ask",  new Native(Lib.ask, 1)},
            {"pow",  new Native(Lib.pow, 2)},
            {"rand",  new Native(Lib.rand, 1)},
            {"show",  new Native(Lib.show, 1)},
            {"sqrt",  new Native(Lib.sqrt, 1)},
            {"locals",  new Native(Lib.locals, 0)},
        };

        public static bool runTimeErrorOccurred;

        public Interpreter(List<Stmt> statements)
        {
            this.statements = statements;
        }

        public object? Interpret(List<Stmt> stmts)
        {
            foreach (Stmt stmt in stmts)
            {
                if (runTimeErrorOccurred) { return null; }
                try
                {
                    Interpret(stmt);
                } catch (RTE e)
                {
                    Console.WriteLine($"[Run Time Error] Error in: {e.location} because: {e.reason}.");
                    runTimeErrorOccurred = true;
                } 
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
            } else if (expr.ntype == CALL)
            {
                Call resolvedExpr = (Call)expr;
                return resolvedExpr.Execute(this);
            }
            else
            {
                Curt.error(-1, "Congrats! You broke my expression evaluator");
                return null;
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