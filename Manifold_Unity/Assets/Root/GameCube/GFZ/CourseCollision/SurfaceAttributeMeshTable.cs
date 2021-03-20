using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class SurfaceAttributeMeshTable : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        public const int kCountUnknown = 9;
        public const int kCountAxSurfaceTypes = 11;
        public const int kCountGxSurfaceTypes = 14;

        // Basically a big pile of pointers
        public int[] unk_0x00_0x20;
        public Pointer collisionTrisPtr;
        public Pointer[] collisionTriIndexesPtr;
        public ColiUnknownStruct1 unknownStruct_0x60;
        public Pointer collisionQuadsPtr;
        public Pointer[] collisionQuadIndexesPtr;
        public ColiUnknownStruct1 unknownStruct_0xB4;

        // This data holds the geometry data and indexes
        public float[] tris;
        public float[] quads;
        public MeshIndexes[] triMeshIndexes;
        public MeshIndexes[] quadMeshIndexes;


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            // AX/GX have different amounts of pointers to collision mesh data
            var isFileGX = ColiCourseUtility.IsFileGX(reader);
            var countSurfaceTypes = isFileGX
                ? kCountGxSurfaceTypes
                : kCountAxSurfaceTypes;

            // Deserialize values
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00_0x20, kCountUnknown);
                reader.ReadX(ref collisionTrisPtr);
                reader.ReadX(ref collisionTriIndexesPtr, countSurfaceTypes, true);
                reader.ReadX(ref unknownStruct_0x60, true);
                reader.ReadX(ref collisionQuadsPtr);
                reader.ReadX(ref collisionQuadIndexesPtr, countSurfaceTypes, true);
                reader.ReadX(ref unknownStruct_0xB4, true);
            }
            this.RecordEndAddress(reader);

            // Asserts
            for (int i = 0; i < unk_0x00_0x20.Length; i++)
                Assert.IsTrue(unk_0x00_0x20[i] == 0);

            /////////////////
            // Initialize arrays
            tris = new float[0];
            quads = new float[0];
            triMeshIndexes = new MeshIndexes[countSurfaceTypes];
            quadMeshIndexes = new MeshIndexes[countSurfaceTypes];

            // Read mesh data
            for (int i = 0; i < countSurfaceTypes; i++)
            {
                // Triangles
                var triPointer = collisionTriIndexesPtr[i];
                triMeshIndexes[i] = new MeshIndexes();
                //Debug.Log($"tri{i+1}:{triPointer.HexAddress}");
                reader.JumpToAddress(triPointer);
                reader.ReadX(ref triMeshIndexes[i], false);

                // Quads
                var quadPointer = collisionQuadIndexesPtr[i];
                quadMeshIndexes[i] = new MeshIndexes();
                //Debug.Log($"quad{i+1}:{quadPointer.HexAddress}");
                reader.JumpToAddress(quadPointer);
                reader.ReadX(ref quadMeshIndexes[i], false);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }


        #endregion

    }
}
