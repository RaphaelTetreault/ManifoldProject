using System;
using System.IO;

namespace Manifold.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownTextLogger
    {
        private StreamWriter streamWriter;

        public MarkdownTextLogger(string path)
        {
            var file = File.Create(path);
            streamWriter = new StreamWriter(file);

            // Write a header
            WriteInfo();
        }

        public void WriteInfo()
        {
            var dateTime = DateTime.Now;
            streamWriter.WriteLine($"Created by {nameof(MarkdownTextLogger)}.cs");
            streamWriter.WriteLine($"Date: {dateTime:yyyy-MM-dd}");
            streamWriter.WriteLine($"Time: {dateTime:HH:mm:ss}");
            streamWriter.WriteLine();
        }

        public void WriteLine() => streamWriter.WriteLine();
        public void WriteLine(string value) => streamWriter.WriteLine(value);
        public void Write(string value) => streamWriter.Write(value);
        public void WriteLineColor(string value, byte r, byte g, byte b)
        {
            //streamWriter.WriteLine($"<font color={r:x2}{g:x2}{b:x2}>{value}</font>");
            streamWriter.WriteLine(value);
        }
        public void WriteLineHighlight(string value, byte r, byte g, byte b)
        {
            streamWriter.WriteLine($"<span style=\"background-color:#{r:x2}{g:x2}{b:x2}\">{value}</span>");
        }

        public void WriteLine(object value) => streamWriter.WriteLine(value);
        public void Write(object value) => streamWriter.Write(value);

        public bool WriteNullInArray { get; set; } = true;

        public void WriteAddress<T>(T value, bool inlineType = true, int typePadRight = 24)
            where T : IBinaryAddressable
        {
            var typeDesc = inlineType ?
                $"\t{typeof(T).Name.PadRight(typePadRight)}"
                : string.Empty;

            if (value == null)
            {
                WriteLine($"Address: 0x{0:x8}{typeDesc}\tnull");
            }
            else if (!value.GetPointer().IsNotNullPointer)
            {
                WriteLine($"Address: 0x{0:x8}{typeDesc}\t(pointer is null, reference exists)");
            }
            else
            {
                var address = value.AddressRange.startAddress;
                WriteLine($"Address: 0x{address:x8}{typeDesc}\t{value}");
            }
        }

        public void WriteAddress<T>(T[] values, bool inlineType = false, int typePadRight = 24)
            where T : IBinaryAddressable
        {
            if (values.IsNullOrEmpty())
                return;

            int lengthLength = values.Length.ToString().Length;

            WriteLine($"{typeof(T).Name} Length[{values.Length}]");
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];

                // Do not write null entries if we don't want them
                if (value == null || !value.GetPointer().IsNotNullPointer)
                    if (!WriteNullInArray)
                        continue;

                var indexStr = i.ToString().PadLeft(lengthLength);
                Write($"Index: [{indexStr}]\t");
                WriteAddress(value, inlineType, typePadRight);
            }
            WriteLine();
        }


        public void WriteLineSummary<T>(T[] values, int typeWidth = 32, int countWidth = 5)
        {
            var typeName = typeof(T).Name.PadRight(typeWidth);
            var count = values.Length.ToString().PadLeft(countWidth);

            WriteLine($"{typeName} Count: {count}");
        }

        public void WriteDivider(string value, int times)
        {
            for (int i = 0; i < times; i++)
                streamWriter.Write(value);
            WriteLine();
        }

        public void WriteHeading(string heading, string padding, int width)
        {
            WriteDivider(padding, width);
            WriteLine(heading);
            WriteDivider(padding, width);
        }

        public void Close()
        {
            streamWriter.Flush();
            streamWriter.Close();
        }

        public void Flush()
        {
            streamWriter.Flush();
        }
    }
}
