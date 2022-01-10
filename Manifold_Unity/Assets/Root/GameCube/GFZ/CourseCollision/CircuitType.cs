namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Option for whether the track is an open or closed circuit.
    /// Open circuits are story mode missions (story 2, 4, 5, 6).
    /// </summary>
    public enum CircuitType : int
    {
        OpenCircuit     = 0x00000000,
        ClosedCircuit   = 0x00010000,
    }
}
