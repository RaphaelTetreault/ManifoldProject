using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ColiUnknownStruct3 : IBinarySerializable, IBinaryAddressableRange
    {
        public const int elementCount = 6;

        [SerializeField]
        private AddressRange addressRange;

        public ArrayPointer[] arrayPointers;

        public ColiUnknownStruct4[][] coliUnknownStruct4s = new ColiUnknownStruct4[elementCount][];


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref arrayPointers, elementCount, true);
            }
            this.RecordEndAddress(reader);
            {
                for (int i = 0; i < arrayPointers.Length; i++)
                {
                    var arrayPointer = arrayPointers[i];
                    reader.JumpToAddress(arrayPointer);
                    reader.ReadX(ref coliUnknownStruct4s[i], arrayPointer.length, true);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
