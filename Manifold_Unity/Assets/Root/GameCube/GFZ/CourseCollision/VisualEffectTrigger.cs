using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class VisualEffectTrigger : IBinarySerializable, IBinaryAddressableRange
    {
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        public Transform transform;
        public TriggerableAnimation animation;
        public TriggerableVisualEffect visualEffect;

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
                reader.ReadX(ref animation);
                reader.ReadX(ref visualEffect);
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
            throw new NotImplementedException();
        }

    }
}
