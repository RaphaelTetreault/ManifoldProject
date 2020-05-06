using StarkTools.IO;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class Collision : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public uint unk_0x00;
        public uint unk_0x04;
        public uint unk_0x08;
        public uint unk_0x0C;
        public uint unk_0x10;
        public int triCount;
        public int quadCount;
        public uint triRelPtr;
        public uint quadRelPtr;

        public CollisionTri[] tris;
        public CollisionQuad[] quads;

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
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref unk_0x08);
            reader.ReadX(ref unk_0x0C);
            reader.ReadX(ref unk_0x10);
            reader.ReadX(ref triCount);
            reader.ReadX(ref quadCount);
            reader.ReadX(ref triRelPtr);
            reader.ReadX(ref quadRelPtr);

            endAddress = reader.BaseStream.Position;

            // Get/set offsets!
            throw new NotImplementedException();
            reader.ReadX(ref tris, triCount, true);
            reader.ReadX(ref quads, quadCount, true);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(unk_0x08);
            writer.WriteX(unk_0x0C);
            writer.WriteX(unk_0x10);
            writer.WriteX(triCount);
            writer.WriteX(quadCount);
            writer.WriteX(triRelPtr);
            writer.WriteX(quadRelPtr);

            // You need to implement saving the tris/quads
            throw new NotImplementedException();
        }

        #endregion

    }
}