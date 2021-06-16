using Manifold.IO;
using System;
using System.IO;

// TODO: test by creating custom triggers, moving them onto the track.

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Only available in Sand Ocean Lateral Shift
    /// GX: 6 instances, AX: 9 instances
    /// </summary>
    [Serializable]
    public class UnknownSolsTrigger :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public int unk_0x00;
        public Transform transform;


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
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref transform, true);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(transform);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(UnknownSolsTrigger)}(" +
                $"{transform}" +
                $")";
        }

    }
}
