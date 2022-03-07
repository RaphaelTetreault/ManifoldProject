using System.Collections.Generic;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A table for index lists specifically for track checkpoints.
    /// This class acts a wrapper to make serialization easy (in Unity).
    /// </summary>
    [System.Serializable]
    public class TrackCheckpointGrid : IndexGrid
    {
        public const int Subdivisions = 8;
        public const int kListCount = Subdivisions * Subdivisions;
        public override int SubdivisionsX => Subdivisions;
        public override int SubdivisionsZ => Subdivisions;

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
            bounds.numSubdivisionsX = Subdivisions;
            bounds.numSubdivisionsZ = Subdivisions;
            bounds.left = min.x;
            bounds.top = max.z;
            bounds.subdivisionWidth = (max.x - min.x) / Subdivisions; // delta / subdivisions
            bounds.subdivisionLength = (max.z - min.z) / Subdivisions; // delta / subdivisions

            return bounds;
        }

        public void GenerateIndexes(GridXZ matrixBoundsXZ, Checkpoint[] checkpoints)
        {
            // Init. Value is from inherited structure.
            indexLists = new IndexList[kListCount];

            // so if track has no width, we still pick up some points
            //var widthX = math.max(matrixBoundsXZ.subdivisionWidth, 1f);
            //var lengthZ = math.max(matrixBoundsXZ.subdivisionLength, 1f);

            var widthX = matrixBoundsXZ.subdivisionWidth;
            var lengthZ = matrixBoundsXZ.subdivisionLength;

            // Condition where theere is no w/l and so no checkpoints are added
            var hasNoWidthOrHeight = widthX == 0 || lengthZ == 0;
            if (hasNoWidthOrHeight)
            {
                var list = new List<int>();
                for (int i = 0; i < checkpoints.Length; i++)
                    list.Add(i);

                for (int i = 0; i < indexLists.Length; i++)
                    indexLists[i] = IndexList.CreateIndexList(list);

                return;
            }

            // Iterate over each subdivision in the course
            for (int z = 0; z < SubdivisionsZ; z++)
            {
                // Get the minimum and maximum Z coordinates allowed to exist this cell
                var minZIndex = math.clamp(z - 2, 0, SubdivisionsZ - 1);
                var maxZIndex = math.clamp(z + 2, 0, SubdivisionsZ - 1);
                var minZ = matrixBoundsXZ.top - (lengthZ * minZIndex);
                var maxZ = matrixBoundsXZ.top - (lengthZ * maxZIndex);

                for (int x = 0; x < SubdivisionsX; x++)
                {
                    // Get the minimum and maximum X coordinates allowed to exist this cell
                    var minXIndex = math.clamp(x - 2, 0, SubdivisionsX - 1);
                    var maxXIndex = math.clamp(x + 2, 0, SubdivisionsX - 1);
                    var minX = matrixBoundsXZ.left - (widthX * minXIndex);
                    var maxX = matrixBoundsXZ.left - (widthX * maxXIndex);

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
                    indexLists[cell] = IndexList.CreateIndexList(indexes);
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
