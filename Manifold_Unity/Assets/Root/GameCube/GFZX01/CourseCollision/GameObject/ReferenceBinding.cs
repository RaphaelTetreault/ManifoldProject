using StarkTools.IO;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace GameCube.GFZX01.CourseCollision
{
    [Serializable]
    public class ReferenceBinding : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public uint unk_0x00;
        public uint nameAbsPtr;
        public uint unk_0x08;
        public float unk_0x0C;

        public string name;

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
            startAddress = reader.BaseStream.Position;
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref nameAbsPtr);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
            }
            endAddress = reader.BaseStream.Position;
            {
                reader.BaseStream.Seek(nameAbsPtr, SeekOrigin.Begin);
                reader.ReadXCString(ref name, Encoding.ASCII);
            }
            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
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