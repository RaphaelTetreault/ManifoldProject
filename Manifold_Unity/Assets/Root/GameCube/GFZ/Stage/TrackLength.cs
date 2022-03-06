namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Represents the total length of the track.
    /// </summary>
    [System.Serializable]
    public sealed class TrackLength : FloatRef
    {
        public TrackLength()
        {
            value = 0f;
        }

        public TrackLength(float length)
        {
            value = length;
        }
    }
}
