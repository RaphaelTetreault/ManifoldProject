using System.IO;

namespace Manifold.IO
{
    public static partial class StreamReaderExtensions
    {
        public static string ReadLineSkipChars(this StreamReader reader, int skipCount)
        {
            var value = reader.ReadLine();
            return value.Substring(skipCount);
        }

        public static string ReadLineSkipPrefix(this StreamReader reader, string skipText)
        {
            // Read a line of text
            var value = reader.ReadLine();

            // Ensure prefix we want to skip matches our new line of text's prefix
            Assert.IsTrue(skipText.Equals(value.Substring(0, skipText.Length)));

            // Remove prefix from line, return value
            value = value.Remove(0, skipText.Length);
            return value;
        }
    }
}
