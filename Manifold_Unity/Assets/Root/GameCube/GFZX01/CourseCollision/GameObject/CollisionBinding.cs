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

            {
                Assert.IsTrue(referenceBindingRelPtr != 0);
                reader.BaseStream.Seek(referenceBindingRelPtr, SeekOrigin.Begin);
                reader.ReadX(ref referenceBinding, true);
            }

            //if (collisionRelPtr > 0)
            //{
            //    Debug.Log(referenceBinding.name);
            //    var absPtr = collisionRelPtr;
            //    reader.BaseStream.Seek(absPtr, SeekOrigin.Begin);
            //    reader.ReadX(ref collision, true);
            //}

            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
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