using System.IO;

namespace Manifold.IO
{
    public static partial class StreamReaderExtensions
    {
        /// <summary>
        /// Read a line from <paramref name="reader"/>, skipping <paramref name="skipCount"/> characters.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="skipCount">The number of characters to skip.</param>
        /// <returns>
        /// Returns string from ReadLine() after <paramref name="skipCount"/> characters.
        /// </returns>
        public static string ReadLineSkipChars(this StreamReader reader, int skipCount)
        {
            var value = reader.ReadLine();
            return value.Substring(skipCount);
        }

        /// <summary>
        /// Read a line from <paramref name="reader"/>, skipping <paramref name="skipText"/> text.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="skipText">The text to skip in the read string.</param>
        /// <returns>
        /// Returns string from ReadLine() stripping out <paramref name="skipText"/> from the beginning.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Exception thrown if the skipped text does not match the supplied <paramref name="skipText"/>.
        /// </exception>
        public static string ReadLineSkipPrefix(this StreamReader reader, string skipText)
        {
            // Read a line of text
            var value = reader.ReadLine();

            // Ensure prefix we want to skip matches our new line of text's prefix
            bool validSkip = skipText.Equals(value.Substring(0, skipText.Length));
            if (!validSkip)
                throw new System.ArgumentException("String skipped is does not match supplied argument.");

            // Remove prefix from line, return value
            value = value.Remove(0, skipText.Length);
            return value;
        }
    }
}
