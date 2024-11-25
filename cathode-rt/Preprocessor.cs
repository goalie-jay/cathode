using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public class PreprocessorException : Exception
    {
        public PreprocessorException(string msg) : base(msg) { }
    }

    public static class Preprocessor
    {
        public static bool IsAlphanumeric(this string str)
        {
            if (str.Length < 1)
                return false;

            for (int i = 0; i < str.Length; ++i)
                if (!char.IsLetterOrDigit(str[i]))
                    return false;

            return true;
        }

        public static string[] ScanIncludes(StreamReader reader)
        {
            List<string> includes = new List<string>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("include "))
                {
                    // Now expand it to make coding a little less annoying
                    string incl = Environment.ExpandEnvironmentVariables(
                        line.Substring("include ".Length).Trim('"'));

                    // Trace includes from that file
                    StreamReader sr = null;

                    try
                    {
                        sr = new StreamReader(File.OpenRead(incl));
                    }
                    catch
                    {
                        throw new PreprocessorException($"Failed to scan includes from file {incl}.");
                    }

                    includes.AddRange(ScanIncludes(sr));
                    includes.Add(incl);
                    sr.Dispose();
                }
            }

            return includes.ToArray();
        }

        public static Dictionary<ZZFunctionDescriptor, string[]> ScanFunctionNamesToFunctionBodies(StreamReader reader, string _namespace = "core")
        {
            Dictionary<ZZFunctionDescriptor, string[]> dict = new Dictionary<ZZFunctionDescriptor, string[]>();

            List<string> allLinesInFile = new List<string>();
            while (!reader.EndOfStream)
                allLinesInFile.Add(reader.ReadLine());

            ZZFunctionDescriptor currentFunc = null;
            List<string> lines = new List<string>();

            for (int i = 0; i < allLinesInFile.Count; ++i)
            {
                string line = allLinesInFile[i];
                // string line = reader.ReadLine();

                if (currentFunc != null)
                {
                    if (line.Trim() == "fnret")
                    {
                        dict.Add(currentFunc, lines.ToArray());
                        lines.Clear();
                        currentFunc = null;
                    }
                    else
                    {
                        while (line.Trim().EndsWith(" \\"))
                            line = line.Trim().Substring(0, line.Trim().Length - 1) + allLinesInFile[++i];

                        lines.Add(line);
                    }
                }
                else if (line.Trim().StartsWith("fndef "))
                {
                    string fnNameAndArgs = line.Trim().Substring("fndef ".Length);
                    string[] goAheadAndSplit = fnNameAndArgs.Split(new string[] { "accepts" }, StringSplitOptions.TrimEntries);

                    string fnName = goAheadAndSplit[0];

                    if (fnName.Length < 1 || fnName.Contains(' ') || char.IsDigit(fnName[0]) ||
                        !fnName.IsAlphanumeric())
                        throw new PreprocessorException("Function \"" + fnName + "\" has a space in its name, " +
                            "has a blank name, has a number as the first character in its name, or is not alphanumeric.");

                    List<string> argNames = new List<string>();

                    if (goAheadAndSplit.Length > 1)
                    {
                        string fnArgs = goAheadAndSplit[1];
                        string[] csl = fnArgs.Split(',');

                        foreach (string c in csl)
                        {
                            string argName = c.Trim().Split(' ')[0];
                            argNames.Add(argName);
                        }
                    }

                    currentFunc = new ZZFunctionDescriptor(fnName, argNames.ToArray(), _namespace);
                }
            }

            return dict;
        }
    }
}
