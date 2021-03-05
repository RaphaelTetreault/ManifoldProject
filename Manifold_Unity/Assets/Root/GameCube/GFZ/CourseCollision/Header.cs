using Manifold.IO;
using System;
using System.IO;
using System.Diagnostics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class Header : IBinarySerializable
    {
        // consts
        public const int kSizeOfZero0x28 = 0x20;
        public const int kSizeOfZero0xD8 = 0x10;

        // metadata
        private bool isFileAX;
        private bool isFileGX;

        // structure
        public float unk_0x00;
        public float unk_0x04;
        public ArrayPointer trackNodesPtr;
        public ArrayPointer collisionEffectsAreasPtr;
        public Bool32 boostPadEnable;
        public Pointer collisionMeshTablePtr;
        public Pointer unkPtr_0x20; // GX: 0xE8, AX: 0xE4
        public Pointer unkPtr_0x24; // GX: 0xFC, AX: 0xF8
        public byte[] zero_0x28; // 0x20 count
        public int gameObjectCount;
        public int unk_gameObjectCount1; // GX exclusive
        public int unk_gameObjectCount2;
        public Pointer gameObjectPtr;
        public Bool32 unkBool32_0x58;
        public ArrayPointer unkArrayPtr_0x5C; // SOLS only. Occurrences = GX:6, AX:9.
        public ArrayPointer collisionObjectReferences;
        public ArrayPointer unk_collisionObjectReferences; // old notes: 0x64 ref back (to name table?). Goes to offset/ptr
        public ArrayPointer unused_0x74_0x78;
        public CircuitType circuitType;
        public Pointer unkPtr_0x80; // Old notes: always 6 count, possibly spline stuff
        public Pointer unkPtr_0x84; // Old notes: LOD Objects... (new note: 0x84, 0x94, 0x9C all use same struct?)
        public ArrayPointer unused_0x88_0x8C;
        public Pointer trackInfo;
        public ArrayPointer unkArrayPtr_0x94; // Old notes: anim type 1
        public ArrayPointer unkArrayPtr_0x9C; // Old notes: anim type 2
        public ArrayPointer pathObjects;
        public ArrayPointer arcadeCheckpoint;
        public ArrayPointer storyModeSpecialObjects;
        public Pointer trackNodeTable;
        public ColiUnknownStruct1 unknownStructure1_0xC0;
        public byte[] zero_0xD8; // 0x10 count


        public bool IsFileAX => isFileAX;
        public bool IsFileGX => isFileGX;


        public void Deserialize(BinaryReader reader)
        {
            // Record some metadata
            isFileAX = ColiCourseUtility.IsFileAX(reader);
            isFileGX = ColiCourseUtility.IsFileGX(reader);

            // Deserialize main structure
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref trackNodesPtr);
            reader.ReadX(ref collisionEffectsAreasPtr);
            reader.ReadX(ref boostPadEnable);
            reader.ReadX(ref collisionMeshTablePtr);
            reader.ReadX(ref unkPtr_0x20);
            reader.ReadX(ref unkPtr_0x24);
            reader.ReadX(ref zero_0x28, kSizeOfZero0x28);
            reader.ReadX(ref gameObjectCount);
            if (isFileGX) reader.ReadX(ref unk_gameObjectCount1);
            reader.ReadX(ref unk_gameObjectCount2);
            reader.ReadX(ref gameObjectPtr);
            reader.ReadX(ref unkBool32_0x58);
            reader.ReadX(ref unkArrayPtr_0x5C);
            reader.ReadX(ref collisionObjectReferences);
            reader.ReadX(ref unk_collisionObjectReferences);
            reader.ReadX(ref unused_0x74_0x78);
            reader.ReadX(ref circuitType);
            reader.ReadX(ref unkPtr_0x80);
            reader.ReadX(ref unkPtr_0x84);
            reader.ReadX(ref unused_0x88_0x8C);
            reader.ReadX(ref trackInfo);
            reader.ReadX(ref unkArrayPtr_0x94);
            reader.ReadX(ref unkArrayPtr_0x9C);
            reader.ReadX(ref pathObjects);
            reader.ReadX(ref arcadeCheckpoint);
            reader.ReadX(ref storyModeSpecialObjects);
            reader.ReadX(ref trackNodeTable);
            reader.ReadX(ref unknownStructure1_0xC0, true);
            reader.ReadX(ref zero_0xD8, kSizeOfZero0xD8);

            // Assert assumptions
            Debug.Assert(unused_0x74_0x78.length == 0 && unused_0x74_0x78.address == 0);
            Debug.Assert(unused_0x88_0x8C.length == 0 && unused_0x88_0x8C.address == 0);

            for (int i = 0; i < zero_0x28.Length; i++)
            {
                if (zero_0x28[i] != 0)
                {
                    UnityEngine.Debug.Log($"{nameof(zero_0x28)}:{i} not always 0!");
                }
            }
            for (int i = 0; i < zero_0xD8.Length; i++)
            {
                if (zero_0xD8[i] != 0)
                {
                    UnityEngine.Debug.Log($"{nameof(zero_0xD8)}:{i} not always 0!");
                }
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
