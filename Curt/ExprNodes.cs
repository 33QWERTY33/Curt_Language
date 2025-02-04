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
                    case (MINUS): return (Func<float, float>)(p1 => -p1);
                    case (INCREMENT): return (Func<float, float>)(p1 => ++p1);
                    case (DECREMENT): return (Func<float, float>)(p1 => --p1);
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
                    case PLUS: return (Func<object, object, object>)((p1, p2) => p1.GetType() == typeof(string) ? Concat((string)p1, (string)p2) : (float)p1 + (float)p2);
                    case MINUS: return (Func<float, float, float>)((p1, p2) => p1 - p2);
                    case SLASH: return (Func<float, float, float>)((p1, p2) => p1 / p2);
                    case STAR: return (Func<float, float, float>)((p1, p2) => p1 * p2);
                    case PERCENT: return (Func<float, float, float>)((p1, p2) => p1 % p2);
                    case GREATER: return (Func<float, float, bool>)((p1, p2) => p1 > p2);
                    case GREATER_EQUAL: return (Func<float, float, bool>)((p1, p2) => p1 >= p2);
                    case LESS: return (Func<float, float, bool>)((p1, p2) => p1 < p2);
                    case LESS_EQUAL: return (Func<float, float, bool>)((p1, p2) => p1 <= p2);
                    case EQUAL_EQUAL: return (Func<object, object, bool>)((p1, p2) => p1.GetType() == typeof(string) ? strEquals((string)p1, (string)p2) : p1.Equals(p2));
                    case BANG_EQUAL: return (Func<object, object, bool>)((p1, p2) => p1.GetType() == typeof(string) ? strEquals((string)p1, (string)p2) : !p1.Equals(p2));
                    case AND: return (Func<bool, bool, bool>)((p1, p2) => p1 && p2);
                    case OR: return (Func<bool, bool, bool>)((p1, p2) => p1 || p2);
                    default: throw new InvalidOperationException("Unexpected binary operator");
                }
            }
        }
        private string Concat(string s1, string s2)
        {
            return s1 + s2;
        }
        private bool strEquals(string s1, string s2)
        {
            return s1 == s2;
        }
        private bool strNotEquals(string s1, string s2)
        {
            return !strEquals(s1, s2);
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
        private float _value;

        public Number(string value, int line)
        {
            _value = float.Parse(value);
            this.line = line;
        }

        public override object value => _value;
    }
    class String : Literal
    {
        private string _value;

        public String(string value, int line)
        {
            _value = value.Substring(1, value.Length - 2);
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

class Ask : Expr
{
    public Expr value;
    public override NodeType ntype => NodeType.ASK;
    public Ask(Expr value)
    {
        this.value = value;
    }

    public string Execute(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
}
class Randint : Expr
{
    public Expr value;
    public override NodeType ntype => NodeType.RANDINT;
    public Randint(Expr value) 
    {
        this.value = value;
    }

    public float Execute(object range)
    {
        Random random = new Random();
        return (float)random.Next(Convert.ToInt32(range));
    }
}