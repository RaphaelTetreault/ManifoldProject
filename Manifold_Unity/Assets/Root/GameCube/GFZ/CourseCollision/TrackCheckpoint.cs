using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Defines a checkpoint along the track spline. This is mostly metadata.
    /// It is used by the game to determine progression and (presumably) find
    /// when the player takes an excessive shortcut.
    /// </summary>
    [Serializable]
    public class TrackCheckpoint :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public float curveTimeStart;
        public float curveTimeEnd;
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
                reader.ReadX(ref curveTimeStart);
                reader.ReadX(ref curveTimeEnd);
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
            this.RecordStartAddress(writer);
            {
                writer.WriteX(curveTimeStart);
                writer.WriteX(curveTimeEnd);
                writer.WriteX(trackDistanceStart);
                writer.WriteX(tangentStart);
                writer.WriteX(positionStart);
                writer.WriteX(trackDistanceEnd);
                writer.WriteX(tangentEnd);
                writer.WriteX(positionEnd);
                writer.WriteX(transformDistanceEnd);
                writer.WriteX(transformDistanceStart);
                writer.WriteX(trackWidth);
                writer.WriteX(isTrackContinuousStart);
                writer.WriteX(isTrackContinuousEnd);
                writer.WriteX(zero_0x4E);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(TrackCheckpoint)}(" +
                $"{nameof(curveTimeStart)}: {curveTimeStart:0.00}, " +
                $"{nameof(curveTimeEnd)}: {curveTimeEnd:0.00}, " +
                $"{nameof(trackDistanceStart)}: {trackDistanceStart:0.0}, " +
                $"{nameof(trackDistanceEnd)}: {trackDistanceEnd:0.0}, " +
                $"{nameof(tangentStart)}(x:{tangentStart.x:0.0}, y:{tangentStart.y:0.0}, z:{tangentStart.z:0.0}), " +
                $"{nameof(tangentEnd)}(x:{tangentEnd.x:0.0}, y:{tangentEnd.y:0.0}, z:{tangentEnd.z:0.0}), " +
                $"{nameof(positionStart)}(x:{positionStart.x:0.0}, y:{positionStart.y:0.0}, z:{positionStart.z:0.0}), " +
                $"{nameof(positionEnd)}(x:{positionEnd.x:0.0}, y:{positionEnd.y:0.0}, z:{positionEnd.z:0.0}), " +
                $"{nameof(transformDistanceStart)}: {transformDistanceStart:0.0}, " +
                $"{nameof(transformDistanceEnd)}: {transformDistanceEnd:0.0}, " +
                $"{nameof(trackWidth)}: {trackWidth:0.0}, " +
                $"{nameof(isTrackContinuousStart)}: {isTrackContinuousStart}, " +
                $"{nameof(isTrackContinuousEnd)}: {isTrackContinuousEnd}" +
                $")";
        }

    }
}
