using Tokenizer;
using Parsing;
using nodes;
using Interpreting;
using System.Text;

public class Curt {
    public static bool hadSyntaxError;
    public static bool hadParseError;
    public static bool hadRuntimeError;
    public static void Main(string[] args) {
        if (args.Length > 1) {
           Console.Error.WriteLine("[ERROR] Invalid number of arguments provided");
        } else if (args.Length == 1) { // for file
            runFile(args[0]);
        } else { // for REPL
            runPrompt();
        }
    }

    private static void runPrompt() {
        while (true) {
            hadSyntaxError = false;
            hadParseError = false;
            Interpreter.runTimeErrorOccurred = false;
            Console.Write(">>> ");
            string stmt = Console.ReadLine() ?? "  ";
            run(stmt);
        }
    }

    private static void runFile(string filePath) {
        if (!File.Exists(filePath)) {
            Console.Error.WriteLine("[ERROR] The file does not exist...");
        } else {
            string sourceCode = File.ReadAllText(filePath, Encoding.UTF8);
            sourceCode = sourceCode.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\t", "");
            run(sourceCode);
        }
    }

    private static void run(string source) {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.scanTokens();

        if (hadSyntaxError) return;

        Parser parser = new Parser(tokens);
        List<Stmt> nodes = parser.parse();

        if (hadParseError) return;

        Interpreter interpreter = new Interpreter(nodes);
        List<object> results = new List<object>();
        interpreter.Interpret(nodes);
    }

    // ######################
    // Error Handling
    // ######################

    public static void error(int line, string msg) {
        hadParseError = true;
        hadSyntaxError = true;
        Console.Error.WriteLine("[Line " + line + "] Error: " + msg);
    }

    public static void report(int line, string where, string msg)
    {
        Console.Error.WriteLine("[Line " + line + "] Error " + where + ": " + msg);

        hadParseError = true;
    }
}

/*
    Credits for the Scanner and Token class go to Robert Nystrom as outlined in his book Crafting Interpreters.
    Those are not my ideas, but it seemed like very efficient logic and doing it another way would just be damaging performance just to save face.
    All of the other specific implementation details are my idea (for better or worse), such as stuffing functionality into the syntax tree nodes.
 */