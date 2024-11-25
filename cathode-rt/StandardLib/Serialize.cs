using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static cathode_rt.SerializationBackend;

namespace cathode_rt
{
    public static class SerializationBackend
    {
        public enum RhsType : byte
        {
            BINARY,
            SERIALDATAARRAY
        }

        public struct SerialData
        {
            public string Name;
            public ZZObjectType Type;
            public object Rhs;
            public RhsType RhsType;
        }

        public static void WriteLongToStream(Stream stream, long value)
        {
            byte[] data = BitConverter.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        public static void WriteStringToStream(Stream stream, string str)
        {
            if (str.Length < 1)
            {
                WriteLongToStream(stream, 0);
                return;
            }

            byte[] data = Encoding.Unicode.GetBytes(str);
            WriteLongToStream(stream, data.LongLength);
            stream.Write(data, 0, data.Length);
        }

        public static void WriteByteArrToStream(Stream stream, byte[] arr)
        {
            WriteLongToStream(stream, arr.LongLength);

            if (arr.LongLength != 0)
                stream.Write(arr, 0, arr.Length);
        }

        public static long ReadLongFromStream(Stream stream)
        {
            byte[] data = new byte[sizeof(long)];
            stream.Read(data, 0, sizeof(long));

            return BitConverter.ToInt64(data);
        }

        public static string ReadStringFromStream(Stream stream)
        {
            long len = ReadLongFromStream(stream);

            if (len == 0)
                return string.Empty;

            byte[] data = new byte[len];
            stream.Read(data, 0, (int)len);

            return Encoding.Unicode.GetString(data);
        }

        public static byte[] ReadByteArrFromStream(Stream stream)
        {
            long len = ReadLongFromStream(stream);

            if (len == 0)
                return Array.Empty<byte>();

            byte[] data = new byte[len];
            stream.Read(data, 0, (int)len);

            return data;
        }

        public static (ZZObject obj, string name) DeserializeRecursive(SerialData topLevel)
        {
            ZZObject retObj = ZZVoid.Void;

            switch (topLevel.Type)
            {
                case ZZObjectType.BYTE:
                    retObj = new ZZByte(((byte[])topLevel.Rhs)[0]);
                    break;

                case ZZObjectType.INTEGER:
                    retObj = (ZZInteger)BitConverter.ToInt64((byte[])topLevel.Rhs);
                    break;

                case ZZObjectType.STRING:
                    retObj = (ZZString)Encoding.Unicode.GetString((byte[])topLevel.Rhs);
                    break;

                case ZZObjectType.LONGPOINTER:
                    retObj = new ZZLongPointer(new UIntPtr(BitConverter.ToUInt64((byte[])topLevel.Rhs)));
                    break;

                case ZZObjectType.FLOAT:
                    retObj = (ZZInteger)BitConverter.ToDouble((byte[])topLevel.Rhs);
                    break;

                case ZZObjectType.VOID:
                    retObj = ZZVoid.Void;
                    break;



                case ZZObjectType.STRUCT:
                    retObj = new ZZStruct();

                    foreach (SerialData v in (SerialData[])topLevel.Rhs)
                    {
                        var tuple = DeserializeRecursive(v);
                        ((ZZStruct)retObj).Fields.Add(tuple.name, tuple.obj);
                    }

                    break;

                case ZZObjectType.ARRAY:
                    List<ZZObject> arrObjects = new List<ZZObject>();
                    foreach (SerialData v in (SerialData[])topLevel.Rhs)
                    {
                        var tuple = DeserializeRecursive(v);
                        arrObjects.Add(tuple.obj);
                    }

                    retObj = new ZZArray(arrObjects.ToArray());
                    break;
            }

            return (retObj, topLevel.Name);
        }

        public static SerialData SerializeRecursive(ZZObject obj, string name = "")
        {
            SerialData data = new SerialData();
            data.Name = name;
            data.Type = obj.ObjectType;
            data.Rhs = null;
            data.RhsType = RhsType.BINARY;

            switch (obj.ObjectType)
            {
                case ZZObjectType.BYTE:
                    data.Rhs = new byte[1] { ((ZZByte)obj).Value };
                    data.RhsType = RhsType.BINARY;
                    return data;
                case ZZObjectType.INTEGER:
                    data.Rhs = BitConverter.GetBytes(((ZZInteger)obj).Value);
                    data.RhsType = RhsType.BINARY;
                    return data;
                case ZZObjectType.STRING:
                    data.Rhs = Encoding.Unicode.GetBytes(((ZZString)obj).Contents);
                    data.RhsType = RhsType.BINARY;
                    return data;
                case ZZObjectType.LONGPOINTER:
                    data.Rhs = BitConverter.GetBytes(((ZZLongPointer)obj).Pointer.ToUInt64());
                    data.RhsType = RhsType.BINARY;
                    return data;
                case ZZObjectType.FLOAT:
                    data.Rhs = BitConverter.GetBytes(((ZZFloat)obj).Value);
                    data.RhsType = RhsType.BINARY;
                    return data;
                case ZZObjectType.VOID:
                    data.Rhs = Array.Empty<byte>();
                    data.RhsType = RhsType.BINARY;
                    return data;

                case ZZObjectType.STRUCT:
                    {
                        List<SerialData> sd = new List<SerialData>();
                        ZZStruct strct = (ZZStruct)obj;
                        foreach (ZZString fieldName in strct.Fields.Keys)
                            sd.Add(SerializeRecursive(strct.Fields[fieldName], fieldName.Contents));

                        data.Rhs = sd.ToArray();
                        data.RhsType = RhsType.SERIALDATAARRAY;
                        return data;
                    }

                case ZZObjectType.ARRAY:
                    {
                        List<SerialData> sd = new List<SerialData>();
                        ZZArray arr = (ZZArray)obj;
                        foreach (ZZObject o in arr.Objects)
                            sd.Add(SerializeRecursive(o));

                        data.Rhs = sd.ToArray();
                        data.RhsType = RhsType.SERIALDATAARRAY;
                        return data;
                    }
            }

            return data;
        }

        public static void SerialDataToStreamRecursive(Stream stream, SerialData sd)
        {
            WriteStringToStream(stream, sd.Name);
            stream.WriteByte((byte)sd.Type);
            stream.WriteByte((byte)sd.RhsType);

            if (sd.RhsType == RhsType.BINARY)
                WriteByteArrToStream(stream, (byte[])sd.Rhs);
            else
            {
                SerialData[] sdArr = (SerialData[])sd.Rhs;
                WriteLongToStream(stream, sdArr.LongLength);

                foreach (SerialData _d in sdArr)
                    SerialDataToStreamRecursive(stream, _d);
            }
        }

        public static SerialData StreamToSerialDataRecursive(Stream stream)
        {
            SerialData retData = new SerialData();

            retData.Name = ReadStringFromStream(stream);
            retData.Type = (ZZObjectType)stream.ReadByte();
            retData.RhsType = (RhsType)stream.ReadByte();

            if (retData.RhsType == RhsType.BINARY)
                retData.Rhs = ReadByteArrFromStream(stream);
            else
            {
                long length = ReadLongFromStream(stream);
                SerialData[] sdArr = new SerialData[length];

                for (int i = 0; i < length; ++i)
                    sdArr[i] = StreamToSerialDataRecursive(stream);

                retData.Rhs = sdArr;
            }

            return retData;
        }
    }

    public static partial class ImplMethods
    {
        [ZZFunction("serialize", "CanSerialize")]
        public static ZZInteger CanSerialize(ZZObject obj)
        {
            switch (obj.ObjectType)
            {
                case ZZObjectType.OBJECT:
                case ZZObjectType.FILEHANDLE:
                    return 0;
                case ZZObjectType.ARRAY:
                    foreach (ZZObject o in ((ZZArray)obj).Objects)
                        if (CanSerialize(o).Value == 0)
                            return 0;

                    return 1;
                case ZZObjectType.STRUCT:
                    foreach (ZZString zz in ((ZZStruct)obj).Fields.Keys)
                        if (CanSerialize(((ZZStruct)obj).Fields[zz]).Value == 0)
                            return 0;

                    return 1;
                default:
                    return 1;
            }
        }

        [ZZFunction("serialize", "SerializeBin")]
        public static ZZArray SerializeBinary(ZZObject obj)
        {
            if (CanSerialize(obj).Value == 0)
                throw new InterpreterRuntimeException("Tried to serialize non-serializable data.");

            SerializationBackend.SerialData sd = SerializationBackend.SerializeRecursive(obj);

            byte[] dat;
            using (MemoryStream ms = new MemoryStream())
            {
                SerializationBackend.SerialDataToStreamRecursive(ms, sd);
                dat = ms.ToArray();
            }

            List<ZZByte> zzBin = new List<ZZByte>();
            foreach (byte b in dat)
                zzBin.Add(new ZZByte(b));

            return new ZZArray(zzBin.ToArray());
        }

        [ZZFunction("serialize", "DeserializeBin")]
        public static ZZObject DeserializeBin(ZZArray arr)
        {
            byte[] realBinary = new byte[arr.Objects.Length];

            for (int i = 0; i < arr.Objects.Length; ++i)
            {
                if (arr.Objects[i].ObjectType != ZZObjectType.BYTE)
                    throw new ArgumentException();

                realBinary[i] = ((ZZByte)arr.Objects[i]).Value;
            }

            SerializationBackend.SerialData sd = new SerializationBackend.SerialData();
            using (MemoryStream ms = new MemoryStream(realBinary))
                sd = SerializationBackend.StreamToSerialDataRecursive(ms);

            return SerializationBackend.DeserializeRecursive(sd).obj;
        }
    }
}
