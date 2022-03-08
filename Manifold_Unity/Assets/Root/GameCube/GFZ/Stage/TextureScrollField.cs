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
    public class TextureScrollField :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        public float x; // range -3 to 6, indexes: 0-3
        public float y; // range -10 to 30, indexes: 0-3


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref x);
                reader.ReadX(ref y);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(x);
                writer.WriteX(y);
            }
            this.RecordEndAddress(writer);
        }

        public string PrintMultiLine(int indentLevel = 0, string indent = "\t")
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            return builder.ToString();
        }

        public string PrintSingleLine()
        {
            return $"{nameof(TextureScrollField)}({nameof(x)}: {x}, {nameof(y)}: {y})";
        }

        public override string ToString() => PrintSingleLine();

    }
}