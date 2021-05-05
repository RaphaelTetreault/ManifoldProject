namespace GameCube.GFZ.CourseCollision
{
    [System.Flags]
    public enum TrackPipeCylinderOptions : byte
    {
        IsOuterPipeOrCylinder = 1 << 0,
        IsOpenPipeOrCylinder = 1 << 1,
    }
}
