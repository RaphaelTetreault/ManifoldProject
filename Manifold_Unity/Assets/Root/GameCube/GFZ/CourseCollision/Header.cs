using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class Header :
        IBinaryAddressable,
        IBinarySerializable
    {
        public enum SerializeFormat
        {
            InvalidFormat,
            AX,
            GX,
        }

        // CONSTANTS
        public const int kSizeOfZero0x28 = 0x20;
        public const int kSizeOfZero0xD8 = 0x10;

        private const int kAxConst0x20 = 0xE4;
        private const int kAxConst0x24 = 0xF8;
        private const int kGxConst0x20 = 0xE8;
        private const int kGxConst0x24 = 0xFC;

        // METADATA
        [UnityEngine.SerializeField] private bool isFileAX;
        [UnityEngine.SerializeField] private bool isFileGX;
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
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
        public ArrayPointer unknownSolsTriggerPtrs;
        public ArrayPointer sceneInstancesListPtrs;
        public ArrayPointer sceneOriginObjectsListPtrs; // refers back to above objects, but through another array-pointer
        public ArrayPointer unused_0x74_0x78;
        public CircuitType circuitType;
        public Pointer unknownStageData2Ptr;
        public Pointer unknownStageData1Ptr;
        public ArrayPointer unused_0x88_0x8C;
        public Pointer trackLengthPtr;
        public ArrayPointer unknownTrigger1sPtr;
        public ArrayPointer visualEffectTriggersPtr;
        public ArrayPointer courseMetadataTriggersPtr;
        public ArrayPointer arcadeCheckpointTriggersPtr;
        public ArrayPointer storyObjectTriggersPtr;
        public Pointer trackIndexTable;
        public ColiUnknownStruct1 unknownStructure1_0xC0; // minimap data?
        public byte[] zero_0xD8; // 0x10 count


        // PROPERTIES
        public bool IsFileAX => isFileAX;
        public bool IsFileGX => isFileGX;
        /// <summary>
        /// Returns true if file is tagged either AX or GX, but not both
        /// </summary>
        public bool IsValidFile => isFileAX ^ isFileGX;
        public SerializeFormat Format { get; set; }

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public static bool IsAX(Pointer ptr0x20, Pointer ptr0x24)
        {
            bool isAx0x20 = ptr0x20.address == kAxConst0x20;
            bool isAx0x24 = ptr0x24.address == kAxConst0x24;
            bool isAX = isAx0x20 & isAx0x24;
            return isAX;
        }

        public static bool IsGX(Pointer ptr0x20, Pointer ptr0x24)
        {
            bool isGx0x20 = ptr0x20.address == kGxConst0x20;
            bool isGx0x24 = ptr0x24.address == kGxConst0x24;
            bool isGX = isGx0x20 & isGx0x24;
            return isGX;
        }


        public void ValidateFileFormatPointers()
        {
            isFileAX = IsAX(unknownData_0x20_Ptr, unknownFloat_0x24_Ptr);
            isFileGX = IsGX(unknownData_0x20_Ptr, unknownFloat_0x24_Ptr);
            Assert.IsTrue(IsValidFile);
        }

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Deserialize main structure
                reader.ReadX(ref unk_0x00, true);
                reader.ReadX(ref trackNodesPtr);
                reader.ReadX(ref surfaceAttributeAreasPtr);
                reader.ReadX(ref boostPadsActive);
                reader.ReadX(ref surfaceAttributeMeshTablePtr);
                reader.ReadX(ref unknownData_0x20_Ptr);
                reader.ReadX(ref unknownFloat_0x24_Ptr);
                ValidateFileFormatPointers(); // VALIDATE
                reader.ReadX(ref zero_0x28, kSizeOfZero0x28);
                reader.ReadX(ref sceneObjectCount);
                if (isFileGX) reader.ReadX(ref unk_sceneObjectCount1);
                reader.ReadX(ref unk_sceneObjectCount2);
                reader.ReadX(ref sceneObjectsPtr);
                reader.ReadX(ref unkBool32_0x58);
                reader.ReadX(ref unknownSolsTriggerPtrs);
                reader.ReadX(ref sceneInstancesListPtrs);
                reader.ReadX(ref sceneOriginObjectsListPtrs);
                reader.ReadX(ref unused_0x74_0x78);
                reader.ReadX(ref circuitType);
                reader.ReadX(ref unknownStageData2Ptr);
                reader.ReadX(ref unknownStageData1Ptr);
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
            }
            this.RecordEndAddress(reader);
            {
                // Assert assumptions
                Assert.IsTrue(unused_0x74_0x78.Length == 0 && unused_0x74_0x78.Address == 0);
                Assert.IsTrue(unused_0x88_0x8C.Length == 0 && unused_0x88_0x8C.Address == 0);

                for (int i = 0; i < zero_0x28.Length; i++)
                    Assert.IsTrue(zero_0x28[i] == 0);

                for (int i = 0; i < zero_0xD8.Length; i++)
                    Assert.IsTrue(zero_0xD8[i] == 0);

                // Record some metadata
                Format = IsFileAX ? SerializeFormat.AX : SerializeFormat.GX;
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                isFileAX = Format == SerializeFormat.AX;
                isFileGX = Format == SerializeFormat.GX;
            }
            this.RecordStartAddress(writer);
            {
                // Serialize header. This is done twice.
                // 1st time: serialize structure as placeholder.
                // 2nd time: serialize structure but now with pointers resolved.
                writer.WriteX(unk_0x00);
                writer.WriteX(trackNodesPtr);
                writer.WriteX(surfaceAttributeAreasPtr);
                writer.WriteX(boostPadsActive);
                writer.WriteX(surfaceAttributeMeshTablePtr);
                writer.WriteX(unknownData_0x20_Ptr);
                writer.WriteX(unknownFloat_0x24_Ptr);
                writer.WriteX(new byte[kSizeOfZero0x28], false); // write const zeros
                writer.WriteX(sceneObjectCount);
                if (Format == SerializeFormat.GX)
                    writer.WriteX(unk_sceneObjectCount1);
                writer.WriteX(unk_sceneObjectCount2);
                writer.WriteX(sceneObjectsPtr);
                writer.WriteX(unkBool32_0x58);
                writer.WriteX(unknownSolsTriggerPtrs);
                writer.WriteX(sceneInstancesListPtrs);
                writer.WriteX(sceneOriginObjectsListPtrs);
                writer.WriteX(new ArrayPointer()); // const unused
                writer.WriteX(circuitType);
                writer.WriteX(unknownStageData2Ptr);
                writer.WriteX(unknownStageData1Ptr);
                writer.WriteX(new ArrayPointer()); // const unused
                writer.WriteX(trackLengthPtr);
                writer.WriteX(unknownTrigger1sPtr);
                writer.WriteX(visualEffectTriggersPtr);
                writer.WriteX(courseMetadataTriggersPtr);
                writer.WriteX(arcadeCheckpointTriggersPtr);
                writer.WriteX(storyObjectTriggersPtr);
                writer.WriteX(trackIndexTable);
                writer.WriteX(unknownStructure1_0xC0);
                writer.WriteX(new byte[kSizeOfZero0xD8], false); // write const zeros
            }
            this.RecordEndAddress(writer);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            var addressRange = new AddressRange();
            addressRange.RecordStartAddress(writer.BaseStream);
            {
                Serialize(writer);
            }
            addressRange.RecordEndAddress(writer.BaseStream);
            return addressRange;
        }

    }
}
