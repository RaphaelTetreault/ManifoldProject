using System;
using System.IO;

namespace Manifold.IO
{
    [Serializable]
    public struct ArrayPointer2D : IBinarySerializable
    {
        [UnityEngine.SerializeField]
        private ArrayPointer[] arrayPointers;

        public ArrayPointer[] ArrayPointers => arrayPointers;

        public ArrayPointer2D(int length = 0)
        {
            arrayPointers = new ArrayPointer[length];
        }

        public void Deserialize(BinaryReader reader)
        {
            // Read array lengths
            for (int i = 0; i < arrayPointers.Length; i++)
            {
                int length = 0;
                reader.ReadX(ref length);
                arrayPointers[i].Length = length;
            }

            // Read array addresses
            for (int i = 0; i < arrayPointers.Length; i++)
            {
                int address = 0;
                reader.ReadX(ref address);
                arrayPointers[i].Address = address;
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            // Write array lengths
            for (int i = 0; i < arrayPointers.Length; i++)
            {
                writer.WriteX(arrayPointers[i].Length);
            }

            // Write array addresses
            for (int i = 0; i < arrayPointers.Length; i++)
            {
                writer.WriteX(arrayPointers[i].Address);
            }
        }

    }
}
