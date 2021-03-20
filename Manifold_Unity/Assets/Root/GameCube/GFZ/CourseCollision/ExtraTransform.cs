using Manifold.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ExtraTransform : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;

        [Space]
        public float unk_0x00;
        public float unk_0x04;
        public float unk_0x08;
        public float unk_0x0C;
        public float unk_0x10;
        public float unk_0x14;
        public float unk_0x18;
        public float unk_0x1C;
        public float unk_0x20;
        public float unk_0x24;
        public float unk_0x28;
        public float unk_0x2C;
        public float unk_0x30;
        public float unk_0x34;

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
                reader.ReadX(ref unk_0x10);
                reader.ReadX(ref unk_0x14);
                reader.ReadX(ref unk_0x18);
                reader.ReadX(ref unk_0x1C);
                reader.ReadX(ref unk_0x20);
                reader.ReadX(ref unk_0x24);
                reader.ReadX(ref unk_0x28);
                reader.ReadX(ref unk_0x2C);
                reader.ReadX(ref unk_0x30);
                reader.ReadX(ref unk_0x34);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            //writer.WriteX();
            throw new NotImplementedException();
        }


        #endregion

    }
}