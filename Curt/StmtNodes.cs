namespace nodes {
    class Stmt {

    }
    class Assignment : Stmt
    {
        private string identifier;
        private object value;
        public Assignment(string identifier, object value)
        {
            this.identifier = identifier;
            this.value = value;
        }
    }
    class If : Stmt
    {
        private Expr ifCondition;
        private Block ifBlock;
        private List<Expr> elifConditions;
        private List<Block> elifBlocks;
        private Block elseBlock;
        public If(Expr ifCondition, Block ifBlock, List<Expr>? elifConditions, List<Block>? elifBlocks, Block? elseBlock)
        {
            this.ifCondition = ifCondition;
            this.ifBlock = ifBlock;
            this.elifConditions = elifConditions;
            this.elifBlocks = elifBlocks;
            this.elseBlock = elseBlock;
        }
    }
    class While : Stmt
    {
        private Expr condition;
        private Block block;

        public While(Expr condition, Block block)
        {
            this.condition = condition;
            this.block = block;
        }
    }
    class For : Stmt
    {
        private Stmt start;
        private Expr stop;
        private Expr step;
        private Block block;

        public For(Stmt start, Expr stop, Expr step, Block block)
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