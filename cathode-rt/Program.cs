using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace cathode_rt
{
    class Program
    {
        public static ExecutionContext GlobalContext;
        public static ExecutionContext CurrentlyExecutingContext;
        public static string ExecutingFile = string.Empty;
        public const int MajorVersionNumber = 0;
        public const int MinorVersionNumber = 0;
        public const int IncrementVersionNumber = 5;

        static int Main(string[] args)
        {
            // Initialize C library
            FastOps.Setup();

            bool compile = false;
            Console.Title = "cathode-rt";

            GlobalContext = new ExecutionContext(true);

            if (args.Length == 0)
            {
                CurrentlyExecutingContext = GlobalContext;
                Console.WriteLine("No source file was supplied; interpreter will run interactively.");
                Console.WriteLine("Control flow statements are NOT supported. Attempting to use them will throw errors/not function as intended.");
                Console.WriteLine("Auto-importing \"conio\" for interactive session...");
                CurrentlyExecutingContext.ImportNamespace("conio");

                string input;
                while (true)
                {
                    Console.Write("cathode-rt> ");
                    input = Console.ReadLine();
                    try
                    {
                        new Parser(new Lexer(new string[] { input }).Lex()).Parse()
                            .GetMainNode().Execute(GlobalContext);
                            // Holy one-liner, Batman!
                    }
                    catch (InterpreterRuntimeException ex)
                    {
                        Console.WriteLine("*****");
                        Console.WriteLine($"Runtime Error: {ex.Message}");
                        Console.WriteLine("*****");
                    }
                }
            }
            else
            {
                if (args[0] == "--compile" && args.Length >= 1)
                    compile = true;

                string filename = compile ? args[1] : args[0];

                if (!File.Exists(filename))
                {
                    Console.WriteLine("Source file does not exist.");
                    return 2;
                }

                StreamReader sr;
                
                try
                {
                    FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    sr = new StreamReader(fs);
                }
                catch
                {
                    Console.WriteLine("Failed to open primary source file.");
                    return 3;
                }

                ExecutingFile = Path.GetFullPath(filename);

                string[] includedFiles;

                try
                {
                    includedFiles = Preprocessor.ScanIncludes(sr);
                }
                catch (PreprocessorException ex)
                {
                    Console.WriteLine(ex.Message);
                    return 1;
                }

#if !DEBUG
                try
                {
#endif
                    bool executionStarted = false;
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    Dictionary<ZZFunctionDescriptor, string[]> fnBodiesText = Preprocessor.ScanFunctionNamesToFunctionBodies(sr);

                    foreach (ZZFunctionDescriptor desc in fnBodiesText.Keys)
                    {
                        Lexer lex = new Lexer(fnBodiesText[desc]);
                        Token[] toks = lex.Lex();
                        SyntaxTree.FunctionBodySyntaxTreeNode primaryNode = new Parser(toks).Parse().GetMainNode();
                        GlobalContext.AddFunc(desc, primaryNode);
                    }

                    // Scan functions from includes
                    foreach (string file in includedFiles)
                    {
                        try
                        {
                            FileStream fs = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                            StreamReader includeReader = new StreamReader(fs);
                            long posBackup = includeReader.BaseStream.Position;
                            string line = includeReader.ReadLine();

                            string ns = "core";
                            if (line.StartsWith("namespace "))
                                ns = line.Substring("namespace ".Length).Trim();
                            else
                                includeReader.BaseStream.Position = posBackup;

                            Dictionary<ZZFunctionDescriptor, string[]> included = Preprocessor.ScanFunctionNamesToFunctionBodies(includeReader,
                                    ns);
                            foreach (ZZFunctionDescriptor desc in fnBodiesText.Keys)
                            {
                                Lexer lex = new Lexer(fnBodiesText[desc]);
                                Token[] toks = lex.Lex();
                                SyntaxTree.FunctionBodySyntaxTreeNode primaryNode = new Parser(toks).Parse().GetMainNode();
                                GlobalContext.AddFunc(desc, primaryNode);
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"Failed to parse included file {file}.");
                            return 5;
                        }
                    }

                    sr.Dispose();

                    SyntaxTree.FunctionBodySyntaxTreeNode fnMain = GlobalContext.GetFunctionOrReturnNullIfNotPresent("Main", 
                        out ZZFunctionDescriptor mainDescriptor);
                    if (fnMain == null)
                    {
                        Console.WriteLine("No main function in source file, aborting...");
                        return 1;
                    }

                    ZZObject retVal = null;

                    if (mainDescriptor.Arguments.Length > 1)
                    {
                        Console.WriteLine("Main() had an argument count greater than one.");
                        return 6;
                    }
                    else if (mainDescriptor.Arguments.Length == 1)
                    {
                        List<ZZString> argumentsPassed = new List<ZZString>();
                        for (int i = 1; i < args.Length; ++i)
                            argumentsPassed.Add(new ZZString(args[i]));

                        GlobalContext.Variables.Add(mainDescriptor.Arguments[0], new ZZArray(argumentsPassed.ToArray()));
                    }

                    executionStarted = true;
                    retVal = fnMain.Execute(GlobalContext);
#if !DEBUG
                }
                catch (ExecutorRuntimeException ex)
                {
                    Console.WriteLine("*****");
                    if (executionStarted)
                        Console.WriteLine("[IN EXECUTION]");
                    else
                        Console.WriteLine("[IN PARSING]");

                    Console.WriteLine(ex.Message);
                    Console.WriteLine("*****");

                    retVal = GlobalContext.LastReturnValue;
                }
#endif

                // Use stored retVal because global context is dead atp

                if (retVal.ObjectType == ZZObjectType.INTEGER)
                    // Program returned integer, return it
                    return (int)((ZZInteger)retVal).Value;

                return 0;
            }
        }
    }
}
