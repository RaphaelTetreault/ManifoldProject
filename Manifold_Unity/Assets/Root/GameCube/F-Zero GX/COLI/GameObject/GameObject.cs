using StarkTools.IO;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class GameObject : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public uint unk_0x00;
        public uint unk_0x04;
        public uint lodRelPtr;
        public Vector3 position;
        public uint unk_0x18;
        public uint unk_0x1C;
        public Vector3 scale;
        public uint collisionPtr; // Abs or Rel?
        public uint animationPtr; // Abs or Rel?
        public uint unkPtr_0x34; // Abs or Rel?
        public uint unkPtr_0x38; // Abs or Rel?
        public uint transformPtr; // Abs or Rel?

        //public LOD lod;
        public CollisionBinding collisionBinding;



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
            reader.ReadX(ref lodRelPtr);
            reader.ReadX(ref position);
            reader.ReadX(ref unk_0x18);
            reader.ReadX(ref unk_0x1C);
            reader.ReadX(ref scale);
            reader.ReadX(ref collisionPtr);
            reader.ReadX(ref animationPtr);
            reader.ReadX(ref unkPtr_0x34);
            reader.ReadX(ref unkPtr_0x38);
            reader.ReadX(ref transformPtr);

            endAddress = reader.BaseStream.Position;

            // Set address
            throw new NotImplementedException();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(lodRelPtr);
            writer.WriteX(position);
            writer.WriteX(unk_0x18);
            writer.WriteX(unk_0x1C);
            writer.WriteX(scale);
            writer.WriteX(collisionPtr);
            writer.WriteX(animationPtr);
            writer.WriteX(unkPtr_0x34);
            writer.WriteX(unkPtr_0x38);
            writer.WriteX(transformPtr);

            // Write values pointed at by, update ptrs above
            throw new NotImplementedException();
        }

        #endregion

    }
}