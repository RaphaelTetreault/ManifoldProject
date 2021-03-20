using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class UnknownObjectAttributes2 : IBinarySerializable, IBinaryAddressableRange
    {
        [SerializeField]
        private AddressRange addressRange;

        public Pointer collisionObjectReferencePtr;

        public UnknownObjectAttributes collisionObjectReference;


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref collisionObjectReferencePtr);
            }
            this.RecordEndAddress(reader);
            {
                reader.JumpToAddress(collisionObjectReferencePtr);
                reader.ReadX(ref collisionObjectReference, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}