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

namespace GameCube.GFZ.Stage
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
    public sealed class StaticColliderMeshManager :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // CONSTANTS
        public const int kCountAxSurfaceTypes = 11;
        public const int kCountGxSurfaceTypes = 14;
        public const int kZeroesGroup1 = 0x24; // 36 bytes
        public const int kZeroesGroup2 = 0x20; // 32 bytes
        public const int kZeroesGroup3 = 0x10; // 16 bytes
        public const int kZeroesGroup4 = 0x14; // 20 bytes
        public const int kZeroesGroup5 = 0x3A0; // 926 bytes


        // FIELDS
        private byte[] zeroes_group1;
        private Pointer staticColliderTrisPtr;
        private Pointer[] triMeshGridPtrs; // variable AX/GX
        private GridXZ meshGridXZ;
        private Pointer staticColliderQuadsPtr;
        private Pointer[] quadMeshGridPtrs; // variable AX/GX
        private byte[] zeroes_group2;
        private ArrayPointer unknownCollidersPtr;
        private ArrayPointer staticSceneObjectsPtr;
        private byte[] zeroes_group3;
        private Pointer boundingSpherePtr;
        private byte[] zeroes_group4;
        private float unk_float;
        private byte[] zeroes_group5;
        // REFERENCE FIELDS
        private ColliderTriangle[] colliderTris = new ColliderTriangle[0];
        private ColliderQuad[] colliderQuads = new ColliderQuad[0];
        private StaticColliderMeshGrid[] triMeshGrids;
        private StaticColliderMeshGrid[] quadMeshGrids;
        private BoundingSphere boundingSphere = new BoundingSphere();
        private UnknownCollider[] unknownColliders;
        private SceneObjectStatic[] staticSceneObjects; // Some of these used to be name-parsed colliders! (eg: *_CLASS2, etc)


        // CONSTRUCTORS
        public StaticColliderMeshManager(SerializeFormat serializeFormat)
        {
            SerializeFormat = serializeFormat;
            int count = SurfaceCount;
            TriMeshGrids = new StaticColliderMeshGrid[count];
            QuadMeshGrids = new StaticColliderMeshGrid[count];

            // initialize arrays
            for (int i = 0; i < count; i++)
            {
                TriMeshGrids[i] = new StaticColliderMeshGrid();
                QuadMeshGrids[i] = new StaticColliderMeshGrid();
            }
        }


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public BoundingSphere BoundingSphere { get => boundingSphere; set => boundingSphere = value; }
        public SerializeFormat SerializeFormat { get; set; } = SerializeFormat.InvalidFormat;
        public int SurfaceCount
        {
            get
            {
                switch (SerializeFormat)
                {
                    case SerializeFormat.AX:
                        return kCountAxSurfaceTypes;

                    case SerializeFormat.GX:
                        return kCountGxSurfaceTypes;

                    case SerializeFormat.InvalidFormat:
                        throw new ArgumentException("Invalid serialization format!");

                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public Pointer StaticColliderTrisPtr { get => staticColliderTrisPtr; set => staticColliderTrisPtr = value; }
        public Pointer[] TriMeshGridPtrs { get => triMeshGridPtrs; set => triMeshGridPtrs = value; }
        public GridXZ MeshGridXZ { get => meshGridXZ; set => meshGridXZ = value; }
        public Pointer StaticColliderQuadsPtr { get => staticColliderQuadsPtr; set => staticColliderQuadsPtr = value; }
        public Pointer[] QuadMeshGridPtrs { get => quadMeshGridPtrs; set => quadMeshGridPtrs = value; }
        public ArrayPointer UnknownCollidersPtr { get => unknownCollidersPtr; set => unknownCollidersPtr = value; }
        public ArrayPointer StaticSceneObjectsPtr { get => staticSceneObjectsPtr; set => staticSceneObjectsPtr = value; }
        public Pointer BoundingSpherePtr { get => boundingSpherePtr; set => boundingSpherePtr = value; }
        public float Unk_float { get => unk_float; set => unk_float = value; }
        public ColliderTriangle[] ColliderTris { get => colliderTris; set => colliderTris = value; }
        public ColliderQuad[] ColliderQuads { get => colliderQuads; set => colliderQuads = value; }
        public StaticColliderMeshGrid[] TriMeshGrids { get => triMeshGrids; set => triMeshGrids = value; }
        public StaticColliderMeshGrid[] QuadMeshGrids { get => quadMeshGrids; set => quadMeshGrids = value; }
        public UnknownCollider[] UnknownColliders { get => unknownColliders; set => unknownColliders = value; }
        public SceneObjectStatic[] StaticSceneObjects { get => staticSceneObjects; set => staticSceneObjects = value; }


        // METHODS
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
                // ASSERTIONS
                {
                    // Not a needed struct, but existing data is never null.
                    Assert.IsTrue(BoundingSpherePtr.IsNotNull, $"{BoundingSpherePtr}");

                    // Assert that all of this other bytes are empty
                    for (int i = 0; i < kZeroesGroup1; i++)
                        Assert.IsTrue(zeroes_group1[i] == 0, $"{nameof(zeroes_group1)} {i} is {zeroes_group1[i]:x8}");
                    for (int i = 0; i < kZeroesGroup2; i++)
                        Assert.IsTrue(zeroes_group2[i] == 0, $"{nameof(zeroes_group2)} {i} is {zeroes_group2[i]:x8}");
                    for (int i = 0; i < kZeroesGroup3; i++)
                        Assert.IsTrue(zeroes_group3[i] == 0, $"{nameof(zeroes_group3)} {i} is {zeroes_group3[i]:x8}");
                    for (int i = 0; i < kZeroesGroup4; i++)
                        Assert.IsTrue(zeroes_group4[i] == 0, $"{nameof(zeroes_group4)} {i} is {zeroes_group4[i]:x8}");
                    for (int i = 0; i < kZeroesGroup5; i++)
                        Assert.IsTrue(zeroes_group5[i] == 0, $"{nameof(zeroes_group5)} {i} is {zeroes_group5[i]:x8}");
                }

                // Read mesh data made of tris and quads
                triMeshGrids = new StaticColliderMeshGrid[countSurfaceTypes];
                quadMeshGrids = new StaticColliderMeshGrid[countSurfaceTypes];
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    // Triangles
                    var triIndexesPointer = triMeshGridPtrs[i];
                    reader.JumpToAddress(triIndexesPointer);
                    reader.ReadX(ref triMeshGrids[i]);

                    // Quads
                    var quadPointer = quadMeshGridPtrs[i];
                    reader.JumpToAddress(quadPointer);
                    reader.ReadX(ref quadMeshGrids[i]);
                }

                // Find out how many tris and quads this structure has. There is no direct reference available,
                // so it is infered based on the max vertex index for tris and quads, respectively.
                int numTriVerts = 0;
                int numQuadVerts = 0;
                for (int i = 0; i < countSurfaceTypes; i++)
                {
                    numTriVerts = math.max(triMeshGrids[i].LargestIndex, numTriVerts);
                    numQuadVerts = math.max(quadMeshGrids[i].LargestIndex, numQuadVerts);
                }

                reader.JumpToAddress(staticColliderTrisPtr);
                reader.ReadX(ref colliderTris, numTriVerts);

                reader.JumpToAddress(staticColliderQuadsPtr);
                reader.ReadX(ref colliderQuads, numQuadVerts);

                reader.JumpToAddress(boundingSpherePtr);
                reader.ReadX(ref boundingSphere);

                // I don't read the StaticSceneObjects and UnknownColliders since it's easier to assign the
                // reference in the main Scene class directly and saves some deserialization time.
            }
            this.SetReaderToEndAddress(reader);
        }
        public void Serialize(BinaryWriter writer)
        {
            {
                // POINTERS
                // For tris/quads, we don't need to store the length (ie: ArrayPointers).
                // The game kinda just figures it out on the base pointer alone.
                staticColliderTrisPtr = colliderTris.GetBasePointer();
                staticColliderQuadsPtr = colliderQuads.GetBasePointer();
                triMeshGridPtrs = triMeshGrids.GetPointers();
                quadMeshGridPtrs = quadMeshGrids.GetPointers();
                // For these values, get proper pointers
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
            if (ColliderTris.Length > 0)
            {
                Assert.ValidateReferencePointer(ColliderTris, StaticColliderTrisPtr);

                // Ensure that we have at least a list to point to tris
                int listCount = 0;
                foreach (var list in TriMeshGrids)
                    listCount += list.IndexesLength;
                Assert.IsTrue(listCount > 0);
            }
            // QUADS
            if (ColliderQuads != null && ColliderQuads.Length > 0)
            {
                Assert.ValidateReferencePointer(ColliderQuads, StaticColliderQuadsPtr);

                // Ensure that we have at least a list to point to quads
                int listCount = 0;
                foreach (var list in QuadMeshGrids)
                    listCount += list.IndexesLength;
                Assert.IsTrue(listCount > 0);
            }

            // Grids
            for (int i = 0; i < SurfaceCount; i++)
            {
                Assert.ReferencePointer(TriMeshGrids[i], TriMeshGridPtrs[i]);
                Assert.ReferencePointer(QuadMeshGrids[i], QuadMeshGridPtrs[i]);
            }
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(StaticColliderMeshManager));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(SerializeFormat)}: {SerializeFormat}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(SurfaceCount)}: {SurfaceCount}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_float)}: {unk_float}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(ColliderTris)}[{ColliderTris.Length}]");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(ColliderQuads)}[{ColliderQuads.Length}]");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(StaticSceneObjects)}[{StaticSceneObjects.Length}]");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(UnknownColliders)}[{UnknownColliders.Length}]");
            builder.AppendLineIndented(indent, indentLevel, meshGridXZ);
            builder.AppendLineIndented(indent, indentLevel, boundingSphere);

            int index = 0;
            foreach (var triMeshGrid in triMeshGrids)
            {
                builder.AppendLineIndented(indent, indentLevel, $"[{index++}] {nameof(TriMeshGrids)}");
                builder.AppendLineIndented(indent, indentLevel, triMeshGrid);
            }

            index = 0;
            foreach (var quadMeshGrid in quadMeshGrids)
            {
                builder.AppendLineIndented(indent, indentLevel, $"[{index++}] {nameof(quadMeshGrid)}");
                builder.AppendLineIndented(indent, indentLevel, quadMeshGrid);
            }
        }

        public string PrintSingleLine()
        {
            return nameof(StaticColliderMeshManager);
        }

        public override string ToString() => PrintSingleLine();

    }
}
