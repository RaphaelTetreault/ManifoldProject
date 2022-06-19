namespace Manifold.EditorTools.GC.GFZ.GMA
{
    public static class GfzMenuItems
    {
        public const string RootMenu = Const.Menu.Manifold + "GMA/";

        // GmaImport
        public const string ImportGma = RootMenu + "Import all models from Source Folder";
        public const string ImportGmaAllRegions = RootMenu + "Test - Import all models from all regions";
        public const int ImportGmaPriority = 1;
        public const int ImportGmaAllRegionsPriority = 101;

        // GmaTestMenuItems
        public const string GmaLoadSaveToDisk = RootMenu + "Test - Load\\Save source folder GMAs to Disk (file output)";
        public const string GmaLoadSaveToMemory = RootMenu + "Test - Load\\Save source folder GMAs in RAM (no file output)";
        public const int GmaLoadSaveToDiskPriority = 102;
        public const int GmaLoadSaveToMemoryPriority = 103;

    }
}