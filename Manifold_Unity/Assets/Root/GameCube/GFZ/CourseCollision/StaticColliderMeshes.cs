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
        [UnityEngine.SerializeField] private ColiScene.SerializeFormat serializeFormat;

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
        public ColliderTriangle[] colliderTriangles = new ColliderTriangle[0];
        public ColliderQuad[] colliderQuads = new ColliderQuad[0];
        public StaticColliderMeshMatrix[] triMeshIndexMatrices;
        public StaticColliderMeshMatrix[] quadMeshIndexMatrices;

        public StaticColliderMeshes()
        {
            serializeFormat = ColiScene.SerializeFormat.InvalidFormat;
        }

        public StaticColliderMeshes(ColiScene.SerializeFormat serializeFormat)
        {
            this.serializeFormat = serializeFormat;
            int count = SurfaceCount;
            triMeshIndexMatrices = new StaticColliderMeshMatrix[count];
            quadMeshIndexMatrices = new StaticColliderMeshMatrix[count];

            // initialize arrays
            //for (int i = 0; i < count; i++)
        }

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public ColiScene.SerializeFormat SerializeFormat
        {
            get => serializeFormat;
            set => serializeFormat = value;
        }

        public int SurfaceCount
        {
            get
            {
                switch (serializeFormat)
                {
                    case ColiScene.SerializeFormat.AX:
                        return kCountAxSurfaceTypes;

                    case ColiScene.SerializeFormat.GX:
                        return kCountGxSurfaceTypes;

                    case ColiScene.SerializeFormat.InvalidFormat:
                        throw new ArgumentException("Invalid serialization format!");

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            var countSurfaceTypes = SurfaceCount;

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
                triMeshIndexMatrices = new StaticColliderMeshMatrix[countSurfaceTypes];
                quadMeshIndexMatrices = new StaticColliderMeshMatrix[countSurfaceTypes];

                // Read mesh data
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    // Triangles
                    var triIndexesPointer = collisionTriIndexesPtr[i];
                    triMeshIndexMatrices[i] = new StaticColliderMeshMatrix();
                    //DebugConsole.Log($"tri{i+1}:{triPointer.HexAddress}");
                    reader.JumpToAddress(triIndexesPointer);
                    reader.ReadX(ref triMeshIndexMatrices[i], false);

                    // Quads
                    var quadPointer = collisionQuadIndexesPtr[i];
                    quadMeshIndexMatrices[i] = new StaticColliderMeshMatrix();
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
        public void Serialize(BinaryWriter writer)
        {
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

        public void ValidateReferences()
        {
            // if we have tris, ensure pointer exists
            if (colliderTriangles != null && colliderTriangles.Length > 0)
            {
                Assert.IsTrue(collisionTrisPtr.IsNotNullPointer);

                // Ensure that we have at least a list to point to quads
                int listCount = 0;
                foreach (var list in triMeshIndexMatrices)
                    listCount += list.IndexesLength;
                Assert.IsTrue(listCount > 0);
            }

            // if we have quads, ensure pointer exists
            if (colliderQuads != null && colliderQuads.Length > 0)
            {
                Assert.IsTrue(collisionQuadsPtr.IsNotNullPointer);
                
                // Ensure that we have at least a list to point to quads
                int listCount = 0;
                foreach (var list in quadMeshIndexMatrices)
                    listCount += list.IndexesLength;
                Assert.IsTrue(listCount > 0);
            }
            //


        }
    }
}
