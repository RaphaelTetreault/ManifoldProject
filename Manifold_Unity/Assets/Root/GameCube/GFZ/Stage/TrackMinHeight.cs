namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Representes the lowest Y coordinate of any track checkpoint.
    /// May be used to generate a kill plane dynamically.
    /// </summary>
    [System.Serializable]
    public sealed class TrackMinHeight : FloatRef
    {
        // CONSTRUCTORS
        public TrackMinHeight()
        {
            // Default value as seen in AX test files.
            Value = -10000f;
        }

        public TrackMinHeight(float minHeight)
        {
            Value = minHeight;
        }

        // METHODS

        /// <summary>
        /// Returns the lowest Y coordinate among all track nodes.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static float GetTrackMinHeight(Checkpoint[] checkpoints)
        {
            float minY = float.PositiveInfinity;

            foreach (var checkpoint in checkpoints)
                if (checkpoint.PlaneStart.origin.y < minY)
                    minY = checkpoint.PlaneEnd.origin.y;

            return minY;
        }

        /// <summary>
        /// Set's this value to the minimum Y coordinate of the associated scene.
        /// </summary>
        /// <param name="scene">The scene to iterate over for a minumum Y coordinate.</param>
        public void SetMinHeight(Checkpoint[] checkpoints)
        {
            Value = GetTrackMinHeight(checkpoints);
        }
    }
}
