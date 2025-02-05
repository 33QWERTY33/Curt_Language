namespace nodes
{
    enum NodeType
    {
        // Statements
        ASSIGNMENT, IF, WHILE, FOR, BLOCK, SHOW, FUNCTION,

        // Expressions
        UNARY, BINARY, LITERAL, GROUPING, ASK, RANDINT, IDENTIFIER, CALL
    }
}
