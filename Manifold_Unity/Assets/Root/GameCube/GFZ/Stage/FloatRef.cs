using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A referenceable 'float'. Has associated address.
    /// </summary>
    [Serializable]
    public abstract class FloatRef :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        private float value;


        // CONSTRUCTORS
        public static implicit operator float(FloatRef floatRef)
        {
            return floatRef.Value;
        }


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public float Value { get => value; set => this.value = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            AddressRange.RecordStartAddress(reader);
            {
                reader.ReadX(ref value);
            }
            AddressRange.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            AddressRange.RecordStartAddress(writer);
            {
                writer.WriteX(value);
            }
            AddressRange.RecordEndAddress(writer);
        }

        public override string ToString() => PrintSingleLine();

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
        }

        public string PrintSingleLine()
        {
            return $"{GetType().Name}({Value})";
        }

    }
}
