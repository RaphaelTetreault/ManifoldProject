using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ColliderGeometry :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public uint unk_0x00;
        public uint unk_0x04;
        public uint unk_0x08;
        public uint unk_0x0C;
        public uint unk_0x10;
        // this is a ArrayPointer2D, consider refactor
        public int triCount;
        public int quadCount;
        public Pointer trisPtr;
        public Pointer quadsPtr;
        // FIELDS (deserialized from pointers)
        public ColliderTriangle[] tris;
        public ColliderQuad[] quads;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public void ValidateReferences()
        {
            // Sanity check
            if (tris.Length > 0)
            {
                Assert.IsTrue(triCount == tris.Length);
                Assert.IsTrue(trisPtr.IsNotNullPointer);

                foreach (var tri in tris)
                {
                    Assert.IsTrue(tri != null);
                }
            }

        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref triCount);
                reader.ReadX(ref quadCount);
                reader.ReadX(ref trisPtr);
                reader.ReadX(ref quadsPtr);
            }
            this.RecordEndAddress(reader);
            {
                if (triCount > 0)
                {
                    reader.JumpToAddress(trisPtr);
                    reader.ReadX(ref tris, triCount, true);
                }

                if (quadCount > 0)
                {
                    reader.JumpToAddress(quadsPtr);
                    reader.ReadX(ref quads, quadCount, true);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                triCount = tris.Length;
                quadCount = quads.Length;
                trisPtr = tris.GetBasePointer();
                quadsPtr = quads.GetBasePointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(unk_0x08);
                writer.WriteX(unk_0x0C);
                writer.WriteX(unk_0x10);
                writer.WriteX(triCount);
                writer.WriteX(quadCount);
                writer.WriteX(trisPtr);
                writer.WriteX(quadsPtr);
            }
            this.RecordEndAddress(writer);
        }

    }
}