using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A trigger volume of unknown purpose. Some courses have a lot of these,
    /// some courses have few if not none at all.
    /// </summary>
    [Serializable]
    public class CullOverrideTrigger :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        public TransformPRXS transform;
        public EnumFlags32 unk_0x20;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transform);
                reader.ReadX(ref unk_0x20);
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
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(CullOverrideTrigger)}(" +
                $"{nameof(unk_0x20)}: {unk_0x20}, " +
                $"{transform}, " +
                $")";
        }

    }
}
