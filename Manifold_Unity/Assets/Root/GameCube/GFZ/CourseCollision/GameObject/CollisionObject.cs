using Manifold.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class CollisionObject : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        [Hex(numDigits: 4)]
        public uint unk_0x00;
        [Hex(numDigits: 4)]
        public uint unk_0x04;
        public uint referenceBindingAbsPtr;
        public uint collisionAbsPtr;

        public CollisionObjectReference referenceBinding;
        public Collision collision;


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
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref referenceBindingAbsPtr);
                reader.ReadX(ref collisionAbsPtr);
            }
            this.RecordEndAddress(reader);
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
            this.SetReaderToEndAddress(reader);
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