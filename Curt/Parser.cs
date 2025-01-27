using nodes;
using Tokenizer;
using static Tokenizer.TokType;

namespace Parsing
{
    internal class Parser
    {
        private List<Token> tokens;
        private int currentToken;
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!isAtEnd())
            {
                statements.Add(statement());
            }
            return statements;
        }
        public List<Stmt> parseBlock()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!isRightBrace())
            {
                statements.Add(statement());
            }
            advance(); // this would be the } character
            return statements;
        }

        // statement -> assignment_statement | if_statement | while_statement | for_statement | block
        private Stmt statement()
        {
            Token token = peek();
            switch (token.type)
            {
                case MAKE: return assignment();
                case IF: return ifStmt();
                case WHILE: return whileStmt();
                case FOR: return forStmt();
                default:
                    return expression();
            }
        }
        // assignment_statement -> "make" IDENTIFIER expression ";"
        private Stmt assignment()
        {
            Console.WriteLine("[INFO] INSIDE ASSIGNMENT");
            advance(); // this was the 'make' keyword
            Token identifier = consume(IDENTIFIER, "Expected identifier after 'make' keyword.");
            consume(EQUAL, "Expected '=' after identifier in assignment statement");
            object value = expression();
            return new Assignment(identifier.lexeme, value);
        }
        // if_statement -> "if" "(" expression ")" block("elif" "(" expression ")" block)* ("else" block)?
        private Stmt ifStmt()
        {
            Console.WriteLine("[INFO] INSIDE IF");
            advance(); // this was the 'if' keyword
            consume(LEFT_PAREN, "Expected '(' character after 'if' keyword.");
            Expr ifCondition = expression();
            consume(RIGHT_PAREN, "Expected ')' character after 'if' condition.");
            Block ifBlock = block();

            List<Expr> elifConditions = new List<Expr>();
            List<Block> elifBlocks = new List<Block>();

            while (peek().type == ELIF)
            {
                Console.WriteLine("[INFO] INSIDE ELIF");
                advance(); // this was the 'elif' keyword
                consume(LEFT_PAREN, "Expected '(' character after 'elif' keyword.");
                elifConditions.Append(expression());
                consume(RIGHT_PAREN, "Expected ')' character after 'elif' condition.");
                elifBlocks.Append(block());
            }

            Block elseBlock = null;

            if (peek().type == ELSE)
            {
                Console.WriteLine("[INFO] INSIDE ELSE");
                advance(); // this was the 'else' keyword
                elseBlock = block();
            }

            return new If(ifCondition, ifBlock, elifConditions, elifBlocks, elseBlock);

        }
        // while_statement -> "while" "(" expression ")" block
        private Stmt whileStmt()
        {
            Console.WriteLine("[INFO] INSIDE WHILE");
            advance(); // this was the 'while' keyword
            consume(LEFT_PAREN, "Expected ')' character after 'while' keyword.");
            Expr whileCondition = expression();
            consume(RIGHT_PAREN, "Expected ')' character after 'while' condition.");
            Block whileBlock = block();
            return new While(whileCondition, whileBlock);
        }
        // for_statement -> "for" "(" assignment_statement ";" expression ";" expression ")" block
        private Stmt forStmt()
        {
            Console.WriteLine("[INFO] INSIDE FOR");
            advance(); // this was the 'for' keyword
            consume(LEFT_PAREN, "Expected '(' character after 'for' keyword.");
            Stmt start = assignment();
            consume(SEMICOLON, "Expected ';' after 'start' portion of for loop");
            Expr stop = expression();
            consume(SEMICOLON, "Expected ';' after 'stop' portion of for loop");
            Expr step = expression();
            consume(RIGHT_PAREN, "Expected ')' character after 'for' condition.");
            Block forBlock = block();
            return new For(start, stop, step, forBlock);
        }
        // block -> "{" statement_list "}"
        private Block block()
        {
            consume(LEFT_BRACE, "Expected '}' here");
            return new Block(parseBlock());
        }

        // expression -> comparison ;
        private Expr expression()
        {
            return comparison();
        }

        // comparison -> arithmetic((">" | ">=" | "<" | "<=" | "!=" | "==" | "and" | "or") arithmetic )* ;
        private Expr comparison()
        {
            Console.WriteLine("[INFO] INSIDE COMPARISON");
            Expr expr = arithmetic();
            while (check(GREATER) || check(GREATER_EQUAL) || check(LESS) || check(LESS_EQUAL) ||
                   check(BANG_EQUAL) || check(EQUAL_EQUAL) || check(AND) || check(OR))
            {
                Token operatorToken = advance();
                Expr right = arithmetic();
                expr = new Comparison(operatorToken.line, operatorToken.type, expr, right);
            }
            return expr;
        }

        // arithmetic -> unary(("+" | "-" | "*" | "/" | "%") unary )* ;
        private Expr arithmetic()
        {
            Console.WriteLine("[INFO] INSIDE ARITHMETIC");
            Expr expr = unary();
            while (check(PLUS) || check(MINUS) || check(STAR) || check(SLASH) || check(PERCENT))
            {
                Token token = advance(); // this is our operator
                Expr right = unary();
                expr = new Arithmetic(token.line, token.type, expr, right);
            }
            return expr;
        }

        // unary -> ("not" | "-" | "++" | "--") unary | literal
        private Expr unary()
        {
            Console.WriteLine("[INFO] INSIDE UNARY");
            TokType t = peek().type;
            if (t == INCREMENT || t == DECREMENT || t == NOT || t == MINUS)
            {
                Token op = advance();
                Expr right = unary();
                if (t == INCREMENT || t == DECREMENT)
                {
                    return new Step(op.line, op.type, right);
                }
                else if (t == NOT || t == MINUS)
                {
                    return new Negation(op.line, op.type, right);
                }
            }
            return literal();
        }
        // literal -> IDENTIFIER | NUMBER | STRING | BOOLEAN | "(" expression ")" ;
        private Expr literal()
        {
            Console.WriteLine("[INFO] INSIDE LITERAL");
            Token t = peek();
            switch (t.type)
            {
                case IDENTIFIER: advance(); return new Identifier(t.lexeme, t.line);
                case NUMBER: advance(); return new Number(t.lexeme, t.line);
                case STRING: advance(); return new nodes.String(t.lexeme, t.line);
                case TRUE: advance(); return new nodes.Boolean(t.lexeme, t.line);
                case FALSE: advance(); return new nodes.Boolean(t.lexeme, t.line);
                default:
                    if (check(LEFT_PAREN))
                    {
                        advance(); // consume the left paren
                        Expr expr = expression();
                        consume(RIGHT_PAREN, "Expected ')' after expression.");
                        return new Grouping(expr, t.line);
                    }
                    error(peek(), $"Unexpected token: {peek().lexeme}");
                    advance(); // Skip the unexpected token to avoid infinite loop
                    return null; // [WIP] Handle this error more robustly in the future
            }
        }

        // ###################
        // Helper Methods
        // ###################
        private Token consume(TokType type, string msg)
        {
            if (check(type)) return advance();

            error(peek(), msg);
            return new Token(EOF, "ERROR", "ERROR", 0);
            // [WIP] I want to find a better way to handle this later
        }

        static void error(Token token, string msg)
        {
            if (token.type == EOF)
            {
                Curt.report(token.line, " at end", msg);
            }
            else
            {
                Curt.report(token.line, " at '" + token.lexeme + "'", msg);
            }
            Curt.hadParseError = true;
        }

        private bool match(TokType type)
        {
            if (check(type))
            {
                advance();
                return true;
            }
            return false;
        }

        private bool check(TokType type)
        {
            if (isAtEnd()) return false;
            return peek().type == type;
        }

        private Token advance()
        {
            if (!isAtEnd()) currentToken++;
            Console.WriteLine("[INFO] ADVANCED OVER " + tokens[currentToken - 1] + " NOW ON " + tokens[currentToken]);
            return previous();
        }

        private bool isAtEnd()
        {
            return peek().type == EOF;
        }

        private bool isRightBrace()
        {
            return peek().type == RIGHT_BRACE;
        }

        private Token peek()
        {
            return tokens[currentToken];
        }

        private Token? peekAhead()
        {
            if (!isAtEnd()) { return tokens[currentToken + 1]; }
            return null;
        }

        private Token previous()
        {
            return tokens[currentToken - 1];
        }
    }
}