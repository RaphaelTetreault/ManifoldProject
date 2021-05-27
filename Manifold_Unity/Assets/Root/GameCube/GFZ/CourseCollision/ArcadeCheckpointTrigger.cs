using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class ArcadeCheckpointTrigger : IBinarySerializable, IBinaryAddressableRange
    {
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        public Transform transform;
        public ArcadeCheckpointType type;

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
                reader.ReadX(ref type);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
