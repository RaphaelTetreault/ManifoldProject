using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    public class TrackIndexesTable : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        public const int kNumEntries = 64;

        [SerializeField]
        private AddressRange addressRange;

        public Pointer[] trackIndexesAbsPtrs = new Pointer[0];
        public TrackIndexes[] trackIndexArray = new TrackIndexes[0];


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
                reader.ReadX(ref trackIndexesAbsPtrs, kNumEntries, true);
            }
            this.RecordEndAddress(reader);
            {
                trackIndexArray = new TrackIndexes[64];
                for (int i = 0; i < trackIndexesAbsPtrs.Length; i++)
                {
                    var pointer = trackIndexesAbsPtrs[i];
                    if (pointer.IsNotNullPointer)
                    {
                        reader.JumpToAddress(pointer);
                        reader.ReadX(ref trackIndexArray[i], true);
                    }
                    else
                    {
                        trackIndexArray[i] = new TrackIndexes();
                    }
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }


        #endregion

    }
}
