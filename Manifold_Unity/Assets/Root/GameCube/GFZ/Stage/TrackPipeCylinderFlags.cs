namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Parameters to dictate how to interpret a cylindrical or capsule track segment.
    /// </summary>
    [System.Flags]
    public enum TrackPipeCylinderFlags : byte
    {
        /// <summary>
        /// Is the driveable surface on the outside (cylinder) or inside (pipe)?
        /// If set is cylinder, if not set is pipe.
        /// </summary>
        IsCylinderNotPipe = 1 << 0,
        
        /// <summary>
        /// Is the cylinder or pipe open (transitioning from another segment type).
        /// </summary>
        IsOpenPipeOrCylinder = 1 << 1,
    }
}
