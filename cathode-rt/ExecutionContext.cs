using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public enum FunctionType
    { 
        BuiltIn,
        DefinedInLanguage
    }

    //public struct IncrementInformation
    //{
    //    public const int Increment = 1;
    //    public int Pointer = 0;
    //    public long[] Values = Array.Empty<long>();

    //    public IncrementInformation() { }
    //};

    public class ExecutionContext : IDisposable
    {
        bool _disposed = false;
        public Dictionary<ZZFunctionDescriptor, string[]> FunctionsAndBodies;
        public Dictionary<string, ZZObject> Variables;
        public List<string> ImportedNamespaces;
        public Stack<ZZInteger> ComparisonStack;
        public Stack<ZZInteger> IfSkipStack;
        public Dictionary<string, System.Reflection.MethodInfo> HotFunctions 
            = new Dictionary<string, System.Reflection.MethodInfo>();
        public ZZObject LastReturnValue;
        // Used for optimizations involving inc()
        // public Dictionary<ZZInteger, IncrementInformation> IncrementTable;

        public Stack<byte> WhileSkipStack; // Used for the interpreter to know if it should skip a while() loop's
                                           //   body
        public bool HasReturned;    // Used to indicate that we are done executing

        bool _returnWhileValue;
        bool _forceNextWhileToEvaluateFalseValue;

        /// <summary>
        /// MUST BE SET *AFTER* RETURNWHILE IF SETTING TO TRUE
        /// </summary>
        public bool ForceNextWhileToEvaluateFalse
        {
            get
            {
                return _forceNextWhileToEvaluateFalseValue;
            }
            set
            {
                if (value == true)
                    Debug.Assert(ReturnWhile);

                _forceNextWhileToEvaluateFalseValue = value;
            }
        }

        public bool ReturnWhile
        {
            get
            {
                return _returnWhileValue;
            }
            set
            {
                if (value == true)
                    Debug.Assert(WhileReturnLines.Count > 0);

                _returnWhileValue = value;
            }
        } // Used to indicate that we would like to loop on a while() block

        public bool RequestedWhileLoop; // A boolean used to indicate that the interpreter has requested a while
                                        //      loop to begin

        public Stack<int> WhileReturnLines; // A stack of lines to return to at the end of while loops

        public void AddFunctionDictionary(Dictionary<ZZFunctionDescriptor, string[]> dict)
        {
            foreach (ZZFunctionDescriptor key in dict.Keys)
                if (GetFunctionOrReturnNullIfNotPresent(key.Name, out _) == null)
                    FunctionsAndBodies.Add(key, dict[key]);
                else
                    throw new PreprocessorException("There was a function name collision involving one or " +
                        "more imported functions. This is not allowed.");
        }

        public void EnterWhileLoop(string line)
        {
            RequestedWhileLoop = true;
        }

        public void AcceptWhileLoop()
        {
            RequestedWhileLoop = false;
        }

        public static ExecutionContext Copy(ExecutionContext other)
        {
            ExecutionContext nw = new ExecutionContext();

            foreach (ZZFunctionDescriptor key in other.FunctionsAndBodies.Keys)
                nw.FunctionsAndBodies.Add(key, other.FunctionsAndBodies[key]);

            foreach (string varName in other.Variables.Keys)
                nw.Variables.Add(varName, other.Variables[varName]);

            foreach (string hfName in other.HotFunctions.Keys)
                nw.HotFunctions.Add(hfName, other.HotFunctions[hfName]);

            //foreach (ZZInteger incVar in other.IncrementTable.Keys)
            //    nw.IncrementTable.Add(incVar, other.IncrementTable[incVar]);

            nw.ImportedNamespaces.AddRange(other.ImportedNamespaces);
            nw.ComparisonStack = new Stack<ZZInteger>(new Stack<ZZInteger>(other.ComparisonStack));
            nw.IfSkipStack = new Stack<ZZInteger>(new Stack<ZZInteger>(other.IfSkipStack));

            nw.WhileSkipStack = new Stack<byte>(new Stack<byte>(other.WhileSkipStack));
            nw.RequestedWhileLoop = other.RequestedWhileLoop;
            nw.WhileReturnLines = new Stack<int>(new Stack<int>(other.WhileReturnLines));

            nw.LastReturnValue = other.LastReturnValue;

            nw.HasReturned = other.HasReturned;
            nw._returnWhileValue = other._returnWhileValue;
            nw._forceNextWhileToEvaluateFalseValue = other._forceNextWhileToEvaluateFalseValue;

            return nw;
        }

        public ExecutionContext(bool main = false)
        {
            FunctionsAndBodies = new Dictionary<ZZFunctionDescriptor, string[]>();
            Variables = new Dictionary<string, ZZObject>();
            ImportedNamespaces = new List<string>();
            ComparisonStack = new Stack<ZZInteger>();
            IfSkipStack = new Stack<ZZInteger>();

            WhileSkipStack = new Stack<byte>();
            RequestedWhileLoop = false;
            WhileReturnLines = new Stack<int>();

            // IncrementTable = new Dictionary<ZZInteger, IncrementInformation>();

            HotFunctions = new Dictionary<string, System.Reflection.MethodInfo>();

            if (main)
                LastReturnValue = ZZInteger.Zero;
            else
                LastReturnValue = ZZVoid.Void;

            HasReturned = false;
            ReturnWhile = false;
            ForceNextWhileToEvaluateFalse = false;

            ImportedNamespaces.Add("core"); // Always present
        }

        public string[] GetFunctionOrReturnNullIfNotPresent(string fnName, out ZZFunctionDescriptor outDescriptor)
        {
            if (_disposed)
                throw new ObjectDisposedException("ExecutionContext");

            outDescriptor = null;

            foreach (var f in FunctionsAndBodies.Keys)
                if (f.Name == fnName && ImportedNamespaces.Contains(f.Namespace))
                {
                    outDescriptor = f;
                    return FunctionsAndBodies[f];
                }

            return null;
        }

        //public long GetIncrementResultUsingTable(ZZInteger obj)
        //{
        //    IncrementInformation cInfo = new IncrementInformation();

        //    if (IncrementTable.ContainsKey(obj))
        //        cInfo = IncrementTable[obj];

        //    if (cInfo.Pointer >= cInfo.Values.Length)
        //    {
        //        // Generate more values
        //        cInfo.Values = new long[100];
        //        cInfo.Pointer = 0;

        //        for (long i = 0; i < cInfo.Values.Length; ++i)
        //            cInfo.Values[i] = obj.Value + i + 1;
        //    }

        //    int ptr = cInfo.Pointer;
        //    ++cInfo.Pointer;

        //    return cInfo.Values[ptr];
        //}

        public System.Reflection.MethodInfo LookupBuiltInFunction(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException("ExecutionContext");

            if (HotFunctions.ContainsKey(name))
                return HotFunctions[name];

            Type impl = typeof(ImplMethods);
            System.Reflection.MethodInfo[] methods = impl.GetMethods();

            foreach (System.Reflection.MethodInfo method in methods)
            {
                // In language name attribute
                ZZFunction inLanguageNameAttr = 
                    (ZZFunction)method.GetCustomAttributes(typeof(ZZFunction), false).FirstOrDefault();

                if (inLanguageNameAttr != null && inLanguageNameAttr.InLanguageName == name && 
                    ImportedNamespaces.Contains(inLanguageNameAttr.Namespace))
                {
                    // Cache it so that we don't have to do this shit again. This saves *A TON* of time
                    HotFunctions.Add(name, method);
                    return method;
                }
            }

            return null;
        }

        public void AddFunc(ZZFunctionDescriptor desc, string[] contents)
        {
            FunctionsAndBodies.Add(desc, contents);
        }

        public void ImportNamespace(string ns)
        {
            if (!ImportedNamespaces.Contains(ns))
                ImportedNamespaces.Add(ns);
        }

        public void UnloadNamespace(string ns)
        {
            ImportedNamespaces.Remove(ns);
        }

        public void Dispose()
        {
            _disposed = true;

            ImportedNamespaces.Clear();
            Variables.Clear();
            ComparisonStack.Clear();
            FunctionsAndBodies.Clear();
        }
    }
}
