using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZX01.CourseCollision
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
        public uint referenceBindingAbsPtr;
        public uint collisionAbsPtr;

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
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref referenceBindingAbsPtr);
                reader.ReadX(ref collisionAbsPtr);
            }
            endAddress = reader.BaseStream.Position;
            {
                Assert.IsTrue(referenceBindingAbsPtr != 0);
                reader.BaseStream.Seek(referenceBindingAbsPtr, SeekOrigin.Begin);
                reader.ReadX(ref referenceBinding, true);

                // Collision is not required, load only if pointer is not null
                if (collisionAbsPtr > 0)
                {
                    var absPtr = collisionAbsPtr;
                    reader.BaseStream.Seek(absPtr, SeekOrigin.Begin);
                    reader.ReadX(ref collision, true);
                }
            }
            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(referenceBindingAbsPtr);
            writer.WriteX(collisionAbsPtr);

            //
            throw new NotImplementedException();
        }

        #endregion

    }
}