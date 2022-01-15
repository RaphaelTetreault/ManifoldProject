using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Highest-level structure which consolidates all static collider meshes in a scene.
    /// Two tables store the triangles and quads proper. Many matrices index these triangles
    /// and quads (11 in AX, 14 in GX). Thus, a single tri/quad can technically have more
    /// than 1 property.
    /// </summary>
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
        public const int kZeroesA = 8; // 0-7
        //                          4; // 8, 9, 10, 11
        public const int kZeroesB = 4; // 12-15
        //                          1; // 16
        public const int kZeroesC = 5; // 17-21
        //                          1; // 22
        public const int kZeroesD = 232;// 23-254

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
        public int[] zeroes_a; // size: 8 ints
        public ArrayPointer unknownSolsTriggersPtr; // # 8, 9
        public ArrayPointer staticSceneObjectsPtr; // # 10, 11
        public int[] zeroes_b; // size: 4 ints
        public Pointer unkBounds2DPtr; // # 16
        public int[] zeroes_c; // size: 5 ints
        public float unk_float; // #22
        public int[] zeroes_d; // size: 233 ints
        // REFERENCE FIELDS
        // This data holds the geometry data and indexes
        public ColliderTriangle[] colliderTriangles = new ColliderTriangle[0];
        public ColliderQuad[] colliderQuads = new ColliderQuad[0];
        public StaticColliderMeshMatrix[] triMeshIndexMatrices;
        public StaticColliderMeshMatrix[] quadMeshIndexMatrices;
        public Bounds2D unkBounds2D = new Bounds2D();
        public UnknownSolsTrigger[] UnknownSolsTrigger;
        public SceneObjectStatic[] staticSceneObjects;


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
                reader.ReadX(ref zeroes_a, kZeroesA);
                reader.ReadX(ref unknownSolsTriggersPtr);
                reader.ReadX(ref staticSceneObjectsPtr);
                reader.ReadX(ref zeroes_b, kZeroesB);
                reader.ReadX(ref unkBounds2DPtr);
                reader.ReadX(ref zeroes_c, kZeroesC);
                reader.ReadX(ref unk_float);
                reader.ReadX(ref zeroes_d, kZeroesD);
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

                // NEWER STUFF
                reader.JumpToAddress(unkBounds2DPtr);
                reader.ReadX(ref unkBounds2D, true);
                // I don't read the SceneObjectTemplates and UnknownSolsTriggers
                // since it's easier to patch that in ColiScene directly and saves
                // some deserialization time

                // 2022-01-14: all sorts of asserts
                // All these are always populated in GX J
                Assert.IsTrue(staticSceneObjectsPtr.Length != 0);
                Assert.IsTrue(staticSceneObjectsPtr.IsNotNullPointer);
                Assert.IsTrue(unkBounds2DPtr.IsNotNullPointer);
                DebugConsole.Log($"idx16: {unkBounds2DPtr.HexAddress}");

                // Assert that all of this other junk is empty
                for (int i = 0; i < kZeroesA; i++)
                    Assert.IsTrue(zeroes_a[i] == 0, $"Index A {00+i} is {zeroes_a[i]:x8}");
                for (int i = 0; i < kZeroesB; i++)
                    Assert.IsTrue(zeroes_b[i] == 0, $"Index B {08+i} is {zeroes_b[i]:x8}");
                for (int i = 0; i < kZeroesC; i++)
                    Assert.IsTrue(zeroes_c[i] == 0, $"Index C {16+i} is {zeroes_c[i]:x8}");
                for (int i = 0; i < kZeroesD; i++)
                    Assert.IsTrue(zeroes_d[i] == 0, $"Index D {22+i} is {zeroes_d[i]:x8}");

                if (unknownSolsTriggersPtr.IsNotNullPointer)
                    DebugConsole.Log($"idx 8/9: {unknownSolsTriggersPtr.Length}, {unknownSolsTriggersPtr.HexAddress}");
                if (unk_float != 0)
                    DebugConsole.Log($"idx 22: {unk_float}");

                // Analysis: 2022-01-14

                // ALWAYS USED
                // idx: 10, 11, 16
                //
                // OPTIONAL
                // idx 22, 3 times
                // idx  8, 1 time
                // idx  9, 1 time
                //
                // ST16 CPSO  : 22 : 480f
                // ST25 SOLS  : 8,9: len:6, addr:00001730
                // ST29 CPDB  : 22 : 480f
                // ST41 Story5: 22 : 60f
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
                //
                unkBounds2DPtr = unkBounds2D.GetPointer();
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
                writer.WriteX(zeroes_a, false);
                writer.WriteX(unknownSolsTriggersPtr);
                writer.WriteX(staticSceneObjectsPtr);
                writer.WriteX(zeroes_b, false);
                writer.WriteX(unkBounds2DPtr);
                writer.WriteX(zeroes_c, false);
                writer.WriteX(unk_float);
                writer.WriteX(zeroes_d, false);
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
