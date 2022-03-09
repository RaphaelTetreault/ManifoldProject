using Manifold;
using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ
{
    public struct BoundingSphere :
        IBinarySerializable,
        IBinaryAddressable,
        ITextPrintable
    {
        // FIELDS
        public float3 origin;
        public float radius;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            var addressRange = new AddressRange();
            addressRange.RecordStartAddress(reader);
            {
                reader.ReadX(ref origin);
                reader.ReadX(ref radius);
            }
            addressRange.RecordEndAddress(reader);
            AddressRange = addressRange;
        }

        public void Serialize(BinaryWriter writer)
        {
            var addressRange = new AddressRange();
            addressRange.RecordStartAddress(writer);
            {
                writer.WriteX(origin);
                writer.WriteX(radius);
            }
            addressRange.RecordEndAddress(writer);
            AddressRange = addressRange;
        }


        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(BoundingSphere));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(origin)}: {origin}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(radius)}: {radius}");
        }

        public string PrintSingleLine()
        {
            return nameof(BoundingSphere);
        }

        public override string ToString() => PrintSingleLine();

    }
}
