namespace GameCube.GFZX01.Camera
{
    public enum CameraPanInterpolation : short
    {
        /// <summary>
        /// Used for shots where there is no lerping of any kind.
        /// </summary>
        Linear = 0,
        /// <summary>
        /// 
        /// </summary>
        EaseOut = 2,
        /// <summary>
        /// 
        /// </summary>
        EaseInOut = 3,
    }
}