using System;
using System.Collections.Generic;
using System.IO;

namespace cathode_rt
{
    class Program
    {
        public static ExecutionContext GlobalContext;
        public static ExecutionContext CurrentlyExecutingContext;
        public static string ExecutingFile = string.Empty;
        public const int MajorVersionNumber = 0;
        public const int MinorVersionNumber = 0;
        public const int IncrementVersionNumber = 2;

        static int Main(string[] args)
        {
            // Initialize C library
            FastOps.Setup();

            Console.Title = "cathode-rt";

            GlobalContext = new ExecutionContext(true);

            if (args.Length == 0)
            {
                CurrentlyExecutingContext = GlobalContext;
                Console.WriteLine("No source file was supplied; interpreter will run interactively.");
                string input;
                while (true)
                {
                    Console.Write("cathode-rt> ");
                    input = Console.ReadLine();
                    try
                    {
                        new Interpreter(GlobalContext, input).Execute();
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
                string filename = args[0];

                if (!File.Exists(filename))
                {
                    Console.WriteLine("Source file does not exist.");
                    return 2;
                }

                StreamReader sr;
                
                try
                {
                    sr = new StreamReader(filename);
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

                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                GlobalContext.FunctionsAndBodies = Preprocessor.ScanFunctionNamesToFunctionBodies(sr);

                // Scan functions from includes
                foreach (string file in includedFiles)
                {
                    try
                    {
                        StreamReader includeReader = new StreamReader(file);
                        long posBackup = includeReader.BaseStream.Position;
                        string line = includeReader.ReadLine();

                        string ns = "core";
                        if (line.StartsWith("namespace "))
                            ns = line.Substring("namespace ".Length).Trim();
                        else
                            includeReader.BaseStream.Position = posBackup;

                        GlobalContext.AddFunctionDictionary(Preprocessor.ScanFunctionNamesToFunctionBodies(includeReader,
                                ns));
                    }
                    catch
                    {
                        Console.WriteLine($"Failed to parse included file {file}.");
                        return 5;
                    }
                }

                string[] fnMain = GlobalContext.GetFunctionOrReturnNullIfNotPresent("Main", 
                    out ZZFunctionDescriptor mainDescriptor);
                if (fnMain == null)
                {
                    Console.WriteLine("No main function in source file, aborting...");
                    return 1;
                }

                ZZObject retVal = null;
#if !DEBUG
                try
                {
#endif
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

                    retVal = Executor.Execute(GlobalContext, "Main", fnMain);
#if !DEBUG
            }
                catch (ExecutorRuntimeException ex)
                {
                    Console.WriteLine("*****");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("*****");

                    retVal = GlobalContext.LastReturnValue;
                }
#endif
                sr.Dispose();

                // Use stored retVal because global context is dead atp

                if (retVal.ObjectType == ZZObjectType.INTEGER)
                    // Program returned integer, return it
                    return (int)((ZZInteger)retVal).Value;

                return 0;
            }
        }
    }
}
