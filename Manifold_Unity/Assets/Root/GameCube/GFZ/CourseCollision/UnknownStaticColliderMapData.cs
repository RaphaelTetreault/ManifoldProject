using System;
using System.IO;
using Manifold.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnknownStaticColliderMapData :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public float unk_0x00;
        public float unk_0x04;
        public float unk_0x08;
        public float unk_0x0C;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

     
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(unk_0x08);
                writer.WriteX(unk_0x0C);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(UnknownStaticColliderMapData)}(" +
                $"{nameof(unk_0x00)}: {unk_0x00}, " +
                $"{nameof(unk_0x04)}: {unk_0x04}, " +
                $"{nameof(unk_0x08)}: {unk_0x08}, " +
                $"{nameof(unk_0x0C)}: {unk_0x0C})";
        }

    }
}
