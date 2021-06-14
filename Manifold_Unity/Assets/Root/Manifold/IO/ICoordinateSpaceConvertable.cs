namespace Manifold.IO
{
    /// <summary>
    /// Interface for converting between an object's own space and Unity's XYZ space.
    /// </summary>
    public interface ICoordinateSpaceConvertable
    {
        public enum CoordinateSpace
        {
            OwnSpace,
            UnitySpace,
        }

        /// <summary>
        /// This object's current coordinate space.
        /// </summary>
        CoordinateSpace Space { get; }

        /// <summary>
        /// Converts from the object's own space coordinates to Unity's XYZ space coordinates.
        /// </summary>
        void ConvertToUnitySpace();

        /// <summary>
        /// Converts from the object's Unity's space coordinates back to its own XYZ space.
        /// </summary>
        void ConvertToOwnSpace();

    }
}
