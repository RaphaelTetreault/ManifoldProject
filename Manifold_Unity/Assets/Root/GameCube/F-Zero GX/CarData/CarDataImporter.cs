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
    [CreateAssetMenu(menuName = "Manifold/Import/" + "CarData Importer")]
    public class CarDataImporter : ImportSobjs<CarDataSobj>
    {
        public override string ProcessMessage
            => null;

        public override string HelpBoxMessage
            => null;

        public override string TypeName
            => "CarData";

        protected override string DefaultQueryFormat
            => "cardata,lz";



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
                    using (var reader = new BinaryReader(fileStream))
                    {
                        var unityPath = UnityPath(importFolder, importFile, destinationDirectory);
                        var fileName = Path.GetFileName(importFile);

                        // Load cardata as type only
                        var carData = new CarData();
                        carData.Deserialize(reader);
                        carData.FileName = fileName;

                        // Create the cardata file
                        var carDataSobj = CreateInstance<CarDataSobj>();
                        var filePath = $"Assets/{unityPath}/{fileName}.asset";
                        AssetDatabase.CreateAsset(carDataSobj, filePath);
                        carDataSobj.FileName = fileName;

                        // For progress bar
                        var baseIndex = 0;
                        var totalIndices =
                            CarDataSobj.MachineCount +
                            CarDataSobj.BodyCount +
                            CarDataSobj.CockpitCount +
                            CarDataSobj.BoosterCount;

                        // MACHINE
                        var machines = carData.Machines;
                        for (int i = 0; i < CarDataSobj.MachineCount; i++)
                        {
                            var index = carDataSobj.VehicleIndex[i];
                            var name = (MachineName)index;
                            var indexPrint = index.ToString("D2");
                            var assetName = $"cardata_machine_{indexPrint}_{name}";

                            UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.vehicleParameters = machines[i];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetVehicle(index, asset);
                        }
                        baseIndex += CarDataSobj.MachineCount;

                        // BODY
                        var bodyParts = carData.BodyParts;
                        for (int i = 0; i < CarDataSobj.BodyCount; i++)
                        {
                            var index = i;
                            var name = (CustomBodyPartName)index;
                            var indexPrint = (index + 1).ToString("D2");
                            var assetName = $"cardata_body_{indexPrint}_{name}";

                            UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.vehicleParameters = bodyParts[i];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetBody(index, asset);
                        }
                        baseIndex += CarDataSobj.BodyCount;

                        // COCKPIT
                        var cockpitParts = carData.CockpitParts;
                        for (int i = 0; i < CarDataSobj.CockpitCount; i++)
                        {
                            var index = i;
                            var name = (CustomCockpitPartName)index;
                            var indexPrint = (index + 1).ToString("D2");
                            var assetName = $"cardata_cockpit_{indexPrint}_{name}";

                            UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.vehicleParameters = cockpitParts[i];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetCockpit(index, asset);
                        }
                        baseIndex += CarDataSobj.CockpitCount;

                        // BOOSTER
                        var boosterParts = carData.BoosterParts;
                        for (int i = 0; i < CarDataSobj.BoosterCount; i++)
                        {
                            var index = i;
                            var name = (CustomBoosterPartName)index;
                            var indexPrint = (index + 1).ToString("D2");
                            var assetName = $"cardata_booster_{indexPrint}_{name}";

                            UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.vehicleParameters = boosterParts[i];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetBooster(index, asset);
                        }
                        //baseIndex += CarDataSobj.BoosterCount;

                        //
                        carDataSobj.padding = carData.padding;
                        carDataSobj.machineNames = carData.machineNames;

                        EditorUtility.SetDirty(carDataSobj);
                    }
                }
                count++;
            }
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public string UnityPath(string importFolder, string importFile, string destinationDirectory)
        {
            // Get path to root import folder
            var path = UnityPathUtility.GetUnityDirectory(UnityPathUtility.UnityFolder.Assets);
            var dest = UnityPathUtility.CombineSystemPath(path, destinationDirectory);

            // get path to file import folder
            // TODO: Regex instead of this hack
            var length = importFolder.Length;
            var folder = importFile.Remove(0, length + 1);
            folder = Path.GetDirectoryName(folder);

            // (A) prevent null/empty AND (B) prevent "/" or "\\"
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

        public void UpdateProgressBar(int index, int total, string unityPath, string fileName)
        {
            var currentIndexStr = (index + 1).ToString().PadLeft(total.ToString().Length);
            var title = $"Importing {TypeName} ({currentIndexStr}/{total})";
            var info = $"{unityPath}/{fileName}";
            var progress = index / (float)total;
            EditorUtility.DisplayProgressBar(title, info, progress);
        }

    }
}