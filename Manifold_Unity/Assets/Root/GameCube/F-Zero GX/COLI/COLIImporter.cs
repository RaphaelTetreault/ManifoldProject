using StarkTools.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


namespace GameCube.FZeroGX.COLI_COURSE
{
    [CreateAssetMenu]
    public class ColiImporter : ImportSobjs<ColiSceneSobj>
    {
        public override string ProcessMessage
            => null;

        public override string HelpBoxMessage
            => null;

        public override string TypeName
            => null;

        protected override string DefaultQueryFormat
            => null;

        public override void Import()
        {
            if (importMode != ImportMode.ImportFilesList)
                importFiles = GetFilesFromDirectory(importMode, importFolder, queryFormat);

            var count = 0;
            var total = importFiles.Length;

            foreach (var importFile in importFiles)
            {
                using (var fileStream = File.Open(importFile, read.mode, read.access, read.share))
                {
                    var requiresLzDecompression = Path.GetExtension(importFile).ToLower() == ".lz";

                    Stream file = fileStream;
                    if (requiresLzDecompression)
                    {
                        file = new MemoryStream();
                        LibGxFormat.Lz.Lz.UnpackAvLz(fileStream, file, LibGxFormat.AvGame.SuperMonkeyBall_1);
                        file.Seek(0, SeekOrigin.Begin);

                        var dir = Path.GetDirectoryName(importFile);
                        var filename = Path.GetFileNameWithoutExtension(importFile);
                        var path = UnityPathUtility.CombineSystemPath(dir, filename + ".decompressed");
                        if (!File.Exists(path))
                        {
                            using (var writer = File.Create(path, (int)file.Length))
                            {
                                file.CopyTo(writer);
                                file.Seek(0, SeekOrigin.Begin);
                            }
                        }
                    }

                    using (var reader = new BinaryReader(file))
                    {
                        var unityPath = GetOutputUnityPath(importFolder, importFile, destinationDirectory);
                        var fileName = Path.GetFileName(importFile);

                        var filePath = UnityPathUtility.CombineUnityPath(unityPath, fileName);
                        var assetPath = UnityPathUtility.CombineUnityPath("Assets", unityPath, $"{fileName}.asset");
                        var sobj = AssetDatabase.LoadAssetAtPath<ColiSceneSobj>(assetPath);
                        var isCreateSobj = sobj == null;
                        if (isCreateSobj)
                        {
                            sobj = CreateFromBinaryFile<ColiSceneSobj>(unityPath, fileName, reader);
                        }
                        else
                        {
                            sobj.scene = new ColiScene();
                            sobj.scene.FileName = fileName;
                            sobj.Deserialize(reader);
                        }
                        sobj.FileName = fileName;
                        sobj.filePath = filePath;

                        // Progress bar update
                        var currentIndexStr = (count + 1).ToString().PadLeft(total.ToString().Length);
                        var title = $"Importing {TypeName} ({currentIndexStr}/{total})";
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
