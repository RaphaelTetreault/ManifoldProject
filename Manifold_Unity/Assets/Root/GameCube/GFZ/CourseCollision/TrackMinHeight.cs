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


        // METHODS

        /// <summary>
        /// Returns the lowest Y coordinate among all track nodes.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static float GetTrackMinHeight(ColiScene scene)
        {
            float minY = float.PositiveInfinity;

            foreach (var node in scene.trackNodes)
                foreach (var point in node.points)
                    if (point.positionStart.y < minY)
                        minY = point.positionStart.y;

            return minY;
        }

        /// <summary>
        /// Set's this value to the minimum Y coordinate of the associated scene.
        /// </summary>
        /// <param name="scene">The scene to iterate over for a minumum Y coordinate.</param>
        public void SetMinHeight(ColiScene scene)
        {
            value = GetTrackMinHeight(scene);
        }
    }
}
