using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Defines an object with skeletal animations.
    /// </summary>
    [Serializable]
    public sealed class SkeletalAnimator :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // FIELDS
        private uint zero_0x00;
        private uint zero_0x04;
        private uint one_0x08; // Always 1. Bool?
        private Pointer propertiesPtr;
        // REFERENCE FIELDS
        private SkeletalProperties properties;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public Pointer PropertiesPtr { get => propertiesPtr; set => propertiesPtr = value; }
        public SkeletalProperties Properties { get => properties; set => properties = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zero_0x00);
                reader.ReadX(ref zero_0x04);
                reader.ReadX(ref one_0x08);
                reader.ReadX(ref propertiesPtr);
            }
            this.RecordEndAddress(reader);
            {
                // 2021/06/16: should ALWAYS exist
                Assert.IsTrue(propertiesPtr.IsNotNull);
                reader.JumpToAddress(propertiesPtr);
                reader.ReadX(ref properties);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                propertiesPtr = properties.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(zero_0x00);
                writer.WriteX(zero_0x04);
                writer.WriteX(one_0x08);
                writer.WriteX(propertiesPtr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            Assert.ReferencePointer(properties, propertiesPtr);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, properties);
        }

        public string PrintSingleLine()
        {
            return nameof(SkeletalAnimator);
        }

        public override string ToString() => PrintSingleLine();
    }
}