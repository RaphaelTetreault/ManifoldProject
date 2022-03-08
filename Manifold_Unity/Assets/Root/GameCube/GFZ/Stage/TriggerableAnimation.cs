namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Animation trigger index for volume. How an animation knows to
    /// play when based on index is not yet investigated.
    /// </summary>
    [System.Flags]
    public enum TriggerableAnimation : ushort
    {
        AnimationID_00 = 1 << 00,
        AnimationID_01 = 1 << 01,
        AnimationID_02 = 1 << 02,
        AnimationID_03 = 1 << 03,
        AnimationID_04 = 1 << 04,
        AnimationID_05 = 1 << 05,
        AnimationID_06 = 1 << 06,
        AnimationID_07 = 1 << 07,
        AnimationID_08 = 1 << 08,
        AnimationID_09 = 1 << 09,
        AnimationID_10 = 1 << 10,
        AnimationID_11 = 1 << 11,
        AnimationID_12 = 1 << 12,
        AnimationID_13 = 1 << 13,
        AnimationID_14 = 1 << 14,
        AnimationID_15 = 1 << 15,
    }
}
