using System.Text;

namespace Manifold
{
    public static class StringBuilderExtension
    {
        /// <summary>
        /// Appends <paramref name="value"/> as many times as specified by <paramref name="repetitions"/>
        /// </summary>
        /// <param name="stringBuilder">The string builder to use.</param>
        /// <param name="value">The value to append.</param>
        /// <param name="repetitions">How many times to append the value.</param>
        public static void AppendRepeat(this StringBuilder stringBuilder, string value, int repetitions)
        {
            for (int i = 0; i < repetitions; i++)
            {
                stringBuilder.Append(value);
            }
        }

        /// <summary>
        /// Appends <paramref name="value"/> as many times as specified by <paramref name="repetitions"/>
        /// </summary>
        /// <param name="stringBuilder">The string builder to use.</param>
        /// <param name="value">The value to append.</param>
        /// <param name="repetitions">How many times to append the value.</param>
        public static void AppendRepeat(this StringBuilder stringBuilder, char value, int repetitions)
        {
            for (int i = 0; i < repetitions; i++)
            {
                stringBuilder.Append(value);
            }
        }

        /// <summary>
        /// Appends <paramref name="indent"/> as many times as specified by <paramref name="indentLevel"/>
        /// </summary>
        /// <param name="stringBuilder">The string builder to use.</param>
        /// <param name="indent">The value to use for indenting.</param>
        /// <param name="indentLevel">How many times to append the indent character.</param>
        /// <param name="value">The value to append.</param>
        public static void AppendLineIndented(this StringBuilder stringBuilder, string indent, int indentLevel, string value)
        {
            stringBuilder.AppendRepeat(indent, indentLevel);
            stringBuilder.AppendLine(value);
        }

        /// <summary>
        /// Appends <paramref name="indent"/> as many times as specified by <paramref name="indentLevel"/>
        /// </summary>
        /// <param name="stringBuilder">The string builder to use.</param>
        /// <param name="indent">The value to use for indenting.</param>
        /// <param name="indentLevel">How many times to append the indent character.</param>
        /// <param name="value">The value to append.</param>
        public static void AppendLineIndented(this StringBuilder stringBuilder, char indent, int indentLevel, string value)
        {
            stringBuilder.AppendRepeat(indent, indentLevel);
            stringBuilder.AppendLine(value);
        }



    }
}
