namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Denotes what surface area type a SurfaceAttributeArea is. Presumably
    /// used for AI logic.
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
