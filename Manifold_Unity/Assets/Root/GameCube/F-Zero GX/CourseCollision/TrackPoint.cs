using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZX01.CourseCollision
{
    [Serializable]
    public class TrackPoint : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public float unk_0x00;
        public float unk_0x04;
        // Make Struct?
        public float trackDistanceStart;
        public Vector3 tangentStart;
        public Vector3 positionStart;
        // Make Struct?
        public float trackDistanceEnd;
        public Vector3 tangentEnd;
        public Vector3 positionEnd;
        //
        public float transformDistanceEnd;
        public float transformDistanceStart;
        public float trackWidth;
        public bool isTrackContinuousStart;
        public bool isTrackContinuousEnd;
        public ushort zero_0x4E;

        #endregion

        #region PROPERTIES

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

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

            endAddress = reader.BaseStream.Position;
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

        #endregion

    }
}
