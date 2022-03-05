using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    // Regarding LODs: LODs appear to be ordered using 2 criteria. First, they are sorted alphabetically by name.
    // Then, they are sorted by LOD distance ASCENDING. So an example sorting looks like the following. However,
    // if this is a hard fact, I do not know. The only thing that is consistent is using a null name after to denote
    // reusing the same model. In such a case, as noted above, the distance is ascending.
    //
    // SAMPLE 1 (ST01)
    // 30	PODHOUSE02_A_LOD
    // 10	PODHOUSE02_B_LOD
    // 5	PODHOUSE02_C_LOD
    // 10	[string.Empty]
    //
    // SAMPLE 2 (confusing sample from ST01, consider WTF)
    // 0	STADIUM_CEILING_LOD
    // 351	STADIUM_CEILING_A_LOD
    // 61	STADIUM_CEILING_B_LOD
    // 82	[string.Empty]

    /// <summary>
    /// An object to display in a scene. Refers to LODs.
    /// </summary>
    [Serializable]
    public class SceneObject :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // STRUCTURE
        public LodRenderFlags lodRenderFlags;
        public ArrayPointer lodsPtr;
        public Pointer colliderGeometryPtr;
        // FIELDS (deserialized from pointers)
        public SceneObjectLOD[] lods;
        public ColliderMesh colliderMesh;




        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public string Name => lods[0].name;
        public SceneObjectLOD PrimaryLOD => lods[0];
        public SceneObjectLOD[] LODs => lods;


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
                Assert.IsTrue(lodsPtr.IsNotNull);
                reader.JumpToAddress(lodsPtr);
                reader.ReadX(ref lods, lodsPtr.Length);

                // Collision is not required, load only if pointer is not null
                if (colliderGeometryPtr.IsNotNull)
                {
                    reader.JumpToAddress(colliderGeometryPtr);
                    reader.ReadX(ref colliderMesh);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                lodsPtr = lods.GetArrayPointer();
                colliderGeometryPtr = colliderMesh.GetPointer();
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
            Assert.IsTrue(lods != null);
            Assert.IsTrue(lodsPtr.IsNotNull);
            // Assert that instance/pointer is correct
            Assert.ReferencePointer(lods, lodsPtr);
            Assert.ReferencePointer(colliderMesh, colliderGeometryPtr);
        }

        public override string ToString()
        {
            return 
                $"{nameof(SceneObject)}(" +
                $"{nameof(lodRenderFlags)}: {lodRenderFlags}, " +
                $"LOD Count: {lodsPtr.Length}, " +
                $"Has {nameof(ColliderMesh)}: {colliderGeometryPtr.IsNotNull}" +
                $")";
        }
    }
}