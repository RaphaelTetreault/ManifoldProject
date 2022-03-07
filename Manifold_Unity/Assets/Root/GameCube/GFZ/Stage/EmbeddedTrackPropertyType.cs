namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Denotes what surface area type a SurfaceAttributeArea is. Presumably
    /// used for AI logic.
    /// </summary>
    public enum EmbeddedTrackPropertyType : byte
    {
        /// <summary>
        /// Indicates this is a terminating node (signifies end of array).
        /// </summary>
        TerminateCode,

        /// <summary>
        /// Indicates area is a energy pit.
        /// </summary>
        Recover = 1,

        ///// <summary>
        ///// 
        ///// </summary>
        //idx2,

        /// <summary>
        /// Indicates area is a boost pad.
        /// </summary>
        BoostPad = 3,

        /// <summary>
        /// Indicates area is a jump pad.
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
