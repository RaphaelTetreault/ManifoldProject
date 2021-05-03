using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Column-major matrix with 3 rows and 4 columns.
    /// </summary>
    [Serializable]
    public class TransformMatrix3x4 : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        [SerializeField]
        private Matrix4x4 matrix;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public Vector3 Position => matrix.GetColumn(3);
        public Quaternion Rotation => matrix.rotation;
        public Vector3 RotationEuler => matrix.rotation.eulerAngles;
        public Vector3 Scale => matrix.lossyScale;

        public Matrix4x4 Matrix
        {
            get => matrix;
            set => matrix = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            // The data is stored as rows
            Vector4 row0 = new Vector4();
            Vector4 row1 = new Vector4();
            Vector4 row2 = new Vector4();
            // The final row is implied (constant) in a 3x4 matrix
            Vector4 row3 = new Vector4(0, 0, 0, 1);
            
            this.RecordStartAddress(reader);
            {
                // Read rows
                reader.ReadX(ref row0);
                reader.ReadX(ref row1);
                reader.ReadX(ref row2);
            }
            this.RecordEndAddress(reader);
            {
                // Unity Matrix4x4 is constructed from 4 columns -NOT- rows...
                matrix = new Matrix4x4(row0, row1, row2, row3);
                // ...thus we need to transpose afterwards to swap rows and columns
                matrix =  matrix.transpose;
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            // The data is stored as row-major
            var row0 = matrix.GetRow(0);
            var row1 = matrix.GetRow(1);
            var row2 = matrix.GetRow(2);

            // Write rows
            writer.WriteX(row0);
            writer.WriteX(row1);
            writer.WriteX(row2);
        }

    }
}