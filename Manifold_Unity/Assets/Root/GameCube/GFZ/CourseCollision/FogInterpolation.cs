namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Fog interpolation
    /// </summary>
    public enum FogInterpolation
    {
        // Could be exponential?
        Cubic = 0x02000000,
        
        // Very likely linear.
        Linear = 0x04000000,
        
        // Could be Exponential Squared?
        Exponential = 0x05000000,
    }
}
