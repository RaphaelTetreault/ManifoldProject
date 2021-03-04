using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    public class TrackReferencesTable : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        public const int kNumEntries = 64;

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public int[] trackReferencesAbsPtrs = new int[0];
        public TrackReference[] trackReferences = new TrackReference[0];

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
            reader.ReadX(ref trackReferencesAbsPtrs, kNumEntries);
            this.RecordEndAddress(reader);

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

            // Reset
            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}
