using static Tokenizer.TokType;

namespace Tokenizer
{
    class Scanner
    {
        private string source;
        private List<Token> tokens = new List<Token> { };
        private Dictionary<string, TokType> keywords = new Dictionary<string, TokType> {
            {"else", ELSE },
            {"true", TRUE },
            {"false", FALSE },
            {"for", FOR },
            {"if", IF },
            {"zilch", ZILCH },
            {"and", AND },
            {"or", OR },
            {"not", NOT },
            {"while", WHILE },
            {"elif", ELIF },
            {"make", MAKE },
            {"func", FUNC },
            {"call", CALL },
            {"return", RETURN }};
        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source)
        {
            this.source = source;
        }

        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                start = current;
                scanToken();
            }

            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }

        private void scanToken()
        {
            char c = advance();
            switch (c)
            {
                case '(': addToken(LEFT_PAREN); break;
                case ')': addToken(RIGHT_PAREN); break;
                case '{': addToken(LEFT_BRACE); break;
                case '}': addToken(RIGHT_BRACE); break;
                case ',': addToken(COMMA); break;
                case '.': addToken(DOT); break;
                case ';': addToken(SEMICOLON); break;
                case '*': addToken(STAR); break;
                case '%': addToken(PERCENT); break;
                case '@': addToken(CALL); break;
                case '~': addToken(MAKE); break;
                case '$': addToken(FUNC); break;
                case '\n': line++; break;
                case '-':
                    addToken(match('-') ? DECREMENT : MINUS);
                    break;
                case '+':
                    addToken(match('+') ? INCREMENT : PLUS);
                    break;
                case '!':
                    addToken(match('=') ? BANG_EQUAL : BANG);
                    break;
                case '=':
                    addToken(match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    addToken(match('=') ? GREATER_EQUAL : GREATER);
                    break;
                case '/':
                    if (match('/'))
                    {
                        while (peek() != '\n' && !isAtEnd()) advance();
                    } else
                    {
                        addToken(SLASH);
                    }
                    break;
                case '"': consume_string(); break;
                case ' ': break;
                default:

                    if (char.IsDigit(c))
                    {
                        consume_number();
                    } else if (char.IsLetter(c)) {
                        consume_identifier();
                    } else { Curt.error(line, "Unexpected character. CHAR: " + c + "| UTF8: " + Convert.ToInt32(c)); }
                    break;
            }
        }

        // Helper methods
        private void consume_identifier()
        {
            while (char.IsLetter(peek()) || char.IsDigit(peek())) advance();

            string text = source.Substring(start, current - start);
            bool lookupResult = keywords.TryGetValue(text, out TokType type);
            if (lookupResult == false) type = IDENTIFIER;
            addToken(type);
        }
        private void consume_number()
        {
            while (char.IsDigit(peek())) advance();

            if (peek() == '.' && char.IsDigit(peekNext())) {
                advance();

                while (char.IsDigit(peek())) advance();
            }

            addToken(NUMBER, Double.Parse(source.Substring(start, current-start)));
        }
        private void consume_string()
        {
            while(peek() != '"' && !isAtEnd())
            {
                if (peek() == '\n') line++;
                advance();
            }
            if (isAtEnd())
            {
                Curt.error(line, "Unterminated string");
                return;
            }

            advance();

            string value = source.Substring(start + 1, current - (start+2));
            addToken(STRING, value);
        }
        private char peek()
        {
            if (isAtEnd()) return '\0';
            return source[current];
        }
        private char peekNext()
        {
            if (isAtEnd()) return '\0';
            return source[current+1];
        }
        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }


        private bool isAtEnd()
        {
            return current >= source.Length;
        }

        private char advance()
        {
            return source.ElementAt(current++);
        }

        private void addToken(TokType type)
        {
            addToken(type, null);
        }

        private void addToken(TokType type, object? literal)
        {
            string text = source.Substring(start, current-start);
            tokens.Add(new Token(type, text, literal, line));
        }

    }
}
