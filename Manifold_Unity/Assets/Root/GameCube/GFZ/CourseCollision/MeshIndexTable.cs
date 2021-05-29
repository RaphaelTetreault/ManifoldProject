namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public sealed class MeshIndexTable : IndexTable
    {
        public const int kListCount = 256;
        public override int ListCount => kListCount;
    }
}
