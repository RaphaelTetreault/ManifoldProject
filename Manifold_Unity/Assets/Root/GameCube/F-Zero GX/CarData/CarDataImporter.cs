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

                        // Create the cardata file
                        var carData = new CarData();
                        carData.Deserialize(reader);
                        carData.FileName = fileName;



                        //BinaryIoUtility.PushEndianess(true);
                        //BinaryIoUtility.PushEncoding(System.Text.Encoding.ASCII);

                        //var unityPath = UnityPath(importFolder, importFile, destinationDirectory);
                        //var fileName = Path.GetFileName(importFile);

                        //// Create the cardata file
                        //var sobj = CreateInstance<CarDataSobj>();
                        //var filePath = $"Assets/{unityPath}/{fileName}.asset";
                        //AssetDatabase.CreateAsset(sobj, filePath);
                        //sobj.FileName = fileName;

                        //// For progress bar
                        //var baseIndex = 0;
                        //var totalIndices =
                        //    CarDataSobj.MachineCount +
                        //    CarDataSobj.BodyCount +
                        //    CarDataSobj.CockpitCount +
                        //    CarDataSobj.BoosterCount;

                        //// MACHINE
                        //for (int i = 0; i < CarDataSobj.MachineCount; i++)
                        //{
                        //    var vehicleIndex = sobj.VehicleIndex[i];
                        //    var vehicleName = (VehicleName)vehicleIndex;
                        //    var vehicleIndexPrint = vehicleIndex.ToString("D2");
                        //    var assetName = $"cardata_machine_{vehicleIndexPrint}_{vehicleName}";

                        //    UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                        //    var asset = CreateFromBinary<VehicleParametersSobj>(unityPath, assetName, reader);
                        //    sobj.SetVehicle(vehicleIndex, asset);
                        //}
                        //baseIndex += CarDataSobj.MachineCount;

                        //// Read some padding
                        //reader.ReadX(ref sobj.padding, 12);
                        //foreach (var pad in sobj.padding)
                        //    Assert.IsTrue(pad == 0);

                        //BinaryIoUtility.PopEndianess();
                        //sobj.vehicleNames = new string[43];
                        //for (int i = 0; i < sobj.vehicleNames.Length; i++)
                        //{
                        //    reader.ReadXCString(ref sobj.vehicleNames[i], System.Text.Encoding.ASCII);
                        //}
                        //BinaryIoUtility.PushEndianess(true);

                        //// Skip area we don't know things about
                        ////reader.BaseStream.Seek(0x1F04, SeekOrigin.Begin);

                        //// BODY
                        //for (int i = 0; i < CarDataSobj.BodyCount; i++)
                        //{
                        //    var index = i;
                        //    var name = (CustomBodyPartName)index;
                        //    var indexPrint = (index + 1).ToString("D2");
                        //    var assetName = $"cardata_body_{indexPrint}_{name}";

                        //    UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                        //    var asset = CreateFromBinary<VehicleParametersSobj>(unityPath, assetName, reader);
                        //    sobj.SetBody(index, asset);
                        //}
                        //baseIndex += CarDataSobj.BodyCount;

                        //// COCKPIT
                        //for (int i = 0; i < CarDataSobj.CockpitCount; i++)
                        //{
                        //    var index = i;
                        //    var name = (CustomCockpitPartName)index;
                        //    var indexPrint = (index + 1).ToString("D2");
                        //    var assetName = $"cardata_cockpit_{indexPrint}_{name}";

                        //    UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                        //    var asset = CreateFromBinary<VehicleParametersSobj>(unityPath, assetName, reader);
                        //    sobj.SetCockpit(index, asset);
                        //}
                        //baseIndex += CarDataSobj.CockpitCount;

                        //// BOOSTER
                        //for (int i = 0; i < CarDataSobj.BoosterCount; i++)
                        //{
                        //    var index = i;
                        //    var name = (CustomBoosterPartName)index;
                        //    var indexPrint = (index + 1).ToString("D2");
                        //    var assetName = $"cardata_booster_{indexPrint}_{name}";

                        //    UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                        //    var asset = CreateFromBinary<VehicleParametersSobj>(unityPath, assetName, reader);
                        //    sobj.SetBooster(index, asset);
                        //}
                        ////baseIndex += CarDataSobj.BoosterCount;

                        //EditorUtility.SetDirty(sobj);
                        //BinaryIoUtility.PopEndianess();
                        //BinaryIoUtility.PopEncoding();
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