using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static partial class ImplMethods
    {
        [ZZFunction("fileio", "dExists")]
        public static ZZInteger DirectoryExists(ZZString dirName)
        {
            return Directory.Exists(dirName.Contents) ? 1 : 0;
        }

        [ZZFunction("fileio", "dCreate")]
        public static ZZInteger CreateDirectory(ZZString dirName)
        {
            try
            {
                Directory.CreateDirectory(dirName.Contents);
                return 1;
            }
            catch { return 0; }
        }

        [ZZFunction("fileio", "dUnlink")]
        public static ZZInteger DeleteDirectory(ZZString dirName)
        {
            try
            {
                Directory.Delete(dirName.Contents);
                return 1;
            }
            catch { return 0; }
        }

        [ZZFunction("fileio", "fUnlink")]
        public static ZZInteger DeleteFile(ZZString name)
        {
            try
            {
                File.Delete(name.Contents);
                return 1;
            }
            catch { return 0; }
        }

        //[ZZFunction("fileio", "fCheckHandle")]
        //public static ZZInteger CheckHandle(ZZFileHandle handle)
        //{
        //    if (handle.Stream == null)
        //        return 0;

        //    return 1;
        //}

        [ZZFunction("fileio", "fOpen")]
        public static ZZObject OpenFile(ZZString name, ZZString perms)
        {
            FileStream stream = null;
            try
            {
                FileMode _mode = FileMode.OpenOrCreate;
                FileAccess _perms = FileAccess.Read;

                switch (perms.ToString())
                {
                    case "r":
                        _perms = FileAccess.Read;
                        break;
                    case "rw":
                        _perms = FileAccess.ReadWrite;
                        break;
                    case "w":
                        _perms = FileAccess.Write;
                        break;
                    default:
                        return ZZVoid.Void;
                }

                stream = File.Open(name.ToString(), _mode, _perms);
            }
            catch { }

            return ZZVoid.Void;
        }
        [ZZFunction("fileio", "fExists")]
        public static ZZInteger FileExists(ZZString filename)
        {
            return File.Exists(filename.Contents) ? 1 : 0;
        }
        
        [ZZFunction("fileio", "fCreate")]
        public static ZZObject CreateFile(ZZString filename)
        {
            try
            {
                FileStream fs = File.Create(filename.Contents);
                return new ZZFileHandle(fs);
            }
            catch { return ZZVoid.Void; }
        }

        [ZZFunction("fileio", "fGetPath")]
        public static ZZObject GetFullPath(ZZString filename)
        {
            try
            {
                return new ZZString(Path.GetFullPath(filename.Contents));
            }
            catch { return ZZVoid.Void; }
        }

        [ZZFunction("fileio", "fClose")]
        public static ZZVoid FileClose(ZZFileHandle handle)
        {
            handle.Stream.Dispose();
            return ZZVoid.Void;
        }

        [ZZFunction("fileio", "fReadLine")]
        public static ZZObject FileReadline(ZZFileHandle handle)
        {
            try
            {
                List<char> vs = new List<char>();

                while (handle.Stream.CanRead)
                {
                    int bt = handle.Stream.ReadByte();

                    if (bt == -1 || (char)bt == '\n')
                        break;

                    vs.Add((char)bt);
                }

                return new ZZString(new string(vs.ToArray()));
            }
            catch { return ZZVoid.Void; }
        }

        [ZZFunction("fileio", "fLen")]
        public static ZZObject GetFileLength(ZZFileHandle handle)
        {
            try
            {
                return new ZZInteger(handle.Stream.Length);
            }
            catch { return ZZVoid.Void; }
        }

        [ZZFunction("fileio", "fGetPos")]
        public static ZZInteger GetFilePosition(ZZFileHandle handle)
        {
            return handle.Stream.Position;
        }

        [ZZFunction("fileio", "fSetPos")]
        public static ZZVoid SetFilePosition(ZZFileHandle handle, ZZInteger position)
        {
            if (position.Value < 0)
                throw new ArgumentException();

            handle.Stream.Position = position.Value;
            return ZZVoid.Void;
        }

        [ZZFunction("fileio", "fRead")]
        public static ZZObject FileRead(ZZFileHandle handle, int count)
        {
            byte[] arr = new byte[count];
            int bytesRead = -1;
            try
            {
                bytesRead = handle.Stream.Read(arr, 0, count);
            }
            catch { return ZZVoid.Void; }

            if (bytesRead == -1)
                return new ZZArray(new ZZObject[] { });

            List<ZZObject> objects = new List<ZZObject>();
            for (int i = 0; i < bytesRead; ++i)
                objects.Add(new ZZByte(arr[i]));

            return new ZZArray(objects.ToArray());
        }

        [ZZFunction("fileio", "fWrite")]
        public static ZZInteger FileWrite(ZZFileHandle handle, ZZArray arr)
        {
            byte[] bytesNative = new byte[arr.Objects.Length];

            for (int i = 0; i < bytesNative.Length; ++i)
                if (arr.Objects[i].ObjectType == ZZObjectType.BYTE)
                    bytesNative[i] = ((ZZByte)arr.Objects[i]).Value;
                else
                    throw new ArgumentException();

            try
            {
                handle.Stream.Write(bytesNative, 0, bytesNative.Length);
                return 1;
            }
            catch { return 0; }
        }

        [ZZFunction("fileio", "fWriteLine")]
        public static ZZInteger FileWriteLine(ZZFileHandle handle, ZZString line, ZZString encoding)
        {
            byte[] data;

            switch (encoding.Contents)
            {
                case "ascii":
                    data = System.Text.Encoding.ASCII.GetBytes(line.Contents + "\n");
                    break;
                case "unicode":
                    data = System.Text.Encoding.Unicode.GetBytes(line.Contents + "\n");
                    break;
                case "utf8":
                    data = System.Text.Encoding.UTF8.GetBytes(line.Contents + "\n");
                    break;
                default:
                    throw new ArgumentException();
            }

            try
            {
                handle.Stream.Write(data);
                return 1;
            }
            catch { return 0; }
        }
    }
}
