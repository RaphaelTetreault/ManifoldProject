// Legacy notes
// 0, 2, 4, 8, 16, 32, 64, 128

namespace GameCube.GFZ.CourseCollision
{
    [System.Flags]
    public enum TrackProperty : byte
    {
        None = 0,
        IS_UNUSED = 1 << 0,
        IsLava = 1 << 1,
        IsDirt = 1 << 2,
        IsIce = 1 << 3,
        IsHealPit = 1 << 4,
        IsModulated = 1 << 5,
        IsCapsulePipe = 1 << 6,
        IsPipeOrCylinder = 1 << 7,
    }
}
