using Manifold.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools
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
                EditorUtility.SetDirty(sobj);
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
            var sobj = Create<TSobj>(destinationDir, fileName);
            sobj.FileName = fileName;
            sobj.Deserialize(reader);
            return sobj;
        }

        public static TSobj ImportAs<TSobj>(BinaryReader reader, string file, string importFrom, string importTo, out string filePath)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var unityPath = GetUnityOutputPath(file, importFrom, importTo);
            var fileName = Path.GetFileName(file);

            // GENERIC
            var sobj = CreateFromBinaryFile<TSobj>(unityPath, fileName, reader);
            sobj.FileName = fileName;

            EditorUtility.SetDirty(sobj);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            // Out params
            filePath = $"{unityPath}/{fileName}";

            return sobj;
        }

        public static TSobj[] ImportManyAs<TSobj>(string[] importFiles, string importFrom, string importTo, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read)
            where TSobj : ScriptableObject, IBinarySerializable, IFile
        {
            var count = 0;
            var total = importFiles.Length;
            var sobjs = new TSobj[total];

            foreach (var file in importFiles)
            {
                using (var fileStream = File.Open(file, mode, access, share))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        var filepath = string.Empty;
                        var sobj = ImportAs<TSobj>(reader, file, importFrom, importTo, out filepath);
                        sobjs[count] = sobj;
                        var userCancelled = ProgressBar<TSobj>(count, total, filepath);

                        if (userCancelled)
                        {
                            break;
                        }
                    }
                }
                count++;
            }

            FinalizeAssetImport();

            return sobjs;
        }

        public static string GetUnityOutputPath(string importFile, string importFrom, string importTo)
        {
            // Get path to root import folder
            var path = UnityPathUtility.GetUnityDirectory(UnityPathUtility.UnityFolder.Assets);
            var dest = UnityPathUtility.CombineSystemPath(path, importTo);

            // get path to file import folder
            // TODO: use Regex instead of this hack
            // NOTE: The +1 is for the final / fwd slash assuming the "BrowseButtonField"
            //       is used and does not return the final / on the path.
            var folder = importFile.Remove(0, importFrom.Length + 1);
            folder = Path.GetDirectoryName(folder);

            // (A) prevent null/empty && (B) prevent "/" or "\\"
            if (!string.IsNullOrEmpty(folder) && folder.Length > 1)
            {
                dest = $"{dest}/{folder}";
            }

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
                Debug.Log($"Created path <i>{dest}</i>");
            }

            var unityPath = UnityPathUtility.ToUnityFolderPath(dest, UnityPathUtility.UnityFolder.Assets);
            unityPath = UnityPathUtility.EnforceUnitySeparators(unityPath);

            return unityPath;
        }

        public static bool ProgressBar<T>(int count, int total, string info)
        {
            // Progress bar update
            var digitCount = total.ToString().Length;
            var currentIndexStr = (count + 1).ToString().PadLeft(digitCount);
            var title = $"Importing {typeof(T).Name} ({currentIndexStr}/{total})";
            var progress = count / (float)total;
            var userCancelled = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
            return userCancelled;
        }

        public static bool ProgressBar(int count, int total, string info, string title)
        {
            // Progress bar update
            var digitCount = total.ToString().Length;
            var currentIndexStr = (count + 1).ToString().PadLeft(digitCount);
            var progress = count / (float)total;
            var userCancelled = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
            return userCancelled;
        }

        // path w/o Assets
        public static void EnsureAssetFolderExists(string folderPath)
        {
            // Ensure folder path is indeed a path
            if (string.IsNullOrEmpty(folderPath))
                throw new ArgumentException($"Folder path \"{folderPath}\" is null or empty.");

            //
            var assetFolders = $"Assets/{folderPath}";
            var folderNames = assetFolders.Split('/');
            var directoriesCount = folderNames.Length;

            // Start with root folder which should always be "Assets"
            var parentFolder = folderNames[0];

            // Append each directory to parent directory in succession
            for (int i = 0; i < directoriesCount - 1; i++)
            {
                var nextFolder = folderNames[i+1];
                var fullPath = $"{parentFolder}/{nextFolder}";

                var doCreateFolder = !AssetDatabase.IsValidFolder(fullPath);
                if (doCreateFolder)
                {
                    AssetDatabase.CreateFolder(parentFolder, nextFolder);
                }

                parentFolder = fullPath;
            }
        }

        public static string GetUnityAssetDirectory(UnityEngine.Object asset)
        {
            // Finds asset in database.
            var assetDirectory = AssetDatabase.GetAssetPath(asset);

            // Ensure we aren't doing anything with nothing
            if (string.IsNullOrEmpty(assetDirectory))
                throw new ArgumentException($"Asset \"{asset.name}\" is not in asset database.");

            // Make sure path is valid Unity asset path
            assetDirectory = Path.GetDirectoryName(assetDirectory);
            assetDirectory = UnityPathUtility.EnforceUnitySeparators(assetDirectory);

            return assetDirectory;
        }

        public static void FinalizeAssetImport()
        {
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }







        //public static void CreatePrefabFromModel(Mesh mesh, string assetPath)
        //{
        //    // Create new GameObject (ends up in current scene)
        //    var prefab = new GameObject();

        //    // Add mesh components, assign mesh
        //    var meshFilter = prefab.AddComponent<MeshFilter>();
        //    meshFilter.mesh = mesh;

        //    // Save to Asset Database
        //    PrefabUtility.SaveAsPrefabAsset(prefab, assetPath);
        //    // Remove asset from scene
        //    UnityEngine.Object.DestroyImmediate(prefab);
        //}



        public static GameObject CreatePrefabFromModel(Mesh mesh, Material[] sharedMaterials, string assetPath)
        {
            // Create new GameObject (ends up in current scene)
            var tempObject = new GameObject();

            // Add mesh components, assign mesh
            var meshRenderer = tempObject.AddComponent<MeshRenderer>();
            var meshFilter = tempObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshRenderer.sharedMaterials = sharedMaterials;

            // Save to Asset Database
            var prefab = PrefabUtility.SaveAsPrefabAsset(tempObject, assetPath);
            // Remove asset from scene
            UnityEngine.Object.DestroyImmediate(tempObject);

            return prefab;
        }




    }
}