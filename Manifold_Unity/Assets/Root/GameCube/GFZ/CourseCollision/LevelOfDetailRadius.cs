using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Manifold.IO;

namespace GameCube.GFZ.CourseCollision
{
    [System.Serializable]
    public struct LevelOfDetailRadius : IBinarySerializable
    {
        public float radius;
        private int radiusSquared;

        public int RadiusSquared
        {
            get
            {
                return radiusSquared;
            }

            private set
            {
                radiusSquared = value;
                radius = Mathf.Sqrt(radiusSquared);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref radiusSquared);
            RadiusSquared = radiusSquared;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(radiusSquared);
        }

    }
}
