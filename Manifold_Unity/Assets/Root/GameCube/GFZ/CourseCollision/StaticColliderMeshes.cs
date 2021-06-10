using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class StaticColliderMeshes :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // CONSTANTS
        /// <summary>
        /// 0x24 = 0x4 * 9
        /// </summary>
        public const int kCountZeros = 9;
        public const int kCountAxSurfaceTypes = 11;
        public const int kCountGxSurfaceTypes = 14;

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public int[] zero_0x00_0x20;
        public Pointer collisionTrisPtr;
        public Pointer[] collisionTriIndexesPtr;
        public BoundsXZ meshBounds;
        public Pointer collisionQuadsPtr;
        public Pointer[] collisionQuadIndexesPtr;
        public BoundsXZ ununsedMeshBounds;
        // REFERENCE FIELDS
        // This data holds the geometry data and indexes
        public ColliderTriangle[] colliderTriangles;
        public ColliderQuad[] colliderQuads;
        public StaticColliderMeshMatrix16x16[] triMeshIndexMatrices;
        public StaticColliderMeshMatrix16x16[] quadMeshIndexMatrices;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public static int GetSurfacesCount(ColiScene scene)
        {
            Assert.IsTrue(scene.IsValidFile);
            return scene.IsFileAX
                ? kCountAxSurfaceTypes
                : kCountGxSurfaceTypes;
        }

        public void Deserialize(BinaryReader reader)
        {
            // AX/GX have different amounts of pointers to collision mesh data
            var isFileGX = ColiCourseUtility.IsFileGX(reader);
            var isFileAX = ColiCourseUtility.IsFileAX(reader);
            // Ensure file is valid. XOR file flags.
            Assert.IsTrue(isFileAX ^ isFileGX);

            var countSurfaceTypes = isFileGX
                ? kCountGxSurfaceTypes
                : kCountAxSurfaceTypes;

            // Deserialize values
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zero_0x00_0x20, kCountZeros);
                reader.ReadX(ref collisionTrisPtr);
                reader.ReadX(ref collisionTriIndexesPtr, countSurfaceTypes, true);
                reader.ReadX(ref meshBounds, true);
                reader.ReadX(ref collisionQuadsPtr);
                reader.ReadX(ref collisionQuadIndexesPtr, countSurfaceTypes, true);
                reader.ReadX(ref ununsedMeshBounds, true);
            }
            this.RecordEndAddress(reader);
            {
                // Asserts
                for (int i = 0; i < zero_0x00_0x20.Length; i++)
                    Assert.IsTrue(zero_0x00_0x20[i] == 0);

                /////////////////
                // Initialize arrays
                triMeshIndexMatrices = new StaticColliderMeshMatrix16x16[countSurfaceTypes];
                quadMeshIndexMatrices = new StaticColliderMeshMatrix16x16[countSurfaceTypes];

                // Read mesh data
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    // Triangles
                    var triIndexesPointer = collisionTriIndexesPtr[i];
                    triMeshIndexMatrices[i] = new StaticColliderMeshMatrix16x16();
                    //DebugConsole.Log($"tri{i+1}:{triPointer.HexAddress}");
                    reader.JumpToAddress(triIndexesPointer);
                    reader.ReadX(ref triMeshIndexMatrices[i], false);

                    // Quads
                    var quadPointer = collisionQuadIndexesPtr[i];
                    quadMeshIndexMatrices[i] = new StaticColliderMeshMatrix16x16();
                    //DebugConsole.Log($"quad{i+1}:{quadPointer.HexAddress}");
                    reader.JumpToAddress(quadPointer);
                    reader.ReadX(ref quadMeshIndexMatrices[i], false);
                }

                //
                int numTriVerts = 0;
                int numQuadVerts = 0;
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    numTriVerts = math.max(triMeshIndexMatrices[i].IndexesLength, numTriVerts);
                    numQuadVerts = math.max(quadMeshIndexMatrices[i].IndexesLength, numQuadVerts);
                }

                reader.JumpToAddress(collisionTrisPtr);
                reader.ReadX(ref colliderTriangles, numTriVerts, true);

                reader.JumpToAddress(collisionQuadsPtr);
                reader.ReadX(ref colliderQuads, numQuadVerts, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        //public void Serialize(BinaryWriter writer)
        //{
        //    // Store current address since we will have to come back and overwrite it
        //    var addressRange = writer.GetPositionAsPointer();

        //    // Serialize structure with null/garbage pointers 
        //    SerializeStructure(writer);
        //    // Serialize references
        //    SerializeReferences(writer);
        //    // Got back to structure, rewrite with real pointers
        //    writer.JumpToAddress(addressRange);
        //    SerializeStructure(writer);

        //    // Go back to end of stream
        //    writer.SeekEnd();
        //}

        public void Serialize(BinaryWriter writer)
        {
            //// Save pointer to this structure for debug comments in sub-structures
            //ColiCourseUtility.Pointer = writer.GetPositionAsPointer();

            {
                // POINTERS
                // We don't need to store the length (from ArrayPointers).
                // The game kinda just figures it out on pointer alone.
                collisionTrisPtr = colliderTriangles.GetBasePointer();
                collisionTriIndexesPtr = triMeshIndexMatrices.GetPointers();
                collisionQuadsPtr = colliderQuads.GetBasePointer();
                collisionQuadIndexesPtr = quadMeshIndexMatrices.GetPointers();
            }
            this.RecordStartAddress(writer);
            {
                // Write empty int array for unknown
                writer.WriteX(new int[kCountZeros], false);
                writer.WriteX(collisionTrisPtr);
                writer.WriteX(collisionTriIndexesPtr, false);
                writer.WriteX(meshBounds);
                writer.WriteX(collisionQuadsPtr);
                writer.WriteX(collisionQuadIndexesPtr, false);
                writer.WriteX(ununsedMeshBounds);
            }
            this.RecordEndAddress(writer);
        }

        //private void SerializeReferences(BinaryWriter writer)
        //{
        //    // TRIANGLES
        //    writer.InlineDesc(ColiCourseUtility.SerializeVerbose, ColiCourseUtility.Pointer, colliderTriangles);
        //    writer.WriteX(colliderTriangles, false);
        //    writer.InlineDesc(ColiCourseUtility.SerializeVerbose, ColiCourseUtility.Pointer, triMeshIndexTable);
        //    writer.Comment($"var: {nameof(triMeshIndexTable)}", ColiCourseUtility.SerializeVerbose);
        //    writer.WriteX(triMeshIndexTable, false);
        //    // Using a static value to embed metadata in the above type upon serialization
        //    ColiCourseUtility.ResetDebugIndex();

        //    // QUADS
        //    writer.InlineDesc(ColiCourseUtility.SerializeVerbose, ColiCourseUtility.Pointer, colliderQuads);
        //    writer.WriteX(colliderQuads, false);
        //    writer.InlineDesc(ColiCourseUtility.SerializeVerbose, ColiCourseUtility.Pointer, quadMeshIndexTable);
        //    writer.Comment($"var: {nameof(quadMeshIndexTable)}", ColiCourseUtility.SerializeVerbose);
        //    writer.WriteX(quadMeshIndexTable, false);
        //    // Using a static value to embed metadata in the above type upon serialization
        //    ColiCourseUtility.ResetDebugIndex();

        //    // POINTERS
        //    // We don't need to store the length (from ArrayPointers).
        //    // The game kinda just figures it out on pointer alone.
        //    collisionTrisPtr = colliderTriangles.GetArrayPointer().Pointer;
        //    collisionTriIndexesPtr = triMeshIndexTable.GetPointers();
        //    collisionQuadsPtr = colliderQuads.GetArrayPointer().Pointer;
        //    collisionQuadIndexesPtr = quadMeshIndexTable.GetPointers();
        //}

        public void ValidateReferences()
        {
            throw new NotImplementedException();
        }
    }
}
