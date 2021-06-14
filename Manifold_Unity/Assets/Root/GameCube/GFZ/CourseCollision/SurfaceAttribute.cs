namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Denotes what the surface area on the track has (presumably for AI).
    /// </summary>
    public enum SurfaceAttribute : byte
    {
        /// <summary>
        /// 
        /// </summary>
        TerminateCode,

        /// <summary>
        /// Recharge areas collision type.
        /// </summary>
        Recover = 1,

        ///// <summary>
        ///// 
        ///// </summary>
        //idx2,

        /// <summary>
        /// 
        /// </summary>
        BoostPad = 3,

        /// <summary>
        /// Boost pad collision type.
        /// </summary>
        JumpPad = 4,

        ///// <summary>
        ///// 
        ///// </summary>
        //idx5,

        ///// <summary>
        ///// 
        ///// </summary>
        //idx6,

        ///// <summary>
        ///// 
        ///// </summary>
        //idx7,

        ///// <summary>
        ///// 
        ///// </summary>
        //idx8,

        ///// <summary>
        ///// 
        ///// </summary>
        //idx9,
    }
}
