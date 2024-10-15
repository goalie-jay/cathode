using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {
        [ZZFunction("core", "PtrZero")]
        public static ZZInteger PointerIsZero(ZZLongPointer lp)
        {
            if (lp.Pointer == UIntPtr.Zero)
                return 1;

            return 0;
        }

        [ZZFunction("core", "VoidArr")]
        public static ZZArray VoidArr(ZZInteger length)
        {
            if (length.Value == 0)
                return new ZZArray(Array.Empty<ZZObject>());

            ZZObject[] objects = new ZZObject[length.Value];
            for (int i = 0; i < length.Value; ++i)
                objects[i] = ZZVoid.Void;

            return new ZZArray(objects);
        }

        [ZZFunction("core", "PtrAddOffs")]
        public static ZZLongPointer PointerAddOffs(ZZLongPointer lp, ZZInteger offs)
        {
            return new ZZLongPointer(UIntPtr.Add(lp.Pointer, (int)offs.Value));
        }

        [ZZFunction("core", "RuntimeInfo")]
        public static ZZStruct GetRuntimeInfo()
        {
            ZZStruct strct = new ZZStruct();

            strct.Fields.Add("ExecutingFile", new ZZString(Program.ExecutingFile));
            strct.Fields.Add("ProcessId", new ZZInteger(Environment.ProcessId));
            strct.Fields.Add("NameOfUser", new ZZString(Environment.UserName));
            strct.Fields.Add("NameOfMachine", new ZZString(Environment.MachineName));
            strct.Fields.Add("X64", new ZZInteger(Environment.Is64BitOperatingSystem ? 1 : 0));

            ZZStruct interpreterInfo = new ZZStruct();
            interpreterInfo.Fields.Add("MajorVersionNumber", new ZZInteger(Program.MajorVersionNumber));
            interpreterInfo.Fields.Add("MinorVersionNumber", new ZZInteger(Program.MinorVersionNumber));
            interpreterInfo.Fields.Add("IncrementVersionNumber", new ZZInteger(Program.IncrementVersionNumber));

            strct.Fields.Add("InterpreterInfo", interpreterInfo);

            return strct;
        }

        [ZZFunction("core", "fAbs")]
        public static ZZFloat fAbs(ZZFloat value)
        {
            return new ZZFloat(Math.Abs(value.Value));
        }

        [ZZFunction("core", "Abs")]
        public static ZZInteger Abs(ZZInteger value)
        {
            return new ZZInteger(Math.Abs(value.Value));
        }

        [ZZFunction("core", "BytesToStr")]
        public static ZZObject BytesToStr(ZZArray bytes, ZZString encoding)
        {
            try
            {
                List<byte> realBytes = new List<byte>();

                foreach (ZZObject obj in bytes.Objects)
                    if (obj.ObjectType == ZZObjectType.BYTE)
                        realBytes.Add(((ZZByte)obj).Value);
                    else
                        throw new ArgumentException();

                switch (encoding.Contents)
                {
                    case "ascii":
                        return new ZZString(Encoding.ASCII.GetString(realBytes.ToArray()));
                    case "unicode":
                        return new ZZString(Encoding.Unicode.GetString(realBytes.ToArray()));
                    case "utf8":
                        return new ZZString(Encoding.UTF8.GetString(realBytes.ToArray()));
                    default:
                        throw new ArgumentException();
                }
            }
            catch { return ZZVoid.Void; }
        }

        [ZZFunction("core", "StrToBytes")]
        public static ZZObject StrToBytes(ZZString str, ZZString encoding)
        {
            try
            {
                byte[] bytes = { };

                switch (encoding.Contents)
                {
                    case "ascii":
                        bytes = Encoding.ASCII.GetBytes(str.Contents);
                        break;
                    case "unicode":
                        bytes = Encoding.Unicode.GetBytes(str.Contents);
                        break;
                    case "utf8":
                        bytes = Encoding.UTF8.GetBytes(str.Contents);
                        break;
                    default:
                        throw new ArgumentException();
                }

                List<ZZObject> arrContents = new List<ZZObject>();
                foreach (byte b in bytes)
                    arrContents.Add(new ZZByte(b));

                return new ZZArray(arrContents.ToArray());
            }
            catch { return ZZVoid.Void; }
        }

        [ZZFunction("core", "Arraylen")]
        public static ZZInteger ArrayLength(ZZArray arr)
        {
            return arr.Objects.Length;
        }

        [ZZFunction("core", "Strlen")]
        public static ZZInteger StrLen(ZZString str)
        {
            return str.Length;
        }

        [ZZFunction("core", "Format")]
        public static ZZString FormatStr(ZZString str, ZZArray objs)
        {
            ZZString nw = str;

            for (int i = 0; i < objs.Objects.Length; ++i)
                nw.Contents = str.Contents.Replace("$" + i.ToString(), 
                    objs.Objects[i].ToInLanguageString().Contents);

            return nw;
        }

        // TODO: Document
        [ZZFunction("core", "Bytes")]
        public static ZZArray ConvertToBinary(ZZObject obj)
        {
            switch (obj.ObjectType)
            {
                case ZZObjectType.BYTE:
                    return new ZZArray(new ZZObject[] { obj });
                case ZZObjectType.INTEGER:
                    {
                        byte[] binary = BitConverter.GetBytes(((ZZInteger)obj).Value);
                        List<ZZByte> bytes = new List<ZZByte>();
                        foreach (byte b in binary)
                            bytes.Add(b);

                        return new ZZArray(bytes.ToArray());
                    }
                case ZZObjectType.FLOAT:
                    {
                        byte[] binary = BitConverter.GetBytes(((ZZFloat)obj).Value);
                        List<ZZByte> bytes = new List<ZZByte>();
                        foreach (byte b in binary)
                            bytes.Add(b);

                        return new ZZArray(bytes.ToArray());
                    }
                default:
                    throw new InterpreterRuntimeException("Tried to convert object to binary which has no direct binary representation.");
            }
        }

        [ZZFunction("core", "Strcat")]
        public static ZZString ConcatenateStrings(ZZArray arr, ZZObject separator)
        {
            if (arr.Objects.Length < 1)
                return new ZZString(string.Empty);

            List<string> realStrings = new List<string>();

            foreach (ZZObject obj in arr.Objects)
                if (obj.ObjectType == ZZObjectType.STRING)
                    realStrings.Add(((ZZString)obj).Contents);
                else
                    throw new ArgumentException();

            if (separator.ObjectType == ZZObjectType.VOID)
                return new ZZString(string.Concat(realStrings));
            else if (separator.ObjectType == ZZObjectType.STRING)
                return new ZZString(string.Join(((ZZString)separator).Contents, realStrings));

            throw new ArgumentException();
        }

        [ZZFunction("core", "EnumFields")]
        public static ZZArray EnumFields(ZZStruct strct)
        {
            return new ZZArray(strct.Fields.Keys.ToArray());
        }

        [ZZFunction("core", "HasField")]
        public static ZZInteger HasField(ZZStruct strct, ZZString name)
        {
            return strct.Fields.ContainsKey(name) ? 1 : 0;
        }

        [ZZFunction("core", "Field")]
        public static ZZObject Field(ZZStruct strct, ZZString name)
        {
            if (!strct.Fields.ContainsKey(name))
                throw new InterpreterRuntimeException("Struct does not contain the given field.");

            return strct.Fields[name];
        }

        [ZZFunction("core", "SetField")]
        public static ZZVoid SetField(ZZStruct strct, ZZString name, ZZObject obj)
        {
            if (!strct.Fields.ContainsKey(name))
                strct.Fields.Add(name, obj);
            else
                strct.Fields[name] = obj;

            return ZZVoid.Void;
        }

        //[ZZFunction("core", "StructData")]
        //public static ZZStruct StructData(ZZArray arr)
        //{
        //    return new ZZStruct(arr);
        //}

        [ZZFunction("core", "Struct")]
        public static ZZStruct BlankStruct()
        {
            return new ZZStruct();
        }

        [ZZFunction("core", "CloneStruct")]
        public static ZZStruct CloneStruct(ZZStruct other)
        {
            ZZStruct strct = BlankStruct();

            foreach (ZZString fieldName in other.Fields.Keys)
            {
                ZZObject obj = other.Fields[fieldName];
                strct.Fields.Add(fieldName, other.Fields[fieldName]);
            }

            return strct;
        }

        [ZZFunction("core", "CloneString")]
        public static ZZString CloneString(ZZString other)
        {
            return new ZZString(other.Contents);
        }

        [ZZFunction("core", "CloneArray")]
        public static ZZArray CloneArray(ZZArray other)
        {
            List<ZZObject> objs = new List<ZZObject>();

            foreach (ZZObject obj in other.Objects)
                objs.Add(obj);

            return new ZZArray(objs.ToArray());
        }

        [ZZFunction("core", "Assert")]
        public static ZZVoid Assert(ZZInteger test, ZZString failureMsg)
        {
            if (test.Value == 0)
                ThrowException(failureMsg);

            return ZZVoid.Void;
        }

        [ZZFunction("core", "Except")]
        public static ZZVoid ThrowException(ZZString msg)
        {
            throw new InterpreterRuntimeException(msg.Contents);
        }

        [ZZFunction("core", "Negate")]
        public static ZZInteger Negate(ZZInteger value)
        {
            return (value.Value == 0) ? 1 : 0;
        }

        [ZZFunction("core", "Both")]
        public static ZZInteger Both(ZZInteger first, ZZInteger second)
        {
            if (first.Value == 0)
                return 0;

            if (second.Value == 0)
                return 0;

            return 1;
        }

        [ZZFunction("core", "Either")]
        public static ZZInteger Either(ZZInteger first, ZZInteger second)
        {
            if (first.Value != 0)
                return 1;

            if (second.Value != 0)
                return 1;

            return 0;
        }

        [ZZFunction("core", "LessThan")]
        public static ZZInteger LessThan(ZZObject first, ZZObject second)
        {
            switch (first.ObjectType)
            {
                case ZZObjectType.INTEGER:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.INTEGER:
                            return (((ZZInteger)first).Value < ((ZZInteger)second).Value) ? 1 : 0;
                        case ZZObjectType.FLOAT:
                            return (((ZZInteger)first).Value < ((ZZFloat)second).Value) ? 1 : 0;
                        case ZZObjectType.BYTE:
                            return (((ZZInteger)first).Value < ((ZZByte)second).Value) ? 1 : 0;
                        default:
                            throw new ArgumentException("Tried to use LessThan() on a non-number data type.");
                    }
                case ZZObjectType.FLOAT:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.INTEGER:
                            return (((ZZFloat)first).Value < ((ZZInteger)second).Value) ? 1 : 0;
                        case ZZObjectType.FLOAT:
                            return (((ZZFloat)first).Value < ((ZZFloat)second).Value) ? 1 : 0;
                        case ZZObjectType.BYTE:
                            return (((ZZFloat)first).Value < ((ZZByte)second).Value) ? 1 : 0;
                        default:
                            throw new ArgumentException("Tried to use LessThan() on a non-number data type.");
                    }
                case ZZObjectType.BYTE:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.INTEGER:
                            return (((ZZByte)first).Value < ((ZZInteger)second).Value) ? 1 : 0;
                        case ZZObjectType.FLOAT:
                            return (((ZZByte)first).Value < ((ZZFloat)second).Value) ? 1 : 0;
                        case ZZObjectType.BYTE:
                            return (((ZZByte)first).Value < ((ZZByte)second).Value) ? 1 : 0;
                        default:
                            throw new ArgumentException("Tried to use LessThan() on a non-number data type.");
                    }
                default:
                    throw new ArgumentException("Tried to use LessThan() on a non-number data type.");
            }
        }

        [ZZFunction("core", "GreaterThan")]
        public static ZZInteger GreaterThan(ZZObject first, ZZObject second)
        {
            switch (first.ObjectType)
            {
                case ZZObjectType.INTEGER:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.INTEGER:
                            return (((ZZInteger)first).Value > ((ZZInteger)second).Value) ? 1 : 0;
                        case ZZObjectType.FLOAT:
                            return (((ZZInteger)first).Value > ((ZZFloat)second).Value) ? 1 : 0;
                        case ZZObjectType.BYTE:
                            return (((ZZInteger)first).Value > ((ZZByte)second).Value) ? 1 : 0;
                        default:
                            throw new ArgumentException("Tried to use GreaterThan() on a non-number data type.");
                    }
                case ZZObjectType.FLOAT:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.INTEGER:
                            return (((ZZFloat)first).Value > ((ZZInteger)second).Value) ? 1 : 0;
                        case ZZObjectType.FLOAT:
                            return (((ZZFloat)first).Value > ((ZZFloat)second).Value) ? 1 : 0;
                        case ZZObjectType.BYTE:
                            return (((ZZFloat)first).Value > ((ZZByte)second).Value) ? 1 : 0;
                        default:
                            throw new ArgumentException("Tried to use GreaterThan() on a non-number data type.");
                    }
                case ZZObjectType.BYTE:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.INTEGER:
                            return (((ZZByte)first).Value > ((ZZInteger)second).Value) ? 1 : 0;
                        case ZZObjectType.FLOAT:
                            return (((ZZByte)first).Value > ((ZZFloat)second).Value) ? 1 : 0;
                        case ZZObjectType.BYTE:
                            return (((ZZByte)first).Value > ((ZZByte)second).Value) ? 1 : 0;
                        default:
                            throw new ArgumentException("Tried to use GreaterThan() on a non-number data type.");
                    }
                default:
                    throw new ArgumentException("Tried to use GreaterThan() on a non-number data type.");
            }
        }

        [ZZFunction("core", "Equal")]
        public static ZZInteger Compare(ZZObject first, ZZObject second)
        {
            if (ReferenceEquals(first, second))
                return 1;

            if ((first.ObjectType == ZZObjectType.VOID) && (second.ObjectType == ZZObjectType.VOID))
                return 1;

            if (first.ObjectType == ZZObjectType.FILEHANDLE)
            {
                if (second.ObjectType != ZZObjectType.FILEHANDLE)
                    return 0;

                return (((ZZFileHandle)first).Stream.Handle == ((ZZFileHandle)second).Stream.Handle) ? 1 : 0;
            }

            if (first.ObjectType == ZZObjectType.LONGPOINTER)
            {
                if (second.ObjectType != ZZObjectType.LONGPOINTER)
                    return 0;

                return (((ZZLongPointer)first).Pointer == ((ZZLongPointer)second).Pointer) ? 1 : 0;
            }

            switch (first.ObjectType)
            {
                case ZZObjectType.STRING:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.STRING:
                            return (((ZZString)first).Contents == ((ZZString)second).Contents) ? 1 : 0;
                        default:
                            return 0;
                    }
                case ZZObjectType.INTEGER:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.INTEGER:
                            return (((ZZInteger)first).Value == ((ZZInteger)second).Value) ? 1 : 0;
                        case ZZObjectType.FLOAT:
                            return (((ZZInteger)first).Value == ((ZZFloat)second).Value) ? 1 : 0;
                        case ZZObjectType.BYTE:
                            return (((ZZInteger)first).Value == ((ZZByte)second).Value) ? 1 : 0;
                        default:
                            return 0;
                    }
                case ZZObjectType.FLOAT:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.INTEGER:
                            return (((ZZFloat)first).Value == ((ZZInteger)second).Value) ? 1 : 0;
                        case ZZObjectType.FLOAT:
                            return (((ZZFloat)first).Value == ((ZZFloat)second).Value) ? 1 : 0;
                        case ZZObjectType.BYTE:
                            return (((ZZFloat)first).Value == ((ZZByte)second).Value) ? 1 : 0;
                        default:
                            return 0;
                    }
                case ZZObjectType.BYTE:
                    switch (second.ObjectType)
                    {
                        case ZZObjectType.INTEGER:
                            return (((ZZByte)first).Value == ((ZZInteger)second).Value) ? 1 : 0;
                        case ZZObjectType.FLOAT:
                            return (((ZZByte)first).Value == ((ZZFloat)second).Value) ? 1 : 0;
                        case ZZObjectType.BYTE:
                            return (((ZZByte)first).Value == ((ZZByte)second).Value) ? 1 : 0;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }

        [ZZFunction("core", "NotEqual")]
        public static ZZInteger InverseCompare(ZZObject first, ZZObject second)
        {
            return Negate(Compare(first, second));
        }

        [ZZFunction("core", "typeof")]
        public static ZZString GetTypeName(ZZObject any)
        {
            return any.GetInLanguageTypeName();
        }

        [ZZFunction("core", "Exit")]
        public static ZZVoid Exit(ZZInteger code)
        {
            Environment.Exit((int)code.Value);
            return null;
        }

        [ZZFunction("core", "Byte")]
        public static ZZObject ConvertToByte(ZZObject obj)
        {
            ZZObject result = ConvertToInteger(obj);

            if (result.ObjectType == ZZObjectType.VOID)
                return result;

            return new ZZByte((byte)((ZZInteger)result).Value);
        }

        [ZZFunction("core", "ByteArrSmall")]
        public static ZZObject ByteArrSmall(ZZObject obj)
        {
            switch (obj.ObjectType)
            {
                case ZZObjectType.INTEGER:
                    {
                        List<ZZByte> bytes = new List<ZZByte>();
                        byte[] realBytes = BitConverter.GetBytes((int)((ZZInteger)obj).Value);
                        Debug.Assert(realBytes.Length == sizeof(int));

                        for (int i = 0; i < realBytes.Length; ++i)
                            bytes.Add(new ZZByte(realBytes[i]));

                        return new ZZArray(bytes.ToArray());
                    }
                default:
                    return ZZVoid.Void;
            }
        }

        [ZZFunction("core", "ByteArr")]
        public static ZZObject ByteArr(ZZObject obj)
        {
            switch (obj.ObjectType)
            {
                case ZZObjectType.INTEGER:
                    {
                        List<ZZByte> bytes = new List<ZZByte>();
                        byte[] realBytes = BitConverter.GetBytes(((ZZInteger)obj).Value);
                        Debug.Assert(realBytes.Length == sizeof(long));

                        for (int i = 0; i < realBytes.Length; ++i)
                            bytes.Add(new ZZByte(realBytes[i]));

                        return new ZZArray(bytes.ToArray());
                    }
                case ZZObjectType.FLOAT:
                    {
                        List<ZZByte> bytes = new List<ZZByte>();
                        byte[] realBytes = BitConverter.GetBytes(((ZZFloat)obj).Value);
                        Debug.Assert(realBytes.Length == sizeof(double));

                        for (int i = 0; i < realBytes.Length; ++i)
                            bytes.Add(new ZZByte(realBytes[i]));

                        return new ZZArray(bytes.ToArray());
                    }
                default:
                    return ZZVoid.Void;
            }
        }

        [ZZFunction("core", "Integer")]
        public static ZZObject ConvertToInteger(ZZObject obj)
        {
            switch (obj.ObjectType)
            {
                case ZZObjectType.INTEGER:
                    return obj;
                case ZZObjectType.FLOAT:
                    return new ZZInteger((long)((ZZFloat)obj).Value);
                case ZZObjectType.STRING:
                    if (int.TryParse(((ZZString)obj).Contents, out int strParsed))
                        return new ZZInteger(strParsed);
                    else
                        return ZZVoid.Void;
                case ZZObjectType.BYTE:
                    return new ZZInteger(((ZZByte)obj).Value);
                case ZZObjectType.FILEHANDLE:
                    return new ZZInteger(((ZZFileHandle)obj).Stream.Handle.ToInt64());
                case ZZObjectType.LONGPOINTER:
                    return new ZZInteger((long)((ZZLongPointer)obj).Pointer.ToUInt64());
                case ZZObjectType.ARRAY:
                    {
                        ZZArray arr = (ZZArray)obj;

                        byte[] realBytes = new byte[arr.Objects.Length];
                        for (int i = 0; i < arr.Objects.Length; ++i)
                        {
                            ZZObject elem = arr.Objects[i];

                            if (elem.ObjectType != ZZObjectType.BYTE)
                                return ZZVoid.Void;

                            realBytes[i] = ((ZZByte)elem).Value;
                        }

                        switch (realBytes.Length)
                        {
                            case sizeof(int):
                                return (ZZInteger)Convert.ToInt32(realBytes);
                            case sizeof(long):
                                return (ZZInteger)Convert.ToInt64(realBytes);
                            default:
                                return ZZVoid.Void;
                        }
                    }
                default:
                    return ZZVoid.Void;
            }
        }

        [ZZFunction("core", "Float")]
        public static ZZObject ConvertToFloat(ZZObject obj)
        {
            switch (obj.ObjectType)
            {
                case ZZObjectType.FLOAT:
                    return obj;
                case ZZObjectType.INTEGER:
                    return new ZZFloat(((ZZInteger)obj).Value);
                case ZZObjectType.BYTE:
                    return new ZZFloat(((ZZByte)obj).Value);
                case ZZObjectType.STRING:
                    if (double.TryParse(((ZZString)obj).Contents, out double strParsed))
                        return new ZZFloat(strParsed);
                    else
                        return ZZVoid.Void;
                default:
                    return ZZVoid.Void;
            }
        }

        [ZZFunction("core", "String")]
        public static ZZString ConvertToString(ZZObject any)
        {
            return any.ToInLanguageString();
        }

        [ZZFunction("core", "import")]
        public static ZZVoid ImportNamespace(ZZString ns)
        {
            Program.CurrentlyExecutingContext.ImportNamespace(ns.ToString());

            return ZZVoid.Void;
        }

        [ZZFunction("core", "Strcmp")]
        public static ZZInteger StrCmp(ZZString str1, ZZString str2)
        {
            if (str2.Length != str1.Length)
                return 1;

            string native1, native2;
            native1 = str1.ToString();
            native2 = str2.ToString();
            for (int i = 0; i < native1.Length; ++i)
                if (native1[i] != native2[i])
                    return native1[i] - native2[i];

            return 0;
        }

        [ZZFunction("core", "Uppercase")]
        public static ZZString Uppercase(ZZString str)
        {
            return new ZZString(str.Contents.ToUpper());
        }

        [ZZFunction("core", "Lowercase")]
        public static ZZString Lowercase(ZZString str)
        {
            return new ZZString(str.Contents.ToLower());
        }
    }
}
