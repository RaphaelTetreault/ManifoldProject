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
        public UnknownFloatPair unk_0x00;
        public ArrayPointer trackNodesPtr;
        public ArrayPointer surfaceAttributeAreasPtr;
        public BoostPadsActive boostPadsActive;
        public Pointer surfaceAttributeMeshTablePtr;
        public Pointer unknownData_0x20_Ptr; // GX: 0xE8, AX: 0xE4
        public Pointer unknownFloat_0x24_Ptr; // GX: 0xFC, AX: 0xF8
        public byte[] zero_0x28; // 0x20 count
        public int sceneObjectCount;
        public int unk_sceneObjectCount1; // GX exclusive
        public int unk_sceneObjectCount2;
        public Pointer sceneObjectsPtr;
        public Bool32 unkBool32_0x58;
        public ArrayPointer unknownTrigger2sPtr;
        public ArrayPointer collisionObjectReferences;
        public ArrayPointer unk_collisionObjectReferences; // refers back to above objects, but through another array-pointer
        public ArrayPointer unused_0x74_0x78;
        public CircuitType circuitType;
        public Pointer unknownStageData2Ptr;
        public Pointer unkPtr_0x84;
        public ArrayPointer unused_0x88_0x8C;
        public Pointer trackLengthPtr;
        public ArrayPointer unknownTrigger1sPtr;
        public ArrayPointer visualEffectTriggersPtr;
        public ArrayPointer courseMetadataTriggersPtr;
        public ArrayPointer arcadeCheckpointTriggersPtr;
        public ArrayPointer storyObjectTriggersPtr;
        public Pointer trackIndexTable;
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
            reader.ReadX(ref unk_0x00, true);
            reader.ReadX(ref trackNodesPtr);
            reader.ReadX(ref surfaceAttributeAreasPtr);
            reader.ReadX(ref boostPadsActive);
            reader.ReadX(ref surfaceAttributeMeshTablePtr);
            reader.ReadX(ref unknownData_0x20_Ptr);
            reader.ReadX(ref unknownFloat_0x24_Ptr);
            reader.ReadX(ref zero_0x28, kSizeOfZero0x28);
            reader.ReadX(ref sceneObjectCount);
            if (isFileGX) reader.ReadX(ref unk_sceneObjectCount1);
            reader.ReadX(ref unk_sceneObjectCount2);
            reader.ReadX(ref sceneObjectsPtr);
            reader.ReadX(ref unkBool32_0x58);
            reader.ReadX(ref unknownTrigger2sPtr);
            reader.ReadX(ref collisionObjectReferences);
            reader.ReadX(ref unk_collisionObjectReferences);
            reader.ReadX(ref unused_0x74_0x78);
            reader.ReadX(ref circuitType);
            reader.ReadX(ref unknownStageData2Ptr);
            reader.ReadX(ref unkPtr_0x84);
            reader.ReadX(ref unused_0x88_0x8C);
            reader.ReadX(ref trackLengthPtr);
            reader.ReadX(ref unknownTrigger1sPtr);
            reader.ReadX(ref visualEffectTriggersPtr);
            reader.ReadX(ref courseMetadataTriggersPtr);
            reader.ReadX(ref arcadeCheckpointTriggersPtr);
            reader.ReadX(ref storyObjectTriggersPtr);
            reader.ReadX(ref trackIndexTable);
            reader.ReadX(ref unknownStructure1_0xC0, true);
            reader.ReadX(ref zero_0xD8, kSizeOfZero0xD8);

            // Assert assumptions
            Assert.IsTrue(unused_0x74_0x78.length == 0 && unused_0x74_0x78.address == 0);
            Assert.IsTrue(unused_0x88_0x8C.length == 0 && unused_0x88_0x8C.address == 0);

            for (int i = 0; i < zero_0x28.Length; i++)
                Assert.IsTrue(zero_0x28[i] != 0);

            for (int i = 0; i < zero_0xD8.Length; i++)
                Assert.IsTrue(zero_0xD8[i] != 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
