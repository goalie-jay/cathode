using System;
using System.Collections.Generic;
using System.IO;

namespace cathode_rt
{
    class Program
    {
        public static ExecutionContext GlobalContext;
        public static ExecutionContext CurrentlyExecutingContext;

        static int Main(string[] args)
        {
            Console.Title = "zz-rt";

            GlobalContext = new ExecutionContext();

            if (args.Length == 0)
            {
                Console.WriteLine("No source file was supplied; interpreter will run interactively.");
                string input;
                while (true)
                {
                    Console.Write("zz-int> ");
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

                StreamReader sr = new StreamReader(filename);

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
                    out ZZFunctionDescriptor descriptor);
                if (fnMain == null)
                {
                    Console.WriteLine("No main function in source file, aborting...");
                    return 1;
                }

                ZZObject retVal = null;
                try
                {
                    retVal = Executor.Execute(GlobalContext, "Main", fnMain);
                }
                catch (ExecutorRuntimeException ex)
                {
                    Console.WriteLine("*****");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("*****");
                }
                sr.Dispose();

                // Use stored retVal because global context is dead atp

                if (retVal is ZZInteger zint)
                    // Program returned integer, return it
                    return (int)zint.Value;

                return 0;
            }
        }
    }
}
