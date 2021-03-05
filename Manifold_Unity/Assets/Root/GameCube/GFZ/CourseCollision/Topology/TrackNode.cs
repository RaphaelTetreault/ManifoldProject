using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class TrackNode : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        public int trackBranchCount;
        public int trackPointAbsPtr;
        public int trackTransformAbsPtr;

        public TrackPoint point;
        public TrackTransform transform;


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref trackBranchCount);
                reader.ReadX(ref trackPointAbsPtr);
                reader.ReadX(ref trackTransformAbsPtr);
            }
            this.RecordEndAddress(reader);
            {
                // Get point
                reader.BaseStream.Seek(trackPointAbsPtr, SeekOrigin.Begin);
                reader.ReadX(ref point, true);

                // Get transform
                //reader.BaseStream.Seek(trackTransformAbsPtr, SeekOrigin.Begin);
                //reader.ReadX(ref transform, true);
            }
            this.SetReaderToEndAddress(reader);
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
