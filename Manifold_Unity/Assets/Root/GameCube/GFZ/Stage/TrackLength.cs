namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Represents the total length of the track.
    /// </summary>
    [System.Serializable]
    public sealed class TrackLength : FloatRef
    {
        // CONSTRUCTORS
        public TrackLength()
        {
            Value = 0f;
        }

        public TrackLength(float length)
        {
            Value = length;
        }
    }
}
