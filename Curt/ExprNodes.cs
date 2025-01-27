using Tokenizer;
using static Tokenizer.TokType;

namespace nodes
{
    class Expr : Stmt { }
    class Unary : Expr
    {
        protected TokType type;
        public Delegate operation;
        protected Expr? value;
        protected int line;
        public static object Execute(Unary node)
        {
            return node.operation.DynamicInvoke(node.value);
        }
    }
    class Negation : Unary
    {
        private static readonly Func<int, int> arithmeticNegation = (int p1) => -p1;
        private static readonly Func<bool, bool> logicalNegation = (bool p1) => !p1;
        public Negation(int line, TokType type, Expr value)
        {
            base.line = line;
            base.type = type;
            base.value = value;
            if (type == NOT)
            {
                base.operation = logicalNegation;
            }
            else if (type == MINUS)
            {
                base.operation = arithmeticNegation;
            }
            else
            {
                Curt.error(line, "Unexpected operand types in unary expression");
                Curt.hadSyntaxError = true;
                return;
            }
        }
    }
    class Step : Unary
    {
        private static readonly Func<int, int> increment = (int p1) => p1++;
        private static readonly Func<int, int> decrement = (int p1) => p1--;
        public Step(int line, TokType op, Expr value)
        {
            base.line = line;
            base.type = op;
            base.value = value;
            if (type == INCREMENT)
            {
                base.operation = increment;
            } else
            {
                base.operation = decrement;
            }
        }

    }
    class Binary : Expr
    {
        protected TokType type;
        public Delegate operation;
        protected object value1;
        protected object value2;
        protected int line;

        public static object Execute(Binary node)
        {
            return node.operation.DynamicInvoke(node.value1, node.value2);
        }
    }

    class Comparison : Binary
    {
        private static readonly Func<int, int, bool> gt = (int p1, int p2) => p1 > p2;
        private static readonly Func<int, int, bool> ge = (int p1, int p2) => p1 >= p2;
        private static readonly Func<int, int, bool> lt = (int p1, int p2) => p1 < p2;
        private static readonly Func<int, int, bool> le = (int p1, int p2) => p1 <= p2;
        private static readonly Func<int, int, bool> eq = (int p1, int p2) => p1 == p2;
        private static readonly Func<bool, bool, bool> and = (bool p1, bool p2) => p1 && p2;
        private static readonly Func<bool, bool, bool> or = (bool p1, bool p2) => p1 || p2;

        public Comparison(int line, TokType op, Expr value1, Expr value2)
        {
            base.line = line;
            base.type = op;
            base.value1 = value1;
            base.value2 = value2;
            switch (op) {
                case GREATER:
                    base.operation = gt;
                    break;
                case GREATER_EQUAL:
                    base.operation = ge;
                    break;
                case LESS:
                    base.operation = lt;
                    break;
                case LESS_EQUAL:
                    base.operation = le;
                    break;
                case EQUAL_EQUAL:
                    base.operation = eq;
                    break;
                case AND:
                    base.operation = and;
                    break;
                case OR:
                    base.operation = or;
                    break;
                default:
                    Curt.error(line, "Unexpected operator");
                    break;
            }
        }
    }
    class Arithmetic : Binary
    {
        private static readonly Func<int, int, int> add = (int p1, int p2) => p1 + p2;
        private static readonly Func<int, int, int> sub = (int p1, int p2) => p1 - p2;
        private static readonly Func<int, int, int> div = (int p1, int p2) => p1 / p2;
        private static readonly Func<int, int, int> mul = (int p1, int p2) => p1 * p2;
        private static readonly Func<int, int, int> mod = (int p1, int p2) => p1 % p2;


        public Arithmetic(int line, TokType op, Expr value1, Expr value2)
        {
            base.line = line;
            base.type = op;
            base.value1 = value1;
            base.value2 = value2;

            switch (op)
            {
                case PLUS:
                    base.operation = add;
                    break;
                case MINUS:
                    base.operation = sub;
                    break;
                case SLASH:
                    base.operation = div;
                    break;
                case STAR:
                    base.operation = mul;
                    break;
                case PERCENT:
                    base.operation = mod;
                    break;
                default:
                    Curt.error(line, "Unexpected operator");
                    break;
            }
        }
    }
    class Literal : Expr
    {
        protected object value;
        protected int line;
    }
    class Identifier : Literal
    {
        private string name;
        public Identifier(string name, int line)
        {
            this.name = name;
            this.line = line;
        }
    }
    class Number : Literal
    {
        public Number(string value, int line)
        {
            this.value = value;
            this.line = line;
        }
    }
    class String : Literal   
    {
        public String(string value, int line)
        {
            this.value = value;
            this.line = line;
        }
    }
    class Boolean : Literal
    {
        public Boolean(string value, int line)
        {
            this.value = value;
            this.line = line;
        }
    }
    class Grouping : Literal
    {
        private Expr expressions;
        public Grouping(Expr expression, int line)
        {
            this.expressions = expression;
            base.line = line;
        }
    }
}