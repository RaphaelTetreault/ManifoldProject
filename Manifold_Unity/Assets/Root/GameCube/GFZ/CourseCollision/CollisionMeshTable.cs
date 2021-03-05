using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class CollisionMeshTable : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        public const int kCountUnknown = 9;
        public const int kCountAxCollisionTypes = 11;
        public const int kCountGxCollisionTypes = 14;

        // Basically a big pile of pointers
        public int[] unk_0x00_0x20;
        [Hex(8)]
        public Pointer collisionTris;
        [Hex(8)]
        public Pointer[] collisionTriIndices;
        public ColiUnknownStruct1 unknownStruct_0x60;
        [Hex(8)]
        public Pointer collisionQuads;
        [Hex(8)]
        public Pointer[] collisionQuadIndices;
        public ColiUnknownStruct1 unknownStruct_0xB4;

        // This data holds the geometry data and indices
        public float[] tris;
        public float[] quads;
        public CollisionMeshIndices[] triMeshIndices;
        public CollisionMeshIndices[] quadMeshIndices;


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
            var countCollisionTypes = isFileGX
                ? kCountGxCollisionTypes
                : kCountAxCollisionTypes;

            // Deserialize values
            this.RecordStartAddress(reader);
            reader.ReadX(ref unk_0x00_0x20, kCountUnknown);
            reader.ReadX(ref collisionTris);
            reader.ReadX(ref collisionTriIndices, countCollisionTypes, true);
            reader.ReadX(ref unknownStruct_0x60, true);
            reader.ReadX(ref collisionQuads);
            reader.ReadX(ref collisionQuadIndices, countCollisionTypes, true);
            reader.ReadX(ref unknownStruct_0xB4, true);
            this.RecordEndAddress(reader);

            // Asserts
            foreach (var ptr in unk_0x00_0x20)
            {
                if (ptr > 0)
                {
                    Debug.LogError($"Assertion false. {ptr:x8}");
                }
            }

            /////////////////
            // Initial arrays
            tris = new float[0];
            quads = new float[0];
            triMeshIndices = new CollisionMeshIndices[countCollisionTypes];
            quadMeshIndices = new CollisionMeshIndices[countCollisionTypes];

            // Read mesh data
            for (int i = 0; i < countCollisionTypes; i++)
            {
                // Triangles
                var triPointer = collisionTriIndices[i];
                triMeshIndices[i] = new CollisionMeshIndices();
                //Debug.Log($"tri{i+1}:{triPointer.HexAddress}");
                reader.JumpToAddress(triPointer);
                reader.ReadX(ref triMeshIndices[i], false);

                // Quads
                var quadPointer = collisionQuadIndices[i];
                quadMeshIndices[i] = new CollisionMeshIndices();
                //Debug.Log($"quad{i+1}:{quadPointer.HexAddress}");
                reader.JumpToAddress(quadPointer);
                reader.ReadX(ref quadMeshIndices[i], false);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }


        #endregion

    }
}
