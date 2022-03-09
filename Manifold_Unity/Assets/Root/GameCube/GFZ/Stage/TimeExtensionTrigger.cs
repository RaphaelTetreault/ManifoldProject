using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    // 2022/01/27: previously ArcadeCheckpointTrigger

    /// <summary>
    /// A checkpoint used in the F-Zero AX style arcade mode. When passed through, the
    /// game will add extend the remaining race time. This is used in the AX cup courses
    /// Port Town [Cylinder Wave], Lightning [Thunder Road], and Green Plant [Spiral].
    /// </summary>
    /// <remarks>
    /// TODO: it is unclear where the actual time is defined per checkpoint or course.
    /// </remarks>
    [Serializable]
    public sealed class TimeExtensionTrigger :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        private TransformTRXS transform;
        private TimeExtensionOption option;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public TimeExtensionOption Option { get => option; set => option = value; }
        public TransformTRXS Transform { get => transform; set => transform = value; }


        //METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transform);
                reader.ReadX(ref option);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(transform);
                writer.WriteX(option);
            }
            this.RecordEndAddress(writer);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(TimeExtensionTrigger));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Option)}: {Option}");
            builder.AppendLineIndented(indent, indentLevel, Transform);
        }

        public string PrintSingleLine()
        {
            return $"{nameof(TimeExtensionTrigger)}({Option})";
        }

        public override string ToString() => PrintSingleLine();

    }
}
