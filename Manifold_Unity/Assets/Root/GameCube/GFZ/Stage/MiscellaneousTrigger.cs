using Manifold;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A trigger volume that has many different use cases depending on
    /// which course it appears. Consult enum comments for more details.
    /// </summary>
    [Serializable]
    public sealed class MiscellaneousTrigger :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        private TransformTRXS transform;
        private CourseMetadataType metadataType;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }

        // PROPERTIES USED TO MAKE SENSE OF THIS NONSENSE
        public float3 Position => Transform.Position;
        public float3 PositionFrom => Transform.Position;
        public float3 PositionTo => Transform.Scale;
        public float3 Scale => Transform.Scale;
        public quaternion Rotation => Transform.Rotation;
        public float3 RotationEuler => Transform.RotationEuler;

        public TransformTRXS Transform { get => transform; set => transform = value; }
        public CourseMetadataType MetadataType { get => metadataType; set => metadataType = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transform);
                reader.ReadX(ref metadataType);
            }
            this.RecordEndAddress(reader);
        }



        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(transform);
                writer.WriteX(metadataType);
            }
            this.RecordEndAddress(writer);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(MiscellaneousTrigger));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(metadataType)}: {metadataType}");
            builder.AppendLineIndented(indent, indentLevel, transform);
        }

        public string PrintSingleLine()
        {
            return $"{nameof(MiscellaneousTrigger)}({nameof(MetadataType)}: {MetadataType})";
        }

        public override string ToString() => PrintSingleLine();

    }
}
