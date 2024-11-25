using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static cathode_rt.SyntaxTree;
using System.Reflection;

namespace cathode_rt
{
    public static class ThreadBackend
    {
        public class ThreadData
        {
            public Thread InternalThread;
            public ExecutionContext Context;
        }

        public static List<ThreadData> Threads = new List<ThreadData>();
        public static object ListLockObject = new object();

        public static int AddThreadAndReturnIndex(ThreadData thread)
        {
            lock (ListLockObject)
            {
                for (int i = 0; i < Threads.Count; ++i)
                    if (Threads[i] == null)
                    {
                        // Free space, we can use it
                        Threads[i] = thread;
                        return i;
                    }

                int idx = Threads.Count;
                Threads.Add(thread);
                return idx;
            }
        }

        public static ThreadData GetThreadAtIdx(int idx)
        {
            lock (ListLockObject)
            {
                if (idx < 0)
                    return null;

                if (idx >= Threads.Count)
                    return null;

                return Threads[idx];
            }
        }

        public static void RemoveThreadAtIndex(int idx)
        {
            lock (ListLockObject)
            {
                Threads[idx].Context.Dispose();
                Threads[idx] = null;
            }
        }
    }

    public static partial class ImplMethods
    {
        [ZZFunction("thread", "Thread")]
        public static ZZInteger InitThread(ZZString fnName, ZZArray arguments)
        {
            ExecutionContext threadContext = new ExecutionContext();
            foreach (ZZFunctionDescriptor fnDesc in Program.CurrentlyExecutingContext.FunctionsAndBodies.Keys)
                threadContext.FunctionsAndBodies.Add(fnDesc, 
                    Program.CurrentlyExecutingContext.FunctionsAndBodies[fnDesc]);

            Thread thread = new Thread(() =>
            {
                string name = fnName.Contents;

                SyntaxTreeNode fn = Program.CurrentlyExecutingContext.GetFunctionOrReturnNullIfNotPresent(name, out ZZFunctionDescriptor desc);

                if (fn == null)
                {
                    var methodInf = Program.CurrentlyExecutingContext.LookupBuiltInFunction(name);

                    if (methodInf == null)
                        throw new InterpreterRuntimeException("Could not resolve function for dynamic call.");

                    try
                    {
                        ParameterInfo[] invokeParams = methodInf.GetParameters();
                        if (invokeParams.Length != arguments.Objects.Length)
                            throw new TargetParameterCountException();

                        object[] fixedParams = new object[arguments.Objects.Length];
                        for (int i = 0; i < arguments.Objects.Length; ++i)
                            fixedParams[i] = arguments.Objects[i];

                        ZZObject retVal = (ZZObject)methodInf.Invoke(null, fixedParams);
                        threadContext.HasReturned = true;
                        threadContext.LastReturnValue = retVal;
                    }
                    catch (InterpreterRuntimeException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        if (ex is ArgumentException || ex is TargetParameterCountException)
                            throw new InterpreterRuntimeException("Tried to supply either an incorrect amount of parameters " +
                                "or parameters with incorrect types to a standard function. " +
                                "Are you calling the correct function? Did you forget " +
                                "a type conversion?");
                        else if (ex is TargetInvocationException)
                            throw ex.InnerException;
                        else
                            throw;
                    }
                    // throw new InterpreterRuntimeException("Could not resolve target of dynamic call.");
                }
                else
                {
                    if (arguments.Objects.Length != desc.Arguments.Length)
                        throw new InterpreterRuntimeException("Number of arguments for dynamic call did not match number of arguments for target function.");

                    for (int i = 0; i < arguments.Objects.Length; ++i)
                        threadContext.Variables.Add(desc.Arguments[i],
                            arguments.Objects[i]); // Line up our values with the parameter names

                    ZZObject callResult = fn.Execute(threadContext);
                    threadContext.HasReturned = true;
                    threadContext.LastReturnValue = callResult;
                }
            });

            thread.Start();
            return ThreadBackend.AddThreadAndReturnIndex(new ThreadBackend.ThreadData()
            {
                InternalThread = thread,
                Context = threadContext
            });
        }

        [ZZFunction("thread", "tJoin")]
        public static ZZObject Join(ZZInteger threadIdx)
        {
            ThreadBackend.ThreadData td = ThreadBackend.GetThreadAtIdx((int)threadIdx.Value);

            if (td == null)
                throw new InterpreterRuntimeException("Tried to join nonexistent thread.");
            else
            {
                td.InternalThread.Join();
                return td.Context.LastReturnValue;
            }
        }

        [ZZFunction("thread", "tDone")]
        public static ZZInteger ThreadHasFinished(ZZInteger threadIdx)
        {
            ThreadBackend.ThreadData td = ThreadBackend.GetThreadAtIdx((int)threadIdx.Value);

            if (td == null)
                throw new InterpreterRuntimeException("Tried to read attribute of nonexistent thread.");
            else
                return td.Context.HasReturned ? 1 : 0;
        }
    }
}
