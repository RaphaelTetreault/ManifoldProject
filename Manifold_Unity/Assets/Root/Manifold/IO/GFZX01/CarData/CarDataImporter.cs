using GameCube.GFZX01;
using StarkTools.IO;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manifold.IO.GFZX01.CarData
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_CarData + "CarData Importer")]
    public class CarDataImporter : ExecutableScriptableObject,
        IImportable
    {
        [Header("Import Settings")]
        [SerializeField]
        protected SearchOption fileSearchOption = SearchOption.AllDirectories;

        [SerializeField]
        protected string searchPattern = "cardata*";

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importFrom;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importTo;

        [Header("Import Files")]
        [SerializeField]
        protected string[] importFiles;


        public override string ExecuteText => "Import CarData";

        public override void Execute() => Import();


        public void Import()
        {
            importFiles = Directory.GetFiles(importFrom, searchPattern, fileSearchOption);
            importFiles = UnityPathUtility.EnforceUnitySeparators(importFiles);
            importFiles = GFZX01Utility.DecompressEachLZ(importFiles);

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
                        var carData = new GameCube.GFZX01.CarData.CarData();
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
                            GameCube.GFZX01.CarData.CarData.MachineCount + GameCube.GFZX01.CarData.CarData.BodyCount +
                            GameCube.GFZX01.CarData.CarData.CockpitCount + GameCube.GFZX01.CarData.CarData.BoosterCount;

                        // MACHINE
                        var machines = carData.Machines;
                        for (int i = 0; i < GameCube.GFZX01.CarData.CarData.MachineCount; i++)
                        {
                            var index = carDataSobj.MachineIndex[i];
                            var name = (MachineName)index;
                            var indexPrint = index.ToString("D2");
                            var assetName = $"cardata_machine_{indexPrint}_{name}";

                            UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.value = machines[i];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetMachine(index, asset);
                        }
                        baseIndex += GameCube.GFZX01.CarData.CarData.MachineCount;

                        // BODY
                        var bodyParts = carData.BodyParts;
                        for (int i = 0; i < GameCube.GFZX01.CarData.CarData.BodyCount; i++)
                        {
                            var index = i;
                            var name = (CustomBodyPartName)index;
                            var indexPrint = (index + 1).ToString("D2");
                            var assetName = $"cardata_body_{indexPrint}_{name}";

                            UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.value = bodyParts[i];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetBody(index, asset);
                        }
                        baseIndex += CarDataSobj.BodyCount;

                        // COCKPIT
                        var cockpitParts = carData.CockpitParts;
                        for (int i = 0; i < GameCube.GFZX01.CarData.CarData.CockpitCount; i++)
                        {
                            var index = i;
                            var name = (CustomCockpitPartName)index;
                            var indexPrint = (index + 1).ToString("D2");
                            var assetName = $"cardata_cockpit_{indexPrint}_{name}";

                            UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.value = cockpitParts[i];
                            var assetPath = $"Assets/{unityPath}/{assetName}.asset";
                            AssetDatabase.CreateAsset(asset, assetPath);
                            carDataSobj.SetCockpit(index, asset);
                        }
                        baseIndex += CarDataSobj.CockpitCount;

                        // BOOSTER
                        var boosterParts = carData.BoosterParts;
                        for (int i = 0; i < GameCube.GFZX01.CarData.CarData.BoosterCount; i++)
                        {
                            var index = i;
                            var name = (CustomBoosterPartName)index;
                            var indexPrint = (index + 1).ToString("D2");
                            var assetName = $"cardata_booster_{indexPrint}_{name}";

                            UpdateProgressBar(i + baseIndex, totalIndices, unityPath, assetName);

                            var asset = CreateInstance<VehicleParametersSobj>();
                            asset.value = boosterParts[i];
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
            var title = $"Importing {typeof(CarDataSobj).Name} ({currentIndexStr}/{total})";
            var info = $"{unityPath}/{fileName}";
            var progress = index / (float)total;
            EditorUtility.DisplayProgressBar(title, info, progress);
        }
    }
}