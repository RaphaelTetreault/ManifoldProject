using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class CollisionMeshTable : IBinarySerializable, IBinaryAddressable
    {
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public const int kCountUnknown = 9;
        public const int kCountCollisionTypes = 14;

        // Basically a big pile of pointers
        public int[] unk_0x00_0x20;
        [Hex(8)]
        public int collisionTrisAbsPtr;
        [Hex(8)]
        public int[] collisionTriIndicesAbsPtrs;
        public UnknownStruct1 unknownStruct_0x60;
        [Hex(8)]
        public int collisionQuadsAbsPtrs;
        [Hex(8)]
        public int[] collisionQuadIndicesAbsPtrs;
        public UnknownStruct1 unknownStruct_0xB4;

        // This data holds the geometry data and indices
        public float[] tris = new float[0];
        public float[] quads = new float[0];
        public CollisionMeshIndices[] triMeshIndices = new CollisionMeshIndices[kCountCollisionTypes];
        public CollisionMeshIndices[] quadMeshIndices = new CollisionMeshIndices[kCountCollisionTypes];


        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);

            reader.ReadX(ref unk_0x00_0x20, kCountUnknown);
            reader.ReadX(ref collisionTrisAbsPtr);
            reader.ReadX(ref collisionTriIndicesAbsPtrs, kCountCollisionTypes);
            reader.ReadX(ref unknownStruct_0x60, true);
            reader.ReadX(ref collisionQuadsAbsPtrs);
            reader.ReadX(ref collisionQuadIndicesAbsPtrs, kCountCollisionTypes);
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

            // Read mesh data
            for (int i = 0; i < kCountCollisionTypes; i++)
            {
                // Triangles
                var triPointer = collisionTriIndicesAbsPtrs[i];
                triMeshIndices[i] = new CollisionMeshIndices();
                reader.BaseStream.Seek(triPointer, SeekOrigin.Begin);
                reader.ReadX(ref triMeshIndices[i], false);

                // Quads
                var quadPointer = collisionQuadIndicesAbsPtrs[i];
                quadMeshIndices[i] = new CollisionMeshIndices();
                reader.BaseStream.Seek(quadPointer, SeekOrigin.Begin);
                reader.ReadX(ref quadMeshIndices[i], false);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }
}
