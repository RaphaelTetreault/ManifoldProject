namespace GameCube.GFZ.CourseCollision
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
            value = -10000f;
        }

        public TrackMinHeight(float minHeight)
        {
            value = minHeight;
        }

        // METHODS

        /// <summary>
        /// Returns the lowest Y coordinate among all track nodes.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static float GetTrackMinHeight(TrackCheckpoint[] checkpoints)
        {
            float minY = float.PositiveInfinity;

            foreach (var point in checkpoints)
                if (point.planeStart.position.y < minY)
                    minY = point.planeEnd.position.y;

            return minY;
        }

        /// <summary>
        /// Set's this value to the minimum Y coordinate of the associated scene.
        /// </summary>
        /// <param name="scene">The scene to iterate over for a minumum Y coordinate.</param>
        public void SetMinHeight(TrackCheckpoint[] checkpoints)
        {
            value = GetTrackMinHeight(checkpoints);
        }
    }
}
