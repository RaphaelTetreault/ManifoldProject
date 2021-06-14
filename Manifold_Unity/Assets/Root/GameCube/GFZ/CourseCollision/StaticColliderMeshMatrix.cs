namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public sealed class StaticColliderMeshMatrix : IndexMatrix
    {
        public const int Subdivisions = 16;
        public const int kListCount = Subdivisions * Subdivisions;
        public override int SubdivisionsX => Subdivisions;
        public override int SubdivisionsZ => Subdivisions;
    }
}
