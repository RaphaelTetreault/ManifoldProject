using GameCube.GX;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.FMI
{
    [Serializable]
    public class ExhaustParticle :
        IBinarySerializable,
        IBinaryAddressable
    {
        // FIELDS
        public float3 position;
        public uint unk_0x0C;
        public uint unk_0x10;
        public float scaleMin;
        public float scaleMax;
        // Engine Color of Normal Acceleration
        public GXColor colorMin;
        // Engine Color of Strong Acceleration
        public GXColor colorMax;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref scaleMin);
                reader.ReadX(ref scaleMax);
                reader.ReadX(ref colorMin);
                reader.ReadX(ref colorMax);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(position);
                writer.WriteX(unk_0x0C);
                writer.WriteX(unk_0x10);
                writer.WriteX(scaleMin);
                writer.WriteX(scaleMax);
                writer.WriteX(colorMin);
                writer.WriteX(colorMax);
            }
            this.RecordEndAddress(writer);
        }

    }
}