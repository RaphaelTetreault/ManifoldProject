using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class StaticMeshTable :
        IBinarySeralizableReference
    {
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        /// <summary>
        /// 0x24 = 0x4 * 9
        /// </summary>
        public const int kCountZeros = 9;
        public const int kCountAxSurfaceTypes = 11;
        public const int kCountGxSurfaceTypes = 14;

        // Basically a big pile of pointers
        public int[] zero_0x00_0x20;
        public Pointer collisionTrisPtr;
        public Pointer[] collisionTriIndexesPtr;
        public ColiUnknownStruct1 unknownStruct_0x60;
        public Pointer collisionQuadsPtr;
        public Pointer[] collisionQuadIndexesPtr;
        public ColiUnknownStruct1 unknownStruct_0xB4;

        // This data holds the geometry data and indexes
        public ColliderTriangle[] colliderTriangles;
        public ColliderQuad[] colliderQuads;
        public MeshIndexTable[] triMeshIndexTable;
        public MeshIndexTable[] quadMeshIndexTable;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public static int SurfacesCount(ColiScene scene)
        {
            Assert.IsTrue(scene.header.IsValidFile);
            return scene.header.IsFileAX ? kCountAxSurfaceTypes : kCountGxSurfaceTypes;
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
                reader.ReadX(ref unknownStruct_0x60, true);
                reader.ReadX(ref collisionQuadsPtr);
                reader.ReadX(ref collisionQuadIndexesPtr, countSurfaceTypes, true);
                reader.ReadX(ref unknownStruct_0xB4, true);
            }
            this.RecordEndAddress(reader);
            {
                // Asserts
                for (int i = 0; i < zero_0x00_0x20.Length; i++)
                    Assert.IsTrue(zero_0x00_0x20[i] == 0);

                /////////////////
                // Initialize arrays
                triMeshIndexTable = new MeshIndexTable[countSurfaceTypes];
                quadMeshIndexTable = new MeshIndexTable[countSurfaceTypes];

                // Read mesh data
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    // Triangles
                    var triIndexesPointer = collisionTriIndexesPtr[i];
                    triMeshIndexTable[i] = new MeshIndexTable();
                    //DebugConsole.Log($"tri{i+1}:{triPointer.HexAddress}");
                    reader.JumpToAddress(triIndexesPointer);
                    reader.ReadX(ref triMeshIndexTable[i], false);

                    // Quads
                    var quadPointer = collisionQuadIndexesPtr[i];
                    quadMeshIndexTable[i] = new MeshIndexTable();
                    //DebugConsole.Log($"quad{i+1}:{quadPointer.HexAddress}");
                    reader.JumpToAddress(quadPointer);
                    reader.ReadX(ref quadMeshIndexTable[i], false);
                }

                //
                int numTriVerts = 0;
                int numQuadVerts = 0;
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    numTriVerts = math.max(triMeshIndexTable[i].largestIndex, numTriVerts);
                    numQuadVerts = math.max(quadMeshIndexTable[i].largestIndex, numQuadVerts);
                }

                reader.JumpToAddress(collisionTrisPtr);
                reader.ReadX(ref colliderTriangles, numTriVerts, true);

                reader.JumpToAddress(collisionQuadsPtr);
                reader.ReadX(ref colliderQuads, numQuadVerts, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            var addressRange = writer.GetPositionAsPointer();
            // Serialize structure with null/grabage pointers 
            SerializeStructure(writer);
            // Serialize references
            SerializeReferences(writer);
            // Got back to structure, rewrite with real pointers
            writer.JumpToAddress(addressRange);
            SerializeStructure(writer);
        }

        private void SerializeStructure(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                // Write empty int array for unknown
                writer.WriteX(new int[kCountZeros], false);
                writer.WriteX(collisionTrisPtr);
                writer.WriteX(collisionTriIndexesPtr, false);
                writer.WriteX(unknownStruct_0x60);
                writer.WriteX(collisionQuadsPtr);
                writer.WriteX(collisionQuadIndexesPtr, false);
                writer.WriteX(unknownStruct_0xB4);
            }
            this.RecordEndAddress(writer);
        }

        private void SerializeReferences(BinaryWriter writer)
        {
            // Write sub classes to get pointers
            // TRIANGLES
            writer.CommentTypeDesc(colliderTriangles, new Pointer(-1), ColiCourseUtility.SerializeVerbose);
            // We don't need to store the length. The game kinda just figures it out.
            collisionTrisPtr = colliderTriangles.SerializeWithReferences(writer).GetArrayPointer().Pointer;

            writer.CommentTypeDesc(triMeshIndexTable, new Pointer(-1), ColiCourseUtility.SerializeVerbose);
            collisionTriIndexesPtr = triMeshIndexTable.SerializeWithReferences(writer).GetPointers();
            ColiCourseUtility.ResetDebugIndex();

            // QUADS
            writer.CommentType<ColliderQuad>(ColiCourseUtility.SerializeVerbose);
            writer.Comment($"Count: {colliderQuads.Length}", ColiCourseUtility.SerializeVerbose);
            {
                collisionQuadsPtr = colliderQuads.SerializeWithReferences(writer).GetArrayPointer().Pointer;
            }
            writer.CommentNewLine(ColiCourseUtility.SerializeVerbose);
            writer.Comment("Quad Indexes", ColiCourseUtility.SerializeVerbose);
            {
                collisionTriIndexesPtr = quadMeshIndexTable.SerializeWithReferences(writer).GetPointers();
            }
            ColiCourseUtility.ResetDebugIndex();
        }

        // Rename SerializeAsReference?
        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            Serialize(writer);
            return addressRange;
        }

    }
}
