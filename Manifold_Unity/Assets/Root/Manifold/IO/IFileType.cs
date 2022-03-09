namespace Manifold.IO
{
    /// <summary>
    /// Interface for file types.
    /// </summary>
    public interface IFileType
    {
        string FileExtension { get; }
        string FileName { get; set; }
    }
}
