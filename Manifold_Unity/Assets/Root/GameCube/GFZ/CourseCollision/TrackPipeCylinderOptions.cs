namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Parameters to dictate how to interpret a cylindrical or capsule track segment.
    /// </summary>
    [System.Flags]
    public enum TrackPipeCylinderOptions : byte
    {
        /// <summary>
        /// Is the driveable surface on the outside (cylinder) or inside (pipe).
        /// </summary>
        IsOuterPipeOrCylinder = 1 << 0,
        
        /// <summary>
        /// Is the cylinder or pipe open (transitioning from another segment type).
        /// </summary>
        IsOpenPipeOrCylinder = 1 << 1,
    }
}
