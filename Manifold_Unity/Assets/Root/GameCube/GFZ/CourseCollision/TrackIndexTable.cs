namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class TrackIndexTable : IndexTable
    {
        public static int kListCount = 64;
        public override int ListCount => kListCount;
    }
}
