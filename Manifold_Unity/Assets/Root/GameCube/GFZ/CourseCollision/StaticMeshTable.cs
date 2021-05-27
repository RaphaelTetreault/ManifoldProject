using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class StaticMeshTable : IBinarySerializable, IBinaryAddressableRange
    {
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        /// <summary>
        /// 0x24 = 0x4 * 9
        /// </summary>
        public const int kCountUnknown = 9;
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
        public MeshIndexes[] triMeshIndexes;
        public MeshIndexes[] quadMeshIndexes;


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
                reader.ReadX(ref zero_0x00_0x20, kCountUnknown);
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
                triMeshIndexes = new MeshIndexes[countSurfaceTypes];
                quadMeshIndexes = new MeshIndexes[countSurfaceTypes];

                // Read mesh data
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    // Triangles
                    var triIndexesPointer = collisionTriIndexesPtr[i];
                    triMeshIndexes[i] = new MeshIndexes();
                    //DebugConsole.Log($"tri{i+1}:{triPointer.HexAddress}");
                    reader.JumpToAddress(triIndexesPointer);
                    reader.ReadX(ref triMeshIndexes[i], false);

                    // Quads
                    var quadPointer = collisionQuadIndexesPtr[i];
                    quadMeshIndexes[i] = new MeshIndexes();
                    //DebugConsole.Log($"quad{i+1}:{quadPointer.HexAddress}");
                    reader.JumpToAddress(quadPointer);
                    reader.ReadX(ref quadMeshIndexes[i], false);
                }

                //
                int numTriVerts = 0;
                int numQuadVerts = 0;
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    numTriVerts = math.max(triMeshIndexes[i].largestIndex, numTriVerts);
                    numQuadVerts = math.max(quadMeshIndexes[i].largestIndex, numQuadVerts);
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
            throw new System.NotImplementedException();
        }

    }
}
