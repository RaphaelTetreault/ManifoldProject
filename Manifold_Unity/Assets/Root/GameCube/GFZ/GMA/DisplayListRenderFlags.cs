namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// Flags indicating which display list(s) are serialized.
    /// </summary>
    [System.Flags]
    public enum DisplayListRenderFlags : byte
    {
        /// <summary>
        /// If set, indicates that the submesh has a primary opaque display list.
        /// </summary>
        renderPrimaryOpaque = 1 << 0,

        /// <summary>
        /// If set, indicates that the submesh has a primary translucid display list.
        /// </summary>
        renderPrimaryTranslucid = 1 << 1,

        /// <summary>
        /// If set, indicates that the submesh has a secondary opaque display list.
        /// </summary>
        renderSecondaryOpaque = 1 << 2,

        /// <summary>
        /// If set, indicates that the submesh has a secondary translucid display list.
        /// </summary>
        renderSecondaryTranslucid = 1 << 3,
    }
}