using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static class ConsoleBackend
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadConsoleOutputCharacter(
            IntPtr hConsoleOutput,
            [Out] StringBuilder lpCharacter,
            uint length,
            COORD bufferCoord,
            out uint lpNumberOfCharactersRead);

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }
    }

    public static partial class ImplMethods
    {
        [ZZFunction("conio", "cClear")]
        public static ZZVoid ClearConsole()
        {
            Console.Clear();
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "cWidth")]
        public static ZZInteger GetConsoleWidth()
        {
            return Console.BufferWidth;
        }

        [ZZFunction("conio", "cHeight")]
        public static ZZInteger GetConsoleHeight()
        {
            return Console.BufferHeight;
        }

        [ZZFunction("conio", "Title")]
        public static ZZVoid SetConsoleTitle(ZZString str)
        {
            Console.Title = str.Contents;
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "Print")]
        public static ZZVoid Print(ZZObject obj)
        {
            Console.Write(obj.ToInLanguageString().ToString());
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "PrintLn")]
        public static ZZVoid PrintLn(ZZObject obj)
        {
            string prntStr = obj.ToInLanguageString().ToString();
            Console.WriteLine(prntStr);
            return ZZVoid.Void;
        }

        [ZZFunction("conio", "ReadLn")]
        public static ZZString ReadLn()
        {
            return new ZZString(Console.ReadLine());
        }

        [ZZFunction("conio", "cGetX")]
        public static ZZInteger ConsoleGetXPos()
        {
            return Console.CursorLeft;
        }

        [ZZFunction("conio", "cGetY")]
        public static ZZInteger ConsoleGetYPos()
        {
            return Console.CursorTop;
        }

        [ZZFunction("conio", "cSetPos")]
        public static ZZInteger ConsoleSetPos(ZZInteger x, ZZInteger y)
        {
            try
            {
                Console.SetCursorPosition((int)x.Value, (int)y.Value);
                return 1;
            }
            catch { return 0; }
        }

        [ZZFunction("conio", "cCharAt")]
        public static ZZObject cCharAt(ZZInteger x, ZZInteger y)
        {
            IntPtr consoleHandle = ConsoleBackend.GetStdHandle(-11);
            if (consoleHandle == IntPtr.Zero)
            {
                return ZZVoid.Void;
            }
            ConsoleBackend.COORD position = new ConsoleBackend.COORD
            {
                X = (short)x.Value,
                Y = (short)y.Value
            };
            StringBuilder result = new StringBuilder(1);
            uint read = 0;
            if (ConsoleBackend.ReadConsoleOutputCharacter(consoleHandle, result, 1, position, out read))
            {
                return (ZZString)result[0].ToString();
            }
            else
            {
                return ZZVoid.Void;
            }
        }
    }
}
