using Manifold.IO;
using System.IO;
using System.Collections.Generic;

namespace GameCube.GFZ.CourseCollision
{
    [System.Serializable]
    public class IndexList :
        IBinarySeralizableReference
    {
        // CONSTANTS
        public const int kUshortArrayTerminator = 0xFFFF;

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        [UnityEngine.SerializeField] private ushort[] indexes = new ushort[0];



        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public ushort[] Indexes
        {
            get => indexes;
            set => indexes = value;
        }

        public int Length => indexes.Length;

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            var list = new List<ushort>();
            while (true)
            {
                // Read next value
                var value = reader.ReadX_UInt16();
                if (value == kUshortArrayTerminator)
                {
                    break;
                }
                list.Add(value);
            }

            indexes = list.ToArray();
        }

        public void Serialize(BinaryWriter writer)
        {
            foreach (var index in indexes)
            {
                writer.WriteX(index);
            }
            writer.WriteX(kUshortArrayTerminator);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            this.RecordStartAddress(writer.BaseStream);
            Serialize(writer);
            this.RecordEndAddress(writer.BaseStream);
            return addressRange;
        }

    }
}
