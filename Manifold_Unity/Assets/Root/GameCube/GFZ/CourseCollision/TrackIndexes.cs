using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    public class TrackIndexes : IBinarySerializable, IBinaryAddressableRange
    {
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
                indexes = ColiCourseUtility.ReadUshortArray(reader);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }
}
