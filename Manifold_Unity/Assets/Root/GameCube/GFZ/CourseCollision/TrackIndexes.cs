using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    public class TrackIndexes : IBinarySerializable, IBinaryAddressableRange
    {
        //public const int kNumEntries = 64;

        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        public ushort[] indexes = new ushort[0];

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                indexes = ColiCourseUtility.ReadUShortArray(reader);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }
}
