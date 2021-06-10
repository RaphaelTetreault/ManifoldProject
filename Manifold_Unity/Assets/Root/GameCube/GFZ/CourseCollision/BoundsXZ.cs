using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct BoundsXZ :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] AddressRange addressRange;

        // FIELDS
        public float maxX; // bounds -x. Value = max x value of track node. Negative in GX space.
        public float maxZ; // bounds -z. Value = max z value of track node. Negative in GX space.
        public float width; // x axis. Width between min/max tracknodes.x * 1.25f
        public float length; // z axis. Length between min/max tracknodes.z * 1.25f
        public BoundsOption xAxis; // educated guess
        public BoundsOption zAxis; // educated guess


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
                reader.ReadX(ref maxX);
                reader.ReadX(ref maxZ);
                reader.ReadX(ref width);
                reader.ReadX(ref length);
                reader.ReadX(ref xAxis);
                reader.ReadX(ref zAxis);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(maxX);
                writer.WriteX(maxZ);
                writer.WriteX(width);
                writer.WriteX(length);
                writer.WriteX(xAxis);
                writer.WriteX(zAxis);
            }
            this.RecordEndAddress(writer);
        }

    }
}
