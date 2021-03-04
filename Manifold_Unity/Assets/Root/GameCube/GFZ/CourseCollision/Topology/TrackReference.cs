using Manifold.IO;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    public class TrackReference : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        public const int kNumEntries = 64;

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public ushort[] references = new ushort[0];

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
            this.RecordStartAddress(reader);
            references = ColiCourseUtility.ReadUShortArray(reader);
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}
