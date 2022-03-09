using System.Text;

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
        public void PrintMultiLine(StringBuilder builder, int indentLevel = 0, string indent = "\t");
    }
}
