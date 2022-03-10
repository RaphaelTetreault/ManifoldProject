using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    // NOTES:
    // Indexes 0-3 ARE USED
    //  idx0: uv.xy scrolling (or, at least, on some models)
    //  idx1: ?
    //  idx2: ?
    //  idx3: ?
    // Indexes 4-11 unused, always (0f, 0f)

    /// <summary>
    /// Texture metadata. In some instasnces defines how a texture scrolls.
    /// </summary>
    [Serializable]
    public sealed class TextureScroll :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // CONSTANTS
        public const int kCount = 12;


        // FIELDS
        private Pointer[] fieldPtrs;
        // REFERENCE FIELDS
        private TextureScrollField[] fields = new TextureScrollField[0];


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public TextureScrollField[] Fields { get => fields; set => fields = value; }
        public Pointer[] FieldPtrs { get => fieldPtrs; set => fieldPtrs = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref fieldPtrs, kCount);
            }
            this.RecordEndAddress(reader);
            {
                fields = new TextureScrollField[kCount];
                for (int i = 0; i < kCount; i++)
                {
                    var pointer = fieldPtrs[i];
                    if (pointer.IsNotNull)
                    {
                        reader.JumpToAddress(pointer);
                        reader.ReadX(ref fields[i]);
                    }
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                fieldPtrs = fields.GetPointers();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(fieldPtrs);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Validate each field/pointer
            for (int i = 0; i < kCount; i++)
            {
                // reference can be to float2(0, 0)
                Assert.ReferencePointer(fields[i], fieldPtrs[i]);

                //if (fields[i] != null)
                //    Assert.IsTrue(fields[i].x != 0 && fields[i].y != 0);
            }
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(TextureScroll));
            indentLevel++;
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] == null)
                    continue;
                builder.AppendLineIndented(indent, indentLevel, $"[{i}] {fields[i]}");
            }
        }

        public string PrintSingleLine()
        {
            return nameof(TextureScroll);
        }

        public override string ToString() => PrintSingleLine();

    }
}