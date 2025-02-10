using Tokenizer;
using static Tokenizer.TokType;
using static nodes.NodeType;
using nodes;
using Interpreting;
using System.Reflection;
using System;

namespace nodes
{
    class Expr : Stmt
    {
        public virtual Delegate operation { get; }
    }

    class Call : Expr
    {
        protected int line;
        protected string funcName;
        private List<Expr> args;

        public override NodeType ntype => NodeType.CALL;
        public Call(int line, string funcName, List<Expr> args)
        {
            this.line = line;
            this.funcName = funcName;
            this.args = args ?? new List<Expr>();
        }

        public object? Execute(Interpreter interpreter)
        {
            if (Interpreter.globals.TryGetValue(funcName, out object result) != false)
            {
                Function functionDef = (Function)Interpreter.globals[funcName];

                if (args.Count == functionDef.paramCount)
                {
                    if (functionDef.native) return NativeFuncExec(interpreter, (Native)functionDef);
                    else { return UserFuncExec(interpreter, functionDef); }
                }
                else { Curt.error(line, $"parameter count and argument count mismatched. Check the function signature."); return null; }
            }
            else { Curt.error(line, $"Undefined function call for {funcName} attempted."); return null; }
        }

        public object? NativeFuncExec(Interpreter interpreter, Native native)
        {
            int numParams = native.func.GetMethodInfo().GetParameters().Length;

            // Extra ugly switch statement because Dynamic Invoke swallows exceptions and halts execution (which means I can't handle them)
            // this means I have to deal with strong typing restrictions and manual type conversions which is hard to make "DRY" because of Invoke method
            // also it messes with my call stack :(
            switch (numParams)
            {
                case 0:
                    Func<object?> func1 = (Func<object?>)native.func;
                    object result1 = func1.Invoke();
                    if (result1 is string) return (string)result1;
                    else if (result1 is bool) return (bool)result1;
                    else if (result1 is float) return (float)result1;
                    else if (result1 is null) return null;
                    else { throw new RTE("native function call", "Invalid type returned from native function"); }
                case 1:
                    Func<object?, object?> func2 = (Func<object?, object?>)native.func;
                    object result2 = func2.Invoke(interpreter.Interpret(args[0]));
                    if (result2 is string) return (string)result2;
                    else if (result2 is bool) return (bool)result2;
                    else if (result2 is float) return (float)result2;
                    else if (result2 is null) return null;
                    else { throw new RTE("native function call", "Invalid type returned from native function"); }
                case 2:
                    Func<object?, object?, object?> func3 = (Func<object?, object?, object?>)native.func;
                    object result3 = func3.Invoke(interpreter.Interpret(args[0]), interpreter.Interpret(args[1]));
                    if (result3 is string) return (string)result3;
                    else if (result3 is bool) return (bool)result3;
                    else if (result3 is float) return (float)result3;
                    else if (result3 is null) return null;
                    else { throw new RTE("native function call", "Invalid type returned from native function"); }
                case 3:
                    Func<object?, object?, object?, object?> func4 = (Func<object?, object?, object?, object?>)native.func;
                    object result4 = func4.Invoke(interpreter.Interpret(args[0]), interpreter.Interpret(args[1]), interpreter.Interpret(args[2]));
                    if (result4 is string) return (string)result4;
                    else if (result4 is bool) return (bool)result4;
                    else if (result4 is float) return (float)result4;
                    else if (result4 is null) return null;
                    else { throw new RTE("native function call", "Invalid type returned from native function"); }
                default:
                    Curt.error(line, "Unsupported number of params in built-in function detected.");
                    return null;
            }
        }

        public object? UserFuncExec(Interpreter interpreter, Function functionDef)
        {
            Dictionary<string, object> globalCopy;

            Dictionary<string, object> locals = Interpreter.globals.ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value
                        );
            globalCopy = Interpreter.globals;
            Interpreter.globals = locals;

            for (int i = 0; i < args.Count; i++)
            {
                Interpreter.globals[functionDef.parameters[i]] = interpreter.Interpret(args[i]);
            }

            try
            {
                interpreter.Interpret(functionDef.funcBlock);
            }
            catch (ValuePackage delivery)
            {
                Interpreter.globals = globalCopy;
                return delivery.value;
            }
            Interpreter.globals = globalCopy;
            return null;
        }
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
                    default: Curt.error(line, "Unexpected unary operator"); return null;
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
                    case PLUS:
                        return (Func<object, object, object?>)((p1, p2) =>
                    {
                        try { return p1.GetType() == typeof(string) ? Concat((string)p1, (string)p2) : (float)p1 + (float)p2; }
                        catch (InvalidCastException e) { Curt.error(line, $"There is a mismatch of typings in the addition operation involving: {p1} and {p2}. Returned zilch by default."); return null; }
                    });
                    case MINUS: return (Func<float, float, float>)((p1, p2) => p1 - p2);
                    case SLASH: return (Func<float, float, float>)((p1, p2) => p1 / p2);
                    case STAR: return (Func<float, float, float>)((p1, p2) => p1 * p2);
                    case PERCENT: return (Func<float, float, float>)((p1, p2) => p1 % p2);
                    case GREATER: return (Func<float, float, bool>)((p1, p2) => p1 > p2);
                    case GREATER_EQUAL: return (Func<float, float, bool>)((p1, p2) => p1 >= p2);
                    case LESS: return (Func<float, float, bool>)((p1, p2) => p1 < p2);
                    case LESS_EQUAL: return (Func<float, float, bool>)((p1, p2) => p1 <= p2);
                    case EQUAL_EQUAL:
                        return (Func<object, object, bool?>)((p1, p2) =>
                        {
                            try { return p1.GetType() == typeof(string) ? strEquals((string)p1, (string)p2) : p1.Equals(p2); }
                            catch (InvalidCastException ICE) { Curt.error(line, $"There is a mismatch of typings in the equality comparison operation involving: {p1} and {p2}. Returned zilch by default."); return null; }
                        });
                    case BANG_EQUAL:
                        return (Func<object, object, bool?>)((p1, p2) =>
                        {
                            try { return p1.GetType() == typeof(string) ? strNotEquals((string)p1, (string)p2) : !p1.Equals(p2); }
                            catch (InvalidCastException e) { Curt.error(line, $"There is a mismatch of typings in your inequality comparison operation involving: {p1} and {p2}. Returned zilch by default."); return null; }
                        });
                    case AND: return (Func<bool, bool, bool>)((p1, p2) => p1 && p2);
                    case OR: return (Func<bool, bool, bool>)((p1, p2) => p1 || p2);
                    default: Curt.error(line, "Unexpected binary operator"); return null;
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
        public int line;
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