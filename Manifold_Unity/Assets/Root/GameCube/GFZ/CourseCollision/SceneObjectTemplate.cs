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
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // STRUCTURE
        public UnkInstanceFlag unk_0x00; // TODO: bool32 is array/has many?
        public ArrayPointer sceneObjectsPtr;
        public Pointer colliderGeometryPtr;
        // FIELDS (deserialized from pointers)
        public SceneObject[] sceneObjects;
        public ColliderGeometry colliderGeometry;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public string Name => sceneObjects[0].name;

        public SceneObject PrimarySceneObject => sceneObjects[0];

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref sceneObjectsPtr);
                reader.ReadX(ref colliderGeometryPtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(sceneObjectsPtr.IsNotNullPointer);
                reader.JumpToAddress(sceneObjectsPtr);
                reader.ReadX(ref sceneObjects, sceneObjectsPtr.Length, true);

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
                sceneObjectsPtr = sceneObjects.GetArrayPointer();
                colliderGeometryPtr = colliderGeometry.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(sceneObjectsPtr);
                writer.WriteX(colliderGeometryPtr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // This pointer CANNOT be null and must refer to an object.
            Assert.IsTrue(sceneObjectsPtr.IsNotNullPointer);
            Assert.IsTrue(sceneObjects != null);
            // Assert that instance/pointer is correct
            Assert.ReferencePointer(sceneObjects, sceneObjectsPtr);
            Assert.ReferencePointer(colliderGeometry, colliderGeometryPtr);
        }

        public override string ToString()
        {
            return 
                $"{nameof(SceneObjectTemplate)}(" +
                $"{nameof(unk_0x00)}: {unk_0x00}, " +
                //$"{nameof(unk_0x04)}: {unk_0x04}, " +
                $"Has {nameof(ColliderGeometry)}: {colliderGeometryPtr.IsNotNullPointer}, " +
                $")";
        }

    }
}