using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A trigger volume of unknown purpose. Some courses have a lot of these,
    /// some courses have few if not none at all.
    /// </summary>
    /// <remarks>
    /// The default shape of the trigger appears to be a cube. The default scale of the cube
    /// is 10 units per side.
    /// </remarks>
    [Serializable]
    public sealed class CullOverrideTrigger :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        private TransformTRXS transform;
        private EnumFlags32 unk_0x20;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public TransformTRXS Transform { get => transform; set => transform = value; }
        public EnumFlags32 Unk_0x20 { get => unk_0x20; set => unk_0x20 = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transform);
                reader.ReadX(ref unk_0x20);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
               writer.WriteX(transform);
               writer.WriteX(unk_0x20);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString() => PrintSingleLine();

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(CullOverrideTrigger));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x20)}: {unk_0x20}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(transform)}: {transform}");
        }

        public string PrintSingleLine()
        {
            return $"{nameof(CullOverrideTrigger)}({nameof(unk_0x20)}: {unk_0x20})";
        }

    }
}
