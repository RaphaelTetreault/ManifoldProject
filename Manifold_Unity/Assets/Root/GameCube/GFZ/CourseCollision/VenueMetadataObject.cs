using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class VenueMetadataObject : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public Vector3 position;
        public ushort unk_0x0C;
        public ushort unk_0x0E;
        public ushort unk_0x10;
        public ushort unk_0x12;
        public Vector3 positionOrScale;
        public VenueMetadataFlag venue;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x0E);
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref unk_0x12);
                reader.ReadX(ref positionOrScale);
                reader.ReadX(ref venue);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
