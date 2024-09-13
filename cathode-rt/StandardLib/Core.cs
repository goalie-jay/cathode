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
        [ZZFunction("core", "RandomBytes")]
        public static ZZArray RandomBytes(ZZInteger length)
        {
            if (length.Value < 0)
                throw new ArgumentException();

            byte[] bytes = RandomNumberGen.GetRandomBytes((int)length.Value);

            List<ZZObject> arrContents = new List<ZZObject>();
            foreach (byte b in bytes)
                arrContents.Add(new ZZByte(b));

            return new ZZArray(arrContents.ToArray());
        }

        [ZZFunction("core", "BytesToStr")]
        public static ZZObject BytesToStr(ZZArray bytes, ZZString encoding)
        {
            try
            {
                List<byte> realBytes = new List<byte>();

                foreach (ZZObject obj in bytes.Objects)
                    if (obj is ZZByte bt)
                        realBytes.Add(bt.Value);
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
            catch { return new ZZVoid(); }
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
            catch { return new ZZVoid(); }
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
        public static ZZString ConcatenateStrings(ZZArray arr, ZZObject separator)
        {
            if (arr.Objects.Length < 1)
                return new ZZString(string.Empty);

            List<string> realStrings = new List<string>();

            foreach (ZZObject obj in arr.Objects)
                if (obj is ZZString _str)
                    realStrings.Add(_str.Contents);
                else
                    throw new ArgumentException();

            if (separator is ZZVoid)
                return new ZZString(string.Concat(realStrings));
            else if (separator is ZZString str)
                return new ZZString(string.Join("str", realStrings));

            throw new ArgumentException();
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
            if (test.Value == 0)
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
            if (first is ZZInteger zint1)
                if (second is ZZInteger zint2)
                    return (zint1.Value < zint2.Value) ? 1 : 0;
                else if (second is ZZFloat zfloat2)
                    return ((float)zint1.Value < zfloat2.Value) ? 1 : 0;
                else if (second is ZZByte zbyte2)
                    return (zint1.Value < zbyte2.Value) ? 1 : 0;
                else
                    throw new ArgumentException("Tried to use LessThan() on a non-number data type.");
            else if (first is ZZFloat zfloat1)
                if (second is ZZInteger zint2)
                    return (zfloat1.Value < (float)zint2.Value) ? 1 : 0;
                else if (second is ZZFloat zfloat2)
                    return (zfloat1.Value < zfloat2.Value) ? 1 : 0;
                else if (second is ZZByte zbyte2)
                    return (zfloat1.Value < zbyte2.Value) ? 1 : 0;
                else
                    throw new ArgumentException("Tried to use LessThan() on a non-number data type.");
            else if (first is ZZFloat zbyte1)
                if (second is ZZInteger zint2)
                    return (zbyte1.Value < zint2.Value) ? 1 : 0;
                else if (second is ZZFloat zfloat2)
                    return (zbyte1.Value < zfloat2.Value) ? 1 : 0;
                else if (second is ZZByte zbyte2)
                    return (zbyte1.Value < zbyte2.Value) ? 1 : 0;
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
                    return (zint1.Value > zfloat2.Value) ? 1 : 0;
                else if (second is ZZByte zbyte2)
                    return (zint1.Value > zbyte2.Value) ? 1 : 0;
                else
                    throw new ArgumentException("Tried to use GreaterThan() on a non-number data type.");
            else if (first is ZZFloat zfloat1)
                if (second is ZZInteger zint2)
                    return (zfloat1.Value > zint2.Value) ? 1 : 0;
                else if (second is ZZFloat zfloat2)
                    return (zfloat1.Value > zfloat2.Value) ? 1 : 0;
                else if (second is ZZByte zbyte2)
                    return (zfloat1.Value > zbyte2.Value) ? 1 : 0;
                else
                    throw new ArgumentException("Tried to use GreaterThan() on a non-number data type.");
            else if (first is ZZByte zbyte1)
                if (second is ZZInteger zint2)
                    return (zbyte1.Value > zint2.Value) ? 1 : 0;
                else if (second is ZZFloat zfloat2)
                    return (zbyte1.Value > zfloat2.Value) ? 1 : 0;
                else if (second is ZZByte zbyte2)
                    return (zbyte1.Value > zbyte2.Value) ? 1 : 0;
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
                if (second is ZZInteger zintb)
                    return (zintA == zintb) ? 1 : 0;
                else if (second is ZZFloat zfloatb)
                    return (zintA == zfloatb) ? 1 : 0;
                else if (second is ZZByte zbyteb)
                    return (zintA.Value == zbyteb.Value) ? 1 : 0;

                return 0;
            }

            if (first is ZZByte zbytea)
            {
                if (second is ZZByte zbyteb)
                    return (zbytea.Value == zbyteb.Value) ? 1 : 0;
                else if (second is ZZInteger zintb)
                    return (zbytea.Value == zintb.Value) ? 1 : 0;
                else if (second is ZZFloat zfloatb)
                    return (zbytea.Value == zfloatb.Value) ? 1 : 0;

                return 0;
            }

            if (first is ZZFloat zfloata)
            {
                if (second is ZZFloat zfloatb)
                    return (zfloata == zfloatb) ? 1 : 0;
                else if (second is ZZInteger zintb)
                    return (zfloata == zintb) ? 1 : 0;
                else if (second is ZZByte zbyteb)
                    return (zfloata.Value == zbyteb.Value) ? 1 : 0;

                return 0;
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
        public static ZZVoid Exit(ZZInteger code)
        {
            Environment.Exit((int)code.Value);
            return null;
        }

        [ZZFunction("core", "Byte")]
        public static ZZObject ConvertToByte(ZZObject obj)
        {
            ZZObject result = ConvertToInteger(obj);

            if (result is ZZVoid)
                return result;

            return new ZZByte((byte)((ZZInteger)result).Value);
        }

        [ZZFunction("core", "Integer")]
        public static ZZObject ConvertToInteger(ZZObject obj)
        {
            if (obj is ZZInteger)
                return (ZZInteger)obj;

            if (obj is ZZFloat zflt)
                return new ZZInteger((int)(zflt.Value));

            if (obj is ZZString zstr)
                if (int.TryParse(zstr.Contents, out int strParsed))
                    return new ZZInteger(strParsed);
                else
                    return new ZZVoid();

            if (obj is ZZByte zbyte)
                return new ZZInteger((int)zbyte.Value);

            if (obj is ZZFileHandle zhandle)
                return new ZZInteger(zhandle.Stream == null ? 0 : zhandle.Stream.Handle.ToInt32());

            throw new InterpreterRuntimeException("Tried to convert to integer where conversion is undefined.");
        }

        [ZZFunction("core", "Float")]
        public static ZZObject ConvertToFloat(ZZObject obj)
        {
            if (obj is ZZFloat)
                return (ZZFloat)obj;

            if (obj is ZZInteger zint)
                return new ZZFloat(zint.Value);

            if (obj is ZZByte zbyte)
                return new ZZFloat(zbyte.Value);

            if (obj is ZZString zstr)
                if (float.TryParse(zstr.Contents, out float strParsed))
                    return new ZZFloat(strParsed);
                else
                    return new ZZVoid();

            return new ZZVoid();
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
    }
}
