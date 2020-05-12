using System;
using System.IO;

namespace Manifold.IO
{
    public static class AnalyzerUtility
    {
        public static string FileTimestamp()
        {
            return DateTime.Now.ToString("(yyyy-MM-dd) (HH-mm-ss)");
        }

        public static StreamWriter OpenWriter(
            string filePath,
            FileMode mode = FileMode.OpenOrCreate,
            FileAccess access = FileAccess.ReadWrite,
            FileShare share = FileShare.Read
            )
        {
            var fileStream = File.Open(filePath, mode, access, share);
            var writer = new StreamWriter(fileStream);
            return writer;
        }
    }
}