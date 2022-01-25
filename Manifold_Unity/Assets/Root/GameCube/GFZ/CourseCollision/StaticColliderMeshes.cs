using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Highest-level structure which consolidates all collider of a scene.
    /// 
    /// Two tables store static triangles and quads proper. Many matrices index these triangles
    /// and quads (11 in AX, 14 in GX). Thus, a single tri/quad can technically have more
    /// than 1 property (road, heal, boost...).
    /// 
    /// It also points to some data which the ColiScene header points to. Notably, it points to 
    /// </summary>
    [Serializable]
    public class StaticColliderMeshes :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // CONSTANTS
        public const int kCountAxSurfaceTypes = 11;
        public const int kCountGxSurfaceTypes = 14;
        public const int kZeroesGroup1 = 0x24; // 36 bytes
        public const int kZeroesGroup2 = 0x20; // 32 bytes
        public const int kZeroesGroup3 = 0x10; // 16 bytes
        public const int kZeroesGroup4 = 0x14; // 20 bytes
        public const int kZeroesGroup5 = 0x3A0; // 926 bytes

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        [UnityEngine.SerializeField] private ColiScene.SerializeFormat serializeFormat;

        // FIELDS
        public byte[] zeroes_group1;
        public Pointer staticColliderTrisPtr;
        public Pointer[] triMeshMatrixPtrs; // variable AX/GX
        public MatrixBoundsXZ meshBounds;
        public Pointer staticColliderQuadsPtr;
        public Pointer[] quadMeshMatrixPtrs; // variable AX/GX
        public byte[] zeroes_group2;
        public ArrayPointer unknownCollidersPtr;
        public ArrayPointer staticSceneObjectsPtr;
        public byte[] zeroes_group3;
        public Pointer unkDataPtr;
        public byte[] zeroes_group4;
        public float unk_float;
        public byte[] zeroes_group5;
        // REFERENCE FIELDS
        public ColliderTriangle[] colliderTris = new ColliderTriangle[0];
        public ColliderQuad[] colliderQuads = new ColliderQuad[0];
        public StaticColliderMeshMatrix[] triMeshMatrices;
        public StaticColliderMeshMatrix[] quadMeshMatrices;
        public UnknownStaticColliderMapData unkData = new UnknownStaticColliderMapData();
        public UnknownCollider[] unknownColliders;
        public SceneObjectStatic[] staticSceneObjects; // Some of these used to be name-parsed colliders! (eg: *_CLASS2, etc)


        public StaticColliderMeshes()
        {
            serializeFormat = ColiScene.SerializeFormat.InvalidFormat;
        }

        public StaticColliderMeshes(ColiScene.SerializeFormat serializeFormat)
        {
            this.serializeFormat = serializeFormat;
            int count = SurfaceCount;
            triMeshMatrices = new StaticColliderMeshMatrix[count];
            quadMeshMatrices = new StaticColliderMeshMatrix[count];

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
                reader.ReadX(ref zeroes_group1, kZeroesGroup1);
                reader.ReadX(ref staticColliderTrisPtr);
                reader.ReadX(ref triMeshMatrixPtrs, countSurfaceTypes, true);
                reader.ReadX(ref meshBounds, true);
                reader.ReadX(ref staticColliderQuadsPtr);
                reader.ReadX(ref quadMeshMatrixPtrs, countSurfaceTypes, true);
                reader.ReadX(ref zeroes_group2, kZeroesGroup2);
                reader.ReadX(ref unknownCollidersPtr);
                reader.ReadX(ref staticSceneObjectsPtr);
                reader.ReadX(ref zeroes_group3, kZeroesGroup3);
                reader.ReadX(ref unkDataPtr);
                reader.ReadX(ref zeroes_group4, kZeroesGroup4);
                reader.ReadX(ref unk_float);
                reader.ReadX(ref zeroes_group5, kZeroesGroup5);
            }
            this.RecordEndAddress(reader);
            {
                // Asserts
                for (int i = 0; i < zeroes_group1.Length; i++)
                    Assert.IsTrue(zeroes_group1[i] == 0);

                /////////////////
                // Initialize arrays
                triMeshMatrices = new StaticColliderMeshMatrix[countSurfaceTypes];
                quadMeshMatrices = new StaticColliderMeshMatrix[countSurfaceTypes];

                // Read mesh data
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    // Triangles
                    var triIndexesPointer = triMeshMatrixPtrs[i];
                    triMeshMatrices[i] = new StaticColliderMeshMatrix();
                    //DebugConsole.Log($"tri{i+1}:{triPointer.HexAddress}");
                    reader.JumpToAddress(triIndexesPointer);
                    reader.ReadX(ref triMeshMatrices[i], false);

                    // Quads
                    var quadPointer = quadMeshMatrixPtrs[i];
                    quadMeshMatrices[i] = new StaticColliderMeshMatrix();
                    //DebugConsole.Log($"quad{i+1}:{quadPointer.HexAddress}");
                    reader.JumpToAddress(quadPointer);
                    reader.ReadX(ref quadMeshMatrices[i], false);
                }

                //
                int numTriVerts = 0;
                int numQuadVerts = 0;
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    numTriVerts = math.max(triMeshMatrices[i].IndexesLength, numTriVerts);
                    numQuadVerts = math.max(quadMeshMatrices[i].IndexesLength, numQuadVerts);
                }

                reader.JumpToAddress(staticColliderTrisPtr);
                reader.ReadX(ref colliderTris, numTriVerts, true);

                reader.JumpToAddress(staticColliderQuadsPtr);
                reader.ReadX(ref colliderQuads, numQuadVerts, true);

                // NEWER STUFF
                reader.JumpToAddress(unkDataPtr);
                reader.ReadX(ref unkData, true);
                // I don't read the SceneObjectTemplates and UnknownSolsTriggers
                // since it's easier to patch that in ColiScene directly and saves
                // some deserialization time

                // 2022-01-14: all sorts of asserts
                // All these are always populated in GX J
                // 2022-01-24: disabled for testin export
                //Assert.IsTrue(staticSceneObjectsPtr.Length != 0);
                //Assert.IsTrue(staticSceneObjectsPtr.IsNotNullPointer);
                Assert.IsTrue(unkDataPtr.IsNotNullPointer);
                //DebugConsole.Log($"idx16: {unkBounds2DPtr.HexAddress}");

                // Assert that all of this other junk is empty
                for (int i = 0; i < kZeroesGroup2; i++)
                    Assert.IsTrue(zeroes_group2[i] == 0, $"Index A {00+i} is {zeroes_group2[i]:x8}");
                for (int i = 0; i < kZeroesGroup3; i++)
                    Assert.IsTrue(zeroes_group3[i] == 0, $"Index B {08+i} is {zeroes_group3[i]:x8}");
                for (int i = 0; i < kZeroesGroup4; i++)
                    Assert.IsTrue(zeroes_group4[i] == 0, $"Index C {16+i} is {zeroes_group4[i]:x8}");
                for (int i = 0; i < kZeroesGroup5; i++)
                    Assert.IsTrue(zeroes_group5[i] == 0, $"Index D {22+i} is {zeroes_group5[i]:x8}");

                //if (unknownSolsTriggersPtr.IsNotNullPointer)
                //    DebugConsole.Log($"idx 8/9: {unknownSolsTriggersPtr.Length}, {unknownSolsTriggersPtr.HexAddress}");
                //if (unk_float != 0)
                //    DebugConsole.Log($"idx 22: {unk_float}");

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
                staticColliderTrisPtr = colliderTris.GetBasePointer();
                staticColliderQuadsPtr = colliderQuads.GetBasePointer();
                triMeshMatrixPtrs = triMeshMatrices.GetPointers();
                quadMeshMatrixPtrs = quadMeshMatrices.GetPointers();
                //
                unkDataPtr = unkData.GetPointer();
                unknownCollidersPtr = unknownColliders.GetArrayPointer();
                staticSceneObjectsPtr = staticSceneObjects.GetArrayPointer();
            }
            this.RecordStartAddress(writer);
            {
                // Write empty int array for unknown
                writer.WriteX(zeroes_group1, false);
                writer.WriteX(staticColliderTrisPtr);
                writer.WriteX(triMeshMatrixPtrs, false);
                writer.WriteX(meshBounds);
                writer.WriteX(staticColliderQuadsPtr);
                writer.WriteX(quadMeshMatrixPtrs, false);
                writer.WriteX(zeroes_group2, false);
                writer.WriteX(unknownCollidersPtr);
                writer.WriteX(staticSceneObjectsPtr);
                writer.WriteX(zeroes_group3, false);
                writer.WriteX(unkDataPtr);
                writer.WriteX(zeroes_group4, false);
                writer.WriteX(unk_float);
                writer.WriteX(zeroes_group5, false);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // SANITY CHECK
            // If we have triangles or quads, make sure they found their way into
            // the index lists! Otherwise we have colliders but they are not referenced.
            // TRIS
            if (colliderTris.Length > 0)
            {
                Assert.ValidateReferencePointer(colliderTris, staticColliderTrisPtr);

                // Ensure that we have at least a list to point to tris
                int listCount = 0;
                foreach (var list in triMeshMatrices)
                    listCount += list.IndexesLength;
                Assert.IsTrue(listCount > 0);
            }
            // QUADS
            if (colliderQuads != null && colliderQuads.Length > 0)
            {
                Assert.ValidateReferencePointer(colliderQuads, staticColliderQuadsPtr);

                // Ensure that we have at least a list to point to quads
                int listCount = 0;
                foreach (var list in quadMeshMatrices)
                    listCount += list.IndexesLength;
                Assert.IsTrue(listCount > 0);
            }

            // Matrices
            for (int i = 0; i < SurfaceCount; i++)
            {
                Assert.ReferencePointer(triMeshMatrices[i], triMeshMatrixPtrs[i]);
                Assert.ReferencePointer(quadMeshMatrices[i], quadMeshMatrixPtrs[i]);
            }
        }

    }
}
