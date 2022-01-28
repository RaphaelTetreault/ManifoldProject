namespace Manifold
{
    public interface ITextPrintable
    {
        /// <summary>
        /// Prints this value without newlines.
        /// </summary>
        /// <returns></returns>
        public string PrintSingleLine();

        /// <summary>
        /// Prints this value, possibly including newlines.
        /// </summary>
        /// <returns></returns>
        public string PrintMultiLine(string indent = "\t", int indentLevel = 0);
    }
}
