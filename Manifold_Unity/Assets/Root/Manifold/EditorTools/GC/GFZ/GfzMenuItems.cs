namespace Manifold.EditorTools.GC.GFZ
{
    public static class GfzMenuItems
    {
        public static class Colliders
        {
            public const string ColiMenu = Const.Menu.Manifold + "Colliders/";
            // ColliderImport
            public const string Import = ColiMenu + "Create collider meshes";
            public const string Import256 = ColiMenu + "Create 256 static collider meshes (single scene, select type)";
            public const int ImportPriority = 1;
            public const int Import256Priority = 101;
        }

        public static class GMA
        {
            public const string GmaMenu = Const.Menu.Manifold + "GMA/";
            // GmaImport
            public const string ImportGma = GmaMenu + "Import all models from Source Folder";
            public const string ImportGmaAllRegions = GmaMenu + "Test - Import all models from all regions";
            public const int ImportGmaPriority = 1;
            public const int ImportGmaAllRegionsPriority = 101;
            // GmaTestMenuItems
            public const string LoadSaveToDisk = GmaMenu + "Test - Load\\Save source folder GMAs to Disk (file output)";
            public const string LoadSaveToMemory = GmaMenu + "Test - Load\\Save source folder GMAs in RAM (no file output)";
            public const int LoadSaveToDiskPriority = 102;
            public const int LoadSaveToMemoryPriority = 103;
        }

        public static class Stage
        {
            public const string SceneMenu = Const.Menu.Manifold + "Scenes/";
            // SceneImportUtility
            public const string ImportAll = SceneMenu + "Import All Stages";
            public const string ImportSingle = SceneMenu + "Import Stage (Single)";
            public const string ImportSingleSelect = SceneMenu + "Import Stage (Single, Select File)";
            public const int ImportAllPriority = 1;
            public const int ImportSinglePriority = 2;
            public const int ImportSingleSelectPriority = 3;
            // SceneExportUtility
            public const string ExportActiveScene = SceneMenu + "Export (Active Scene)";
            public const int ExportActiveScenePriority = 101;
        }

    }
}