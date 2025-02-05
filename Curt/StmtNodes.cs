﻿using Interpreting;
using static nodes.NodeType;

namespace nodes
{
    class Stmt
    {
        public virtual NodeType ntype { get; }
        public virtual void Execute(Interpreter interpreter) { }
    }
    class Function : Stmt
    {
        public string identifier;
        public List<string> parameters;
        public Block funcBlock;
        public override NodeType ntype => FUNCTION;
        public Function(string identifier, List<string> parameters, Block funcBlock)
        {
            this.identifier = identifier;
            this.parameters = parameters ?? new List<string>();
            this.funcBlock = funcBlock;
        }
        public override void Execute(Interpreter interpreter)
        {
            Interpreter.globals[identifier] = this;
        }
    }
    class Assignment : Stmt
    {
        public string identifier;
        public Expr value = null;
        public override NodeType ntype => ASSIGNMENT;
        public Assignment(string identifier, Expr value)
        {
            this.identifier = identifier;
            this.value = value;
        }
        public Assignment(string identifier)
        {
            this.identifier = identifier;
        }
        public override void Execute (Interpreter interpreter)
        {
            Interpreter.globals[identifier] = interpreter.Interpret(value);
        }
    }
    class If : Stmt
    {
        private Comparison ifCondition;
        private Block ifBlock;
        private List<Comparison> elifConditions;
        private List<Block> elifBlocks;
        private Block elseBlock;
        public override NodeType ntype => IF;
        public If(Comparison ifCondition, Block ifBlock, List<Comparison>? elifConditions, List<Block>? elifBlocks, Block? elseBlock)
        {
            this.ifCondition = ifCondition;
            this.ifBlock = ifBlock;
            this.elifConditions = elifConditions;
            this.elifBlocks = elifBlocks;
            this.elseBlock = elseBlock ?? null;
        }
        public override void Execute(Interpreter interpreter)
        {
            if ((bool)interpreter.Evaluate(ifCondition))
            {
                interpreter.Interpret(ifBlock);
                return;
            }
            foreach (Comparison condition in elifConditions)
            {
                if ((bool)interpreter.Evaluate(condition))
                {

                    interpreter.Interpret(elifBlocks[elifConditions.IndexOf(condition)]);
                    return;
                }
            }
            if (elseBlock != null)
            {
                interpreter.Interpret(elseBlock);
            }
        }
    }
    class While : Stmt
    {
        private Comparison condition;
        private Block block;
        public override NodeType ntype => WHILE;
        public While(Comparison condition, Block block)
        {
            this.condition = condition;
            this.block = block;
        }

        public override void Execute(Interpreter interpreter)
        {
            while ((bool)interpreter.Evaluate(condition))
            {
                interpreter.Interpret(block);
            }
        }
    }
    class For : Stmt
    {
        private Assignment start;
        private Comparison stop;
        private Step step;
        private Block block;
        public override NodeType ntype => FOR;

        public For(Assignment start, Comparison stop, Step step, Block block)
        {
            this.start = start;
            this.stop = stop;
            this.step = step;
            this.block = block;
        }

        public override void Execute(Interpreter interpreter)
        {
            Interpreter.globals[start.identifier] = interpreter.Interpret(start.value);
            for (object start = Interpreter.globals[this.start.identifier]; (bool)interpreter.Evaluate(stop); Interpreter.globals[this.start.identifier] = interpreter.Evaluate(step))
            {
                interpreter.Interpret(block);
            }
        }
    }
    class Show : Stmt
    {
        private Expr value;
        public override NodeType ntype => SHOW;
        public Show(Expr value)
        {
            this.value = value;
        }

        public override void Execute(Interpreter interpreter)
        {
            Console.WriteLine(interpreter.Evaluate(value));
        }
    }

    class Block : Stmt
    {
        public List<Stmt> statements;
        public override NodeType ntype => BLOCK;
        public Block(List<Stmt> statements)
        {
            this.statements = statements;
        }
        public override void Execute(Interpreter interpreter)
        {
            interpreter.Interpret(statements);
        }
    }
}