using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Represent a "dynamic" collider object. Static collision is stored in a
    /// separate table, while colliders attached to object (which may animate)
    /// is stored in this structure.
    /// 
    /// Example: rotary collider in Port Town [Long Pipe]
    /// </summary>
    [Serializable]
    public class ColliderGeometry :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

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
        // REFERENCE FIELDS
        public ColliderTriangle[] tris;// = new ColliderTriangle[0];
        public ColliderQuad[] quads;// = new ColliderQuad[0];


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public ArrayPointer TrisArrayPtr => new ArrayPointer(triCount, trisPtr);
        public ArrayPointer QuadsArrayPtr => new ArrayPointer(quadCount, quadsPtr);

        // METHODS
        public void ValidateReferences()
        {
            Assert.ReferencePointer(tris, TrisArrayPtr);
            Assert.ReferencePointer(quads, QuadsArrayPtr);

            // SANITY CHECK
            // Make sure counts line up
            if (tris != null)
            {
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

            if (quads != null)
            {
                if (quads.Length > 0)
                {
                    Assert.IsTrue(quadCount == quads.Length);
                    Assert.IsTrue(quadsPtr.IsNotNullPointer);

                    foreach (var quad in quads)
                    {
                        Assert.IsTrue(quad != null);
                    }
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
                if (trisPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(trisPtr);
                    reader.ReadX(ref tris, triCount, true);
                }

                if (quadsPtr.IsNotNullPointer)
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
                triCount = tris == null ? 0 : tris.Length;
                quadCount = quads == null ? 0 : quads.Length;
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