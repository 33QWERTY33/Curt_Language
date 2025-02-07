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
                case MAKE: return assignmentStmt();
                case IF: return ifStmt();
                case WHILE: return whileStmt();
                case FOR: return forStmt();
                case SHOW: return showStmt();
                case FUNC: return funcDefStmt();
                case RETURN: return returnStmt();
                default:
                    return expression();
            }
        }

        // "return" expression
        private Stmt returnStmt()
        {
            advance(); // this was the "return" keyword
            Expr expr = expression();
            return new Return(expr);
        }

        // funcdef_statement -> "func" IDENTIFIER "(" (IDENTIFIER)* ")" block
        private Stmt funcDefStmt()
        {
            List<string> parameters = new List<string>();

            advance(); // this was the 'func' keyword
            Token identifier = consume(IDENTIFIER, "Expected identifier after 'func' keyword.");
            consume(LEFT_PAREN, "Expected '(' after identifier.");
            while (!match(RIGHT_PAREN))
            {
                Token paramName = consume(IDENTIFIER, "invalid parameter name found.");
                parameters.Add(paramName.lexeme);
                if (peek().type != RIGHT_PAREN) { consume(COMMA, "parameters should be seperated by commas."); }
            }

            Block funcBlock = block();

            return new Function(identifier.lexeme, parameters, funcBlock);
        }

        // assignment_statement -> "make" IDENTIFIER state ";"
        private Stmt assignmentStmt()
        {
            advance(); // this was the 'make' keyword
            Token identifier = consume(IDENTIFIER, "Expected identifier after 'make' keyword.");
            consume(EQUAL, "Expected '=' after identifier in assignment statement");
            Expr value = expression();
            return new Assignment(identifier.lexeme, value);
        }
        // if_statement -> "if" "(" expression ")" block("elif" "(" expression ")" block)* ("else" block)?
        private Stmt ifStmt()
        {
            advance(); // this was the 'if' keyword
            consume(LEFT_PAREN, "Expected '(' character after 'if' keyword.");
            Comparison ifCondition = (Comparison)expression();
            consume(RIGHT_PAREN, "Expected ')' character after 'if' condition.");
            Block ifBlock = block();

            List<Comparison> elifConditions = new List<Comparison>();
            List<Block> elifBlocks = new List<Block>();

            while (peek().type == ELIF)
            {
                advance(); // this was the 'elif' keyword
                consume(LEFT_PAREN, "Expected '(' character after 'elif' keyword.");
                elifConditions.Add((Comparison)expression());
                consume(RIGHT_PAREN, "Expected ')' character after 'elif' condition.");
                elifBlocks.Add(block());
            }

            Block elseBlock = null;

            if (peek().type == ELSE)
            {
                advance(); // this was the 'else' keyword
                elseBlock = block();
            }

            return new If(ifCondition, ifBlock, elifConditions, elifBlocks, elseBlock);

        }
        // while_statement -> "while" "(" expression ")" block
        private Stmt whileStmt()
        {
            advance(); // this was the 'while' keyword
            consume(LEFT_PAREN, "Expected ')' character after 'while' keyword.");
            Comparison whileCondition = (Comparison)expression();
            consume(RIGHT_PAREN, "Expected ')' character after 'while' condition.");
            Block whileBlock = block();
            return new While(whileCondition, whileBlock);
        }
        // for_statement -> "for" "(" assignment_statement ";" expression ";" expression ")" block
        private Stmt forStmt()
        {
            advance(); // this was the 'for' keyword
            consume(LEFT_PAREN, "Expected '(' character after 'for' keyword.");
            Assignment start = (Assignment)assignmentStmt();
            consume(SEMICOLON, "Expected ';' after 'start' portion of for loop");
            Comparison stop = (Comparison)expression();
            consume(SEMICOLON, "Expected ';' after 'stop' portion of for loop");
            Step step = (Step)expression();
            consume(RIGHT_PAREN, "Expected ')' character after 'for' condition.");
            Block forBlock = block();
            return new For(start, stop, step, forBlock);
        }
        // show -> "show" "(" expression ")"
        private Stmt showStmt()
        {
            advance(); // this was the 'show' keyword
            consume(LEFT_PAREN, "Expected '(' character after 'show' keyword.");
            Expr value = expression();
            consume(RIGHT_PAREN, "Expected ')' character after 'show' value.");
            return new Show(value);
        }

        // block -> "{" statement_list "}"
        private Block block()
        {
            consume(LEFT_BRACE, "Expected '{' here");
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
            return primary();
        }

        // primary ->  call | literal | "(" (expression)* ")" ;
        private Expr primary()
        {
            if (peek().type == CALL)
            {
                return call();
            } else
            {
                return literal();
            }
        }

        // call -> "call" IDENTIFIER "(" (expression ("," expression)*)? ")" 
        private Expr call()
        {
            advance(); // this was the 'call' keyword
            List<Expr> args = new List<Expr>();

            Token funcTok = consume(IDENTIFIER, "Expected function identifier for call expression");
            string funcName = funcTok.lexeme;
            consume(LEFT_PAREN, "Expected '(' after function identifier for call expression");
            while(!match(RIGHT_PAREN))
            {
                args.Add(expression());
                if (peek().type != RIGHT_PAREN) { consume(COMMA, "arguments should be seperated by commas."); }
            }

            return new Call(funcTok.line, funcName, args);
        }

        // literal -> IDENTIFIER | NUMBER | STRING | BOOLEAN | "(" expression ")" ;
        private Expr literal()
        {
            Token t = peek();
            switch (t.type)
            {
                case IDENTIFIER: advance(); return new Identifier(t.lexeme, t.line);
                case NUMBER: advance(); return new Number(t.lexeme, t.line);
                case STRING: advance(); return new nodes.String(t.lexeme, t.line);
                case TRUE: advance(); return new nodes.Boolean(t.lexeme, t.line);
                case FALSE: advance(); return new nodes.Boolean(t.lexeme, t.line);
                case ASK: return ask();
                case RANDINT: return randint();
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

        // ask -> "ask" "(" """ NUMBER """ ")"
        private Expr randint()
        {
            advance(); // this was the 'randint' keyword
            consume(LEFT_PAREN, "Expected '(' character after 'randint' keyword.");
            Expr value = primary(); // random range
            consume(RIGHT_PAREN, "Expected ')' character after 'randint' value.");
            return new Randint(value);
        }

        // randint -> "randint" "(" INT ")"
        private Expr ask()
        {
            advance(); // this was the 'ask' keyword
            consume(LEFT_PAREN, "Expected '(' character after 'ask' keyword.");
            Expr value = primary(); // input prompt
            consume(RIGHT_PAREN, "Expected ')' character after 'ask' value.");
            return new Ask(value);
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