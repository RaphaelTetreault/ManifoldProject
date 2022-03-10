using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Represent a "dynamic" collider object. Static collision is stored in a
    /// separate table, while colliders attached to object (which may animate)
    /// is stored in this structure.
    /// </summary>
    /// <remarks>
    /// Example: rotary collider in Port Town [Long Pipe]
    /// </remarks>
    [Serializable]
    public sealed class ColliderMesh :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // CONSTANTS
        public const int kTriIndex = 0;
        public const int kQuadIndex = 1;
        public const int kTotalIndices = 2;

        // FIELDS
        private uint unk_0x00;
        private uint unk_0x04;
        private uint unk_0x08;
        private uint unk_0x0C;
        private uint unk_0x10;
        private ArrayPointer2D collisionArrayPtr2D;
        // REFERENCE FIELDS
        private ColliderTriangle[] tris;
        private ColliderQuad[] quads;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public ArrayPointer2D CollisionArrayPtr2D { get => collisionArrayPtr2D; set => collisionArrayPtr2D = value; }
        public ColliderTriangle[] Tris { get => tris; set => tris = value; }
        public ArrayPointer TrisPtr { get => collisionArrayPtr2D[kTriIndex]; set => collisionArrayPtr2D[kTriIndex] = value; }
        public ColliderQuad[] Quads { get => quads; set => quads = value; }
        public ArrayPointer QuadsPtr { get => collisionArrayPtr2D[kQuadIndex]; set => collisionArrayPtr2D[kQuadIndex] = value; }
        public uint Unk_0x00 { get => unk_0x00; set => unk_0x00 = value; }
        public uint Unk_0x04 { get => unk_0x04; set => unk_0x04 = value; }
        public uint Unk_0x08 { get => unk_0x08; set => unk_0x08 = value; }
        public uint Unk_0x0C { get => unk_0x0C; set => unk_0x0C = value; }
        public uint Unk_0x10 { get => unk_0x10; set => unk_0x10 = value; }


        // METHODS
        public void ValidateReferences()
        {
            Assert.ReferencePointer(Tris, TrisPtr);
            Assert.ReferencePointer(Quads, QuadsPtr);

            // SANITY CHECK
            // Make sure counts line up
            if (Tris != null)
            {
                if (Tris.Length > 0)
                {
                    Assert.IsTrue(TrisPtr.length == Tris.Length);
                    Assert.IsTrue(TrisPtr.IsNotNull);

                    foreach (var tri in Tris)
                    {
                        Assert.IsTrue(tri != null);
                    }
                }
            }

            if (Quads != null)
            {
                if (Quads.Length > 0)
                {
                    Assert.IsTrue(QuadsPtr.length == Quads.Length);
                    Assert.IsTrue(QuadsPtr.IsNotNull);

                    foreach (var quad in Quads)
                    {
                        Assert.IsTrue(quad != null);
                    }
                }
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            {
                // Initialize ArrayPointer2D with constant size, manually deserialized later
                collisionArrayPtr2D = new ArrayPointer2D(kTotalIndices);
            }
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x10);
                collisionArrayPtr2D.Deserialize(reader);
            }
            this.RecordEndAddress(reader);
            {
                if (TrisPtr.IsNotNull)
                {
                    reader.JumpToAddress(TrisPtr);
                    reader.ReadX(ref tris, TrisPtr.length);
                }

                if (QuadsPtr.IsNotNull)
                {
                    reader.JumpToAddress(QuadsPtr);
                    reader.ReadX(ref quads, QuadsPtr.length);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                //
                TrisPtr = tris.GetArrayPointer();
                QuadsPtr = quads.GetArrayPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(unk_0x08);
                writer.WriteX(unk_0x0C);
                writer.WriteX(unk_0x10);
                writer.WriteX(collisionArrayPtr2D);
            }
            this.RecordEndAddress(writer);

        }

        public override string ToString() => PrintSingleLine();

        public string PrintSingleLine()
        {
            return $"{nameof(ColliderMesh)}({nameof(tris)}: {tris.Length}, {nameof(quads)}: {quads.Length})";
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(ColliderMesh));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x00)}: {unk_0x00}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x04)}: {unk_0x04}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x08)}: {unk_0x08}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x0C)}: {unk_0x0C}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(unk_0x10)}: {unk_0x10}");
            //builder.AppendLineIndented(indent, indentLevel, $"{nameof(TrisPtr)}: {TrisPtr}");
            //builder.AppendLineIndented(indent, indentLevel, $"{nameof(QuadsPtr)}: {QuadsPtr}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Tris)}: {Tris}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Quads)}: {Quads}");
        }
    }
}