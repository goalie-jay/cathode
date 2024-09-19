using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public class ExecutorRuntimeException : Exception
    {
        public ExecutorRuntimeException(string msg) : base(msg) { }
    }

    public static class Executor
    {
        public static Dictionary<string, ZZObject> BackupVariables(ExecutionContext ctx)
        {
            Dictionary<string, ZZObject> objs = new Dictionary<string, ZZObject>();

            foreach (string key in ctx.Variables.Keys)
                objs.Add(key, ctx.Variables[key]);

            return objs;
        }

        public static Stack<ZZInteger> BackupComparisonStack(ExecutionContext ctx)
        {
            return new Stack<ZZInteger>(new Stack<ZZInteger>(ctx.ComparisonStack));
        }

        public static void RestoreVariables(ExecutionContext ctx, Dictionary<string, ZZObject> backup)
        {
            // Setting of modified values while ignoring ones that didn't exist before is handled before the call
            // Putting this here because I know you (future me) will question it later

            ctx.Variables.Clear();

            foreach (string key in backup.Keys)
                ctx.Variables.Add(key, backup[key]);
        }

        public static void RestoreComparisonStack(ExecutionContext ctx, Stack<ZZInteger> comparisonStack)
        {
            ctx.ComparisonStack = comparisonStack;
        }

        public static ZZObject Execute(ExecutionContext ctx, string fnName, string[] lines)
        {
            Program.CurrentlyExecutingContext = ctx;

            int i = 0;

            Stack<Dictionary<string, ZZObject>> varsBackupForWhile = new Stack<Dictionary<string, ZZObject>>();
            Stack<Stack<ZZInteger>> comparisonStackBackupForWhile = new Stack<Stack<ZZInteger>>();

#if !DEBUG
            try
            {
#endif
                for (i = 0; i < lines.Length; ++i)
                {
                    new Interpreter(ctx, lines[i]).Execute();

                    if (ctx.HasReturned)
                    {
                        // We're done
                        return ctx.LastReturnValue;
                    }

                    if (ctx.RequestedWhileLoop)
                    {
                        // While loop has started, pop request and push return idx onto the stack
                        ctx.AcceptWhileLoop();
                        ctx.WhileReturnLines.Push(i);

                        // Back up the variables so unexpected behavior doesn't occur
                        varsBackupForWhile.Push(BackupVariables(ctx));

                        // Back up the comparison stack in case they escape from an if statement
                        comparisonStackBackupForWhile.Push(BackupComparisonStack(ctx));
                    }

                    if (ctx.ReturnWhile)
                    {
                        if (!ctx.WhileReturnLines.Any())
                            throw new ExecutorRuntimeException($"A runtime error occurred at line {i + 1} " +
                                $"of function \"{fnName}\" when the code tried to loop to a nonexistent statement.");

                        // Set updated values in the backup but don't add the new vars from this context
                        Dictionary<string, ZZObject> backed = varsBackupForWhile.Pop();
                        foreach (string key in ctx.Variables.Keys)
                            if (backed.ContainsKey(key))
                                backed[key] = ctx.Variables[key];

                        // Restore the backed up variables
                        RestoreVariables(ctx, backed);

                        // Restore the comparison stack
                        RestoreComparisonStack(ctx, comparisonStackBackupForWhile.Pop());

                        // Pop line off the stack and return to it
                        i = ctx.WhileReturnLines.Pop() - 1; // Hack

                        ctx.ReturnWhile = false; // DO NOT FORGET THIS
                    }
                }
#if !DEBUG
            }
            catch (InterpreterRuntimeException ex)
            {
                throw new ExecutorRuntimeException($"A runtime error occurred at line {i + 1} " +
                    $"of function \"{fnName}\": {ex.Message}");
            }
#endif

            ZZObject returnValue = ctx.LastReturnValue;
            ctx.Dispose();
            return returnValue;
        }
    }
}
