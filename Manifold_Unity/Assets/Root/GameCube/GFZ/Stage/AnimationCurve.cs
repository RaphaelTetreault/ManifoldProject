using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A series of animation keys to define how a singular float value changes over "time"
    /// </summary>
    /// <remarks>
    /// Structurally, this class is a simple wrapper around a KeyableAttribute[].
    /// </remarks>
    [Serializable]
    public sealed class AnimationCurve :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        private KeyableAttribute[] keyableAttributes;


        // CONSTRUCTORS
        public AnimationCurve()
        {
            keyableAttributes = new KeyableAttribute[0];
        }
        public AnimationCurve(int numKeyables)
        {
            keyableAttributes = new KeyableAttribute[numKeyables];
        }
        public AnimationCurve(KeyableAttribute[] keyables)
        {
            keyableAttributes = keyables;
        }


        // INDEXERS
        /// <summary>
        /// Indexes into the underlying <cref>KeyableAttribute</cref> array.
        /// </summary>
        /// <param name="index">The index of the desired value.</param>
        /// <returns>
        /// Returns the <cref>KeyableAttribute</cref> at the specified <paramref name="index"/>.
        /// </returns>
        public KeyableAttribute this[int index] { get => keyableAttributes[index]; set => keyableAttributes[index] = value; }

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int Length => keyableAttributes.Length;
        public KeyableAttribute[] KeyableAttributes { get => keyableAttributes; set => keyableAttributes = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref keyableAttributes, KeyableAttributes.Length);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            if (keyableAttributes.Length > 0)
            {
                this.RecordStartAddress(writer);
                {
                    writer.WriteX(keyableAttributes);
                }
                this.RecordEndAddress(writer);
            }
            else
            {
                // If nothing to serialize, zero out address as
                // it will be used to get ptr
                AddressRange = new AddressRange();
            }
        }

        /// <summary>
        /// Forwards the underlying KeyableAttributes[].GetArrayPointer();
        /// </summary>
        /// <returns></returns>
        public ArrayPointer GetArrayPointer()
        {
            return keyableAttributes.GetArrayPointer();
        }

        public override string ToString()
        {
            return PrintSingleLine();
        }

        public string PrintSingleLine()
        {
            // Print a summary of how many animation keys this curve has
            return $"{nameof(AnimationCurve)}({nameof(KeyableAttribute)}s:[{keyableAttributes.Length}])";
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            int lengthKeyables = keyableAttributes.Length;
            var stringBuilder = new System.Text.StringBuilder();

            // Print overview of the animation curve
            stringBuilder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            indentLevel++;
            // Then print each keyable (single line) on their own line
            for (int i = 0; i < lengthKeyables; i++)
            {
                var keyable = keyableAttributes[i];
                var keyableText = keyable.PrintSingleLine();
                stringBuilder.AppendLineIndented(indent, indentLevel, $"[{i}]\t {keyableText}");
            }
        }
    }
}
