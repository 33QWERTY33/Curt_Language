namespace Tokenizer
{
    enum TokType
    {
        // Single character tokens.
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, COMMA, DOT, MINUS, PLUS, SEMICOLON, STAR, PERCENT,
        // 1-2 character tokens
        BANG, BANG_EQUAL, EQUAL, EQUAL_EQUAL, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL, INCREMENT, DECREMENT, SLASH,
        // Literals
        IDENTIFIER, STRING, NUMBER,
        // Keywords
        AND, ELSE, FALSE, FOR, IF, ELIF, ZILCH, OR, NOT, TRUE, MAKE, FUNC, CALL, WHILE, SHOW, ASK, RANDINT, RETURN,
        EOF
    }

    class Token
    {
        public TokType type;
        public string lexeme;
        public object? literal;
        public int line;

        public Token(TokType type, string lexeme, object? literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}