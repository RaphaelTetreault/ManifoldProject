namespace Manifold
{
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
