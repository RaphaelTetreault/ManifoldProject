using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ColliderTriangle : IBinarySerializable, IBinaryAddressableRange
    {

        #region MEMBERS


        [SerializeField]
        private AddressRange addressRange;

        public float unk_0x00;
        public Vector3 normal;
        public Vector3 vertex0;
        public Vector3 vertex1;
        public Vector3 vertex2;
        public Vector3 precomputed0;
        public Vector3 precomputed1;
        public Vector3 precomputed2;


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref normal);
                reader.ReadX(ref vertex0);
                reader.ReadX(ref vertex1);
                reader.ReadX(ref vertex2);
                reader.ReadX(ref precomputed0);
                reader.ReadX(ref precomputed1);
                reader.ReadX(ref precomputed2);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(normal);
            writer.WriteX(vertex0);
            writer.WriteX(vertex1);
            writer.WriteX(vertex2);
            writer.WriteX(precomputed0);
            writer.WriteX(precomputed1);
            writer.WriteX(precomputed2);
        }

        #endregion

    }
}