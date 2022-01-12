using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Binds a SceneObjectDynamic to a SceneObjectReference. Includes some other data.
    /// </summary>
    [Serializable]
    public class SceneObjectTemplate :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        public string nameCopy; // todo: use forwarder instead? Is that not safer?

        // STRUCTURE
        public UnkInstanceFlag unk_0x00;
        public UnkInstanceOption unk_0x04;
        public Pointer sceneObjectPtr;
        public Pointer colliderGeometryPtr;
        // FIELDS (deserialized from pointers)
        public SceneObject sceneObject;
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
                reader.ReadX(ref sceneObjectPtr);
                reader.ReadX(ref colliderGeometryPtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(sceneObjectPtr.IsNotNullPointer);
                reader.JumpToAddress(sceneObjectPtr);
                reader.ReadX(ref sceneObject, true);

                // Collision is not required, load only if pointer is not null
                if (colliderGeometryPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(colliderGeometryPtr);
                    reader.ReadX(ref colliderGeometry, true);
                }

                nameCopy = sceneObject.name;
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                sceneObjectPtr = sceneObject.GetPointer();
                colliderGeometryPtr = colliderGeometry.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(sceneObjectPtr);
                writer.WriteX(colliderGeometryPtr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // This pointer CANNOT be null and must refer to an object.
            Assert.IsTrue(sceneObjectPtr.IsNotNullPointer);
            Assert.IsTrue(sceneObject != null);
        }

        public override string ToString()
        {
            return 
                $"{nameof(SceneObjectTemplate)}(" +
                $"{nameof(unk_0x00)}: {unk_0x00}, " +
                $"{nameof(unk_0x04)}: {unk_0x04}, " +
                $"Has {nameof(ColliderGeometry)}: {colliderGeometryPtr.IsNotNullPointer}, " +
                $"Name: {nameCopy}" +
                $")";
        }

    }
}