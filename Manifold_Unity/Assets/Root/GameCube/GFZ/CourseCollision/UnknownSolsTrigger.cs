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
        IBinarySeralizableReference
    {
        // FIELDS
        public int unk_0x00;
        public Transform transform;

        public AddressRange AddressRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref transform, true);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(transform);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
