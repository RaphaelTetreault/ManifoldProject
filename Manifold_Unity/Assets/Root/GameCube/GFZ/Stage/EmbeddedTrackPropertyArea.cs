using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Descriptor for where along the track a desireble surface attribute is located
    /// such as boost plates, jump plates, and heal strips. Likely for use by AI.
    /// </summary>
    [Serializable]
    public sealed class EmbeddedTrackPropertyArea :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        // Default values represent final terminating node.
        private float lengthFrom = -1f;
        private float lengthTo = -1f;
        private float widthLeft = 0;
        private float widthRight = 0;
        private EmbeddedTrackPropertyType propertyType = EmbeddedTrackPropertyType.TerminateCode;
        private byte trackBranchID = 0;
        private ushort zero_0x12;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public float LengthFrom { get => lengthFrom; set => lengthFrom = value; }
        public float LengthTo { get => lengthTo; set => lengthTo = value; }
        public EmbeddedTrackPropertyType PropertyType { get => propertyType; set => propertyType = value; }
        public byte TrackBranchID { get => trackBranchID; set => trackBranchID = value; }
        public float WidthLeft { get => widthLeft; set => widthLeft = value; }
        public float WidthRight { get => widthRight; set => widthRight = value; }


        // STATIC METHODS
        public static EmbeddedTrackPropertyArea Terminator()
        {
            return new EmbeddedTrackPropertyArea()
            {
                LengthFrom = -1f,
                LengthTo = -1f,
                WidthLeft = 0,
                WidthRight = 0,
                PropertyType = EmbeddedTrackPropertyType.TerminateCode,
                TrackBranchID = 0,
                zero_0x12 = 0,
            };
        }
        public static EmbeddedTrackPropertyArea[] DefaultArray()
        {
            return new EmbeddedTrackPropertyArea[]
            {
                Terminator(),
            };
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref lengthFrom);
                reader.ReadX(ref lengthTo);
                reader.ReadX(ref widthLeft);
                reader.ReadX(ref widthRight);
                reader.ReadX(ref propertyType);
                reader.ReadX(ref trackBranchID);
                reader.ReadX(ref zero_0x12);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero_0x12 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero_0x12 == 0);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(lengthFrom);
                writer.WriteX(lengthTo);
                writer.WriteX(widthLeft);
                writer.WriteX(widthRight);
                writer.WriteX(propertyType);
                writer.WriteX(trackBranchID);
                writer.WriteX(zero_0x12);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString() => PrintSingleLine();

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(EmbeddedTrackPropertyArea));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(trackBranchID)}: {trackBranchID}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(propertyType)}: {propertyType}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(lengthFrom)}: {lengthFrom}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(lengthTo)}: {lengthTo}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(widthLeft)}: {widthLeft}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(widthRight)}: {widthRight}");
        }

        public string PrintSingleLine()
        {
            return $"{nameof(EmbeddedTrackPropertyArea)}({trackBranchID}: {trackBranchID}, {propertyType}: {propertyType})";
        }

    }
}
