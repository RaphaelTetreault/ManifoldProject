using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class TrackNode : IBinarySerializable, IBinaryAddressableRange
    {

        // metadata
        [SerializeField]
        private AddressRange addressRange;

        // structure
        public ArrayPointer pointsPtr;
        public Pointer transformPtr;
        //
        public TrackPoint[] points;
        public TrackTransform transform;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref pointsPtr);
                reader.ReadX(ref transformPtr);
            }
            this.RecordEndAddress(reader);
            {
                // Get point
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
            throw new NotImplementedException();
        }

    }
}
