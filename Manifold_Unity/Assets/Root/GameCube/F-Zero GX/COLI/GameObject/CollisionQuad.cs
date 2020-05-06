using StarkTools.IO;
using System.IO;
using UnityEngine;

namespace GameCube.FZeroGX.COLI_COURSE
{
    public class CollisionQuad : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public float unk_0x00;
        public Vector3 normal;
        public Vector3 vertex0;
        public Vector3 vertex1;
        public Vector3 vertex2;
        public Vector3 vertex3;
        public Vector3 unk_0x44;
        public Vector3 unk_0x50;
        public Vector3 unk_0x5C;
        public Vector3 unk_0x68;

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
            reader.ReadX(ref unk_0x44);
            reader.ReadX(ref unk_0x50);
            reader.ReadX(ref unk_0x5C);
            reader.ReadX(ref unk_0x68);

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
            writer.WriteX(unk_0x44);
            writer.WriteX(unk_0x50);
            writer.WriteX(unk_0x5C);
            writer.WriteX(unk_0x68);
        }

        #endregion

    }
}