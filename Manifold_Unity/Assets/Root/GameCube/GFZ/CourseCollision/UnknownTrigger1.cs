using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnknownTrigger1 :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public Transform transform;
        public EnumFlags16 unk_0x20;
        public EnumFlags16 unk_0x22;


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
                reader.ReadX(ref transform, true);
                reader.ReadX(ref unk_0x20);
                reader.ReadX(ref unk_0x22);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            // Old notes:
            // scale /= 10f;
            // not sure if still valid for /this/ trigger class

            this.RecordStartAddress(writer);
            {
               writer.WriteX(transform);
               writer.WriteX(unk_0x20);
               writer.WriteX(unk_0x22);
            }
            this.RecordEndAddress(writer);
        }

    }
}
