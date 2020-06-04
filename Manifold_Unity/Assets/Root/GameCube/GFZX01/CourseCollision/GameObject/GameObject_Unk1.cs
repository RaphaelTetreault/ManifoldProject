using StarkTools.IO;
using System;
using System.IO;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace GameCube.GFZX01.CourseCollision
{
    [Serializable]
    public class ObjectTable_Unk1 : IBinarySerializable, IBinaryAddressable
    {
        const int kCount = 12;


        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public uint[] unkAbsPtr;

        public ObjectTable_Unk1_Entry[] unk;

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
                reader.ReadX(ref unkAbsPtr, kCount);
            }
            endAddress = reader.BaseStream.Position;
            {
                unk = new ObjectTable_Unk1_Entry[kCount];
                for (int i = 0; i < kCount; i++)
                {
                    unk[i] = new ObjectTable_Unk1_Entry();

                    if (unkAbsPtr[i] != 0)
                    {
                        var absPtr = unkAbsPtr[i];
                        reader.BaseStream.Seek(absPtr, SeekOrigin.Begin);
                        reader.ReadX(ref unk[i], false);
                    }
                }
            }
            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unkAbsPtr, false);
        }

        #endregion

    }
}