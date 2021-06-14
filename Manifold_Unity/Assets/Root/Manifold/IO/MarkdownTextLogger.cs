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


        public void WriteLineSummary<T>(T[] values, int typeWidth = 32, int countWidth = 5)
        {
            var typeName = typeof(T).Name.PadRight(typeWidth);
            var count = values.Length.ToString().PadLeft(countWidth);

            WriteLine($"{typeName} Count: {count}");
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
