using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Only available in Sand Ocean Lateral Shift
    /// GX: 6 instances, AX: 9 instances
    /// </summary>
    [Serializable]
    public struct UnknownTrigger2 : IBinarySerializable
    {
        public int unk_0x00;
        public Vector3 position;
        public ShortRotation3 shortRotation3;
        public Vector3 scale;

        public Quaternion Rotation => shortRotation3.AsQuaternion;
        public Vector3 RotationEuler => shortRotation3.AsVector3;

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref position);
            reader.ReadX(ref shortRotation3, true);
            reader.ReadX(ref scale);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(position);
            writer.WriteX(shortRotation3);
            writer.WriteX(scale);
        }
    }
}
