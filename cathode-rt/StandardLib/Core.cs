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
        [ZZFunction("core", "StrToBytes")]
        public static ZZArray StrToBytes(ZZString str, ZZString format)
        {
            byte[] bytes = { };

            switch (format.Contents)
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
            }

            List<ZZObject> arrContents = new List<ZZObject>();
            foreach (byte b in bytes)
                arrContents.Add(new ZZByte(b));

            return new ZZArray(arrContents.ToArray());
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

        [ZZFunction("core", "Strcat")]
        public static ZZString ConcatenateStrings(ZZString str, ZZString str2)
        {
            return new ZZString(str.Contents + str2.Contents);
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

        [ZZFunction("core", "Setfield")]
        public static ZZVoid SetField(ZZStruct strct, ZZString name, ZZObject obj)
        {
            if (!strct.Fields.ContainsKey(name))
                strct.Fields.Add(name, obj);
            else
                strct.Fields[name] = obj;

            return new ZZVoid();
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

        [ZZFunction("core", "Assert")]
        public static ZZVoid Assert(ZZInteger test, ZZString failureMsg)
        {
            if (test == 0)
                ThrowException(failureMsg);

            return new ZZVoid();
        }

        [ZZFunction("core", "Except")]
        public static ZZVoid ThrowException(ZZString msg)
        {
            throw new InterpreterRuntimeException(msg.Contents);
        }

        [ZZFunction("core", "Negate")]
        public static ZZInteger Negate(ZZInteger value)
        {
            return (value == 0) ? 1 : 0;
        }

        [ZZFunction("core", "Both")]
        public static ZZInteger Both(ZZInteger first, ZZInteger second)
        {
            if (first == 0)
                return 0;

            if (second == 0)
                return 0;

            return 1;
        }

        [ZZFunction("core", "Either")]
        public static ZZInteger Either(ZZInteger first, ZZInteger second)
        {
            if (first != 0)
                return 1;

            if (second != 0)
                return 1;

            return 0;
        }

        [ZZFunction("core", "LessThan")]
        public static ZZInteger LessThan(ZZObject first, ZZObject second)
        {
            if (first is ZZInteger zint1)
                if (second is ZZInteger zint2)
                    return (zint1.Value < zint2.Value) ? 1 : 0;
                else if (second is ZZFloat zfloat2)
                    return ((float)zint1.Value < zfloat2.Value) ? 1 : 0;
                else
                    throw new ArgumentException("Tried to use LessThan() on a non-number data type.");
            else if (first is ZZFloat zfloat1)
                if (second is ZZInteger zint2)
                    return (zfloat1.Value < (float)zint2.Value) ? 1 : 0;
                else if (second is ZZFloat zfloat2)
                    return (zfloat1.Value < zfloat2.Value) ? 1 : 0;
                else
                    throw new ArgumentException("Tried to use LessThan() on a non-number data type.");
            else
                throw new ArgumentException("Tried to use LessThan() on a non-number data type.");
        }

        [ZZFunction("core", "GreaterThan")]
        public static ZZInteger GreaterThan(ZZObject first, ZZObject second)
        {
            if (first is ZZInteger zint1)
                if (second is ZZInteger zint2)
                    return (zint1.Value > zint2.Value) ? 1 : 0;
                else if (second is ZZFloat zfloat2)
                    return ((float)zint1.Value > zfloat2.Value) ? 1 : 0;
                else
                    throw new ArgumentException("Tried to use GreaterThan() on a non-number data type.");
            else if (first is ZZFloat zfloat1)
                if (second is ZZInteger zint2)
                    return (zfloat1.Value > (float)zint2.Value) ? 1 : 0;
                else if (second is ZZFloat zfloat2)
                    return (zfloat1.Value > zfloat2.Value) ? 1 : 0;
                else
                    throw new ArgumentException("Tried to use GreaterThan() on a non-number data type.");
            else
                throw new ArgumentException("Tried to use GreaterThan() on a non-number data type.");
        }

        [ZZFunction("core", "Equal")]
        public static ZZInteger Compare(ZZObject first, ZZObject second)
        {
            if (first is ZZString zstrA)
            {
                if (!(second is ZZString zstrB))
                    return 0;

                return (zstrA.Contents == zstrB.Contents) ? 1 : 0;
            }

            if (first is ZZInteger zintA)
            {
                if (!(second is ZZInteger zintB))
                    return 0;

                return (zintA.Value == zintB.Value) ? 1 : 0;
            }

            if (first is ZZFileHandle zfhandleA)
            {
                if (!(second is ZZFileHandle zfhandleB))
                    return 0;

                return (zfhandleA.Stream.Handle == zfhandleB.Stream.Handle) ? 1 : 0;
            }

            if (first is ZZLongPointer zlpA)
            {
                if (!(second is ZZLongPointer zlpB))
                    return 0;

                return (zlpA.Pointer == zlpB.Pointer) ? 1 : 0;
            }

            if (first is ZZVoid && second is ZZVoid)
                return 1;

            return ReferenceEquals(first, second) ? 1 : 0;
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
        public static ZZVoid Exit(ZZInteger exitCode)
        {
            Environment.Exit((int)exitCode.Value);
            return null;
        }

        [ZZFunction("core", "Byte")]
        public static ZZByte ConvertToByte(ZZObject any)
        {
            return new ZZByte((byte)ConvertToInteger(any).Value);
        }

        [ZZFunction("core", "Integer")]
        public static ZZInteger ConvertToInteger(ZZObject any)
        {
            if (any is ZZInteger)
                return (ZZInteger)any;

            if (any is ZZFloat zflt)
                return new ZZInteger((int)(zflt.Value));

            if (any is ZZString zstr)
                if (int.TryParse(zstr.Contents, out int strParsed))
                    return new ZZInteger(strParsed);
                else
                    throw new InterpreterRuntimeException("Tried to convert a string with non-integer characters" +
                        " to integer.");

            if (any is ZZFileHandle zhandle)
                return new ZZInteger(zhandle.Stream == null ? 0 : zhandle.Stream.Handle.ToInt32());

            throw new InterpreterRuntimeException("Tried to convert to integer where conversion is undefined.");
        }

        [ZZFunction("core", "Float")]
        public static ZZFloat ConvertToFloat(ZZObject any)
        {
            if (any is ZZFloat)
                return (ZZFloat)any;

            if (any is ZZInteger zint)
                return new ZZFloat(zint.Value);

            if (any is ZZString zstr)
                if (float.TryParse(zstr.Contents, out float strParsed))
                    return new ZZFloat(strParsed);
                else
                    throw new InterpreterRuntimeException("Tried to convert a string with non-float characters" +
                        " to float.");

            throw new InterpreterRuntimeException("Tried to convert to float where conversion is undefined.");
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

            return new ZZVoid();
        }

        [ZZFunction("core", "Strcmp")]
        public static ZZInteger StrCmp(ZZString lhs, ZZString rhs)
        {
            if (rhs.Length != lhs.Length)
                return 1;

            string native1, native2;
            native1 = lhs.ToString();
            native2 = rhs.ToString();
            for (int i = 0; i < native1.Length; ++i)
                if (native1[i] != native2[i])
                    return native1[i] - native2[i];

            return 0;
        }
    }
}
