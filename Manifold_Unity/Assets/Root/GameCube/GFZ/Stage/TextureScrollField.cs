using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A single data field for <cref>TextureScroll</cref>.
    /// </summary>
    [Serializable]
    public sealed class TextureScrollField :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        public float u; // range -3 to 6, indexes: 0-3
        public float v; // range -10 to 30, indexes: 0-3


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref u);
                reader.ReadX(ref v);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(u);
                writer.WriteX(v);
            }
            this.RecordEndAddress(writer);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
        }

        public string PrintSingleLine()
        {
            return $"{nameof(TextureScrollField)}({nameof(u)}: {u}, {nameof(v)}: {v})";
        }

        public override string ToString() => PrintSingleLine();

    }
}