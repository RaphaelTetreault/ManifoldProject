using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class TransformMatrix4x3 : IBinarySerializable, IBinaryAddressableRange
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

        public void Deserialize(BinaryReader reader)
        {
            Vector4 mtx0 = new Vector4();
            Vector4 mtx1 = new Vector4();
            Vector4 mtx2 = new Vector4();

            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref mtx0);
                reader.ReadX(ref mtx1);
                reader.ReadX(ref mtx2);
            }
            this.RecordEndAddress(reader);
            {
                matrix = new Matrix4x4(
                    new Vector4(mtx0.x, mtx1.x, mtx2.x, 0),
                    new Vector4(mtx0.y, mtx1.y, mtx2.y, 0),
                    new Vector4(mtx0.z, mtx1.z, mtx2.z, 0),
                    new Vector4(mtx0.w, mtx1.w, mtx2.w, 1)
                    );
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}