using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public abstract class ZZObject
    {
        public virtual ZZString GetInLanguageTypeName()
        {
            return new ZZString("object");
        }

        public virtual ZZString ToInLanguageString()
        {
            return new ZZString("[object]");
        }
    }

    public class ZZString : ZZObject
    {
        public string Contents;

        public ZZInteger Length => new ZZInteger(Contents.Length);

        public ZZString(string str)
        {
            Contents = new string(str);
        }

        public override ZZString GetInLanguageTypeName()
        {
            return new ZZString("string");
        }

        public override ZZString ToInLanguageString()
        {
            return this;
        }

        public override string ToString()
        {
            return Contents;
        }

        public static implicit operator ZZString(string rhs)
        {
            return new ZZString(rhs);
        }

        public static bool operator==(ZZString lhs, ZZString rhs)
        {
            if (ReferenceEquals(lhs, rhs))
                return true;

            return lhs.Contents == rhs.Contents;
        }

        public static bool operator!=(ZZString lhs, ZZString rhs)
        {
            if (ReferenceEquals(lhs, rhs))
                return false;

            return lhs.Contents != rhs.Contents;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (ReferenceEquals(obj, null))
                return false;

            if (!(obj is ZZString))
                return false;

            return Contents == ((ZZString)obj).Contents;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class ZZFloat : ZZObject
    {
        public float Value;

        public ZZFloat(float value)
        {
            Value = value;
        }

        public override ZZString GetInLanguageTypeName()
        {
            return "float";
        }

        public override ZZString ToInLanguageString()
        {
            return new ZZString(Value.ToString());
        }

        public static implicit operator ZZFloat(float value)
        {
            return new ZZFloat(value);
        }

        public static ZZFloat operator+(ZZFloat lhs, ZZFloat rhs)
        {
            return lhs.Value + rhs.Value;
        }

        public static ZZFloat operator-(ZZFloat lhs, ZZFloat rhs)
        {
            return lhs.Value - rhs.Value;
        }

        public static ZZFloat operator*(ZZFloat lhs, ZZFloat rhs)
        {
            return lhs.Value * rhs.Value;
        }

        public static ZZFloat operator/(ZZFloat lhs, ZZFloat rhs)
        {
            return lhs.Value / rhs.Value;
        }

        public static bool operator==(ZZFloat lhs, ZZFloat rhs)
        {
            if (ReferenceEquals(lhs, rhs))
                return true;

            if (ReferenceEquals(rhs, null))
                return false;

            return lhs.Value == rhs.Value;
        }

        public static bool operator!=(ZZFloat lhs, ZZFloat rhs)
        {
            return !(lhs.Value == rhs.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (ReferenceEquals(obj, null))
                return false;

            if (!(obj is ZZFloat))
                return false;

            return Value == ((ZZFloat)obj).Value;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class ZZInteger : ZZObject
    {
        public long Value;

        public ZZInteger(long value)
        {
            Value = value;
        }

        public override ZZString GetInLanguageTypeName()
        {
            return new ZZString("integer");
        }

        public override ZZString ToInLanguageString()
        {
            return new ZZString(Value.ToString());
        }

        //public static implicit operator int(ZZInteger zzint)
        //{
        //    return zzint.Value;
        //}

        public static implicit operator ZZInteger(long v)
        {
            return new ZZInteger(v);
        }

        public static ZZInteger operator+(ZZInteger lhs, ZZInteger rhs)
        {
            return lhs.Value + rhs.Value;
        }

        public static ZZInteger operator-(ZZInteger lhs, ZZInteger rhs)
        {
            return lhs.Value - rhs.Value;
        }

        public static ZZInteger operator*(ZZInteger lhs, ZZInteger rhs)
        {
            return lhs.Value * rhs.Value;
        }

        public static ZZInteger operator/(ZZInteger lhs, ZZInteger rhs)
        {
            return lhs.Value / rhs.Value;
        }
        
        public static bool operator==(ZZInteger lhs, ZZInteger rhs)
        {
            if (ReferenceEquals(lhs, rhs))
                return true;

            if (ReferenceEquals(rhs, null))
                return false;

            return lhs.Value == rhs.Value;
        }

        public static bool operator!=(ZZInteger lhs, ZZInteger rhs)
        {
            return !(lhs.Value == rhs.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (ReferenceEquals(obj, null))
                return false;

            if (!(obj is ZZInteger))
                return false;

            return Value == ((ZZInteger)obj).Value;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class ZZByte : ZZObject
    {
        public byte Value;

        public ZZByte(byte value)
        {
            Value = value;
        }

        public override ZZString GetInLanguageTypeName()
        {
            return new ZZString("byte");
        }

        public override ZZString ToInLanguageString()
        {
            return new ZZString(Value.ToString());
        }

        //public static implicit operator int(ZZInteger zzint)
        //{
        //    return zzint.Value;
        //}

        public static implicit operator ZZByte(byte v)
        {
            return new ZZByte(v);
        }

        public static ZZByte operator+(ZZByte lhs, ZZByte rhs)
        {
            return (byte)(lhs.Value + rhs.Value);
        }

        public static ZZByte operator-(ZZByte lhs, ZZByte rhs)
        {
            return (byte)(lhs.Value - rhs.Value);
        }

        public static ZZByte operator*(ZZByte lhs, ZZByte rhs)
        {
            return (byte)(lhs.Value * rhs.Value);
        }

        public static ZZByte operator/(ZZByte lhs, ZZByte rhs)
        {
            return (byte)(lhs.Value / rhs.Value);
        }

        public static bool operator==(ZZByte lhs, ZZByte rhs)
        {
            if (ReferenceEquals(lhs, rhs))
                return true;

            if (ReferenceEquals(rhs, null))
                return false;

            return lhs.Value == rhs.Value;
        }

        public static bool operator!=(ZZByte lhs, ZZByte rhs)
        {
            return !(lhs.Value == rhs.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (ReferenceEquals(obj, null))
                return false;

            if (!(obj is ZZByte))
                return false;

            return Value == ((ZZByte)obj).Value;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class ZZFileHandle : ZZObject
    {
        public FileStream Stream;

        public ZZFileHandle(FileStream stream)
        {
            Stream = stream;
        }

        public override ZZString ToInLanguageString()
        {
            return new ZZString(Stream.Handle.ToString());
        }

        public override ZZString GetInLanguageTypeName()
        {
            return new ZZString("handle");
        }
    }

    public class ZZLongPointer : ZZObject
    {
        public UIntPtr Pointer;

        public ZZLongPointer(UIntPtr pointer)
        {
            Pointer = pointer;
        }

        public override ZZString ToInLanguageString()
        {
            return Pointer.ToUInt64().ToString();
        }

        public override ZZString GetInLanguageTypeName()
        {
            return "longpointer";
        }
    }

    public class ZZVoid : ZZObject
    {
        public ZZVoid() { }

        public override ZZString GetInLanguageTypeName()
        {
            return new ZZString("void");
        }

        public override ZZString ToInLanguageString()
        {
            return new ZZString("[void]");
        }
    }

    public class ZZStruct : ZZObject
    {
        public Dictionary<ZZString, ZZObject> Fields;

        public ZZStruct()
        {
            Fields = new Dictionary<ZZString, ZZObject>();
        }

        public ZZStruct(ZZArray fieldNames /*In language array of tuples (field name str, value)*/)
        {
            Fields = new Dictionary<ZZString, ZZObject>();

            foreach (ZZObject o in fieldNames.Objects)
            {
                if (!(o is ZZTuple tuple))
                    throw new InterpreterRuntimeException("Struct pilot array must be of tuples of format " +
                        "(field name string, value).");

                Fields.Add(tuple.Object1.ToInLanguageString(), tuple.Object2);
            }
        }

        public override ZZString ToInLanguageString()
        {
            return new ZZString("[struct]");
        }

        public override ZZString GetInLanguageTypeName()
        {
            return new ZZString("struct");
        }
    }

    public class ZZTuple : ZZObject
    {
        public ZZObject Object1;
        public ZZObject Object2;

        public ZZTuple(ZZObject object1, ZZObject object2)
        {
            Object1 = object1;
            Object2 = object2;
        }

        public override ZZString GetInLanguageTypeName()
        {
            return "tuple";
        }

        public override ZZString ToInLanguageString()
        {
            return new ZZString($"({Object1.ToInLanguageString()}, {Object2.ToInLanguageString()})");
        }
    }

    public class ZZArray : ZZObject
    {
        public ZZObject[] Objects;

        public ZZArray(ZZObject[] objects)
        {
            Objects = new ZZObject[objects.Length];
            Array.Copy(objects, 0, Objects, 0, objects.Length);
        }

        public override ZZString ToInLanguageString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{ ");

            for (int i = 0; i < Objects.Length; ++i)
                if (i + 1 == Objects.Length)
                    sb.Append(Objects[i].ToInLanguageString().ToString() + " ");
                else
                    sb.Append(Objects[i].ToInLanguageString().ToString() + ", ");

            sb.Append('}');

            return new ZZString(sb.ToString());
        }

        public override ZZString GetInLanguageTypeName()
        {
            return "array";
        }
    }

    public class ZZFunctionDescriptor
    {
        public string Name;
        public string[] Arguments;
        public string Namespace;

        public ZZFunctionDescriptor(string name)
        {
            Name = name;
            Arguments = Array.Empty<string>();
            Namespace = "core";
        }

        public ZZFunctionDescriptor(string name, string[] args)
        {
            Name = name;
            Arguments = args;
            Namespace = "core";
        }

        public ZZFunctionDescriptor(string name, string[] args, string _namespace)
        {
            Name = name;
            Arguments = args;
            Namespace = _namespace;
        }
    }
}
