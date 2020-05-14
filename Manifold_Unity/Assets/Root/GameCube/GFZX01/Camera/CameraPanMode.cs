namespace GameCube.GFZX01.Camera
{
    public enum CameraPanMode : short
    {
        /// <summary>
        /// Default, no mode modification is applied
        /// </summary>
        Default = 0,
        /// <summary>
        /// Invalid value (only values 0, 2, 3 are valid) so it halts
        /// </summary>
        Halt = 1,
        /// <summary>
        /// 
        /// </summary>
        SlowA = 2,
        /// <summary>
        /// 
        /// </summary>
        SlowB = 3,
    }
}