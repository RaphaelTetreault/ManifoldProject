using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// A referenceable 'float'. Has associated address.
    /// </summary>
    [Serializable]
    public abstract class FloatRef :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        public float value;


        // CONSTRUCTORS
        public static implicit operator float(FloatRef floatRef)
        {
            return floatRef.value;
        }


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref value);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(value);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return $"{GetType().Name}({value})";
        }
    }
}
