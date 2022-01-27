using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ
{
    public struct BoundingSphere :
        IBinarySerializable,
        IBinaryAddressable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public float3 origin;
        public float radius;


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
                reader.ReadX(ref origin);
                reader.ReadX(ref radius);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(origin);
                writer.WriteX(radius);
            }
            this.RecordEndAddress(writer);
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
