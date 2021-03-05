using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    public class TrackReferencesTable : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        public const int kNumEntries = 64;

        [SerializeField]
        private AddressRange addressRange;

        public int[] trackReferencesAbsPtrs = new int[0];
        public TrackReference[] trackReferences = new TrackReference[0];


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
                reader.ReadX(ref trackReferencesAbsPtrs, kNumEntries);
            }
            this.RecordEndAddress(reader);
            {
                trackReferences = new TrackReference[64];
                for (int i = 0; i < trackReferencesAbsPtrs.Length; i++)
                {
                    var absPtr = trackReferencesAbsPtrs[i];
                    var isValidPointer = absPtr > 0;
                    if (isValidPointer)
                    {
                        reader.BaseStream.Seek(absPtr, SeekOrigin.Begin);
                        reader.ReadX(ref trackReferences[i], true);
                    }
                    else
                    {
                        trackReferences[i] = new TrackReference();
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
