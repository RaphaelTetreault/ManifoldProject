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

        public ColiUnknownStruct5Enum unk_0x00; // 0x##000000 - looks like highest byte flags? Big endian?
        public UnknownFloatPair unk_0x04;
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
                reader.ReadX(ref unk_0x04, true);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref unk_0x18);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(unk_0x18 == Vector3.zero);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
