using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public struct TrackLength :
        IBinaryAddressableRange,
        IBinarySerializable,
        IBinarySeralizableReference
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public float trackLength;


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
                reader.ReadX(ref trackLength);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            if (ColiCourseUtility.SerializeVerbose)
            {
                var message = $"{nameof(TrackLength)} is a long string for whatestarngereasonyouknow";
                writer.WriteBinaryComment(message);
            }

            writer.WriteX(trackLength);
        }

        public AddressRange SerializeReference(BinaryWriter writer)
        {
            var addressRange = new AddressRange();
            addressRange.RecordStartAddress(writer.BaseStream);
            {
                Serialize(writer);
            }
            addressRange.RecordEndAddress(writer.BaseStream);
            return addressRange;
        }
    }
}