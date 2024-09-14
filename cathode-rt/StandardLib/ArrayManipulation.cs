using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public partial class ImplMethods
    {
        [ZZFunction("array", "aIdxOf")]
        public static ZZInteger IdxOf(ZZArray arr, ZZObject obj)
        {
            for (int i = 0; i < arr.Objects.Length; ++i)
                if (Compare(arr.Objects[i], obj).Value != 0)
                    return new ZZInteger(i);

            return -1;
        }

        [ZZFunction("array", "aCount")]
        public static ZZInteger aCount(ZZArray arr, ZZObject obj)
        {
            int count = 0;

            for (int i = 0; i < arr.Objects.Length; ++i)
                if (Compare(arr.Objects[i], obj).Value != 0)
                    ++count;

            return count;
        }

        [ZZFunction("array", "aRemoveAll")]
        public static ZZArray aRemoveAll(ZZArray arr, ZZObject obj)
        {
            List<ZZObject> objs = new List<ZZObject>();

            for (int i = 0; i < arr.Objects.Length; ++i)
                if (Compare(arr.Objects[i], obj).Value == 0)
                    objs.Add(arr.Objects[i]);

            return new ZZArray(objs.ToArray());
        }

        [ZZFunction("array", "aRemoveIdx")]
        public static ZZArray aRemoveIdx(ZZArray arr, ZZInteger idx)
        {
            if (idx.Value < 0)
                throw new ArgumentException();

            if (arr.Objects.Length < 1)
                throw new ArgumentException();

            if (idx.Value >= arr.Objects.Length)
                throw new InterpreterRuntimeException("Tried to remove a array element at an index outside the bounds of the array.");

            ZZObject[] newArr = new ZZObject[arr.Objects.Length - 1];

            for (int i = 0; i < arr.Objects.Length; ++i)
                if (i < idx.Value)
                    newArr[i] = arr.Objects[i];
                else if (i == idx.Value)
                    continue;
                else
                    newArr[i - 1] = arr.Objects[i];

            return new ZZArray(newArr);
        }

        [ZZFunction("array", "aAppend")]
        public static ZZArray aAppend(ZZArray arr, ZZObject obj)
        {
            ZZObject[] newArr = new ZZObject[arr.Objects.Length + 1];

            for (int i = 0; i < arr.Objects.Length; ++i)
                newArr[i] = arr.Objects[i];

            newArr[arr.Objects.Length] = obj;
            return new ZZArray(newArr);
        }

        [ZZFunction("array", "aSection")]
        public static ZZArray aSection(ZZArray arr, ZZInteger start, ZZInteger length)
        {
            if (start.Value < 0)
                throw new ArgumentException();

            if (length.Value < 0)
                throw new ArgumentException();

            if ((start.Value + length.Value) >= arr.Objects.Length)
                throw new InterpreterRuntimeException("Tried to get array range with an end point outside of the bounds of the array.");

            ZZObject[] newArr = new ZZObject[length.Value];

            for (int i = 0; i < length.Value; ++i)
                newArr[i] = arr.Objects[start.Value + i];

            return new ZZArray(newArr);
        }

        [ZZFunction("array", "aConcat")]
        public static ZZArray aConcat(ZZArray arr1, ZZArray arr2)
        {
            ZZObject[] newArr = new ZZObject[arr1.Objects.Length + arr2.Objects.Length];

            for (int i = 0; i < arr1.Objects.Length; ++i)
                newArr[i] = arr1.Objects[i];

            for (int i = 0; i < arr2.Objects.Length; ++i)
                newArr[i + arr1.Objects.Length] = arr2.Objects[i];

            return new ZZArray(newArr);
        }
    }
}
