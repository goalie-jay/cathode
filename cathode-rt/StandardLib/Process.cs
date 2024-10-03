using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static class ProcessBackend
    {
        public static List<UIntPtr> Handles = new List<UIntPtr>();
        // Needed for thread safety when I implement multithreading in the future
        public static object ListLockObject = new object();

        public static int AddHandleAndReturnIndex(UIntPtr handle)
        {
            lock (ListLockObject)
            {
                for (int i = 0; i < Handles.Count; ++i)
                    if (Handles[i] == UIntPtr.Zero)
                    {
                        // Free space, we can use it
                        Handles[i] = handle;
                        return i;
                    }

                int idx = Handles.Count;
                Handles.Add(handle);
                return idx;
            }
        }

        public static UIntPtr GetHandleAtIdx(int idx)
        {
            lock (ListLockObject)
            {
                if (idx < 0)
                    return UIntPtr.Zero;

                if (idx >= Handles.Count)
                    return UIntPtr.Zero;

                return Handles[idx];
            }
        }

        public static void RemoveHandleAtIndex(int idx)
        {
            lock (ListLockObject)
            {
                Handles[idx] = UIntPtr.Zero;
            }
        }
    }

    public static partial class ImplMethods
    {
        [ZZFunction("process", "ProcessName")]
        public static ZZObject PIDName(ZZInteger pid)
        {
            try
            {
                Process proc = Process.GetProcessById((int)pid.Value);
                return (ZZString)proc.ProcessName;
            }
            catch
            {
                return ZZVoid.Void;
            }
        }

        [ZZFunction("process", "pKill")]
        public static ZZInteger KillProcess(ZZInteger pid)
        {
            try
            {
                Process.GetProcessById((int)pid.Value).Kill();
                return 1;
            }
            catch { return 0; }
        }

        [ZZFunction("process", "EnumProcesses")]
        public static ZZArray EnumProcesses()
        {
            List<ZZInteger> lst = new List<ZZInteger>();

            try
            {
                foreach (Process p in Process.GetProcesses())
                    lst.Add((ZZInteger)p.Id);
            }
            catch { }

            return new ZZArray(lst.ToArray());
        }

        [ZZFunction("process", "EnableDebug")]
        public static ZZInteger EnableDebugPriv()
        {
            return FastOps.AttemptToEnterDebugPrivilege();
        }

        [ZZFunction("process", "pOpen")]
        public static ZZInteger OpenProcess(ZZInteger processId, ZZString perms)
        {
            byte read = 0;
            byte write = 0;

            switch (perms.Contents)
            {
                case "r":
                    read = 1;
                    break;
                case "w":
                    write = 1;
                    break;
                case "rw":
                    read = write = 1;
                    break;
                default:
                    return -1;
            }

            uint mask = 0x0400; /*PROCESS_QUERY_INFORMATION*/

            if (read == 1)
                mask = mask | 0x0010; /*PROCESS_VM_READ*/

            if (write == 1)
                mask = mask | 0x0020; /*PROCESS_VM_WRITE*/

            if (read == 1 && write == 1)
                mask = mask | 0x0800; /*PROCESS_SUSPEND_RESUME, trust me*/

            UIntPtr procHandle = FastOps.OpenProcess(mask, false, (uint)processId.Value);

            if (procHandle == UIntPtr.Zero)
                return -1;

            int idx = ProcessBackend.AddHandleAndReturnIndex(procHandle);
            return idx;
        }

        [ZZFunction("process", "pRead")]
        public static ZZObject ReadProcess(ZZInteger handleIdx, ZZInteger addr, ZZInteger count)
        {
            UIntPtr pointer = ProcessBackend.GetHandleAtIdx((int)handleIdx.Value);

            if (pointer == UIntPtr.Zero)
                throw new InterpreterRuntimeException("Tried to read from a nonexistent process.");

            byte[] byteArr = FastOps.AttemptToReadProcess(pointer, (UIntPtr)addr.Value, (UIntPtr)count.Value);

            if (byteArr == null)
                return ZZVoid.Void;

            List<ZZByte> newArr = new List<ZZByte>();

            foreach (byte b in byteArr)
                newArr.Add((ZZByte)b);

            return new ZZArray(newArr.ToArray());
        }

        [ZZFunction("process", "pWrite")]
        public static ZZInteger WriteProcess(ZZInteger handleIdx, ZZInteger addr, ZZArray byteArr)
        {
            UIntPtr pointer = ProcessBackend.GetHandleAtIdx((int)handleIdx.Value);

            if (pointer == UIntPtr.Zero)
                throw new InterpreterRuntimeException("Tried to write to a nonexistent process.");

            byte[] binary = new byte[byteArr.Objects.Length];

            for (int i = 0; i < byteArr.Objects.Length; ++i)
            {
                if (byteArr.Objects[i].ObjectType != ZZObjectType.BYTE)
                    throw new ArgumentException();

                binary[i] = ((ZZByte)byteArr.Objects[i]).Value;
            }

            return FastOps.AttemptToWriteProcess(pointer, (UIntPtr)addr.Value, binary);
        }

        [ZZFunction("process", "pClose")]
        public static ZZVoid CloseProcess(ZZInteger idx)
        {
            UIntPtr handle = ProcessBackend.GetHandleAtIdx((int)idx.Value);

            if (handle != UIntPtr.Zero)
            {
                FastOps.CloseHandle(handle);
                ProcessBackend.RemoveHandleAtIndex((int)idx.Value);
            }

            return ZZVoid.Void;
        }

        [ZZFunction("process", "pNativeHandle")]
        public static ZZInteger GetNativeHandleFromIdx(ZZInteger idx)
        {
            UIntPtr handle = ProcessBackend.GetHandleAtIdx((int)idx.Value);

            return new ZZInteger((long)handle.ToUInt64());
        }
    }
}
