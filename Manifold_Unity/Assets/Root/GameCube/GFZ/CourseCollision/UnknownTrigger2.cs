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
        public SceneTransform transform;

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
    }
}
