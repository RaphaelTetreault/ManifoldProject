namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
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
