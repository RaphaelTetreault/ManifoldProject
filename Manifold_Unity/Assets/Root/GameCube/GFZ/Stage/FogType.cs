namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Fog interpolation mode.
    /// F-Zero GX uses Linear, Exponential, and Exponential Squared.
    /// </summary>
    public enum FogType
    {
        /// <summary>
        /// No fog
        /// </summary>
        None = 0x00000000,

        // 0x01 not used
        
        /// <summary>
        /// Linear interpolation for fog color blending
        /// </summary>
        Linear = 0x02000000,

        // 0x03 not used

        /// <summary>
        /// Exponential interpolation for fog color blending
        /// </summary>
        Exponential = 0x04000000,

        /// <summary>
        /// Exponential squared interpolation for fog color blending
        /// </summary>
        ExponentialSquared = 0x05000000,


        /// <summary>
        /// Reverse exponential interpolation for fog color blending
        /// </summary>
        ReverseExponential = 0x06000000,

        /// <summary>
        /// Reverse exponential squared interpolation for fog color blending
        /// </summary>
        ReverseExponentialSquared = 0x07000000,
    }
}
