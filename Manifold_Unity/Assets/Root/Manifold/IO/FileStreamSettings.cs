using System.IO;

namespace Manifold.IO
{
    [System.Serializable]
    public struct FileStreamSettings
    {
        public FileAccess access;
        public FileMode mode;
        public FileShare share;

        // STATIC PROPERTIES
        /// <summary>
        /// Defaults for reading:
        /// FileAccess.Read,
        /// FileMode.Open,
        /// FileShare.Read.
        /// </summary>
        public static FileStreamSettings Read
        {
            get
            {
                return new FileStreamSettings()
                {
                    access = FileAccess.Read,
                    mode = FileMode.Open,
                    share = FileShare.Read,
                };
            }
        }

        /// <summary>
        /// Defaults for writing:
        /// FileAccess.ReadWrite,
        /// FileMode.OpenOrCreate,
        /// FileShare.None.
        /// </summary>
        public static FileStreamSettings Write
        {
            get
            {
                return new FileStreamSettings()
                {
                    access = FileAccess.ReadWrite,
                    mode = FileMode.OpenOrCreate,
                    share = FileShare.Read,
                };
            }
        }

    }
}
