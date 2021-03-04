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
        public const int kGxHeaderSize = 0xFC; // rename: not header
        public const int kAxHeaderSize = 0xF8; // rename: not header

        #region MEMBERS

        public float unk_0x00;
        public float unk_0x04;
        public int trackNodeCount;
        public int trackNodeAbsPtr;
        public int collisionEffectsPlacementCount;
        public int collisionEffectsPlacementAbsPtr;
        public Bool32 boostPadEnable;
        public int unk_0x1C_AbsPtr; // Old notes: collision mesh stuff, but sus.
        public int unk_0x20_AbsPtr; // GX: 0xE8, AX: 0xE4
        public int unk_0x24_AbsPtr; // GX: 0xFC, AX: 0xF8
        public byte[] zero_0x28; // 0x20 count
        public int gameObjectCount;
        public int unk_gameObjectCount1; // Not in AX
        public int unk_gameObjectCount2;
        public int gameObjectAbsPtr;
        public Bool32 unk_0x58;
        public int unk_0x5C_Count; // SOLS only. Occurrences = GX:6, AX:9.
        public int unk_0x60_AbsPtr; // SOLS only
        public int collisionObjectsMeshCount;
        public int collisionObjectsMeshAbsPtr;
        public int unk_0x6C_Count; // old notes: 0x64 ref back (to name table?). Goes to offset/ptr
        public int unk_0x70_AbsPtr;
        public int zero_0x74; // unused count
        public int zero_0x78; // unused pointer
        public CircuitType circuitType;
        public int unk_0x80_AbsPtr; // Old notes: always 6 count, possibly spline stuff
        public int unk_0x84_AbsPtr; // Old notes: LOD Objects... (new note: 0x84, 0x94, 0x9C all use same struct?)
        public int zero_0x88; // unused count
        public int zero_0x8C; // unused pointer
        public int trackInfoAbsPtr;
        public int unk_0x94_Count; // Old notes: anim type 1
        public int unk_0x98_AbsPtr;
        public int unk_0x9C_Count; // Old notes: anim type 2
        public int unk_0xA0_AbsPtr;
        public int pathObjectsCount;
        public int pathObjectsAbsPtr;
        public int arcadeCheckpointCount;
        public int arcadeCheckpointAbsPtr;
        public int storyModeSpecialObjectsCount;
        public int storyModeSpecialObjectsAbsPtr;
        public int trackNodeTableAbsPtr;
        public float unk_0xC0; // from C0 to D4 could be structure?
        public float unk_0xC4; // 
        public float unk_0xC8; // 
        public float unk_0xCC; // 
        public int unk_0xD0; // 8
        public int unk_0xD4; // 8
        public byte[] zero_0xD8;

        #endregion

        #region PROPERTIES

        public bool IsGX => unk_0x24_AbsPtr == kGxHeaderSize;
        public bool IsAX => unk_0x24_AbsPtr == kAxHeaderSize;

        #endregion

        #region METHODS 

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref trackNodeCount);
            reader.ReadX(ref trackNodeAbsPtr);
            reader.ReadX(ref collisionEffectsPlacementCount);
            reader.ReadX(ref collisionEffectsPlacementAbsPtr);
            reader.ReadX(ref boostPadEnable);
            reader.ReadX(ref unk_0x1C_AbsPtr);
            reader.ReadX(ref unk_0x20_AbsPtr);
            reader.ReadX(ref unk_0x24_AbsPtr);
            reader.ReadX(ref zero_0x28, kSizeOfZero0x28);
            reader.ReadX(ref gameObjectCount);
            if (IsGX) reader.ReadX(ref unk_gameObjectCount1);
            reader.ReadX(ref unk_gameObjectCount2);
            reader.ReadX(ref gameObjectAbsPtr);
            reader.ReadX(ref unk_0x58);
            reader.ReadX(ref unk_0x5C_Count);
            reader.ReadX(ref unk_0x60_AbsPtr);
            reader.ReadX(ref collisionObjectsMeshCount);
            reader.ReadX(ref collisionObjectsMeshAbsPtr);
            reader.ReadX(ref unk_0x6C_Count);
            reader.ReadX(ref unk_0x70_AbsPtr);
            reader.ReadX(ref zero_0x74);
            reader.ReadX(ref zero_0x78);
            reader.ReadX(ref circuitType);
            reader.ReadX(ref unk_0x80_AbsPtr);
            reader.ReadX(ref unk_0x84_AbsPtr);
            reader.ReadX(ref zero_0x88);
            reader.ReadX(ref zero_0x8C);
            reader.ReadX(ref trackInfoAbsPtr);
            reader.ReadX(ref unk_0x94_Count);
            reader.ReadX(ref unk_0x98_AbsPtr);
            reader.ReadX(ref unk_0x9C_Count);
            reader.ReadX(ref unk_0xA0_AbsPtr);
            reader.ReadX(ref pathObjectsCount);
            reader.ReadX(ref pathObjectsAbsPtr);
            reader.ReadX(ref arcadeCheckpointCount);
            reader.ReadX(ref arcadeCheckpointAbsPtr);
            reader.ReadX(ref storyModeSpecialObjectsCount);
            reader.ReadX(ref storyModeSpecialObjectsAbsPtr);
            reader.ReadX(ref trackNodeTableAbsPtr);
            reader.ReadX(ref unk_0xC0);
            reader.ReadX(ref unk_0xC4);
            reader.ReadX(ref unk_0xC8);
            reader.ReadX(ref unk_0xCC);
            reader.ReadX(ref unk_0xD0);
            reader.ReadX(ref unk_0xD4);
            reader.ReadX(ref zero_0xD8, kSizeOfZero0xD8);

            // Assert assumptions
            Debug.Assert(unk_0x5C_Count == 0);
            Debug.Assert(zero_0x74 == 0);
            Debug.Assert(zero_0x78 == 0);
            Debug.Assert(zero_0x88 == 0);
            Debug.Assert(zero_0x8C == 0);

            foreach (var zero in zero_0x28)
                Debug.Assert(zero == 0);
            foreach (var zero in zero_0xD8)
                Debug.Assert(zero == 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
