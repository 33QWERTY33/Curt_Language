using Interpreting;
using static nodes.NodeType;

namespace nodes
{
    class Stmt
    {
        public virtual NodeType ntype { get; }
        public virtual void Execute(Interpreter interpreter) { }
    }
    class Assignment : Stmt
    {
        public string identifier;
        public Expr value;
        public override NodeType ntype => ASSIGNMENT;
        public Assignment(string identifier, Expr value)
        {
            this.identifier = identifier;
            this.value = value;
        }
        public override void Execute (Interpreter interpreter)
        {
            Interpreter.globals.Add(identifier, interpreter.Evaluate(value));
        }
    }
    class If : Stmt
    {
        private Comparison ifCondition;
        private Block ifBlock;
        private List<Comparison> elifConditions;
        private List<Block> elifBlocks;
        private Block elseBlock;
        public If(Comparison ifCondition, Block ifBlock, List<Comparison>? elifConditions, List<Block>? elifBlocks, Block? elseBlock)
        {
            this.ifCondition = ifCondition;
            this.ifBlock = ifBlock;
            this.elifConditions = elifConditions ?? new List<Comparison>();
            this.elifBlocks = elifBlocks ?? new List<Block>();
            this.elseBlock = elseBlock ?? new Block(new List<Stmt>());
        }
    }
    class While : Stmt
    {
        private Comparison condition;
        private Block block;
        public While(Comparison condition, Block block)
        {
            this.condition = condition;
            this.block = block;
        }
    }
    class For : Stmt
    {
        private Assignment start;
        private Comparison stop;
        private Step step;
        private Block block;

        public For(Assignment start, Comparison stop, Step step, Block block)
        {
            this.start = start;
            this.stop = stop;
            this.step = step;
            this.block = block;
        }
    }
    class Block : Stmt
    {
        private List<Stmt> statements;
        public Block(List<Stmt> statements)
        {
            this.statements = statements;
        }
    }
}