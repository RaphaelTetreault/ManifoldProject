namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Option for whether the track is an open or closed circuit.
    /// </summary>
    public enum CircuitType : uint
    {
        /// <summary>
        /// Set when the track is a closed circuit. This is most stages.
        /// </summary>
        ClosedCircuit   = 0x00000000,

        /// <summary>
        /// Set when the track is an open circuit. This is story stages 2, 4, 5, and 6.
        /// </summary>
        OpenCircuit = 0x00010000,
    }
}
