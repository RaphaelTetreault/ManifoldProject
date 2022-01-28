namespace Manifold.EditorTools.GC.GFZ
{
    /// <summary>
    /// Allows a class to be converted to a GFZ type.
    /// </summary>
    /// <typeparam name="T">The type the inheritor can become.</typeparam>
    public interface IGfzConvertable<T>
    {
        /// <summary>
        /// Returns a new instance of convertable type.
        /// </summary>
        /// <returns></returns>
        T ExportGfz();

        /// <summary>
        /// Initializes convertable type using <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        void ImportGfz(T value);
    }
}
