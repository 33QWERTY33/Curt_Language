﻿using System;
using System.IO;
using System.Collections.Generic;
using Tokenizer;
using Parsing;
using nodes;
using System.Runtime.CompilerServices;

public class Curt {
    public static bool hadSyntaxError;
    public static bool hadParseError;
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
            Console.Write(">>> ");
            string stmt = Console.ReadLine();
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
            Console.WriteLine(token);
        }

        Parser parser = new Parser(tokens);
        List<Stmt> nodes = parser.parse();

        foreach (Stmt node in nodes)
        {
            Console.WriteLine(node);
        }

        if (hadParseError)
        {
            return;
        }

    }

    // ######################
    // Error Handling
    // ######################

    public static void error(int line, string msg) {
        Console.Error.WriteLine("[Line + " + line + "] Error: " + msg);
    }

    public static void report(int line, string where, string msg)
    {
        Console.Error.WriteLine("[Line " + line + "] Error " + where + ": " + msg);

        hadParseError = true;
    }
}