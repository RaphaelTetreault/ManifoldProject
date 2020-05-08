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
            var sobj = ScriptableObject.CreateInstance<TSobj>();
            var filePath = $"{destinationDir}/{fileName}.asset";
            AssetDatabase.CreateAsset(sobj, filePath);
            return sobj;
        }

        public static TSobj CreateFromBinary<TSobj>(string destinationDir, string fileName, BinaryReader reader)
            where TSobj : ScriptableObject, IBinarySerializable
        {
            var sobj = ScriptableObject.CreateInstance<TSobj>();
            var filePath = $"Assets/{destinationDir}/{fileName}.asset";
            AssetDatabase.CreateAsset(sobj, filePath);
            sobj.Deserialize(reader);
            return sobj;
        }

        public static TSobj CreateFromBinaryFile<TSobj>(string destinationDir, string fileName, BinaryReader reader)
        where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var sobj = ScriptableObject.CreateInstance<TSobj>();
            var filePath = $"Assets/{destinationDir}/{fileName}.asset";
            AssetDatabase.CreateAsset(sobj, filePath);
            sobj.FileName = fileName;
            sobj.Deserialize(reader);
            return sobj;
        }

        public static void ImportFilesAs<TSobj>(string[] importFiles, string importPath, string importDest, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var typeName = typeof(TSobj).Name;

            var count = 0;
            var total = importFiles.Length;

            foreach (var file in importFiles)
            {
                using (var fileStream = File.Open(file, mode, access, share))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        // Get path to root import folder
                        var path = UnityPathUtility.GetUnityDirectory(UnityPathUtility.UnityFolder.Assets);
                        var dest = UnityPathUtility.CombineSystemPath(path, importDest);

                        // get path to file import folder
                        // TODO: Regex instead of this hack
                        var length = importPath.Length;
                        var folder = file.Remove(0, length + 1);
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
                        var fileName = Path.GetFileName(file);

                        // GENERIC
                        var sobj = ImportUtility.CreateFromBinaryFile<TSobj>(unityPath, fileName, reader);
                        sobj.FileName = fileName;

                        // Progress bar update
                        //var filePath = AssetDatabase.GetAssetPath(sobj);
                        var currentIndexStr = (count + 1).ToString().PadLeft(total.ToString().Length);
                        var title = $"Importing {typeName} ({currentIndexStr}/{total})";
                        var info = $"{unityPath}/{fileName}";
                        var progress = count / (float)total;
                        EditorUtility.DisplayProgressBar(title, info, progress);
                        EditorUtility.SetDirty(sobj);
                    }
                }
                count++;
            }
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }


    }
}