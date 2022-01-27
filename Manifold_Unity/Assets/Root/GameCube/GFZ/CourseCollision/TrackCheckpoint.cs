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


        [Serializable]
        public struct CheckpointRange :
            IBinarySerializable
            // addressable?
        {
            public float projection;
            public float3 tangent;
            public float3 position;

            public void Deserialize(BinaryReader reader)
            {
                reader.ReadX(ref projection);
                reader.ReadX(ref tangent);
                reader.ReadX(ref position);
            }

            public void Serialize(BinaryWriter writer)
            {
                writer.WriteX(projection);
                writer.WriteX(tangent);
                writer.WriteX(position);
            }
        }


        // FIELDS
        public float curveTimeStart;
        public float curveTimeEnd;
        public CheckpointRange start;
        public CheckpointRange end;
        public float endDistance;
        public float startDistance;
        public float trackWidth;
        public bool hasTrackIn;
        public bool hasTrackOut;
        public ushort zero_0x4E;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        // track min height
        public static float GetMinHeight(TrackCheckpoint[] checkpoints)
        {
            var min = float.PositiveInfinity;

            // iterate over every position, mo
            foreach (var checkpoint in checkpoints)
            {
                min = math.min(min, checkpoint.start.position.y);
                min = math.min(min, checkpoint.end.position.y);
            }

            return min;
        }

        // track checkpoint matrix bounds
        public float GetMinPositionX()
        {
            return math.min(start.position.x, end.position.x);
        }
        public float GetMinPositionZ()
        {
            return math.min(start.position.z, end.position.z);

        }
        public float GetMaxPositionX()
        {
            return math.max(start.position.x, end.position.x);
        }
        public float GetMaxPositionZ()
        {
            return math.max(start.position.z, end.position.z);

        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref curveTimeStart);
                reader.ReadX(ref curveTimeEnd);
                reader.ReadX(ref start, true);
                reader.ReadX(ref end, true);
                reader.ReadX(ref endDistance);
                reader.ReadX(ref startDistance);
                reader.ReadX(ref trackWidth);
                reader.ReadX(ref hasTrackIn);
                reader.ReadX(ref hasTrackOut);
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
                writer.WriteX(start);
                writer.WriteX(end);
                writer.WriteX(endDistance);
                writer.WriteX(startDistance);
                writer.WriteX(trackWidth);
                writer.WriteX(hasTrackIn);
                writer.WriteX(hasTrackOut);
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
                $"{nameof(start)}.{nameof(start.projection)}: {start.projection:0.0}, " +
                $"{nameof(start)}.{nameof(start.tangent)}(x:{start.tangent.x:0.0}, y:{start.tangent.y:0.0}, z:{start.tangent.z:0.0}), " +
                $"{nameof(start)}.{nameof(start.position)}(x:{start.position.x:0.0}, y:{start.position.y:0.0}, z:{start.position.z:0.0}), " +
                $"{nameof(end)}.{nameof(end.projection)}: {end.projection:0.0}, " +
                $"{nameof(end)}.{nameof(end.tangent)}(x:{end.tangent.x:0.0}, y:{end.tangent.y:0.0}, z:{end.tangent.z:0.0}), " +
                $"{nameof(end)}.{nameof(end.position)}(x:{end.position.x:0.0}, y:{end.position.y:0.0}, z:{end.position.z:0.0}), " +
                $"{nameof(startDistance)}: {startDistance:0.0}, " +
                $"{nameof(endDistance)}: {endDistance:0.0}, " +
                $"{nameof(trackWidth)}: {trackWidth:0.0}, " +
                $"{nameof(hasTrackIn)}: {hasTrackIn}, " +
                $"{nameof(hasTrackOut)}: {hasTrackOut}" +
                $")";
        }

    }
}
