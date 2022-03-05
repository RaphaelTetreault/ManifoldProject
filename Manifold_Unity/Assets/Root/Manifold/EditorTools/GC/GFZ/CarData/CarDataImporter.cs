using GameCube.GFZ;
using Manifold.IO;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.CarData
{
    [CreateAssetMenu(menuName = Const.Menu.GfzCarData + "CarData Importer")]
    public class CarDataImporter : ExecutableScriptableObject,
        IImportable
    {
        #region MEMBERS

        [Header("Import Settings")]
        [SerializeField, BrowseFolderField("Assets/")]
        protected string importFrom;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importTo;

        [SerializeField]
        protected SearchOption fileSearchOption = SearchOption.AllDirectories;

        [SerializeField]
        protected string searchPattern = "cardata*";

        [Header("Import Files")]
        [SerializeField]
        protected string[] importFiles;

        #endregion

        public override string ExecuteText => "Import CarData";

        public override void Execute() => Import();


        public void Import()
        {
            importFiles = Directory.GetFiles(importFrom, searchPattern, fileSearchOption);
            importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);
            //importFiles = GfzUtility.DecompressEachLZ(importFiles);
            // Due to line above commented out, decommission this function
            throw new System.NotImplementedException("API change broke this function");

            var count = 0;
            foreach (var importFile in importFiles)
            {
                using (var file = File.Open(importFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new BinaryReader(file))
                    {
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);

                        var fileName = Path.GetFileName(importFile);
                        var unityPath = ImportUtility.GetUnityOutputPath(importFile, importFrom, importTo);

                        // Load cardata as type only
                        var carData = new GameCube.GFZ.CarData.CarData();
                        carData.Deserialize(reader);
                        carData.FileName = fileName;

                        // Create the cardata file
                        var carDataSobj = CreateInstance<CarDataSobj>();
                        var filePath = $"Assets/{unityPath}/{fileName}.asset";
                        AssetDatabase.CreateAsset(carDataSobj, filePath);
                        carDataSobj.FileName = fileName;

                        // Get quantity of things to iterate over
                        int machineCount = GameCube.GFZ.CarData.CarData.MachineCount;
                        int bodyCount = GameCube.GFZ.CarData.CarData.BodyCount;
                        int cockpitCount = GameCube.GFZ.CarData.CarData.CockpitCount;
                        int boosterCount = GameCube.GFZ.CarData.CarData.BoosterCount;
                        int indexBase = 0;
                        int indexTotal = machineCount + bodyCount + cockpitCount + boosterCount;

                        // MACHINE
                        var machines = carData.MachinesInternalOrder;
                        for (int machineIndex = 0; machineIndex < machineCount; machineIndex++)
                        {
                            // Get machine name, display progress bar
                            var name = (MachineName)machineIndex;
                            var assetName = $"cardata_machine_{machineIndex:00}_{name}";
                            ImportUtility.ProgressBar(indexBase + machineIndex, indexTotal, assetName, "Importing Vehicle Parameters");

                            // Save asset
                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.value = machines[machineIndex];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetMachine(machineIndex, asset);
                        }
                        indexBase += machineCount;

                        // BODY
                        var bodyParts = carData.BodyParts;
                        for (int bodyIndex = 0; bodyIndex < bodyCount; bodyIndex++)
                        {
                            // Get machine name, display progress bar
                            var name = (CustomBodyPartName)bodyIndex;
                            var assetName = $"cardata_body_{bodyIndex:00}_{name}";
                            ImportUtility.ProgressBar(indexBase + bodyIndex, indexTotal, assetName, "Importing Custom Body Parts");

                            // Save asset
                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.value = bodyParts[bodyIndex];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetBody(bodyIndex, asset);
                        }
                        indexBase += bodyCount;

                        // COCKPIT
                        var cockpitParts = carData.CockpitParts;
                        for (int cockpitIndex = 0; cockpitIndex < cockpitCount; cockpitIndex++)
                        {
                            // Get machine name, display progress bar
                            var name = (CustomCockpitPartName)cockpitIndex;
                            var assetName = $"cardata_cockpit_{cockpitIndex:00}_{name}";
                            ImportUtility.ProgressBar(indexBase + cockpitIndex, indexTotal, assetName, "Importing Custom Cockpit Parts");

                            // Save asset
                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.value = cockpitParts[cockpitIndex];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetCockpit(cockpitIndex, asset);
                        }
                        indexBase += cockpitCount;

                        // BOOSTER
                        var boosterParts = carData.BoosterParts;
                        for (int boosterIndex = 0; boosterIndex < boosterCount; boosterIndex++)
                        {
                            // Get machine name, display progress bar
                            var name = (CustomBoosterPartName)boosterIndex;
                            var assetName = $"cardata_booster_{boosterIndex:00}_{name}";
                            ImportUtility.ProgressBar(indexBase + boosterIndex, indexTotal, assetName, "Importing Custom Booster Parts");

                            // Save asset
                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.value = boosterParts[boosterIndex];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetBooster(boosterIndex, asset);
                        }

                        // Apply other values
                        //carDataSobj.padding = carData.Padding;
                        carDataSobj.machineNames = carData.machineNames;
                        carDataSobj.unknownNames = carData.partsInternalNames;

                        EditorUtility.SetDirty(carDataSobj);
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