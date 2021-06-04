using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TrackPoint : IBinarySerializable, IBinaryAddressable
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public float unk_0x00;
        public float unk_0x04;
        // Make Struct?
        public float trackDistanceStart;
        public float3 tangentStart;
        public float3 positionStart;
        // Make Struct?
        public float trackDistanceEnd;
        public float3 tangentEnd;
        public float3 positionEnd;
        //
        public float transformDistanceEnd;
        public float transformDistanceStart;
        public float trackWidth;
        public bool isTrackContinuousStart;
        public bool isTrackContinuousEnd;
        public ushort zero_0x4E;


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
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref trackDistanceStart);
                reader.ReadX(ref tangentStart);
                reader.ReadX(ref positionStart);
                reader.ReadX(ref trackDistanceEnd);
                reader.ReadX(ref tangentEnd);
                reader.ReadX(ref positionEnd);
                reader.ReadX(ref transformDistanceEnd);
                reader.ReadX(ref transformDistanceStart);
                reader.ReadX(ref trackWidth);
                reader.ReadX(ref isTrackContinuousStart);
                reader.ReadX(ref isTrackContinuousEnd);
                reader.ReadX(ref zero_0x4E);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(trackDistanceStart);
            writer.WriteX(tangentStart);
            writer.WriteX(positionStart);
            writer.WriteX(trackDistanceEnd);
            writer.WriteX(tangentEnd);
            writer.WriteX(positionEnd);
            writer.WriteX(transformDistanceEnd);
            writer.WriteX(transformDistanceStart);
            writer.WriteX(isTrackContinuousStart);
            writer.WriteX(isTrackContinuousEnd);
            writer.WriteX(trackWidth);
            writer.WriteX(zero_0x4E);
        }

    }
}
