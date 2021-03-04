using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class TrackNode : IBinarySerializable, IBinaryAddressable
    {
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public int trackBranchCount;
        public int trackPointAbsPtr;
        public int trackTransformAbsPtr;

        public TrackPoint point;
        public TrackTransform transform;


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

            reader.ReadX(ref trackBranchCount);
            reader.ReadX(ref trackPointAbsPtr);
            reader.ReadX(ref trackTransformAbsPtr);

            endAddress = reader.BaseStream.Position;

            // Get point
            reader.BaseStream.Seek(trackPointAbsPtr, SeekOrigin.Begin);
            reader.ReadX(ref point, true);

            // Get transform
            //reader.BaseStream.Seek(trackTransformAbsPtr, SeekOrigin.Begin);
            //reader.ReadX(ref transform, true);

            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(trackBranchCount);
            writer.WriteX(trackPointAbsPtr);
            writer.WriteX(trackTransformAbsPtr);
        }

        #endregion

    }
}
