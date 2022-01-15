using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    // NOTE: OVERLAP GENERATION
    // I think the math to compute the overlapping grid is something like:
    // 1) Take total subdivisions along axis, subtract 1 from each axis
    // 2) Create grid: (n-1 by n-1). Ex: 8x8 subdivisions = 7x7 grid.
    // 3) For each cell, use elements that fall inside grid at index and index+1
    //      Example: cell 0,0 uses subdivision (0,0), (0,1), (1,0), and (1,1)
    // This effectively creates cells with some overlap between each other
    // This concept could be further enhanced by creating more subdivisions and overlapping
    // a small amount of cells. This is ideal in reducing reference counts.

    /// <summary>
    /// Defines an absolute, untransformed bounds along the X and Z axes. This is used as metadata to know
    /// the dimensions of the course as viewed from a top-down view. It is used to deconstruct the racing 
    /// checkpoint matrix (8x8 overlapping grid inside bounds) and the triangle and quad collision matrix 
    /// (16x16 overlapping grid inside bounds).
    /// </summary>
    [Serializable]
    public class BoundsXZ :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        //[UnityEngine.SerializeField] private float3 center;

        // FIELDS
        //public float maxX; // bounds -x. Value = max x value of track node. Negative in GX space.
        //public float maxZ; // bounds -z. Value = max z value of track node. Negative in GX space.
        //// NOTE: prior to adding 25% to W/L, the "box" lines up to the -x/-z edge and is thus properly centered.
        //public float width; // x axis. Width between min/max tracknodes.pos.x * 10f * 1.25f (+25%)
        //public float length; // z axis. Length between min/max tracknodes.pos.z * 10f * 1.25f (+25%)
        public Bounds2D bounds2D;
        public int subdivisionsX; // number of subdivisions along x-axis. 8 for course checkpoints, 16 for collider geo.
        public int subdivisionsZ; // number of subdivisions along z-axis. 8 for course checkpoints, 16 for collider geo.


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        //public float3 Center => center;


        //// METHODS
        //public float3 CalcCenter()
        //{
        //    // center of width/length
        //    float3 halfWL = new float3(width, 0f, length) / 2f;
        //    float3 edge = new float3(maxX, 0f, maxZ);
        //    float3 center = halfWL + edge;
        //    return center;
        //}

        //public float3 Scale
        //{
        //    get
        //    {
        //        return new float3(width, 0f, length);
        //    }
        //}

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                //reader.ReadX(ref maxX);
                //reader.ReadX(ref maxZ);
                //reader.ReadX(ref width);
                //reader.ReadX(ref length);
                reader.ReadX(ref bounds2D, true);
                reader.ReadX(ref subdivisionsX);
                reader.ReadX(ref subdivisionsZ);
            }
            this.RecordEndAddress(reader);
            //{
            //    center = CalcCenter();
            //}
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                //writer.WriteX(maxX);
                //writer.WriteX(maxZ);
                //writer.WriteX(width);
                //writer.WriteX(length);
                writer.WriteX(bounds2D);
                writer.WriteX(subdivisionsX);
                writer.WriteX(subdivisionsZ);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return 
                $"{nameof(BoundsXZ)}(" +
                $"{nameof(bounds2D)}: {bounds2D}, " +
                $"{nameof(subdivisionsX)}: {subdivisionsX}, " +
                $"{nameof(subdivisionsZ)}: {subdivisionsZ}" +
                $")";
        }

    }
}
