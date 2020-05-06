using StarkTools.IO;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class CollisionBinding : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        [Hex(numDigits: 4)]
        public uint unk_0x00;
        [Hex(numDigits: 4)]
        public uint unk_0x04;
        public uint referenceBindingRelPtr;
        public uint collisionRelPtr;

        public ReferenceBinding referenceBinding;
        public Collision collision;

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
            reader.ReadX(ref referenceBindingRelPtr);
            reader.ReadX(ref collisionRelPtr);

            endAddress = reader.BaseStream.Position;

            // Get/set offsets!
            throw new NotImplementedException();
            reader.ReadX(ref referenceBinding, true);
            reader.ReadX(ref collision, true);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(referenceBindingRelPtr);
            writer.WriteX(collisionRelPtr);

            //
            throw new NotImplementedException();
        }

        #endregion

    }
}