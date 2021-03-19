namespace GameCube.GFZ.CourseCollision
{
    public enum ArcadeCheckpointType
    {
        /// <summary>
        /// Flag used for Lightning [Thunder Road] and Green Plant [Spiral]
        /// </summary>
        Default = 0x00000000,

        /// <summary>
        /// Flag used for Port Town [Cylinder Wave]. Might be indication of circular trigger?
        /// </summary>
        PTCW = 0x00000001,
    }
}
