using Manifold;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ
{
    // TODO: this might be a built-in GameCube.GX type.

    /// <summary>
    /// Column-major transform matrix with 3 rows and 4 columns.
    /// </summary>
    [Serializable]
    public class TransformMatrix3x4 :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // "FIELDS" as reconstructed for ease of use (it's really 3 rows x 4 columns)
        private float4x4 matrix;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public float4x4 Matrix { get => matrix; set => matrix = value; }
        public float3 Position => matrix.Position();
        public quaternion Rotation => matrix.Rotation();
        public float3 RotationEuler => matrix.RotationEuler();
        public float3 Scale => matrix.Scale();


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            // The data is stored as rows
            float4 row0 = new float4();
            float4 row1 = new float4();
            float4 row2 = new float4();
            // The final row is implied (constant) in a 3x4 matrix
            float4 row3 = new float4(0, 0, 0, 1);

            this.RecordStartAddress(reader);
            {
                // Read rows
                reader.ReadX(ref row0);
                reader.ReadX(ref row1);
                reader.ReadX(ref row2);
            }
            this.RecordEndAddress(reader);
            {
                // matrix is constructed from 4 columns -NOT- rows...
                var matrixTransposed = new float4x4(row0, row1, row2, row3);
                // ...thus we need to transpose afterwards to swap rows and columns
                matrix = math.transpose(matrixTransposed);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                // the matrix only returns colunms but we need rows, so transpose to swap row/col
                var matrixTransposed = math.transpose(matrix);
                var row0 = matrixTransposed.c0;
                var row1 = matrixTransposed.c1;
                var row2 = matrixTransposed.c2;

                // Write rows
                writer.WriteX(row0);
                writer.WriteX(row1);
                writer.WriteX(row2);
            }
            this.RecordEndAddress(writer);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(TransformMatrix3x4));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Position)}({Position})");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Rotation)}({RotationEuler})");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Scale)}({Scale})");
        }

        public string PrintSingleLine()
        {
            return nameof(TransformMatrix3x4);
        }

        public override string ToString() => PrintSingleLine();

    }
}