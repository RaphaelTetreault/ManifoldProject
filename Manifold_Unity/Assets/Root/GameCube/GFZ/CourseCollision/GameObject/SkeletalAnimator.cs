using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class SkeletalAnimator : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        public uint zero_0x00;
        public uint zero_0x04;
        public uint one_0x08; // Always 1. Bool?
        public uint unkRelPtr;

        public SkeletalProperties properties;


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
                reader.ReadX(ref zero_0x00);
                reader.ReadX(ref zero_0x04);
                reader.ReadX(ref one_0x08);
                reader.ReadX(ref unkRelPtr);
            }
            this.RecordEndAddress(reader);
            {
                if (unkRelPtr != 0)
                {
                    reader.BaseStream.Seek(unkRelPtr, SeekOrigin.Begin);
                    reader.ReadX(ref properties, true);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(zero_0x00);
            writer.WriteX(zero_0x04);
            writer.WriteX(one_0x08);
            writer.WriteX(unkRelPtr);

            throw new NotImplementedException();
        }


        #endregion

    }
}