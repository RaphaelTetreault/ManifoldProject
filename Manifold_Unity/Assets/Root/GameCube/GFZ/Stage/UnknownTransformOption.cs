namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public enum UnknownTransformOption : byte
    {
        /// <summary>
        /// Value used on basically everything.
        /// </summary>
        None = 0b_00000000,

        /// <summary>
        /// Tagged only on 2 objects, "44LASTGATE1" and "44LASTGATE2" of Story 5.
        /// </summary>
        Unknown = 0b_10000000,
    }
}
