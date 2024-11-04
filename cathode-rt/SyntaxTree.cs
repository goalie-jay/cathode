using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cathode_rt
{
    public class SyntaxTree
    {
        public enum SyntaxTreeNodeType : byte
        {
            FUNCTIONBODY,

            // BINARY
            ADD,
            SUB,
            MUL,
            DIV,
            XOR,
            REMAINDERDIV,
            LOGICALAND,
            LOGICALOR,
            LOGICALEQUALS,
            LESSTHAN,
            GREATERTHAN,
            LOGICALNOTEQUALS,
            ASSIGNVARIABLE,
            ASSIGNSTRUCTFIELD,
            ASSIGNARRAYIDX,
            DIMENSIONANDASSIGN,

            // UNARY,
            NOT,
            FLIPSIGN,

            // VALUE
            ARRAY,
            STRUCTFIELD,
            ARRAYIDX,
            INTEGERCONSTANT,
            FLOATCONSTANT,
            STRINGCONSTANT,
            VOID,
            VARIABLE,
            DIMENSION,
            INC,
            FNCALL,

            // CONTROL
            // - IF
            IF,
            THEN,
            // - WHILE
            WHILE,
            FOR,
            LOOP,
            // GENERIC CONTROLS
            ESC,
            RET,
            POST
        }

        public abstract class SyntaxTreeNode
        {
            public abstract SyntaxTreeNodeType NodeType { get; }
            public int Line;
            public string Function;

            public ZZObject ReturnValue = null;

            public abstract ZZObject Execute(ExecutionContext ctx);
        }

        public class FunctionBodySyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode[] Children;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.FUNCTIONBODY;

            public FunctionBodySyntaxTreeNode(SyntaxTreeNode[] children)
            {
                Children = children;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                Program.CurrentlyExecutingContext = ctx;

                for (int i = 0; i < Children.Length; ++i)
                {
                    Children[i].Execute(ctx);
                    if (ctx.HasReturned)
                        return ctx.LastReturnValue;

                    if (ctx.ExitUpper)
                        throw new InterpreterRuntimeException("Tried to use esc keyword outside of a loop.");
                }

                return ctx.LastReturnValue;
            }
        }

        public class AddSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.ADD;

            public AddSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);
                ZZObject returnValue = ZZVoid.Void;

                switch (lhsReturn.ObjectType)
                {
                    case ZZObjectType.INTEGER:
                        if (rhsReturn.ObjectType != ZZObjectType.INTEGER)
                            throw new InterpreterRuntimeException("The right-hand side of an integer binary " +
                                "expression was not an integer.");

                        returnValue = (ZZInteger)(((ZZInteger)lhsReturn).Value + ((ZZInteger)rhsReturn).Value);
                        break;
                    case ZZObjectType.FLOAT:
                        if (rhsReturn.ObjectType != ZZObjectType.FLOAT)
                            throw new InterpreterRuntimeException("The right-hand side of an floating point binary " +
                                "expression was not a floating point.");

                        returnValue = (ZZFloat)(((ZZFloat)lhsReturn).Value + ((ZZFloat)rhsReturn).Value);
                        break;
                    default:
                        throw new InterpreterRuntimeException("Invalid binary expression.");
                }

                return returnValue;
            }
        }

        public class XorSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.XOR;

            public XorSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);
                ZZObject returnValue = ZZVoid.Void;

                switch (lhsReturn.ObjectType)
                {
                    case ZZObjectType.INTEGER:
                        if (rhsReturn.ObjectType != ZZObjectType.INTEGER)
                            throw new InterpreterRuntimeException("The right-hand side of an integer binary " +
                                "expression was not an integer.");

                        returnValue = (ZZInteger)(((ZZInteger)lhsReturn).Value ^ ((ZZInteger)rhsReturn).Value);
                        break;
                    default:
                        throw new InterpreterRuntimeException("Invalid binary expression.");
                }

                return returnValue;
            }
        }

        public class SubSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.SUB;

            public SubSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);
                ZZObject returnValue = ZZVoid.Void;

                switch (lhsReturn.ObjectType)
                {
                    case ZZObjectType.INTEGER:
                        if (rhsReturn.ObjectType != ZZObjectType.INTEGER)
                            throw new InterpreterRuntimeException("The right-hand side of an integer binary " +
                                "expression was not an integer.");

                        returnValue = (ZZInteger)(((ZZInteger)lhsReturn).Value - ((ZZInteger)rhsReturn).Value);
                        break;
                    case ZZObjectType.FLOAT:
                        if (rhsReturn.ObjectType != ZZObjectType.FLOAT)
                            throw new InterpreterRuntimeException("The right-hand side of an floating point binary " +
                                "expression was not a floating point.");

                        returnValue = (ZZFloat)(((ZZFloat)lhsReturn).Value - ((ZZFloat)rhsReturn).Value);
                        break;
                    default:
                        throw new InterpreterRuntimeException("Invalid binary expression.");
                }

                return returnValue;
            }
        }

        public class MulSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.MUL;

            public MulSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);
                ZZObject returnValue = ZZVoid.Void;

                switch (lhsReturn.ObjectType)
                {
                    case ZZObjectType.INTEGER:
                        if (rhsReturn.ObjectType != ZZObjectType.INTEGER)
                            throw new InterpreterRuntimeException("The right-hand side of an integer binary " +
                                "expression was not an integer.");

                        returnValue = (ZZInteger)(((ZZInteger)lhsReturn).Value * ((ZZInteger)rhsReturn).Value);
                        break;
                    case ZZObjectType.FLOAT:
                        if (rhsReturn.ObjectType != ZZObjectType.FLOAT)
                            throw new InterpreterRuntimeException("The right-hand side of an floating point binary " +
                                "expression was not a floating point.");

                        returnValue = (ZZFloat)(((ZZFloat)lhsReturn).Value * ((ZZFloat)rhsReturn).Value);
                        break;
                    default:
                        throw new InterpreterRuntimeException("Invalid binary expression.");
                }

                return returnValue;
            }
        }

        public class DivSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.DIV;

            public DivSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);
                ZZObject returnValue = ZZVoid.Void;

                switch (lhsReturn.ObjectType)
                {
                    case ZZObjectType.INTEGER:
                        if (rhsReturn.ObjectType != ZZObjectType.INTEGER)
                            throw new InterpreterRuntimeException("The right-hand side of an integer binary " +
                                "expression was not an integer.");

                        returnValue = (ZZInteger)(((ZZInteger)lhsReturn).Value / ((ZZInteger)rhsReturn).Value);
                        break;
                    case ZZObjectType.FLOAT:
                        if (rhsReturn.ObjectType != ZZObjectType.FLOAT)
                            throw new InterpreterRuntimeException("The right-hand side of an floating point binary " +
                                "expression was not a floating point.");

                        returnValue = (ZZFloat)(((ZZFloat)lhsReturn).Value / ((ZZFloat)rhsReturn).Value);
                        break;
                    default:
                        throw new InterpreterRuntimeException("Invalid binary expression.");
                }

                return returnValue;
            }
        }

        public class RemainderDivSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.REMAINDERDIV;

            public RemainderDivSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);
                ZZObject returnValue = ZZVoid.Void;

                switch (lhsReturn.ObjectType)
                {
                    case ZZObjectType.INTEGER:
                        if (rhsReturn.ObjectType != ZZObjectType.INTEGER)
                            throw new InterpreterRuntimeException("The right-hand side of an integer binary " +
                                "expression was not an integer.");

                        returnValue = (ZZInteger)(((ZZInteger)lhsReturn).Value % ((ZZInteger)rhsReturn).Value);
                        break;
                    case ZZObjectType.FLOAT:
                        if (rhsReturn.ObjectType != ZZObjectType.FLOAT)
                            throw new InterpreterRuntimeException("The right-hand side of an floating point binary " +
                                "expression was not a floating point.");

                        returnValue = (ZZFloat)(((ZZFloat)lhsReturn).Value % ((ZZFloat)rhsReturn).Value);
                        break;
                    default:
                        throw new InterpreterRuntimeException("Invalid binary expression.");
                }

                return returnValue;
            }
        }

        public class LogicalAndSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.LOGICALAND;

            public LogicalAndSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);

                if (lhsReturn.ObjectType != ZZObjectType.INTEGER)
                    throw new InterpreterRuntimeException("Tried to use non-integer for left side of logical expression.");

                // Logical short-circuit... finally implemented!!
                if (((ZZInteger)lhsReturn).Value == 0)
                    return lhsReturn;

                ZZObject rhsReturn = Rhs.Execute(ctx);

                if (rhsReturn.ObjectType != ZZObjectType.INTEGER)
                    throw new InterpreterRuntimeException("Tried to use non-integer for right side for logical expression.");

                return ImplMethods.Both((ZZInteger)lhsReturn, (ZZInteger)rhsReturn);
            }
        }

        public class LogicalOrSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.LOGICALOR;

            public LogicalOrSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);

                if (lhsReturn.ObjectType != ZZObjectType.INTEGER)
                    throw new InterpreterRuntimeException("Tried to use non-integer for left side of logical expression.");

                // Logical short-circuit... finally implemented!!
                if (((ZZInteger)lhsReturn).Value != 0)
                    return lhsReturn;

                ZZObject rhsReturn = Rhs.Execute(ctx);

                if (rhsReturn.ObjectType != ZZObjectType.INTEGER)
                    throw new InterpreterRuntimeException("Tried to use non-integer for right side for logical expression.");

                return ImplMethods.Either((ZZInteger)lhsReturn, (ZZInteger)rhsReturn);
            }
        }

        public class LogicalEqualsSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.LOGICALEQUALS;

            public LogicalEqualsSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);

                return ImplMethods.Compare(lhsReturn, rhsReturn);
            }
        }

        public class LogicalNotEqualsSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.LOGICALNOTEQUALS;

            public LogicalNotEqualsSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);

                return ImplMethods.InverseCompare(lhsReturn, rhsReturn);
            }
        }

        public class LessThanSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.LESSTHAN;

            public LessThanSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);

                return ImplMethods.LessThan(lhsReturn, rhsReturn);
            }
        }

        public class GreaterThanSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.GREATERTHAN;

            public GreaterThanSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsReturn = Lhs.Execute(ctx);
                ZZObject rhsReturn = Rhs.Execute(ctx);

                return ImplMethods.GreaterThan(lhsReturn, rhsReturn);
            }
        }

        public class ArraySyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode[] Elements;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.ARRAY;

            public ArraySyntaxTreeNode(SyntaxTreeNode[] elements)
            {
                Elements = elements;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                List<ZZObject> objectsExecuted = new List<ZZObject>();
                foreach (SyntaxTreeNode elem in Elements)
                    objectsExecuted.Add(elem.Execute(ctx));

                return new ZZArray(objectsExecuted.ToArray());
            }
        }

        public class AssignVariableSyntaxTreeNode : SyntaxTreeNode
        {
            public string VariableName;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.ASSIGNVARIABLE;

            public AssignVariableSyntaxTreeNode(string variableName, SyntaxTreeNode rhs)
            {
                VariableName = variableName;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ctx.Variables[VariableName] = Rhs.Execute(ctx);
                return ctx.Variables[VariableName];
            }
        }

        public class StructFieldSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public string FieldName;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.STRUCTFIELD;

            public StructFieldSyntaxTreeNode(SyntaxTreeNode lhs, string fieldName)
            {
                Lhs = lhs;
                FieldName = fieldName;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsObj = Lhs.Execute(ctx);

                if (lhsObj.ObjectType != ZZObjectType.STRUCT)
                    throw new InterpreterRuntimeException("Tried to access field of a non-struct.");

                return ImplMethods.Field((ZZStruct)lhsObj, FieldName);
            }
        }

        public class AssignStructFieldSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public string FieldName;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.ASSIGNSTRUCTFIELD;

            public AssignStructFieldSyntaxTreeNode(SyntaxTreeNode lhs, string fieldName, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                FieldName = fieldName;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsResult = Lhs.Execute(ctx);

                if (lhsResult.ObjectType != ZZObjectType.STRUCT)
                    throw new InterpreterRuntimeException("Tried to access a field of a non-struct.");

                ZZStruct structure = (ZZStruct)lhsResult;
                ZZObject rhsResult = Rhs.Execute(ctx);

                if (structure.Fields.ContainsKey(FieldName))
                    structure.Fields[FieldName] = rhsResult;
                else
                    structure.Fields.Add(FieldName, rhsResult);

                return rhsResult;
            }
        }

        public class ArrayIdxSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Accessor;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.ARRAYIDX;

            public ArrayIdxSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode accessor)
            {
                Lhs = lhs;
                Accessor = accessor;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsObject = Lhs.Execute(ctx);
                ZZObject accessorResult = Accessor.Execute(ctx);

                if (accessorResult.ObjectType != ZZObjectType.INTEGER)
                    throw new InterpreterRuntimeException("Tried to use non-integer as an accessor value.");

                switch (lhsObject.ObjectType)
                {
                    case ZZObjectType.ARRAY:
                        {
                            ZZArray arr = (ZZArray)lhsObject;
                            ZZInteger accessor = (ZZInteger)accessorResult;

                            return arr.Objects[accessor.Value];
                        }
                    case ZZObjectType.STRING:
                        {
                            string strVal = ((ZZString)lhsObject).Contents;
                            ZZInteger accessor = (ZZInteger)accessorResult;

                            return (ZZString)strVal[(int)accessor.Value].ToString();
                        }
                    default:
                        throw new InterpreterRuntimeException("Tried to access non-array, non-string value as an array.");
                }
            }
        }

        public class NotSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.NOT;

            public NotSyntaxTreeNode(SyntaxTreeNode rhs)
            {
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject rhsObj = Rhs.Execute(ctx);

                if (rhsObj.ObjectType != ZZObjectType.INTEGER)
                    throw new InterpreterRuntimeException("Tried to invert non-integer value.");

                return ImplMethods.Negate((ZZInteger)rhsObj);
            }
        }

        public class FlipSignSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.FLIPSIGN;

            public FlipSignSyntaxTreeNode(SyntaxTreeNode rhs)
            {
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject rhsObj = Rhs.Execute(ctx);

                switch (rhsObj.ObjectType)
                {
                    case ZZObjectType.INTEGER:
                        return (ZZInteger)(-((ZZInteger)rhsObj).Value);
                    case ZZObjectType.FLOAT:
                        return (ZZFloat)(-((ZZFloat)rhsObj).Value);
                    default:
                        throw new InterpreterRuntimeException("Tried to change sign of non-float, non-integer expression.");
                }
            }
        }

        public class AssignArrayIdxSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Lhs;
            public SyntaxTreeNode Accessor;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.ASSIGNVARIABLE;

            public AssignArrayIdxSyntaxTreeNode(SyntaxTreeNode lhs, SyntaxTreeNode accessor, SyntaxTreeNode rhs)
            {
                Lhs = lhs;
                Accessor = accessor;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject lhsResult = Lhs.Execute(ctx);
                ZZObject accessorResult = Accessor.Execute(ctx);

                if (accessorResult.ObjectType != ZZObjectType.INTEGER)
                    throw new InterpreterRuntimeException("Tried use non-integer as an accessor value.");

                if (((ZZInteger)accessorResult).Value < 0)
                    throw new InterpreterRuntimeException("Tried to use negative integer as an accessor value.");

                ZZObject rhsResult = Rhs.Execute(ctx);

                long intAccess = ((ZZInteger)accessorResult).Value;
                switch (lhsResult.ObjectType)
                {
                    case ZZObjectType.ARRAY:
                        {
                            ZZArray arr = (ZZArray)lhsResult;
                            if (intAccess >= arr.Objects.Length)
                                throw new InterpreterRuntimeException("Tried to access memory outside the bounds of the array.");

                            arr.Objects[intAccess] = rhsResult;
                            break;
                        }
                    case ZZObjectType.STRING:
                        {
                            if (rhsResult.ObjectType != ZZObjectType.STRING)
                                throw new InterpreterRuntimeException("Tried to use non-string for character assignment.");

                            if (intAccess >= ((ZZString)lhsResult).Contents.Length)
                                throw new InterpreterRuntimeException("Tried to access memory outside the bounds of the string.");

                            string set = ((ZZString)rhsResult).Contents;

                            if (set.Length < 1)
                                throw new InterpreterRuntimeException("Tried to use empty string for character assignment.");

                            ZZString str = (ZZString)lhsResult;
                            char[] strContentsMutable = str.Contents.ToArray();
                            strContentsMutable[intAccess] = set[0];
                            str.Contents = new string(strContentsMutable);
                            break;
                        }
                    default:
                        throw new InterpreterRuntimeException("Tried to access non-array, non-string value as an array.");
                }

                return rhsResult;
            }
        }

        public class DimensionSyntaxTreeNode : SyntaxTreeNode
        {
            public string Name;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.DIMENSION;

            public DimensionSyntaxTreeNode(string name)
            {
                Name = name;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                if (ctx.Variables.ContainsKey(Name))
                    throw new InterpreterRuntimeException("Tried to dimension variable that already exists.");

                ctx.Variables.Add(Name, ZZVoid.Void);
                return ZZVoid.Void;
            }
        }

        public class DimensionAndAssignSyntaxTreeNode : SyntaxTreeNode
        {
            public string Name;
            public SyntaxTreeNode Rhs;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.DIMENSIONANDASSIGN;

            public DimensionAndAssignSyntaxTreeNode(string name, SyntaxTreeNode rhs)
            {
                Name = name;
                Rhs = rhs;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                if (ctx.Variables.ContainsKey(Name))
                    throw new InterpreterRuntimeException("Tried to dimension variable that already exists.");

                ZZObject rhsResult = Rhs.Execute(ctx);
                ctx.Variables.Add(Name, rhsResult);
                return rhsResult;
            }
        }

        public class IncrementSyntaxTreeNode : SyntaxTreeNode
        {
            public string Name;

            public IncrementSyntaxTreeNode(string name)
            {
                Name = name;
            }

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.INC;

            public override ZZObject Execute(ExecutionContext ctx)
            {
                if (ctx.Variables[Name].ObjectType != ZZObjectType.INTEGER)
                    throw new InterpreterRuntimeException("Tried to use inc operator on non-integer.");

                ZZInteger intVar = (ZZInteger)ctx.Variables[Name];

                // Be careful not to overwrite the special values
                switch (intVar.Value)
                {
                    case -1:
                    case 0:
                    case 1:
                        ctx.Variables[Name] = new ZZInteger(intVar.Value + 1);
                        break;
                    default:
                        FastOps.FastInc(intVar);
                        break;
                }

                return intVar;
            }
        }

        public class FunctionCallSyntaxTreeNode : SyntaxTreeNode
        {
            public string FunctionName;
            public SyntaxTreeNode[] Parameters;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.FNCALL;

            public FunctionCallSyntaxTreeNode(string fnName, SyntaxTreeNode[] parameters)
            {
                FunctionName = fnName;
                Parameters = parameters;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                FunctionBodySyntaxTreeNode userFnPrimaryNode = ctx.GetFunctionOrReturnNullIfNotPresent(FunctionName,
                    out ZZFunctionDescriptor descriptor);

                if (userFnPrimaryNode != null)
                {
                    if (descriptor.Arguments.Length != Parameters.Length)
                        throw new InterpreterRuntimeException("Tried to supply an incorrect amount of parameters in " +
                            "a function call.");

                    // Backup execution context
                    using (ExecutionContext callContext = new ExecutionContext())
                    {
                        foreach (ZZFunctionDescriptor fnDesc in ctx.FunctionsAndBodies.Keys)
                            callContext.FunctionsAndBodies.Add(fnDesc, ctx.FunctionsAndBodies[fnDesc]);

                        for (int i = 0; i < Parameters.Length; ++i)
                            callContext.Variables.Add(descriptor.Arguments[i],
                                Parameters[i].Execute(ctx)); // Line up our values with the parameter names

                        ZZObject callResult = userFnPrimaryNode.Execute(callContext);
                        return callResult;
                    }
                }
                else
                {
                    var methodInf = ctx.LookupBuiltInFunction(FunctionName);

                    if (methodInf == null)
                        throw new InterpreterRuntimeException("Tried to look up nonexistent function.");

                    try
                    {
                        ParameterInfo[] invokeParams = methodInf.GetParameters();
                        if (invokeParams.Length != Parameters.Length)
                            throw new TargetParameterCountException();

                        object[] fixedParams = new object[Parameters.Length];
                        for (int i = 0; i < Parameters.Length; ++i)
                            fixedParams[i] = Parameters[i].Execute(ctx);

                        ZZObject retVal = (ZZObject)methodInf.Invoke(null, fixedParams);
                        return retVal;
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
                }

                //var methodInf = ctx.LookupBuiltInFunction(FunctionName);

                //if (methodInf == null)
                //{
                //    // Check for user-defined function
                //    string[] userFn = ctx.GetFunctionOrReturnNullIfNotPresent(
                //        ((ZZString)identifier.Value).ToString(),
                //        out ZZFunctionDescriptor descriptor);

                //    if (userFn == null)
                //        throw new InterpreterRuntimeException("Function could not be resolved. Is there a typo, or " +
                //            "have you checked to make sure the appropriate namespace has been imported?");

                //    if (descriptor.Arguments.Length != parameters.Count)
                //        throw new InterpreterRuntimeException("Tried to supply an incorrect amount of parameters in " +
                //            "a function call.");

                //    // Backup execution context
                //    ExecutionContext context = Program.CurrentlyExecutingContext;
                //    using (ExecutionContext callContext = new ExecutionContext())
                //    {
                //        // Enable recursion
                //        foreach (ZZFunctionDescriptor fnDesc in context.FunctionsAndBodies.Keys)
                //            callContext.FunctionsAndBodies.Add(fnDesc, context.FunctionsAndBodies[fnDesc]);

                //        for (int i = 0; i < parameters.Count; ++i)
                //            callContext.Variables.Add(descriptor.Arguments[i],
                //                parameters[i]); // Line up our values with the parameter names

                //        ZZObject obj = EvaluateExpr(Executor.Execute(callContext, descriptor.Name, userFn));

                //        // Restore execution context
                //        Program.CurrentlyExecutingContext = context;
                //        return obj;
                //    }
                //}

                //try
                //{
                //    ZZObject retVal = EvaluateExpr((ZZObject)methodInf.Invoke(null, parameters.ToArray()));

                //    return retVal;
                //}
                //catch (InterpreterRuntimeException)
                //{
                //    throw;
                //}
                //catch (Exception ex)
                //{
                //    if (ex is ArgumentException || ex is System.Reflection.TargetParameterCountException)
                //        throw new InterpreterRuntimeException("Tried to supply either an incorrect amount of parameters " +
                //            "or parameters with incorrect types to a standard function. " +
                //            "Are you calling the correct function? Did you forget " +
                //            "a type conversion?");
                //    else if (ex is TargetInvocationException)
                //        throw ex.InnerException;
                //    else
                //        throw;
                //}
            }
        }

        public class IfSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Condition;
            public SyntaxTreeNode[] Primary;
            public SyntaxTreeNode[] Else;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.IF;

            public IfSyntaxTreeNode(SyntaxTreeNode condition, FunctionBodySyntaxTreeNode primary, FunctionBodySyntaxTreeNode _else = null)
            {
                Condition = condition;
                // Copy elements over so we can execute them our own way
                // We have different rules to contend with
                Primary = new SyntaxTreeNode[primary.Children.Length];
                Array.Copy(primary.Children, 0, Primary, 0, primary.Children.Length);

                if (_else == null)
                    Else = null;
                else
                {
                    Else = new SyntaxTreeNode[_else.Children.Length];
                    Array.Copy(_else.Children, 0, Else, 0, _else.Children.Length);
                }
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ZZObject conditional = Condition.Execute(ctx);

                if (conditional.ObjectType != ZZObjectType.INTEGER)
                    throw new InterpreterRuntimeException("Tried to use an if statement with a non-integer " +
                        "comparison value.");

                SyntaxTreeNode[] NodesToExecute;
                if (((ZZInteger)conditional).Value != 0)
                    NodesToExecute = Primary;
                else if (Else != null)
                    NodesToExecute = Else;
                else
                    return ZZVoid.Void;

                // Clone our context
                ExecutionContext ctxCloned = ExecutionContext.Dup(ctx);
                Program.CurrentlyExecutingContext = ctxCloned;
                for (int i = 0; i < NodesToExecute.Length; ++i)
                {
                    NodesToExecute[i].Execute(ctxCloned);

                    if (ctxCloned.HasReturned)
                    {
                        ctx.HasReturned = ctxCloned.HasReturned;
                        ctx.UpdateSharedValues(ctxCloned);
                        Program.CurrentlyExecutingContext = ctx;
                        return ZZVoid.Void; // It will fall upwards until it meets a function body,
                                            // we don't have to do anything
                    }

                    if (ctxCloned.ExitUpper)
                    {
                        // Quit, but pass it on
                        ctx.UpdateSharedValues(ctxCloned);
                        ctx.ExitUpper = ctxCloned.ExitUpper;
                        Program.CurrentlyExecutingContext = ctx;
                        return ZZVoid.Void;
                    }
                }

                ctx.UpdateSharedValues(ctxCloned);
                Program.CurrentlyExecutingContext = ctx;
                ctx.HasReturned = ctxCloned.HasReturned;
                ctx.ExitUpper = ctxCloned.ExitUpper;

                return ZZVoid.Void;
            }
        }

        public class ThenSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.THEN;

            public ThenSyntaxTreeNode() { }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                return ZZVoid.Void;
            }
        }

        public class WhileSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Condition;
            public SyntaxTreeNode[] Body;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.WHILE;

            public WhileSyntaxTreeNode(SyntaxTreeNode condition, FunctionBodySyntaxTreeNode body)
            {
                Condition = condition;

                // Copy elements over so we can execute them our own way
                // We have different rules to contend with
                Body = new SyntaxTreeNode[body.Children.Length];
                Array.Copy(body.Children, 0, Body, 0, body.Children.Length);
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                while (true)
                {
                    ZZObject conditionEval = Condition.Execute(ctx);
                    if (conditionEval.ObjectType != ZZObjectType.INTEGER)
                        throw new InterpreterRuntimeException("Tried to use a while loop with a non-integer " +
                            "comparison value.");

                    if (((ZZInteger)conditionEval).Value == 0)
                        break;

                    // Create new context
                    ExecutionContext whileCtx = ExecutionContext.Dup(ctx);
                    Program.CurrentlyExecutingContext = whileCtx;
                    for (int i = 0; i < Body.Length; ++i)
                    {
                        Body[i].Execute(whileCtx);

                        if (whileCtx.HasReturned)
                        {
                            ctx.HasReturned = whileCtx.HasReturned;
                            ctx.UpdateSharedValues(whileCtx);
                            Program.CurrentlyExecutingContext = ctx;
                            return ZZVoid.Void; // It will fall upwards until it meets a function body,
                                                // we don't have to do anything
                        }

                        if (whileCtx.ExitUpper)
                        {
                            // Eat it and quit
                            whileCtx.ExitUpper = false;
                            ctx.UpdateSharedValues(whileCtx);
                            Program.CurrentlyExecutingContext = ctx;
                            return ZZVoid.Void;
                        }
                    }

                    ctx.UpdateSharedValues(whileCtx);
                    Program.CurrentlyExecutingContext = ctx;
                }

                return ZZVoid.Void;
            }
        }

        public class ForSyntaxTreeNode : SyntaxTreeNode
        {
            public SyntaxTreeNode Initial;
            public SyntaxTreeNode Condition;
            public SyntaxTreeNode Increment;
            public SyntaxTreeNode[] Body;

            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.FOR;

            public ForSyntaxTreeNode(SyntaxTreeNode initial, SyntaxTreeNode condition, SyntaxTreeNode increment, 
                FunctionBodySyntaxTreeNode body)
            {
                Initial = initial;
                Condition = condition;
                Increment = increment;

                // Copy elements over so we can execute them our own way
                // We have different rules to contend with
                Body = new SyntaxTreeNode[body.Children.Length];
                Array.Copy(body.Children, 0, Body, 0, body.Children.Length);
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                // Create wrapper context for the inner while loop

                ExecutionContext forWrapperCtx = ExecutionContext.Dup(ctx);

                Initial.Execute(forWrapperCtx);

                while (true)
                {
                    ZZObject conditionEval = Condition.Execute(forWrapperCtx);
                    if (conditionEval.ObjectType != ZZObjectType.INTEGER)
                        throw new InterpreterRuntimeException("Tried to use a while loop with a non-integer " +
                            "comparison value.");

                    if (((ZZInteger)conditionEval).Value == 0)
                        break;

                    // Create new context
                    ExecutionContext whileCtx = ExecutionContext.Dup(forWrapperCtx);
                    Program.CurrentlyExecutingContext = whileCtx;
                    for (int i = 0; i < Body.Length; ++i)
                    {
                        Body[i].Execute(whileCtx);

                        if (whileCtx.HasReturned)
                        {
                            ctx.HasReturned = whileCtx.HasReturned;
                            ctx.UpdateSharedValues(whileCtx);
                            Program.CurrentlyExecutingContext = ctx;
                            return ZZVoid.Void; // It will fall upwards until it meets a function body,
                                                // we don't have to do anything
                        }

                        if (whileCtx.ExitUpper)
                        {
                            // Eat it and quit
                            whileCtx.ExitUpper = false;
                            ctx.UpdateSharedValues(whileCtx);
                            Program.CurrentlyExecutingContext = ctx;
                            return ZZVoid.Void;
                        }
                    }

                    forWrapperCtx.UpdateSharedValues(whileCtx);
                    ctx.UpdateSharedValues(whileCtx);
                    Program.CurrentlyExecutingContext = ctx;

                    Increment.Execute(forWrapperCtx);
                }

                return ZZVoid.Void;
            }
        }

        public class LoopSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.LOOP;

            public LoopSyntaxTreeNode() { }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                return ZZVoid.Void;
            }
        }

        public class EscSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.ESC;

            public EscSyntaxTreeNode() { }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ctx.ExitUpper = true;
                return ZZVoid.Void;
            }
        }

        public class RetSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.RET;
            public SyntaxTreeNode Expr;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expr">If expr is null, return value will not be set</param>
            public RetSyntaxTreeNode(SyntaxTreeNode expr)
            {
                Expr = expr;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                if (Expr != null)
                    ctx.LastReturnValue = Expr.Execute(ctx);

                ctx.HasReturned = true;
                return ZZVoid.Void;
            }
        }

        public class PostSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.RET;
            public SyntaxTreeNode Expr;

            public PostSyntaxTreeNode(SyntaxTreeNode expr)
            {
                Expr = expr;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                ctx.LastReturnValue = Expr.Execute(ctx);
                return ZZVoid.Void;
            }
        }

        public class VariableSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.VARIABLE;
            public string Name;

            public VariableSyntaxTreeNode(string name)
            {
                Name = name;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                if (!ctx.Variables.ContainsKey(Name))
                    throw new InterpreterRuntimeException("Tried to look up value of nonexistent variable.");

                return ctx.Variables[Name];
            }
        }

        public class IntegerSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.INTEGERCONSTANT;
            public ZZInteger Value;

            public IntegerSyntaxTreeNode(ZZInteger value)
            {
                Value = value;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                return Value;
            }
        }

        public class VoidSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.VOID;

            public VoidSyntaxTreeNode() { }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                return ZZVoid.Void;
            }
        }

        public class FloatSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.FLOATCONSTANT;
            public ZZFloat Value;

            public FloatSyntaxTreeNode(ZZFloat value)
            {
                Value = value;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                return Value;
            }
        }

        public class StringSyntaxTreeNode : SyntaxTreeNode
        {
            public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.STRINGCONSTANT;
            public ZZString Value;

            public StringSyntaxTreeNode(ZZString value)
            {
                Value = value;
            }

            public override ZZObject Execute(ExecutionContext ctx)
            {
                return Value;
            }
        }

        List<SyntaxTreeNode> PrimaryNodes;

        public SyntaxTree()
        {
            PrimaryNodes = new List<SyntaxTreeNode>();
        }

        public FunctionBodySyntaxTreeNode GetMainNode()
        {
            return new FunctionBodySyntaxTreeNode(PrimaryNodes.ToArray());
        }

        public static SyntaxTree LoadBinaryImage(byte[] binaryImage)
        {
            throw new NotImplementedException();
        }

        public void AddNode(SyntaxTreeNode node) => PrimaryNodes.Add(node);
    }
}
