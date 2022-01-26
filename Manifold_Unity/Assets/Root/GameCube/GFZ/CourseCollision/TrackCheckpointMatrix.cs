using Unity.Mathematics;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// A matrix table for index lists specifically for track checkpoints.
    /// This class acts a wrapper to make serialization easy (in Unity).
    /// </summary>
    [System.Serializable]
    public class TrackCheckpointMatrix : IndexMatrix
    {
        public const int Subdivisions = 8;
        public const int kListCount = Subdivisions * Subdivisions;
        public override int SubdivisionsX => Subdivisions;
        public override int SubdivisionsZ => Subdivisions;

        public static MatrixBoundsXZ GetMatrixBoundsXZ(TrackCheckpoint[] checkpoints)
        {
            // Get min and max XZ values of any checkpoint
            float3 min = new float3(float.MaxValue, 0, float.MaxValue);
            float3 max = new float3(float.MinValue, 0, float.MaxValue);

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
            var bounds = new MatrixBoundsXZ();
            bounds.numSubdivisionsX = Subdivisions;
            bounds.numSubdivisionsZ = Subdivisions;
            bounds.left = min.x;
            bounds.top = max.z;
            bounds.subdivisionWidth = (max.x - min.x) / Subdivisions; // delta / subdivisions
            bounds.subdivisionLength = (max.z - min.z) / Subdivisions; // delta / subdivisions

            return bounds;
        }

        public void GenerateIndexes(MatrixBoundsXZ matrixBoundsXZ, TrackCheckpoint[] checkpoints)
        {
            throw new System.NotImplementedException();
        }
    }
}
