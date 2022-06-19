namespace Manifold.EditorTools.GC.GFZ.GMA
{
    public static class GfzMenuItems
    {
        public const string GmaMenu = Const.Menu.Manifold + "GMA/";
        public const string ColiMenu = Const.Menu.Manifold + "Colliders/";

        // ColliderImport
        public const string ColiImport = ColiMenu + "Create collider meshes";
        public const string ColiImport256 = ColiMenu + "Create 256 static collider meshes (single scene, select type)";
        public const int ColiImportPriority = 1;
        public const int ColiImport256Priority = 101;

        // GmaImport
        public const string ImportGma = GmaMenu + "Import all models from Source Folder";
        public const string ImportGmaAllRegions = GmaMenu + "Test - Import all models from all regions";
        public const int ImportGmaPriority = 1;
        public const int ImportGmaAllRegionsPriority = 101;
        // GmaTestMenuItems
        public const string GmaLoadSaveToDisk = GmaMenu + "Test - Load\\Save source folder GMAs to Disk (file output)";
        public const string GmaLoadSaveToMemory = GmaMenu + "Test - Load\\Save source folder GMAs in RAM (no file output)";
        public const int GmaLoadSaveToDiskPriority = 102;
        public const int GmaLoadSaveToMemoryPriority = 103;

    }
}