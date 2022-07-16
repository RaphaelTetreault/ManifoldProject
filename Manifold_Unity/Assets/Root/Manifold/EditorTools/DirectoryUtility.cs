using System.IO;
using System.Text;

namespace Manifold.EditorTools
{
    public static class DirectoryUtility
    {
        public static string GoUpDirectory(string path, int levels)
        {
            // Go up a directory
            var relativeRoot = Path.GetDirectoryName(path);
            for (int i = 0; i < levels; i++)
                relativeRoot = Path.Combine(relativeRoot, @"..\");
            relativeRoot = Path.GetFullPath(relativeRoot);

            return relativeRoot;
        }

        public static string GetRelativePathBeginningAt(string path, int depth)
        {
            var builder = new StringBuilder(path.Length);
            var cleanPath = path.Replace('\\', '/');
            var paths = cleanPath.Split('/');
            for (int i = depth; i < paths.Length; i++)
            {
                builder.Append(paths[i]);
                builder.Append('/');
            }

            return builder.ToString();
        }

        public static string GetTopDirectory(string path)
        {
            string directory = Path.GetDirectoryName(path);
            var cleanDirectory = directory.Replace('\\', '/');
            string[] directories = cleanDirectory.Split('/');
            // -1 for index
            int lastDirectory = directories.Length - 1;
            // -1 since last dir is "" from trailing '/', ei: "C:/a/dir/"
            lastDirectory -= string.IsNullOrEmpty(directories[lastDirectory]) ? 1 : 0;
            return directories[lastDirectory];
        }

    }
}
