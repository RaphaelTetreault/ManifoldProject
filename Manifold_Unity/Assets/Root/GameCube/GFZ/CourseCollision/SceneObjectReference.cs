﻿using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// SceneObject ptr1
    /// </summary>
    [Serializable]
    public class SceneObjectReference :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public uint zero_0x00;
        public Pointer namePtr;
        public uint zero_0x08;
        public float unk_0x0C; // LOD?
        //  FIELDS (deserialized from pointer)
        public CString name;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public void ValidateReferences()
        {
            // This pointer CANNOT be null and must refer to an object name.
            Assert.IsTrue(namePtr.IsNotNullPointer);
            Assert.IsTrue(name != null);
            Assert.IsTrue(!string.IsNullOrEmpty(name.value));
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zero_0x00);
                reader.ReadX(ref namePtr);
                reader.ReadX(ref zero_0x08);
                reader.ReadX(ref unk_0x0C);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero_0x00 == 0);
                Assert.IsTrue(zero_0x08 == 0);

                reader.JumpToAddress(namePtr);
                reader.ReadX(ref name, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero_0x00 == 0);
                Assert.IsTrue(zero_0x08 == 0);

                // It is assummed that the pointer is set before serialization
                namePtr = name.GetPointer();
                Assert.IsTrue(namePtr.IsNotNullPointer);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(zero_0x00);
                writer.WriteX(namePtr);
                writer.WriteX(zero_0x08);
                writer.WriteX(unk_0x0C);
            }
            this.RecordEndAddress(writer);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            Serialize(writer);
            return addressRange;
        }
    }
}