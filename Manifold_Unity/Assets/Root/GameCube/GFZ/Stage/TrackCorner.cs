using Manifold;
using Manifold.IO;
using System;
using System.IO;


namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// This special metadata defines a 90-degree angle as part of the track geometry.
    /// It is assumed this is necessary for AI rather than anything else.
    /// </summary>
    [Serializable]
    public sealed class TrackCorner :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        private TransformMatrix3x4 transform; // never null
        private float width;
        private byte const_0x34; // Const: 0x02
        private byte zero_0x35; // Const: 0x00
        private TrackPerimeterFlags perimeterOptions;
        private byte zero_0x37; // Const: 0x00


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public TransformMatrix3x4 Transform { get => transform; set => transform = value; }
        public TrackPerimeterFlags PerimeterOptions { get => perimeterOptions; set => perimeterOptions = value; }
        public float Width { get => width; set => width = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transform);
                reader.ReadX(ref width);
                reader.ReadX(ref const_0x34);
                reader.ReadX(ref zero_0x35);
                reader.ReadX(ref perimeterOptions);
                reader.ReadX(ref zero_0x37);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(const_0x34 == 0x02);
                Assert.IsTrue(zero_0x35 == 0x00);
                Assert.IsTrue(zero_0x37 == 0x00);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(Transform != null);
                Assert.IsTrue(const_0x34 == 0x02, $"{nameof(const_0x34)} is not 0x02! Is: {const_0x34}");
                Assert.IsTrue(zero_0x35 == 0x00);
                Assert.IsTrue(zero_0x37 == 0x00);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(transform);
                writer.WriteX(width);
                writer.WriteX(const_0x34);
                writer.WriteX(zero_0x35);
                writer.WriteX(perimeterOptions);
                writer.WriteX(zero_0x37);
            }
            this.RecordEndAddress(writer);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(TrackCorner));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Width)}: {Width}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(PerimeterOptions)}: {PerimeterOptions}");
            builder.AppendLineIndented(indent, indentLevel, transform);
        }

        public string PrintSingleLine()
        {
            return nameof(TrackCorner);
        }

        public override string ToString() => PrintSingleLine();

    }
}