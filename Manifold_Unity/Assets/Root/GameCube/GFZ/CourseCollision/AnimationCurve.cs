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
        public ArrayPointer keyableAttributesPtr;

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
                reader.ReadX(ref keyableAttributesPtr);
            }
            this.RecordEndAddress(reader);
            {
                if (keyableAttributesPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(keyableAttributesPtr);
                    reader.ReadX(ref keyableAttributes, keyableAttributesPtr.Length, true);
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
            writer.WriteX(keyableAttributesPtr);

            // Array pointer address and length needs to be set
            throw new NotImplementedException();
        }


        #endregion

    }
}