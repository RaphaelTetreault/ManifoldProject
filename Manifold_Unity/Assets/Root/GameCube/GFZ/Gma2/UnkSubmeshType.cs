using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace GameCube.GFZ.Gma2
{
    public class UnkSubmeshType :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private float3 origin;
        private uint unk0x0C;
        private UnkFlags0x10 unk0x10;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public float3 Origin { get => origin; set => origin = value; }
        public uint Unk0x3C { get => unk0x0C; set => unk0x0C = value; }
        public UnkFlags0x10 Unk0x40 { get => unk0x10; set => unk0x10 = value; }

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref origin);
                reader.ReadX(ref unk0x0C);
                reader.ReadX(ref unk0x10);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(origin);
                writer.WriteX(unk0x0C);
                writer.WriteX(unk0x10);
            }
            this.RecordEndAddress(writer);
        }

    }

}