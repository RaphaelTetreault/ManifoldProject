using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class UnknownSceneObjectData : IBinarySerializable, IBinaryAddressableRange
    {

        #region FIELDS


        const int kCount = 12;

        [SerializeField]
        private AddressRange addressRange;

        public uint[] unkAbsPtr;

        public UnknownSceneObjectFloatPair[] unk;


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
                reader.ReadX(ref unkAbsPtr, kCount);
            }
            this.RecordEndAddress(reader); ;
            {
                unk = new UnknownSceneObjectFloatPair[kCount];
                for (int i = 0; i < kCount; i++)
                {
                    unk[i] = new UnknownSceneObjectFloatPair();

                    if (unkAbsPtr[i] != 0)
                    {
                        var ptr = unkAbsPtr[i];
                        reader.BaseStream.Seek(ptr, SeekOrigin.Begin);
                        reader.ReadX(ref unk[i], false);
                    }
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unkAbsPtr, false);
        }


        #endregion

    }
}