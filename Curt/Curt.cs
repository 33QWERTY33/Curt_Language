using System;
using System.IO;
using Tokenizer;
using Parsing;
using nodes;
using Interpreting;

public class Curt {
    public static bool hadSyntaxError;
    public static bool hadParseError;
    public static bool hadRuntimeError;
    public static void Main(string[] args) {
        if (args.Length > 2) {
           Console.Error.WriteLine("[ERROR] Invalid number of arguments provided");
        } else if (args.Length == 2) { // for file
            runFile(args[1]);
        } else { // for REPL
            runPrompt();
        }
    }

    private static void runPrompt() {
        while (true) {
            hadSyntaxError = false;
            hadParseError = false;
            Console.Write(">>> ");
            string stmt = Console.ReadLine() ?? "  ";
            run(stmt);
        }
    }

    private static void runFile(string filePath) {
        if (!File.Exists(filePath)) {
            Console.Error.WriteLine("[ERROR] The file does not exist...");
        } else {
            string sourceCode = File.ReadAllText(filePath);
            run(sourceCode);
        }
    }

    private static void run(string source) {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.scanTokens();

        if (hadSyntaxError)
        {
            return;
        }
        foreach (Token token in tokens)
        {
            Console.WriteLine("[INFO: Tokenizer] " + token);
        }

        Parser parser = new Parser(tokens);
        List<Stmt> nodes = parser.parse();

        foreach (Stmt node in nodes)
        {
            Console.WriteLine("[INFO: AST Root] " + node);
        }

        if (hadParseError)
        {
            return;
        }

        Interpreter interpreter = new Interpreter(nodes);
        List<object> results = new List<object>();
        foreach (Stmt node in nodes)
        {
            Console.WriteLine("[INFO: Interpretation Results] " + interpreter.Interpret(nodes[0]));
        }
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
    Credits for the Scanner and Token classes go to Robert Nystrom as outlined in his book Crafting Interpreters.
    Those are not my ideas, but it seemed like very efficient logic and doing it another way would just be damaging performance just to save face.
    All of the other specific implementation details are my idea (for better or worse), such as stuffing functionality into the syntax tree nodes.
 */