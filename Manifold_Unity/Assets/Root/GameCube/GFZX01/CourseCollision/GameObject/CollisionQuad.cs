using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.CourseCollision
{
    [Serializable]
    public class CollisionQuad : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        // Normal's quaternion rotation theta? https://en.wikipedia.org/wiki/Quaternions_and_spatial_rotation#Using_quaternion_as_rotations
        // Value range: -3613.961 through 3595.046, avg: -11 (basically 0)
        // Could possibly be "bounding sphere" radius/diameter from avg of all positions?
        public float unk_0x00;
        public Vector3 normal;
        public Vector3 vertex0;
        public Vector3 vertex1;
        public Vector3 vertex2;
        public Vector3 vertex3;
        public Vector3 precomputed0;
        public Vector3 precomputed1;
        public Vector3 precomputed2;
        public Vector3 precomputed3;

        #endregion

        #region PROPERTIES

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref normal);
            reader.ReadX(ref vertex0);
            reader.ReadX(ref vertex1);
            reader.ReadX(ref vertex2);
            reader.ReadX(ref vertex3);
            reader.ReadX(ref precomputed0);
            reader.ReadX(ref precomputed1);
            reader.ReadX(ref precomputed2);
            reader.ReadX(ref precomputed3);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(normal);
            writer.WriteX(vertex0);
            writer.WriteX(vertex1);
            writer.WriteX(vertex2);
            writer.WriteX(vertex3);
            writer.WriteX(precomputed0);
            writer.WriteX(precomputed1);
            writer.WriteX(precomputed2);
            writer.WriteX(precomputed3);
        }

        #endregion

    }
}