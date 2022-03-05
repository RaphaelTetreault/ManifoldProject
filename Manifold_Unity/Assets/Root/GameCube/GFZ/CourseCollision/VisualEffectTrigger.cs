using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// A volume which triggers some visial effect.
    /// NOTE: assumed mesh scale for trigger is 10xyz
    /// </summary>
    [Serializable]
    public class VisualEffectTrigger :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public TransformPRXS transform;
        public TriggerableAnimation animation;
        public TriggerableVisualEffect visualEffect;


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
                reader.ReadX(ref transform);
                reader.ReadX(ref animation);
                reader.ReadX(ref visualEffect);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(transform);
                writer.WriteX(animation);
                writer.WriteX(visualEffect);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(VisualEffectTrigger)}(" +
                $"{nameof(animation)}: {animation}, " +
                $"{nameof(visualEffect)}: {visualEffect}, " +
                $"{transform}, " +
                $")";
        }

    }
}
