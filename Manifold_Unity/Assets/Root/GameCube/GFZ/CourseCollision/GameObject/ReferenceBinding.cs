using Manifold.IO;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ReferenceBinding : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        [SerializeField]
        private AddressRange addressRange;
        public string name;

        public uint unk_0x00;
        public uint nameAbsPtr;
        public uint unk_0x08;
        public float unk_0x0C;


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
                reader.ReadX(ref nameAbsPtr);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
            }
            this.RecordEndAddress(reader);
            {
                reader.BaseStream.Seek(nameAbsPtr, SeekOrigin.Begin);
                reader.ReadXCString(ref name, Encoding.ASCII);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(nameAbsPtr);
            writer.WriteX(unk_0x08);
            writer.WriteX(unk_0x0C);

            // write ptr into name table
            throw new NotImplementedException();
        }


        #endregion

    }
}