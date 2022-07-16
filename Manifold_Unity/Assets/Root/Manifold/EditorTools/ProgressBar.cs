using UnityEditor;

namespace Manifold.EditorTools
{
    /// <summary>
    /// Simple utility script for showing progress bar
    /// </summary>
    public static class ProgressBar
    {
        public static void Clear()
            => EditorUtility.ClearProgressBar();

        public static void Show(string title, string info, float progress)
            => EditorUtility.DisplayProgressBar(title, info, progress);

        public static bool ShowIndexed<T>(int count, int total, string title, string info)
        {
            // Add type conversion to progress bar
            var fullTitle = $"{title} {typeof(T).Name}";
            var userCancelled = ShowIndexed(count, total, fullTitle, info);
            return userCancelled;
        }

        public static bool ShowIndexed(int count, int total, string title, string info)
        {
            // Get max length of index string, pad digits to length
            var digitCount = total.ToString().Length;
            var currentIndexStr = (count + 1).ToString().PadLeft(digitCount);
            // Append index behind title
            var fullTitle = $"{title} ({currentIndexStr}/{total})";
            var progress = count / (float)total;
            var userCancelled = EditorUtility.DisplayCancelableProgressBar(fullTitle, info, progress);
            return userCancelled;
        }

    }
}