using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.FMI
{
    [Serializable]
    public class ExhaustAnimation :
        IBinarySerializable,
        IBinaryAddressable
    {
        // FIELDS
        public float3 position;
        public int unk_0x0C;
        public int animType;


        // PROEPRTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref animType);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(position);
                writer.WriteX(unk_0x0C);
                writer.WriteX(animType);
            }
            this.RecordEndAddress(writer);
        }

    }
}