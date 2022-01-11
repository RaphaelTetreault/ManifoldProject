namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// A matrix table for index lists specifically for Static Colliders.
    /// This class acts a wrapper to make serialization easy (in Unity).
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
