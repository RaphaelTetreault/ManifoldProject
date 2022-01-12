using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
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
    public class TextureMetadata :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // CONSTANTS
        public const int kCount = 12;

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public Pointer[] fieldPtrs;
        // REFERENCE FIELDS
        public TextureMetadataField[] fields = new TextureMetadataField[0];


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref fieldPtrs, kCount, true);
            }
            this.RecordEndAddress(reader);
            {
                fields = new TextureMetadataField[kCount];
                for (int i = 0; i < kCount; i++)
                {
                    var pointer = fieldPtrs[i];
                    if (pointer.IsNotNullPointer)
                    {
                        reader.JumpToAddress(pointer);
                        reader.ReadX(ref fields[i], true);
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
                writer.WriteX(fieldPtrs, false);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            for (int i = 0; i < kCount; i++)
            {
                // Validate object references if pointer exists
                if (fieldPtrs[i].IsNotNullPointer)
                    Assert.IsTrue(fields[i] != null);

                // Validate pointers if object reference exists
                if (fields[i] != null)
                    Assert.IsTrue(fieldPtrs[i].IsNotNullPointer);
            }
        }

        public override string ToString()
        {
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append(nameof(TextureMetadata));
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] == null)
                    continue;
                stringBuilder.Append($"[{i}]{fields[i]}, ");
            }

            return stringBuilder.ToString();
        }
    }
}