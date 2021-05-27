namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Denotes the style of checkpoint (presumably).
    /// </summary>
    public enum ArcadeCheckpointType
    {
        /// <summary>
        /// Flag used for Lightning [Thunder Road] and Green Plant [Spiral] checkpoints.
        /// </summary>
        Default = 0x00000000,

        /// <summary>
        /// Flag used for Port Town [Cylinder Wave] checkpoint.
        /// THOUGHT: Might be indication of circular trigger?
        /// </summary>
        PTCW = 0x00000001,
    }
}
