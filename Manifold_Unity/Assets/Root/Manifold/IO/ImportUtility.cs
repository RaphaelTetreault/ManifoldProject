using StarkTools.IO;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Manifold.IO
{
    public class ImportUtility
    {
        public static TSobj Create<TSobj>(string destinationDir, string fileName)
            where TSobj : ScriptableObject
        {
            var filePath = $"Assets/{destinationDir}/{fileName}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<TSobj>(filePath);
            if (asset != null)
            {
                return asset;
            }
            else
            {
                var sobj = ScriptableObject.CreateInstance<TSobj>();
                AssetDatabase.CreateAsset(sobj, filePath);
                return sobj;
            }
        }

        public static TSobj CreateFromBinary<TSobj>(string destinationDir, string fileName, BinaryReader reader)
            where TSobj : ScriptableObject, IBinarySerializable
        {
            var sobj = Create<TSobj>(destinationDir, fileName);
            sobj.Deserialize(reader);
            return sobj;
        }

        public static TSobj CreateFromBinaryFile<TSobj>(string destinationDir, string fileName, BinaryReader reader)
        where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var sobj = CreateFromBinary<TSobj>(destinationDir, fileName, reader);
            sobj.FileName = fileName;
            return sobj;
        }

        public static TSobj[] ImportManyAs<TSobj>(string[] importFiles, string importPath, string importDest, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var count = 0;
            var total = importFiles.Length;

            var typeName = typeof(TSobj).Name;
            var sobjs = new TSobj[total];

            foreach (var file in importFiles)
            {
                using (var fileStream = File.Open(file, mode, access, share))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        var filepath = string.Empty;
                        var sobj = ImportAs<TSobj>(reader, file, importPath, importDest, out filepath);
                        sobjs[count] = sobj;

                        // Progress bar update
                        var digitCount = total.ToString().Length;
                        var currentIndexStr = (count + 1).ToString().PadLeft(digitCount);
                        var title = $"Importing {typeName} ({currentIndexStr}/{total})";
                        var info = filepath;
                        var progress = count / (float)total;
                        EditorUtility.DisplayProgressBar(title, info, progress);
                    }
                }
                count++;
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();

            return sobjs;
        }

        public static TSobj ImportAs<TSobj>(BinaryReader reader, string file, string importFrom, string importTo, out string filePath)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var unityPath = GetUnityOutputPath(file, importFrom, importTo);
            var fileName = Path.GetFileName(file);

            // GENERIC
            var sobj = CreateFromBinaryFile<TSobj>(unityPath, fileName, reader);
            sobj.FileName = fileName;

            AssetDatabase.Refresh();
            EditorUtility.SetDirty(sobj);
            // This fixes overwriting assets losing their contents.
            AssetDatabase.SaveAssets();

            // Out params
            filePath = $"{unityPath}/{fileName}";

            return sobj;
        }

        public static string GetUnityOutputPath(string importFile, string importFrom, string importTo)
        {
            // Get path to root import folder
            var path = UnityPathUtility.GetUnityDirectory(UnityPathUtility.UnityFolder.Assets);
            var dest = UnityPathUtility.CombineSystemPath(path, importTo);

            // get path to file import folder
            // TODO: Regex instead of this hack
            // NOTE: The +1 is for the final / fwd slash assuming the "BrowseButtonField"
            // is used and does not return the final / on the path.
            var folder = importFile.Remove(0, importFrom.Length + 1);
            folder = Path.GetDirectoryName(folder);

            // (A) prevent null/empty && (B) prevent "/" or "\\"
            if (!string.IsNullOrEmpty(folder) && folder.Length > 1)
                dest = dest + folder;

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
                Debug.Log($"Created path <i>{dest}</i>");
            }

            var unityPath = UnityPathUtility.ToUnityFolderPath(dest, UnityPathUtility.UnityFolder.Assets);
            unityPath = UnityPathUtility.EnforceUnitySeparators(unityPath);
            
            return unityPath;
        }

    }
}