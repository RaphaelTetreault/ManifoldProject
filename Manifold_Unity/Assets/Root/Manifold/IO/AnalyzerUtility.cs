using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Manifold.IO
{
    public static class AnalyzerUtility
    {
        public static string GetFileTimestamp()
        {
            return DateTime.Now.ToString("(yyyy-MM-dd) (HH-mm-ss)");
        }

        public static string GetAnalysisFilePathTSV(string outputPath, string fileName)
        {
            var time = GetFileTimestamp();
            var analysisFilePath = Path.Combine(outputPath, $"{time} {fileName}.tsv");
            return analysisFilePath;
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

        public static IEnumerable<Enum> GetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
                else
                    yield return null;
        }

        #region

        /// <summary>
        /// Usage: GetVariableName(() => someVar)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string GetVariableName<T>(Expression<Func<T>> expr)
        {
            var body = (MemberExpression)expr.Body;

            return body.Member.Name;
        }


        #endregion
    }
}