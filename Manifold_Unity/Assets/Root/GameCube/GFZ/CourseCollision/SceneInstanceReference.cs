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
        public uint unk_0x00; // 0, 1, 3, 64, 65, 193, 194, 195, 256 (flags? 1, 2, 64, 128, 256)
        // One case of 6. Almost all values > 1 have float in objRef, almost all with value 1 have 0f
        public uint unk_0x04; // 1, 2, 3, 4, 6 (flags? 1, 2, 4)
        public Pointer objectReferencePtr;
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
                reader.ReadX(ref objectReferencePtr);
                reader.ReadX(ref colliderGeometryPtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(objectReferencePtr.IsNotNullPointer);
                reader.JumpToAddress(objectReferencePtr);
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
                writer.WriteX(objectReferencePtr);
                writer.WriteX(colliderGeometryPtr);
            }
            this.RecordEndAddress(writer);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            Serialize(writer);
            return addressRange;
        }
    }
}