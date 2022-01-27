using Manifold;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Defines a checkpoint range along the track spline. This is a vital piece of
    /// metadata as it tells the game which part of the track any vehicle is on and 
    /// uses it compute a lot of the vehicle position and validity of that position.
    /// When incorrect, many strange, buggy things happen.
    /// </summary>
    [Serializable]
    public class TrackCheckpoint :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        /// <summary>
        /// Where along the track's Position.XYZ animation curve 'start' elements were sampled.
        /// </summary>
        public float curveTimeStart;
        /// <summary>
        /// Where along the track's Position.XYZ animation curve 'end' elements were sampled.
        /// </summary>
        public float curveTimeEnd;
        /// <summary>
        /// A plane which partitions global coordinate space into two. Plane points "forward"
        /// along track to indicate what is in the checkpoint. With 'end' plane, creates
        /// a volume which is considered inside this checkpoint.
        /// </summary>
        public Plane planeStart;
        /// <summary>
        /// A plane which partitions global coordinate space into two. Plane points "backwards"
        /// along track to indicate what is in the checkpoint. With 'start' plane, creates
        /// a volume which is considered inside this checkpoint.
        /// </summary>
        public Plane planeEnd;
        /// <summary>
        /// Absolute distance marker indicating where the 'end' plane is for this checkpoint
        /// along the track. Ex: 3500m/8000m
        /// </summary>
        public float endDistance;
        /// <summary>
        /// Absolute distance marker indicating where the 'start' plane is for this checkpoint
        /// along the track. Ex: 3000m/8000m
        /// </summary>
        public float startDistance;
        /// <summary>
        /// How wide the track is. TODO: confirm if width is for start point (assumed) or end point.
        /// </summary>
        public float trackWidth;
        /// <summary>
        /// Whether or not checkpoint is physically connected to the last one. False when there is a
        /// gap between this checkpoint and the last one.
        /// </summary>
        public bool connectToTrackIn;
        /// <summary>
        /// 
        /// </summary>
        public bool connectToTrackOut;
        /// <summary>
        /// Always zero (Asserted in code).
        /// </summary>
        public ushort zero_0x4E;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        /// <summary>
        /// Returns the minimum Y component (height) of any of the checkpoints'
        /// two planes. Needed for other track metatdata elsewhere.
        /// </summary>
        /// <param name="checkpoints"></param>
        /// <returns></returns>
        public static float GetMinHeight(TrackCheckpoint[] checkpoints)
        {
            var min = float.PositiveInfinity;

            // iterate over every position, mo
            foreach (var checkpoint in checkpoints)
            {
                min = math.min(min, checkpoint.planeStart.position.y);
                min = math.min(min, checkpoint.planeEnd.position.y);
            }

            return min;
        }

        // track checkpoint matrix bounds
        public float GetMinPositionX()
        {
            return math.min(planeStart.position.x, planeEnd.position.x);
        }
        public float GetMinPositionZ()
        {
            return math.min(planeStart.position.z, planeEnd.position.z);

        }
        public float GetMaxPositionX()
        {
            return math.max(planeStart.position.x, planeEnd.position.x);
        }
        public float GetMaxPositionZ()
        {
            return math.max(planeStart.position.z, planeEnd.position.z);

        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref curveTimeStart);
                reader.ReadX(ref curveTimeEnd);
                reader.ReadX(ref planeStart, true);
                reader.ReadX(ref planeEnd, true);
                reader.ReadX(ref endDistance);
                reader.ReadX(ref startDistance);
                reader.ReadX(ref trackWidth);
                reader.ReadX(ref connectToTrackIn);
                reader.ReadX(ref connectToTrackOut);
                reader.ReadX(ref zero_0x4E);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero_0x4E == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(curveTimeStart);
                writer.WriteX(curveTimeEnd);
                writer.WriteX(planeStart);
                writer.WriteX(planeEnd);
                writer.WriteX(endDistance);
                writer.WriteX(startDistance);
                writer.WriteX(trackWidth);
                writer.WriteX(connectToTrackIn);
                writer.WriteX(connectToTrackOut);
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
                $"{nameof(planeStart)}.{nameof(planeStart.dotProduct)}: {planeStart.dotProduct:0.0}, " +
                $"{nameof(planeStart)}.{nameof(planeStart.direction)}(x:{planeStart.direction.x:0.0}, y:{planeStart.direction.y:0.0}, z:{planeStart.direction.z:0.0}), " +
                $"{nameof(planeStart)}.{nameof(planeStart.position)}(x:{planeStart.position.x:0.0}, y:{planeStart.position.y:0.0}, z:{planeStart.position.z:0.0}), " +
                $"{nameof(planeEnd)}.{nameof(planeEnd.dotProduct)}: {planeEnd.dotProduct:0.0}, " +
                $"{nameof(planeEnd)}.{nameof(planeEnd.direction)}(x:{planeEnd.direction.x:0.0}, y:{planeEnd.direction.y:0.0}, z:{planeEnd.direction.z:0.0}), " +
                $"{nameof(planeEnd)}.{nameof(planeEnd.position)}(x:{planeEnd.position.x:0.0}, y:{planeEnd.position.y:0.0}, z:{planeEnd.position.z:0.0}), " +
                $"{nameof(startDistance)}: {startDistance:0.0}, " +
                $"{nameof(endDistance)}: {endDistance:0.0}, " +
                $"{nameof(trackWidth)}: {trackWidth:0.0}, " +
                $"{nameof(connectToTrackIn)}: {connectToTrackIn}, " +
                $"{nameof(connectToTrackOut)}: {connectToTrackOut}" +
                $")";
        }

    }
}
