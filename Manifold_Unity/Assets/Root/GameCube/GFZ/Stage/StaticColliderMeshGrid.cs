namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A table for index lists specifically for Static Colliders.
    /// This class acts a wrapper to make serialization easy (in Unity).
    /// </summary>
    [System.Serializable]
    public sealed class StaticColliderMeshGrid : IndexGrid
    {
        //
        public const int Subdivisions = 16;
        public const int kListCount = Subdivisions * Subdivisions;

        //
        public override int SubdivisionsX => Subdivisions;
        public override int SubdivisionsZ => Subdivisions;
    }
}
