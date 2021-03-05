using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ColiUnknownStruct5 : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public int unk_0x00; // 0x##000000 - looks like highest byte flags? Big endian?
        public float unk_0x04; // Smaller numbers, kind of like first 2 values in header (could be 2-float struct?)
        public float unk_0x08; // Large numbers, kind of like first 2 values in header (could be 2-float struct?)
        public Vector3 unk_0x0C; // Looks like scale? Maybe not a vector.
        public Vector3 unk_0x18; // Always (0, 0, 0)? Maybe not a vector.


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x18);
            }
            this.RecordEndAddress(reader);
            {
                Debug.Log($"{unk_0x00:X8}, {unk_0x04}, {unk_0x08}, {unk_0x0C}, {unk_0x18}");
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
