using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnknownSceneObjectData : IBinarySerializable, IBinaryAddressableRange
    {
        // CONSTANTS
        public const int kCount = 12;

        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        public uint[] unkAbsPtr;

        public UnknownSceneObjectFloatPair[] unk = new UnknownSceneObjectFloatPair[0];


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
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

    }
}