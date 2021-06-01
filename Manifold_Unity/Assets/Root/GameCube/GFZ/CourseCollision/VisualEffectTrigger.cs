using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// NOTE: assumed mesh scale for trigger is 10xyz
    /// </summary>
    [Serializable]
    public class VisualEffectTrigger : 
        IBinarySeralizableReference
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public Transform transform;
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
                reader.ReadX(ref transform, true);
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

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            Serialize(writer);
            return addressRange;
        }
    }
}
