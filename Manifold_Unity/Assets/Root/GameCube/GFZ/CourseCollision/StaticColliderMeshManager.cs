using Manifold;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

//////////////////////////
// Analysis: 2022-01-14 //
//////////////////////////
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

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Highest-level structure which consolidates all static colliders of a scene.
    /// 
    /// Two tables store static triangles and quads proper. Many matrices index these triangles
    /// and quads (11 in AX, 14 in GX). Thus, a single tri/quad can technically have more
    /// than 1 property (road, heal, boost...).
    /// 
    /// It also points to some data which the ColiScene header points to. Notably, it points to 
    /// </summary>
    [Serializable]
    public class StaticColliderMeshManager :
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
        public Pointer[] triMeshGridPtrs; // variable AX/GX
        public GridXZ meshGridXZ;
        public Pointer staticColliderQuadsPtr;
        public Pointer[] quadMeshGridPtrs; // variable AX/GX
        public byte[] zeroes_group2;
        public ArrayPointer unknownCollidersPtr;
        public ArrayPointer staticSceneObjectsPtr;
        public byte[] zeroes_group3;
        public Pointer boundingSpherePtr;
        public byte[] zeroes_group4;
        public float unk_float;
        public byte[] zeroes_group5;
        // REFERENCE FIELDS
        public ColliderTriangle[] colliderTris = new ColliderTriangle[0];
        public ColliderQuad[] colliderQuads = new ColliderQuad[0];
        public StaticColliderMeshGrid[] triMeshGrids;
        public StaticColliderMeshGrid[] quadMeshGrids;
        public BoundingSphere boundingSphere = new BoundingSphere();
        public UnknownCollider[] unknownColliders;
        public SceneObjectStatic[] staticSceneObjects; // Some of these used to be name-parsed colliders! (eg: *_CLASS2, etc)


        public StaticColliderMeshManager(ColiScene.SerializeFormat serializeFormat)
        {
            this.serializeFormat = serializeFormat;
            int count = SurfaceCount;
            triMeshGrids = new StaticColliderMeshGrid[count];
            quadMeshGrids = new StaticColliderMeshGrid[count];

            // initialize arrays
            for (int i = 0; i < count; i++)
            {
                triMeshGrids[i] = new StaticColliderMeshGrid();
                quadMeshGrids[i] = new StaticColliderMeshGrid();
            }
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

        public void ComputeMeshGridXZ()
        {
            throw new NotImplementedException();
        }

        public void Deserialize(BinaryReader reader)
        {
            var countSurfaceTypes = SurfaceCount;

            // Deserialize values
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zeroes_group1, kZeroesGroup1);
                reader.ReadX(ref staticColliderTrisPtr);
                reader.ReadX(ref triMeshGridPtrs, countSurfaceTypes);
                reader.ReadX(ref meshGridXZ);
                reader.ReadX(ref staticColliderQuadsPtr);
                reader.ReadX(ref quadMeshGridPtrs, countSurfaceTypes);
                reader.ReadX(ref zeroes_group2, kZeroesGroup2);
                reader.ReadX(ref unknownCollidersPtr);
                reader.ReadX(ref staticSceneObjectsPtr);
                reader.ReadX(ref zeroes_group3, kZeroesGroup3);
                reader.ReadX(ref boundingSpherePtr);
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
                triMeshGrids = new StaticColliderMeshGrid[countSurfaceTypes];
                quadMeshGrids = new StaticColliderMeshGrid[countSurfaceTypes];

                // Read mesh data
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    // Triangles
                    var triIndexesPointer = triMeshGridPtrs[i];
                    reader.JumpToAddress(triIndexesPointer);
                    triMeshGrids[i] = new StaticColliderMeshGrid();
                    triMeshGrids[i].Deserialize(reader);

                    // Quads
                    var quadPointer = quadMeshGridPtrs[i];
                    reader.JumpToAddress(quadPointer);
                    quadMeshGrids[i] = new StaticColliderMeshGrid();
                    quadMeshGrids[i].Deserialize(reader);
                }

                //
                int numTriVerts = 0;
                int numQuadVerts = 0;
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    numTriVerts = math.max(triMeshGrids[i].IndexesLength, numTriVerts);
                    numQuadVerts = math.max(quadMeshGrids[i].IndexesLength, numQuadVerts);
                }

                reader.JumpToAddress(staticColliderTrisPtr);
                reader.ReadX(ref colliderTris, numTriVerts);

                reader.JumpToAddress(staticColliderQuadsPtr);
                reader.ReadX(ref colliderQuads, numQuadVerts);

                // NEWER STUFF
                reader.JumpToAddress(boundingSpherePtr);
                reader.ReadX(ref boundingSphere);
                // I don't read the SceneObjectTemplates and UnknownSolsTriggers
                // since it's easier to patch that in ColiScene directly and saves
                // some deserialization time

                // 2022-01-14: all sorts of asserts
                // All these are always populated in GX J
                // 2022-01-24: disabled for testin export
                //Assert.IsTrue(staticSceneObjectsPtr.Length != 0);
                //Assert.IsTrue(staticSceneObjectsPtr.IsNotNullPointer);
                Assert.IsTrue(boundingSpherePtr.IsNotNull);
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
                triMeshGridPtrs = triMeshGrids.GetPointers();
                quadMeshGridPtrs = quadMeshGrids.GetPointers();
                //
                boundingSpherePtr = boundingSphere.GetPointer();
                unknownCollidersPtr = unknownColliders.GetArrayPointer();
                staticSceneObjectsPtr = staticSceneObjects.GetArrayPointer();
            }
            this.RecordStartAddress(writer);
            {
                // Write empty int array for unknown
                writer.WriteX(new byte[kZeroesGroup1]);
                writer.WriteX(staticColliderTrisPtr);
                writer.WriteX(triMeshGridPtrs);
                writer.WriteX(meshGridXZ);
                writer.WriteX(staticColliderQuadsPtr);
                writer.WriteX(quadMeshGridPtrs);
                writer.WriteX(new byte[kZeroesGroup2]);
                writer.WriteX(unknownCollidersPtr);
                writer.WriteX(staticSceneObjectsPtr);
                writer.WriteX(new byte[kZeroesGroup3]);
                writer.WriteX(boundingSpherePtr);
                writer.WriteX(new byte[kZeroesGroup4]);
                writer.WriteX(unk_float);
                writer.WriteX(new byte[kZeroesGroup5]);
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
                foreach (var list in triMeshGrids)
                    listCount += list.IndexesLength;
                Assert.IsTrue(listCount > 0);
            }
            // QUADS
            if (colliderQuads != null && colliderQuads.Length > 0)
            {
                Assert.ValidateReferencePointer(colliderQuads, staticColliderQuadsPtr);

                // Ensure that we have at least a list to point to quads
                int listCount = 0;
                foreach (var list in quadMeshGrids)
                    listCount += list.IndexesLength;
                Assert.IsTrue(listCount > 0);
            }

            // Matrices
            for (int i = 0; i < SurfaceCount; i++)
            {
                Assert.ReferencePointer(triMeshGrids[i], triMeshGridPtrs[i]);
                Assert.ReferencePointer(quadMeshGrids[i], quadMeshGridPtrs[i]);
            }
        }

    }
}
