using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SceneInstanceReference :
        IBinarySeralizableReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // STRUCTURE
        public uint unk_0x00;
        public uint unk_0x04;
        public Pointer objectAttributesPtr;
        public Pointer colliderGeometryPtr;
        // FIELDS (deserialized from pointers)
        public SceneObjectReference objectReference;
        public ColliderGeometry colliderGeometry;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
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
                reader.ReadX(ref objectReference, true);

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
            {
                //objectAttributesPtr
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(objectAttributesPtr);
                writer.WriteX(colliderGeometryPtr);
            }
            this.RecordEndAddress(writer);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}