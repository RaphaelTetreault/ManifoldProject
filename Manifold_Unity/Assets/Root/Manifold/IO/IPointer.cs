namespace Manifold.IO
{
    public interface IPointer
    {
        /// <summary>
        /// The address of the pointer.
        /// </summary>
        int Address { get; }

        /// <summary>
        /// Checks if this pointer is not null.
        /// </summary>
        bool IsNotNull { get; }

        /// <summary>
        /// Checks if this pointer is null.
        /// </summary>
        bool IsNull { get; }
    }
}
