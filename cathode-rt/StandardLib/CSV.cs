using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace cathode_rt
{
    public static class CSVBackend
    {
        public enum CSVEntryType : byte
        {
            STRING,
            DOUBLE,
            LONG
        }

        public struct CSVEntry
        {
            public CSVEntryType EntryType;
            public object Value;
        }

        public class CSVParser
        {
            public string Text;
            public int Pointer;
            public char Separator;
            public char CurrentChar => Text[Pointer];

            public CSVParser(string text, char sep)
            {
                Text = text.Replace("\r\n", "\n");
                Text = text;
                Pointer = 0;
                Separator = sep;
            }

            private CSVEntry[] ParseLine()
            {
                List<CSVEntry> entries = new List<CSVEntry>();

                while (Pointer < Text.Length && CurrentChar != '\n')
                {
                    if (CurrentChar == '"')
                    {
                        ++Pointer;
                        StringBuilder strLiteralBuilder = new StringBuilder();

                        while (true)
                        {
                            if (CurrentChar == '"')
                            {
                                ++Pointer;
                                if (CurrentChar == '"')
                                {
                                    ++Pointer;
                                    strLiteralBuilder.Append('"');
                                }
                                else
                                    break;
                            }
                            else
                            {
                                strLiteralBuilder.Append(CurrentChar);
                                ++Pointer;
                            }
                        }

                        entries.Add(new CSVEntry() { EntryType = CSVEntryType.STRING, Value = strLiteralBuilder.ToString() });
                    }
                    else
                    {
                        StringBuilder otherLiteralBuilder = new StringBuilder();

                        while (true)
                        {
                            if (Pointer >= Text.Length)
                                break;

                            if (CurrentChar == Separator)
                            {
                                ++Pointer;
                                break;
                            }

                            if (CurrentChar == '\n')
                                break;

                            otherLiteralBuilder.Append(CurrentChar);
                            ++Pointer;
                        }

                        string val = otherLiteralBuilder.ToString();
                        if (double.TryParse(val, out double doubleVal))
                            entries.Add(new CSVEntry() { EntryType = CSVEntryType.DOUBLE, Value = doubleVal });
                        else if (long.TryParse(val, out long longVal))
                            entries.Add(new CSVEntry() { EntryType = CSVEntryType.LONG, Value = longVal });
                        else if (bool.TryParse(val, out bool boolVal))
                            entries.Add(new CSVEntry() { EntryType = CSVEntryType.LONG, Value = boolVal ? 1 : 0 });
                        else
                            entries.Add(new CSVEntry() { EntryType = CSVEntryType.STRING, Value = val });
                    }
                }

                ++Pointer;
                return entries.ToArray();
            }

            public CSVEntry[][] Parse()
            {
                if (Pointer == 0)
                    throw new Exception("GetHeaderLine() was not called before parsing the data");

                List<CSVEntry[]> lines = new List<CSVEntry[]>();

                while (Pointer < Text.Length)
                    lines.Add(ParseLine());

                return lines.ToArray();
            }

            public CSVEntry[] GetHeaderLine()
            {
                if (Pointer != 0)
                    throw new Exception("GetHeaderLine() was already called");

                return ParseLine();
            }
        }
    }

    public static partial class ImplMethods
    {
        [ZZFunction("csv", "ParseCsvFromFile")]
        public static ZZObject ParseCsvFromFile(ZZString filename, ZZObject sep)
        {
            try
            {
                string contents = File.ReadAllText(filename.ToString());
                return ParseCsv(contents, sep);
            }
            catch { return ZZVoid.Void; }
        }

        [ZZFunction("csv", "ParseCsv")]
        public static ZZObject ParseCsv(ZZString data, ZZObject sep)
        {
            try
            {
                ZZStruct strct = new ZZStruct();

                char separator = ',';

                if (sep != ZZVoid.Void)
                {
                    if (sep.ObjectType != ZZObjectType.STRING)
                        throw new ArgumentException();

                    if (string.IsNullOrEmpty(((ZZString)sep).Contents))
                        throw new ArgumentException();

                    separator = ((ZZString)sep).Contents[0];
                }

                // Parse header
                CSVBackend.CSVParser parser = new CSVBackend.CSVParser(data.Contents, separator);
                CSVBackend.CSVEntry[] headItems = parser.GetHeaderLine();

                List<ZZObject>[] listsForHeadItems = new List<ZZObject>[headItems.Length];
                for (int i = 0; i < headItems.Length; ++i)
                {
                    strct.Fields.Add(headItems[i].Value.ToString(), ZZVoid.Void);
                    listsForHeadItems[i] = new List<ZZObject>();
                }

                CSVBackend.CSVEntry[][] mainEntries = parser.Parse();
                for (int i = 0; i < mainEntries.Length; ++i)
                    for (int j = 0; j < mainEntries[i].Length; ++j)
                    {
                        CSVBackend.CSVEntry entry = mainEntries[i][j];

                        switch (entry.EntryType)
                        {
                            case CSVBackend.CSVEntryType.STRING:
                                listsForHeadItems[j].Add(new ZZString((string)entry.Value));
                                break;
                            case CSVBackend.CSVEntryType.LONG:
                                listsForHeadItems[j].Add(new ZZInteger((long)entry.Value));
                                break;
                            case CSVBackend.CSVEntryType.DOUBLE:
                                listsForHeadItems[j].Add(new ZZFloat((double)entry.Value));
                                break;
                        }
                    }

                for (int i = 0; i < headItems.Length; ++i)
                    strct.Fields[(ZZString)headItems[i].Value.ToString()] = new ZZArray(listsForHeadItems[i].ToArray());

                return strct;
            }
            catch { return ZZVoid.Void; }
        }

        [ZZFunction("csv", "MkCsv")]
        public static ZZStruct MkCsv(ZZArray head)
        {
            if (head.Objects.Length < 1)
                throw new ArgumentException();

            ZZStruct strct = new ZZStruct();

            foreach (ZZObject str in head.Objects)
                if (str.ObjectType != ZZObjectType.STRING)
                    throw new ArgumentException();
                else
                    strct.Fields.Add((ZZString)str, new ZZArray(Array.Empty<ZZObject>()));

            return strct;
        }
    }
}
