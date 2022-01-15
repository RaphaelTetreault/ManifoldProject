﻿using System;
using System.IO;
using Manifold.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 2022-01-14: this is currently a copy of BoundsXZ but without the subdivision data.
    /// This is probably a struct inside that one, likely with the same BS *10 and whatever.
    /// </summary>
    [Serializable]
    public class Bounds2D :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        [UnityEngine.SerializeField] private float3 center;

        // FIELDS
        public float maxX; // bounds -x. Value = max x value of track node. Negative in GX space.
        public float maxZ; // bounds -z. Value = max z value of track node. Negative in GX space.
        // NOTE: prior to adding 25% to W/L, the "box" lines up to the -x/-z edge and is thus properly centered.
        public float width; // x axis. Width between min/max tracknodes.pos.x * 10f * 1.25f (+25%)
        public float length; // z axis. Length between min/max tracknodes.pos.z * 10f * 1.25f (+25%)


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public float3 Center => center;


        // METHODS
        public float3 CalcCenter()
        {
            // center of width/length
            float3 halfWL = new float3(width, 0f, length) / 2f;
            float3 edge = new float3(maxX, 0f, maxZ);
            float3 center = halfWL + edge;
            return center;
        }

        public float3 Scale
        {
            get
            {
                return new float3(width, 0f, length);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref maxX);
                reader.ReadX(ref maxZ);
                reader.ReadX(ref width);
                reader.ReadX(ref length);
            }
            this.RecordEndAddress(reader);
            {
                center = CalcCenter();
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(maxX);
                writer.WriteX(maxZ);
                writer.WriteX(width);
                writer.WriteX(length);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(Bounds2D)}(" +
                $"{nameof(maxX)}: {maxX}, " +
                $"{nameof(maxZ)}: {maxZ}, " +
                $"{nameof(width)}: {width}, " +
                $"{nameof(length)}: {length})";
        }

    }
}
