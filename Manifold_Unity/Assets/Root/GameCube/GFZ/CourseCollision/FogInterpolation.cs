namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Fog interpolation
    /// </summary>
    public enum FogInterpolation
    {
        // NOTES FROM SMB2 COMMUNITY:
        // Fog type (Usually 0x04, although this can be any from _GXFogType - NONE: 0x00,
        // LIN: 0x02, EXP: 0x04, EXP2: 0x05, REVEXP: 0x06, REVEXP2: 0x07)

        // Could be exponential?
        Cubic = 0x02000000,
        
        // Very likely linear.
        Linear = 0x04000000,
        
        // Could be Exponential Squared?
        Exponential = 0x05000000,
    }
}
