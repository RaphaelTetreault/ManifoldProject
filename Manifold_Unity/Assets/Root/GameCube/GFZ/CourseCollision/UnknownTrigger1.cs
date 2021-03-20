using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class UnknownTrigger1 : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public SceneTransform transform;
        public EnumFlags32 unk_0x20;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transform, true);
                reader.ReadX(ref unk_0x20);
            }
            this.RecordEndAddress(reader);
            {
                // Volumes used are 10x10x10
                // Since we use a 1x1x1 cube, multiply x10
                transform.Scale *= 10f;
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            // scale /= 10f;

            throw new NotImplementedException();
        }

    }
}
