using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt.Json
{
    public struct JsonValue
    {
       public string Name;
       public object Value;
    }

    public class JsonFormatter
    {
        List<JsonValue> Values;

        public void AddValue(string name, object value)
        {
            Values.Add(new JsonValue() { Name = name, Value = value });
        }

        public JsonFormatter()
        {
            Values = new List<JsonValue>();
        }

        private string ByteArrayToStr(byte[] arr)
        {
            string[] strings = new string[arr.Length];
            for (int i = 0; i < strings.Length; ++i)
                strings[i] = arr[i].ToString();

            return "{ " + string.Join(", ", strings) + " }";
        }

        private void EmitInternalRecursive(ref StringBuilder builder, JsonValue[] values, int indent)
        {
            indent += 2;
            string indentText = new string(' ', indent);

            bool firstTime = true;
            foreach (JsonValue v in values)
            {
                if (!firstTime)
                    builder.AppendLine(",");

                builder.Append(indentText);
                builder.Append($"\"{v.Name}\": ");

                if (v.Value is JsonValue[])
                {
                    builder.AppendLine();
                    builder.Append(indentText);
                    builder.AppendLine("[");

                    EmitInternalRecursive(ref builder, (JsonValue[])v.Value, indent);

                    builder.Append(indentText);
                    builder.Append("]");
                }
                else if (v.Value is string)
                {
                    builder.Append("\"");
                    builder.Append((string)v.Value);
                    builder.Append("\"");
                }
                else if (v.Value is byte)
                {
                    builder.Append("\"");
                    builder.Append((byte)v.Value);
                    builder.Append("\"");
                }
                else if (v.Value is byte[])
                {
                    builder.Append("\"");
                    builder.Append(ByteArrayToStr((byte[])v.Value));
                    builder.Append("\"");
                }
                else
                    throw new ArgumentException("Unintended value type.");

                if (firstTime)
                    firstTime = false;
            }
        }

        public string Emit()
        {
            StringBuilder builder = new StringBuilder();

            EmitInternalRecursive(ref builder, Values.ToArray(), 0);

            return builder.ToString();
        }

        public JsonValue[] GetValues()
        {
            return Values.ToArray();
        }
    }
}
