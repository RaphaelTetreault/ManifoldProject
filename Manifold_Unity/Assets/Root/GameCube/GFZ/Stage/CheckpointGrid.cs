using System.Collections.Generic;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    // 2022/03/09: formerly TrackCheckpointGrid

    /// <summary>
    /// A table for index lists specifically for track checkpoints.
    /// </summary>
    [System.Serializable]
    public sealed class CheckpointGrid : IndexGrid
    {
        // CONSTANTS
        public const int Subdivisions = 8;
        public const int kListCount = Subdivisions * Subdivisions;
        
        // PROPERTIES
        public override int SubdivisionsX => Subdivisions;
        public override int SubdivisionsZ => Subdivisions;


        // STATIC METHODS
        public static GridXZ GetMatrixBoundsXZ(Checkpoint[] checkpoints)
        {
            // Get min and max XZ values of any checkpoint
            float3 min = new float3(float.MaxValue, 0, float.MaxValue);
            float3 max = new float3(float.MinValue, 0, float.MinValue);

            foreach (var checkpoint in checkpoints)
            {
                // MIN
                min.x = math.min(min.x, checkpoint.GetMinPositionX());
                min.z = math.min(min.z, checkpoint.GetMinPositionZ());

                // MAX
                max.x = math.max(max.x, checkpoint.GetMaxPositionX());
                max.z = math.max(max.z, checkpoint.GetMaxPositionZ());
            }

            // Compute bounds
            var bounds = new GridXZ();
            bounds.NumSubdivisionsX = Subdivisions;
            bounds.NumSubdivisionsZ = Subdivisions;
            bounds.Left = min.x;
            bounds.Top = max.z;
            bounds.SubdivisionWidth = (max.x - min.x) / Subdivisions; // delta / subdivisions
            bounds.SubdivisionLength = (max.z - min.z) / Subdivisions; // delta / subdivisions

            return bounds;
        }

        // METHODS
        public void GenerateIndexes(GridXZ matrixBoundsXZ, Checkpoint[] checkpoints)
        {
            // Init. Value is from inherited structure.
            IndexLists = new IndexList[kListCount];

            // so if track has no width, we still pick up some points
            //var widthX = math.max(matrixBoundsXZ.subdivisionWidth, 1f);
            //var lengthZ = math.max(matrixBoundsXZ.subdivisionLength, 1f);
            var widthX = matrixBoundsXZ.SubdivisionWidth;
            var lengthZ = matrixBoundsXZ.SubdivisionLength;

            // Condition where theere is no w/l and so no checkpoints are added
            var hasNoWidthOrHeight = widthX == 0 || lengthZ == 0;
            if (hasNoWidthOrHeight)
            {
                var list = new List<int>();
                for (int i = 0; i < checkpoints.Length; i++)
                    list.Add(i);

                for (int i = 0; i < IndexLists.Length; i++)
                    IndexLists[i] = IndexList.CreateIndexList(list);

                return;
            }

            // Iterate over each subdivision in the course
            for (int z = 0; z < SubdivisionsZ; z++)
            {
                // Get the minimum and maximum Z coordinates allowed to exist this cell
                var minZIndex = math.clamp(z - 2, 0, SubdivisionsZ - 1);
                var maxZIndex = math.clamp(z + 2, 0, SubdivisionsZ - 1);
                var minZ = matrixBoundsXZ.Top - (lengthZ * minZIndex);
                var maxZ = matrixBoundsXZ.Top - (lengthZ * maxZIndex);

                for (int x = 0; x < SubdivisionsX; x++)
                {
                    // Get the minimum and maximum X coordinates allowed to exist this cell
                    var minXIndex = math.clamp(x - 2, 0, SubdivisionsX - 1);
                    var maxXIndex = math.clamp(x + 2, 0, SubdivisionsX - 1);
                    var minX = matrixBoundsXZ.Left - (widthX * minXIndex);
                    var maxX = matrixBoundsXZ.Left - (widthX * maxXIndex);

                    // Iterate over every checkpoint the course has
                    var indexes = new List<int>();
                    for (int i = 0; i < checkpoints.Length; i++)
                    {
                        var checkpoint = checkpoints[i];

                        var posX = checkpoint.PlaneStart.origin.x;
                        var posZ = checkpoint.PlaneStart.origin.z;

                        bool isBetweenX = IsBetween(posX, minX, maxX);
                        bool isBetweenZ = IsBetween(posZ, minZ, maxZ);

                        // if the x and z coordinates are within the region we want, store index to checkpoint
                        bool isInRegion = isBetweenX && isBetweenZ;
                        if (isInRegion)
                        {
                            indexes.Add(i);
                        }
                    }

                    // Turn those indexes into the structure
                    var cell = z * SubdivisionsZ + x;
                    IndexLists[cell] = IndexList.CreateIndexList(indexes);
                }
            }
        }

        private bool IsBetween(float value, float min, float max)
        {
            bool isMoreThanMin = value >= min;
            bool isLessThanMax = value <= max;
            bool isBetween = isMoreThanMin && isLessThanMax;
            return isBetween;
        }

    }
}
