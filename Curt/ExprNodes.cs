using Tokenizer;
using static Tokenizer.TokType;
using static nodes.NodeType;
using nodes;

namespace nodes
{
    class Expr : Stmt
    {
        public virtual Delegate operation { get; }
    }
    class Unary : Expr
    {
        protected TokType type;
        public Expr right;
        protected int line;
        public override NodeType ntype => UNARY;
        public override Delegate operation
        {
            get
            {
                switch (type)
                {
                    case (NOT): return (Func<bool, bool>)(p1 => !p1);
                    case (MINUS): return (Func<int, int>)(p1 => -p1);
                    case (INCREMENT): return (Func<int, int>)(p1 => ++p1);
                    case (DECREMENT): return (Func<int, int>)(p1 => --p1);
                    default: throw new InvalidOperationException("Unexpected unary operator");
                }
            }
        }
        public object Execute(object right)
        {
            return operation.DynamicInvoke(right);
        }
    }
    class Negation : Unary
    {
        public Negation(int line, TokType op, Expr right)
        {
            base.line = line;
            base.type = op;
            base.right = right;
        }
    }
    class Step : Unary
    {
        public Step(int line, TokType op, Expr right)
        {
            base.line = line;
            base.type = op;
            base.right = right;
        }
    }
    class Binary : Expr
    {
        protected TokType type;
        public Expr left;
        public Expr right;
        protected int line;
        public override NodeType ntype => BINARY;
        public override Delegate operation
        {
            get
            {
                switch (type)
                {
                    case PLUS: return (Func<int, int, int>)((p1, p2) => p1 + p2);
                    case MINUS: return (Func<int, int, int>)((p1, p2) => p1 - p2);
                    case SLASH: return (Func<int, int, int>)((p1, p2) => p1 / p2);
                    case STAR: return (Func<int, int, int>)((p1, p2) => p1 * p2);
                    case PERCENT: return (Func<int, int, int>)((p1, p2) => p1 % p2);
                    case GREATER: return (Func<int, int, bool>)((p1, p2) => p1 > p2);
                    case GREATER_EQUAL: return (Func<int, int, bool>)((p1, p2) => p1 >= p2);
                    case LESS: return (Func<int, int, bool>)((p1, p2) => p1 < p2);
                    case LESS_EQUAL: return (Func<int, int, bool>)((p1, p2) => p1 <= p2);
                    case EQUAL_EQUAL: return (Func<int, int, bool>)((p1, p2) => p1 == p2);
                    case BANG_EQUAL: return (Func<int, int, bool>)((p1, p2) => p1 != p2);
                    case AND: return (Func<bool, bool, bool>)((p1, p2) => p1 && p2);
                    case OR: return (Func<bool, bool, bool>)((p1, p2) => p1 || p2);
                    default: throw new InvalidOperationException("Unexpected binary operator");
                }
            }
        }
        public object Execute(object left, object right)
        {
            return operation.DynamicInvoke(left, right);
        }
    }

    class Comparison : Binary
    {
        public Comparison(int line, TokType op, Expr left, Expr right)
        {
            base.line = line;
            base.type = op;
            base.left = left;
            base.right = right;
        }
    }
    class Arithmetic : Binary
    {
        public Arithmetic(int line, TokType op, Expr left, Expr right)
        {
            base.line = line;
            base.type = op;
            base.left = left;
            base.right = right;
        }
    }
    class Literal : Expr
    {
        protected int line;
        public override NodeType ntype => LITERAL;
        public virtual object value { get; }
    }
    class Identifier : Literal
    {
        public string name;
        public override NodeType ntype => NodeType.IDENTIFIER;
        public Identifier(string name, int line)
        {
            this.name = name;
            this.line = line;
        }
        public override Delegate operation
        {
            get
            {
                return (Func<Dictionary<string, object>, string, object>)((dict, name) => dict.TryGetValue(name, out object value) ? value : null);
            }
        }
    }
    class Number : Literal
    {
        private int _value;

        public Number(string value, int line)
        {
            _value = int.Parse(value);
            this.line = line;
        }

        public override object value => _value;
    }
    class String : Literal
    {
        private string _value;

        public String(string value, int line)
        {
            _value = value;
            this.line = line;
        }

        public override object value => _value;
    }
    class Boolean : Literal
    {
        private bool _value;

        public Boolean(string value, int line)
        {
            _value = value == "true";
            this.line = line;
        }

        public override object value => _value;
    }
    class Grouping : Literal
    {
        public Expr expression;
        public override NodeType ntype => GROUPING;
        public Grouping(Expr expression, int line)
        {
            this.expression = expression;
            base.line = line;
        }
    }
}