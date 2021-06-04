using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class Bounds :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] AddressRange addressRange;

        // FIELDS
        public float x; // bounds -x. Value = max x value of track node
        public float z; // bounds -z. Value = max z value of track node
        public float unk_0x08; // looks like rotation, maybe?
        public float unk_0x0C; // looks like rotation, maybe? bounds +z?
        public BoundsOption unk_0x10;
        public BoundsOption unk_0x14;


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
                reader.ReadX(ref x);
                reader.ReadX(ref z);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref unk_0x14);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(x);
                writer.WriteX(z);
                writer.WriteX(unk_0x08);
                writer.WriteX(unk_0x0C);
                writer.WriteX(unk_0x10);
                writer.WriteX(unk_0x14);
            }
            this.RecordEndAddress(writer);
        }
    }
}
