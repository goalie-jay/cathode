using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Reflection.Metadata;

namespace cathode_rt
{
    public static class TcpBackend
    {
        public static List<TcpClient> Clients = new List<TcpClient>();
        // Needed for thread safety when I implement multithreading in the future
        public static object ListLockObject = new object();

        public static int AddClientAndReturnIndex(TcpClient client)
        {
            lock (ListLockObject)
            {
                for (int i = 0; i < Clients.Count; ++i)
                    if (Clients[i] == null)
                    {
                        // Free space, we can use it
                        Clients[i] = client;
                        return i;
                    }

                int idx = Clients.Count;
                Clients.Add(client);
                return idx;
            }
        }

        public static TcpClient GetClientAtIndexOrReturnNullIfNotPresent(int idx)
        {
            lock (ListLockObject)
            {
                if (idx < 0)
                    return null;

                if (idx >= Clients.Count)
                    return null;

                return Clients[idx];
            }
        }

        public static void RemoveClientAtIndex(int idx)
        {
            lock (ListLockObject)
            {
                Clients[idx] = null;
            }
        }
    }

    public static partial class ImplMethods
    {
        [ZZFunction("tcpio", "nTCPSetReadTimeout")]
        public static ZZVoid TCPSetReadTimeout(ZZInteger idx, ZZInteger timeout)
        {
            TcpClient client = TcpBackend.GetClientAtIndexOrReturnNullIfNotPresent((int)idx.Value);

            if (client == null)
                throw new InterpreterRuntimeException("Tried to set the read timeout of a nonexistent TCP connection.");

            if ((int)timeout.Value < 0)
                throw new InterpreterRuntimeException("Tried to set the read timeout to an invalid amount.");

            client.ReceiveTimeout = (int)timeout.Value;

            return ZZVoid.Void;
        }

        [ZZFunction("tcpio", "nTCPSetWriteTimeout")]
        public static ZZVoid TCPSetWriteTimeout(ZZInteger idx, ZZInteger timeout)
        {
            TcpClient client = TcpBackend.GetClientAtIndexOrReturnNullIfNotPresent((int)idx.Value);

            if (client == null)
                throw new InterpreterRuntimeException("Tried to set the write timeout of a nonexistent TCP connection.");

            if ((int)timeout.Value < 0)
                throw new InterpreterRuntimeException("Tried to set the write timeout to an invalid amount.");

            client.SendTimeout = (int)timeout.Value;

            return ZZVoid.Void;
        }

        [ZZFunction("tcpio", "nTCPConnect")]
        public static ZZInteger TCPConnect(ZZString addr, ZZInteger port)
        {
            TcpClient client = new TcpClient();
            client.NoDelay = true;

            try
            {
                client.Connect(new IPEndPoint(IPAddress.Parse(addr.Contents), (int)port.Value));
                int idx = TcpBackend.AddClientAndReturnIndex(client);

                return idx;
            }
            catch { return -1; }
        }

        [ZZFunction("tcpio", "nTCPWrite")]
        public static ZZInteger TCPWrite(ZZInteger idx, ZZArray arr)
        {
            byte[] bytesNative = new byte[arr.Objects.Length];

            for (int i = 0; i < bytesNative.Length; ++i)
                if (arr.Objects[i].ObjectType == ZZObjectType.BYTE)
                    bytesNative[i] = ((ZZByte)arr.Objects[i]).Value;
                else
                    throw new ArgumentException();

            try
            {
                TcpClient client = TcpBackend.GetClientAtIndexOrReturnNullIfNotPresent((int)idx.Value);

                if (client == null)
                    throw new InterpreterRuntimeException("Tried to write to a nonexistent TCP connection.");

                Stream stream = client.GetStream();
                stream.Write(bytesNative, 0, bytesNative.Length);

                return 1;
            }
            catch { return 0; }
        }

        [ZZFunction("tcpio", "nTCPIsOpen")]
        public static ZZInteger TCPIsOpen(ZZInteger idx)
        {
            TcpClient client = TcpBackend.GetClientAtIndexOrReturnNullIfNotPresent((int)idx.Value);

            if (client == null)
                return 0;

            try
            {
                return client.Connected ? 1 : 0;
            }
            catch { return 0; };
        }

        [ZZFunction("tcpio", "nTCPCanRead")]
        public static ZZInteger TCPCanRead(ZZInteger idx)
        {
            TcpClient client = TcpBackend.GetClientAtIndexOrReturnNullIfNotPresent((int)idx.Value);

            if (client == null)
                throw new InterpreterRuntimeException("Tried to get info from a nonexistent TCP connection.");

            try
            {
                return client.GetStream().DataAvailable ? 1 : 0;
            }
            catch { return 0; }
        }

        [ZZFunction("tcpio", "nTCPRead")]
        public static ZZObject TCPRead(ZZInteger idx, ZZInteger count)
        {
            TcpClient client = TcpBackend.GetClientAtIndexOrReturnNullIfNotPresent((int)idx.Value);

            if (client == null)
                throw new InterpreterRuntimeException("Tried to read from a nonexistent TCP connection.");

            byte[] arr = new byte[count.Value];
            int bytesRead = -1;
            try
            {
                Stream stream = client.GetStream();
                bytesRead = stream.Read(arr, 0, (int)count.Value);
            }
            catch { return ZZVoid.Void; }

            if (bytesRead == -1)
                return new ZZArray(Array.Empty<ZZObject>());

            List<ZZObject> objects = new List<ZZObject>();
            for (int i = 0; i < bytesRead; ++i)
                objects.Add(new ZZByte(arr[i]));

            return new ZZArray(objects.ToArray());
        }

        [ZZFunction("tcpio", "nTCPClose")]
        public static ZZVoid TCPClose(ZZInteger idx)
        {
            TcpClient client = TcpBackend.GetClientAtIndexOrReturnNullIfNotPresent((int)idx.Value);

            if (client != null)
            {
                client.Dispose();
                TcpBackend.RemoveClientAtIndex((int)idx.Value);
            }

            return ZZVoid.Void;
        }
    }
}
