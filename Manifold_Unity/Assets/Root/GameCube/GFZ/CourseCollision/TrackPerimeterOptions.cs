namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Denotes data related to perimeter of the current track graph node.
    /// </summary>
    [System.Flags]
    public enum TrackPerimeterOptions : byte
    {
        None = 0,
        //Unused0 = 1 << 0,
        //Unused1 = 1 << 1,
        hasRailHeightLeft   = 1 << 2, // 0x04  4
        hasRailHeightRight  = 1 << 3, // 0x08  8
        has90TurnRight      = 1 << 4, // 0x10 16
        has90TurnLeft       = 1 << 5, // 0x20 32
        //Unused6 = 1 << 6,
        //Unused7 = 1 << 7,
    }
}
