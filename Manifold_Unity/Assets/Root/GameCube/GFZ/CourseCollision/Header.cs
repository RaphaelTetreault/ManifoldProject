using Manifold.IO;
using System;
using System.IO;
using System.Diagnostics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class Header : IBinarySerializable
    {
        public const int kSizeOfZero0x28 = 0x20;
        public const int kSizeOfZero0xD8 = 0x24;
        public const int kGxConst0x24 = 0xFC;
        public const int kAxConst0x24 = 0xF8;

        public float unk_0x00;
        public float unk_0x04;
        public ArrayPointer trackNodes;
        public ArrayPointer collisionEffectsAreas;
        public Bool32 boostPadEnable;
        public Pointer unkPtr_0x1C; // Old notes: collision mesh stuff, but sus.
        public Pointer unkPtr_0x20; // GX: 0xE8, AX: 0xE4
        public Pointer unkPtr_0x24; // GX: 0xFC, AX: 0xF8
        public byte[] zero_0x28; // 0x20 count
        public int gameObjectCount;
        public int unk_gameObjectCount1; // Not in AX
        public int unk_gameObjectCount2;
        public Pointer gameObjectPtr;
        public Bool32 unk_0x58;
        public ArrayPointer unk_0x5C; // SOLS only. Occurrences = GX:6, AX:9.
        public ArrayPointer collisionObjectsMesh;
        public ArrayPointer unkArrayPtr_0x6C; // old notes: 0x64 ref back (to name table?). Goes to offset/ptr
        public ArrayPointer unused_0x74_0x78;
        public CircuitType circuitType;
        public Pointer unkPtr_0x80; // Old notes: always 6 count, possibly spline stuff
        public Pointer unkPtr_0x84; // Old notes: LOD Objects... (new note: 0x84, 0x94, 0x9C all use same struct?)
        public ArrayPointer unused_0x88_0x8C;
        public Pointer trackInfo;
        public ArrayPointer unk_0x94; // Old notes: anim type 1
        public ArrayPointer unk_0x9C; // Old notes: anim type 2
        public ArrayPointer pathObjects;
        public ArrayPointer arcadeCheckpoint;
        public ArrayPointer storyModeSpecialObjects;
        public Pointer trackNodeTable;
        public UnknownStruct1 unk_0xC0;
        public byte[] zero_0xD8;


        public bool IsGX => unkPtr_0x24.address == kGxConst0x24;
        public bool IsAX => unkPtr_0x24.address == kAxConst0x24;


        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref trackNodes);
            reader.ReadX(ref collisionEffectsAreas);
            reader.ReadX(ref boostPadEnable);
            reader.ReadX(ref unkPtr_0x1C);
            reader.ReadX(ref unkPtr_0x20);
            reader.ReadX(ref unkPtr_0x24);
            reader.ReadX(ref zero_0x28, kSizeOfZero0x28);
            reader.ReadX(ref gameObjectCount);
            if (IsGX) reader.ReadX(ref unk_gameObjectCount1);
            reader.ReadX(ref unk_gameObjectCount2);
            reader.ReadX(ref gameObjectPtr);
            reader.ReadX(ref unk_0x58);
            reader.ReadX(ref unk_0x5C);
            reader.ReadX(ref collisionObjectsMesh);
            reader.ReadX(ref unkArrayPtr_0x6C);
            reader.ReadX(ref unused_0x74_0x78);
            reader.ReadX(ref circuitType);
            reader.ReadX(ref unkPtr_0x80);
            reader.ReadX(ref unkPtr_0x84);
            reader.ReadX(ref unused_0x88_0x8C);
            reader.ReadX(ref trackInfo);
            reader.ReadX(ref unk_0x94);
            reader.ReadX(ref unk_0x9C);
            reader.ReadX(ref pathObjects);
            reader.ReadX(ref arcadeCheckpoint);
            reader.ReadX(ref storyModeSpecialObjects);
            reader.ReadX(ref trackNodeTable);
            reader.ReadX(ref unk_0xC0, true);
            reader.ReadX(ref zero_0xD8, kSizeOfZero0xD8);

            // Assert assumptions
            Debug.Assert(unused_0x74_0x78.length == 0 && unused_0x74_0x78.address == 0);
            Debug.Assert(unused_0x88_0x8C.length == 0 && unused_0x88_0x8C.address == 0);
            foreach (var zero in zero_0x28)
                Debug.Assert(zero == 0);
            foreach (var zero in zero_0xD8)
                Debug.Assert(zero == 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
