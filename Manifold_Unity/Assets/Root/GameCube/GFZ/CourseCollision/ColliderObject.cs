using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ColliderObject : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        // metadata
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // structure
        public uint unk_0x00;
        public uint unk_0x04;
        public Pointer objectAttributesPtr;
        public Pointer colliderGeometryPtr;

        // sub-structures
        public UnknownObjectAttributes objectAttributes;
        public ColliderGeometry colliderGeometry;


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
                reader.ReadX(ref objectAttributesPtr);
                reader.ReadX(ref colliderGeometryPtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(objectAttributesPtr.IsNotNullPointer);
                reader.JumpToAddress(objectAttributesPtr);
                reader.ReadX(ref objectAttributes, true);

                // Collision is not required, load only if pointer is not null
                if (colliderGeometryPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(colliderGeometryPtr);
                    reader.ReadX(ref colliderGeometry, true);
                }
            }
            this.SetReaderToEndAddress(reader);
        }


        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(objectAttributesPtr);
            writer.WriteX(colliderGeometryPtr);

            //
            throw new NotImplementedException();
        }

        #endregion

    }
}