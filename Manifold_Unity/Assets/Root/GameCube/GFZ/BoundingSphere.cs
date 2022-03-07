using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ
{
    public struct BoundingSphere :
        IBinarySerializable,
        IBinaryAddressable
    {
        // FIELDS
        public float3 origin;
        public float radius;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            var addressRange = new AddressRange();
            addressRange.RecordStartAddress(reader);
            {
                reader.ReadX(ref origin);
                reader.ReadX(ref radius);
            }
            addressRange.RecordEndAddress(reader);
            AddressRange = addressRange;
        }

        public void Serialize(BinaryWriter writer)
        {
            var addressRange = new AddressRange();
            addressRange.RecordStartAddress(writer);
            {
                writer.WriteX(origin);
                writer.WriteX(radius);
            }
            addressRange.RecordEndAddress(writer);
            AddressRange = addressRange;
        }


        public override string ToString()
        {
            return
                $"{nameof(BoundingSphere)}(" +
                $"{nameof(origin)}: ({origin.x}, {origin.y}, {origin.z}), " +
                $"{nameof(radius)}: {radius})";
        }

    }
}
