namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Fog interpolation mode.
    /// F-Zero GX uses Linear, Exponential, and Exponential Squared.
    /// </summary>
    public enum FogType
    {
        None = 0x00000000,
        // 0x01 not used
        Linear = 0x02000000,
        // 0x03 not used
        Exponential = 0x04000000,
        ExponentialSquared = 0x05000000,
        ReverseExponential = 0x06000000,
        ReverseExponentialSquared = 0x07000000,
    }
}
