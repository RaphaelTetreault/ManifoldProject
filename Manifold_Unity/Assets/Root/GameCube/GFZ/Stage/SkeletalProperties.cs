using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Data (presumably) referenced by a SkeletalAnimator.
    /// </summary>
    [Serializable]
    public sealed class SkeletalProperties :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        private EnumFlags32 unk_0x00;
        private EnumFlags32 unk_0x04;
        private EnumFlags32 unk_0x08;
        private uint zero_0x0C;
        private uint zero_0x10;
        private uint zero_0x14;
        private uint zero_0x18;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        /// <summary>
        /// Values: 0, 3, 7, 10, 15, 20, 50, 60
        /// </summary>
        public EnumFlags32 Unk_0x00 { get => unk_0x00; set => unk_0x00 = value; }
        public EnumFlags32 Unk_0x04 { get => unk_0x04; set => unk_0x04 = value; }
        public EnumFlags32 Unk_0x08 { get => unk_0x08; set => unk_0x08 = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref zero_0x0C);
                reader.ReadX(ref zero_0x10);
                reader.ReadX(ref zero_0x14);
                reader.ReadX(ref zero_0x18);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero_0x0C == 0);
                Assert.IsTrue(zero_0x10 == 0);
                Assert.IsTrue(zero_0x14 == 0);
                Assert.IsTrue(zero_0x18 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(unk_0x08);
                writer.WriteX(zero_0x0C);
                writer.WriteX(zero_0x10);
                writer.WriteX(zero_0x14);
                writer.WriteX(zero_0x18);
            }
            this.RecordEndAddress(writer);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Unk_0x00)}: {Unk_0x00}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Unk_0x04)}: {Unk_0x04}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Unk_0x08)}: {Unk_0x08}");
        }

        public string PrintSingleLine()
        {
            return nameof(SkeletalProperties);
        }

        public override string ToString() => PrintSingleLine();


    }
}