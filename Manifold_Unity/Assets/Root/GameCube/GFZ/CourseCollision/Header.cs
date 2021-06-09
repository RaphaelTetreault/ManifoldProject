using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class Header :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
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
        public const int kAxConst0x20 = 0xE4;
        public const int kAxConst0x24 = 0xF8;
        public const int kGxConst0x20 = 0xE8;
        public const int kGxConst0x24 = 0xFC;

        // METADATA
        [UnityEngine.SerializeField] private bool isFileAX;
        [UnityEngine.SerializeField] private bool isFileGX;
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public Range unk_0x00;
        public ArrayPointer trackNodesPtr;
        public ArrayPointer surfaceAttributeAreasPtr;
        public BoostPadsActive boostPadsActive;
        public Pointer surfaceAttributeMeshTablePtr;
        public Pointer zeroes0x20_Ptr; // GX: 0xE8, AX: 0xE4
        public Pointer trackMinHeightPtr; // GX: 0xFC, AX: 0xF8
        public byte[] zero_0x28; // 0x20 count
        public int sceneObjectCount;
        public int unk_sceneObjectCount1;
        public int unk_sceneObjectCount2; // GX exclusive
        public Pointer sceneObjectsPtr;
        public Bool32 unkBool32_0x58;
        public ArrayPointer unknownSolsTriggerPtrs;
        public ArrayPointer sceneInstancesListPtrs;
        public ArrayPointer sceneOriginObjectsListPtrs;
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
        public Pointer trackCheckpointTable8x8Ptr;
        public Bounds courseBounds;
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
            isFileAX = IsAX(zeroes0x20_Ptr, trackMinHeightPtr);
            isFileGX = IsGX(zeroes0x20_Ptr, trackMinHeightPtr);
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
                reader.ReadX(ref zeroes0x20_Ptr);
                reader.ReadX(ref trackMinHeightPtr);
                ValidateFileFormatPointers(); // VALIDATE
                reader.ReadX(ref zero_0x28, kSizeOfZero0x28);
                reader.ReadX(ref sceneObjectCount);
                reader.ReadX(ref unk_sceneObjectCount1);
                if (isFileGX) reader.ReadX(ref unk_sceneObjectCount2);
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
                reader.ReadX(ref trackCheckpointTable8x8Ptr);
                reader.ReadX(ref courseBounds, true);
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
                writer.WriteX(zeroes0x20_Ptr);
                writer.WriteX(trackMinHeightPtr);
                writer.WriteX(new byte[kSizeOfZero0x28], false); // write const zeros
                writer.WriteX(sceneObjectCount);
                writer.WriteX(unk_sceneObjectCount1);
                if (Format == SerializeFormat.GX)
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
                writer.WriteX(trackCheckpointTable8x8Ptr);
                writer.WriteX(courseBounds);
                writer.WriteX(new byte[kSizeOfZero0xD8], false); // write const zeros
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Assert assumptions
            // perhaps genericize this to ints
            Assert.IsTrue(unused_0x74_0x78.Length == 0 && unused_0x74_0x78.Address == 0);
            Assert.IsTrue(unused_0x88_0x8C.Length == 0 && unused_0x88_0x8C.Address == 0);

            for (int i = 0; i < zero_0x28.Length; i++)
                Assert.IsTrue(zero_0x28[i] == 0);

            for (int i = 0; i < zero_0xD8.Length; i++)
                Assert.IsTrue(zero_0xD8[i] == 0);

            // Assert all pointers which are never null in game files
            Assert.IsTrue(trackNodesPtr.IsNotNullPointer);
            Assert.IsTrue(surfaceAttributeAreasPtr.IsNotNullPointer);
            Assert.IsTrue(surfaceAttributeMeshTablePtr.IsNotNullPointer);
            Assert.IsTrue(zeroes0x20_Ptr.IsNotNullPointer);
            Assert.IsTrue(trackMinHeightPtr.IsNotNullPointer);
            Assert.IsTrue(sceneObjectsPtr.IsNotNullPointer);
            //Assert.IsTrue(unknownSolsTriggerPtrs.IsNotNullPointer);
            Assert.IsTrue(sceneInstancesListPtrs.IsNotNullPointer);
            Assert.IsTrue(sceneOriginObjectsListPtrs.IsNotNullPointer);
            //Assert.IsTrue(unknownStageData2Ptr.IsNotNullPointer);
            Assert.IsTrue(unknownStageData1Ptr.IsNotNullPointer);
            //Assert.IsTrue(unknownTrigger1sPtr.IsNotNullPointer);
            //Assert.IsTrue(visualEffectTriggersPtr.IsNotNullPointer);
            //Assert.IsTrue(courseMetadataTriggersPtr.IsNotNullPointer);
            //Assert.IsTrue(arcadeCheckpointTriggersPtr.IsNotNullPointer);
            //Assert.IsTrue(storyObjectTriggersPtr.IsNotNullPointer);
            Assert.IsTrue(trackCheckpointTable8x8Ptr.IsNotNullPointer);
        }
    }
}
