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


        // metadata
        [SerializeField]
        private AddressRange addressRange;

        // structure
        //public int branchCount; // is this array ptr length??
        public ArrayPointer pointsPtr;
        //public Pointer pointPtr;
        public Pointer transformPtr;
        //
        //public TrackPoint point;
        public TrackPoint[] points;
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
                //reader.ReadX(ref branchCount);
                //reader.ReadX(ref pointPtr);
                reader.ReadX(ref pointsPtr);
                reader.ReadX(ref transformPtr);
            }
            this.RecordEndAddress(reader);
            {
                // Get point
                //reader.JumpToAddress(pointPtr);
                //reader.ReadX(ref point, true);
                reader.JumpToAddress(pointsPtr);
                reader.ReadX(ref points, pointsPtr.length, true);

                // Get transform
                // NOTE: since this data is referenced many times, I instead
                // build a list in ColiScene.
                // TODO: link the references? Does this work in Unity when
                // serialized?
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            //writer.WriteX(branchCount);
            //writer.WriteX(pointPtr);
            //writer.WriteX(transformPtr);

            throw new NotImplementedException();
        }


        #endregion

    }
}
