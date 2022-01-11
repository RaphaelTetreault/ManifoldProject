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
    }
}
