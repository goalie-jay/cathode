using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public class ExecutionContext : IDisposable
    {
        bool _disposed = false;
        public Dictionary<ZZFunctionDescriptor, SyntaxTree.FunctionBodySyntaxTreeNode> FunctionsAndBodies;
        public Dictionary<string, ZZObject> Variables;
        public List<string> ImportedNamespaces;
        public Stack<ZZInteger> ComparisonStack;
        public Stack<ZZInteger> IfSkipStack;
        public Dictionary<string, System.Reflection.MethodInfo> HotFunctions 
            = new Dictionary<string, System.Reflection.MethodInfo>();
        public ZZObject LastReturnValue;
        public bool HasReturned;
        public bool ExitUpper;

        public void AddFunctionDictionary(Dictionary<ZZFunctionDescriptor, SyntaxTree.FunctionBodySyntaxTreeNode> dict)
        {
            foreach (ZZFunctionDescriptor key in dict.Keys)
                if (GetFunctionOrReturnNullIfNotPresent(key.Name, out _) == null)
                    FunctionsAndBodies.Add(key, dict[key]);
                else
                    throw new PreprocessorException("There was a function name collision involving one or " +
                        "more imported functions. This is not allowed.");
        }

        /// <summary>
        /// Copies the values of variables that already exist from source
        /// </summary>
        /// <param name="source"></param>
        public void UpdateSharedValues(ExecutionContext source)
        {
            foreach (string varName in source.Variables.Keys)
                if (Variables.ContainsKey(varName))
                    Variables[varName] = source.Variables[varName];

            LastReturnValue = source.LastReturnValue;
        }

        public static ExecutionContext Dup(ExecutionContext other)
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

            nw.LastReturnValue = other.LastReturnValue;

            nw.HasReturned = other.HasReturned;

            return nw;
        }

        public ExecutionContext(bool main = false)
        {
            Variables = new Dictionary<string, ZZObject>();
            ImportedNamespaces = new List<string>();
            ComparisonStack = new Stack<ZZInteger>();
            IfSkipStack = new Stack<ZZInteger>();
            FunctionsAndBodies = new Dictionary<ZZFunctionDescriptor, SyntaxTree.FunctionBodySyntaxTreeNode>();

            // IncrementTable = new Dictionary<ZZInteger, IncrementInformation>();

            HotFunctions = new Dictionary<string, System.Reflection.MethodInfo>();

            if (main)
                LastReturnValue = ZZInteger.Zero;
            else
                LastReturnValue = ZZVoid.Void;

            HasReturned = false;

            ExitUpper = false;

            ImportedNamespaces.Add("core"); // Always present
        }

        public SyntaxTree.FunctionBodySyntaxTreeNode GetFunctionOrReturnNullIfNotPresent(string fnName, out ZZFunctionDescriptor outDescriptor)
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

        public void AddFunc(ZZFunctionDescriptor desc, SyntaxTree.FunctionBodySyntaxTreeNode contents)
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
