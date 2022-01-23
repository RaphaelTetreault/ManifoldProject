using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// An object to display in a scene. Refers to LODs.
    /// </summary>
    [Serializable]
    public class SceneObject :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // STRUCTURE
        public LodRenderFlags lodRenderFlags;
        public ArrayPointer lodsPtr;
        public Pointer colliderGeometryPtr;
        // FIELDS (deserialized from pointers)
        public SceneObjectLOD[] lods;
        public ColliderGeometry colliderGeometry;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public string Name => lods[0].name;

        public SceneObjectLOD PrimarySceneObject => lods[0];

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref lodRenderFlags);
                reader.ReadX(ref lodsPtr);
                reader.ReadX(ref colliderGeometryPtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(lodsPtr.IsNotNullPointer);
                reader.JumpToAddress(lodsPtr);
                reader.ReadX(ref lods, lodsPtr.Length, true);

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
                lodsPtr = lods.GetArrayPointer();
                colliderGeometryPtr = colliderGeometry.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(lodRenderFlags);
                writer.WriteX(lodsPtr);
                writer.WriteX(colliderGeometryPtr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // This pointer CANNOT be null and must refer to an object.
            Assert.IsTrue(lodsPtr.IsNotNullPointer);
            Assert.IsTrue(lods != null);
            // Assert that instance/pointer is correct
            Assert.ReferencePointer(lods, lodsPtr);
            Assert.ReferencePointer(colliderGeometry, colliderGeometryPtr);
        }

        public override string ToString()
        {
            return 
                $"{nameof(SceneObject)}(" +
                $"{nameof(lodRenderFlags)}: {lodRenderFlags}, " +
                $"Has {nameof(ColliderGeometry)}: {colliderGeometryPtr.IsNotNullPointer}, " +
                $")";
        }

    }
}