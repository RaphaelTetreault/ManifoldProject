﻿using StarkTools.IO;
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
                    var unpackedStream = new MemoryStream();
                    LibGxFormat.Lz.Lz.UnpackAvLz(fileStream, unpackedStream, LibGxFormat.AvGame.FZeroGX);

                    using (var reader = new BinaryReader(unpackedStream))
                    {
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);

                        var unityPath = GetOutputUnityPath(importFolder, importFile, destinationDirectory);
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
                            CarData.MachineCount + CarData.BodyCount +
                            CarData.CockpitCount + CarData.BoosterCount;

                        // MACHINE
                        var machines = carData.Machines;
                        for (int i = 0; i < CarData.MachineCount; i++)
                        {
                            var index = carDataSobj.MachineIndex[i];
                            var name = (MachineName)index;
                            var indexPrint = index.ToString("D2");
                            var assetName = $"cardata_machine_{indexPrint}_{name}";

                            UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.vehicleParameters = machines[i];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetMachine(index, asset);
                        }
                        baseIndex += CarData.MachineCount;

                        // BODY
                        var bodyParts = carData.BodyParts;
                        for (int i = 0; i < CarData.BodyCount; i++)
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
                        for (int i = 0; i < CarData.CockpitCount; i++)
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
                        for (int i = 0; i < CarData.BoosterCount; i++)
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

                        // 
                        carDataSobj.padding = carData.padding;
                        carDataSobj.machineNames = carData.machineNames;
                        carDataSobj.unknownNames = carData.unknownNames;

                        EditorUtility.SetDirty(carDataSobj);
                    }
                }
                count++;
            }
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
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