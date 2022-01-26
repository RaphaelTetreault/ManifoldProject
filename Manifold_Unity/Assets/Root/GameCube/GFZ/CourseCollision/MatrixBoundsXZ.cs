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
    //
    // Notes from the future: looks like it's using (more or less) index +-2 cells in X/Z
    // direction. There appears to be a min include sive. Though, I think what you generate
    // will work using this.

    /// <summary>
    /// Defines an absolute, untransformed bounds along the X and Z axes. This is used as metadata to know
    /// the dimensions of the course as viewed from a top-down view. It is used to deconstruct the racing 
    /// checkpoint matrix (8x8 overlapping grid inside bounds) and the triangle and quad collision matrix 
    /// (16x16 overlapping grid inside bounds).
    /// </summary>
    [Serializable]
    public class MatrixBoundsXZ :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        //[UnityEngine.SerializeField] private float3 center;

        // FIELDS
        /// <summary>
        /// Bounds -X. Denotes left-most edge of bounds. Negative in GX space.
        /// </summary>
        public float left;
        /// <summary>
        /// Bounds -Z. Denotes top-most edge of bounds. Negative in GX space.
        /// </summary>
        public float top;
        /// <summary>
        /// X-axis. Width of subdivision cell.
        /// </summary>
        public float subdivisionWidth;
        /// <summary>
        /// Z-axis. Length of subdivision cell.
        /// </summary>
        public float subdivisionLength;
        /// <summary>
        /// Number of subdivisions along x-axis. 8 for course checkpoints, 16 for static collider geo.
        /// </summary>
        public int numSubdivisionsX;
        /// <summary>
        /// Number of subdivisions along z-axis. 8 for course checkpoints, 16 for static collider geo.
        /// </summary>
        public int numSubdivisionsZ;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public (float2 center, float2 scale) GetCenterAndScale()
        {
            // Calculate full scale.
            var fullWidth = subdivisionWidth * numSubdivisionsX;
            var fullLength = subdivisionLength * numSubdivisionsZ;
            float2 scale = new float2(fullWidth, fullLength);
            // Compute corner/origin of bounds
            float2 corner = new float2(left, top);
            // Center is halfway from the corner, hence /2f
            float2 center = corner + scale / 2f;

            return (center, scale);
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref left);
                reader.ReadX(ref top);
                reader.ReadX(ref subdivisionWidth);
                reader.ReadX(ref subdivisionLength);
                reader.ReadX(ref numSubdivisionsX);
                reader.ReadX(ref numSubdivisionsZ);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(left);
                writer.WriteX(top);
                writer.WriteX(subdivisionWidth);
                writer.WriteX(subdivisionLength);
                writer.WriteX(numSubdivisionsX);
                writer.WriteX(numSubdivisionsZ);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return 
                $"{nameof(MatrixBoundsXZ)}(" +
                $"{nameof(top)}: {top}, " +
                $"{nameof(left)}: {left}, " +
                $"{nameof(subdivisionWidth)}: {subdivisionWidth}, " +
                $"{nameof(subdivisionLength)}: {subdivisionLength}, " +
                $"{nameof(numSubdivisionsX)}: {numSubdivisionsX}, " +
                $"{nameof(numSubdivisionsZ)}: {numSubdivisionsZ}" +
                $")";
        }

    }
}
