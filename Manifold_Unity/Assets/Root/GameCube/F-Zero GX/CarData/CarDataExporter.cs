using StarkTools.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

namespace GameCube.FZeroGX.CarData
{
    [CreateAssetMenu(menuName = "Manifold/Export/" + "CarData Exporter")]
    public class CarDataExporter : ExportSobjs<CarDataSobj>
    {
        public override string HelpBoxMessage
            => null;

        public override string OutputFileExtension
            => null;

        public override void Export()
        {
            foreach (var exportSobj in exportSobjs)
            {
                if (exportSobj is null)
                {
                    Debug.LogWarning($"Did not export null entry in {name}");
                    continue;
                }

                var dest = destinationDirectory;
                if (preserveFolderStructure)
                {
                    var folderPath = AssetDatabase.GetAssetPath(exportSobj);
                    folderPath = UnityPathUtility.EnforceSystemSeparators(folderPath);
                    folderPath = Path.GetDirectoryName(folderPath);
                    var length = "Assets/".Length;
                    folderPath = folderPath.Remove(0, length);
                    dest = UnityPathUtility.CombineSystemPath(dest, folderPath);

                    if (!Directory.Exists(dest))
                    {
                        Directory.CreateDirectory(dest);
                        Debug.Log($"Created path <i>{dest}</i>");
                    }
                }

                // Get file without .asset
                var outputFilePath = $"{dest}/{exportSobj.FileName}";
                outputFilePath = UnityPathUtility.EnforceSystemSeparators(outputFilePath);

                using (var fileStream = File.Open(outputFilePath, write.mode, write.access, write.share))
                {
                    var memoryStream = new MemoryStream();
                    using (var writer = new BinaryWriter(memoryStream))
                    {
                        exportSobj.Serialize(writer);
                        writer.Seek(0, SeekOrigin.Begin);
                        LibGxFormat.Lz.Lz.PackAvLz(writer.BaseStream, fileStream, LibGxFormat.AvGame.FZeroGX);
                    }
                }
            }
        }
    }
}