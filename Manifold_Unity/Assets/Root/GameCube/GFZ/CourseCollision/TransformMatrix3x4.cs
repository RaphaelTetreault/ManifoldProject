using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Column-major matrix with 3 rows and 4 columns.
    /// </summary>
    [Serializable]
    public class TransformMatrix3x4 : IBinarySerializable, IBinaryAddressable
    {
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        [UnityEngine.SerializeField]
        private float4x4 matrix;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public float3 Position => matrix.Position();
        public quaternion Rotation => matrix.RotationBad();
        public float3 rotationEuler => matrix.RotationEuler();
        public float3 Scale => matrix.Scale();

        public float4x4 Matrix
        {
            get => matrix;
            set => matrix = value;
        }

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
            // the matrix returns only colums but we need rows, so transpose to swap row/col
            var matrixTransposed = math.transpose(matrix);
            var row0 = matrixTransposed.c0;
            var row1 = matrixTransposed.c1;
            var row2 = matrixTransposed.c2;

            // Write rows
            writer.WriteX(row0);
            writer.WriteX(row1);
            writer.WriteX(row2);
        }

    }
}