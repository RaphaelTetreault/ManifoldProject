using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class AnimationCurve : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        public uint unk_0x00;
        public uint unk_0x04;
        public uint unk_0x08;
        public uint unk_0x0C;
        public int keyableCount;
        public uint keyableAbsPtr;

        public KeyableAttribute[] keyableAttributes;


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
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref keyableCount);
                reader.ReadX(ref keyableAbsPtr);
            }
            this.RecordEndAddress(reader);
            {
                keyableAttributes = new KeyableAttribute[keyableCount];
                if (keyableCount > 0)
                {
                    reader.BaseStream.Seek(keyableAbsPtr, SeekOrigin.Begin);
                    reader.ReadX(ref keyableAttributes, keyableCount, true);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(unk_0x08);
            writer.WriteX(unk_0x0C);
            writer.WriteX(keyableCount);
            writer.WriteX(keyableAbsPtr);
        }


        #endregion

    }
}