using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    public class TrackIndexes : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        public const int kNumEntries = 64;

        [SerializeField]
        private AddressRange addressRange;

        public ushort[] indexes = new ushort[0];


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
                indexes = ColiCourseUtility.ReadUShortArray(reader);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }


        #endregion

    }
}
