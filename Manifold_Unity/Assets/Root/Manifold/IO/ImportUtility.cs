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

        public static TSobj[] ImportFilesAs<TSobj>(string[] importFiles, string importPath, string importDest, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read)
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
                        var sobj = ImportFileAs<TSobj>(reader, file, importPath, importDest, out filepath);
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
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            return sobjs;
        }

        public static TSobj ImportFileAs<TSobj>(BinaryReader reader, string file, string importFrom, string importTo, out string filePath)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            // Get path to root import folder
            var path = UnityPathUtility.GetUnityDirectory(UnityPathUtility.UnityFolder.Assets);
            var dest = UnityPathUtility.CombineSystemPath(path, importTo);

            // get path to file import folder
            // TODO: Regex instead of this hack
            var length = importFrom.Length;
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
            var sobj = CreateFromBinaryFile<TSobj>(unityPath, fileName, reader);
            sobj.FileName = fileName;

            EditorUtility.SetDirty(sobj);

            // Out params
            filePath = $"{unityPath}/{fileName}";

            //
            return sobj;
        }

    }
}