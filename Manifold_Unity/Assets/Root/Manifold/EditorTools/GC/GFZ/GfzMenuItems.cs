namespace Manifold.EditorTools.GC.GFZ
{
    public static class GfzMenuItems
    {
        public static class Colliders
        {
            public const string Menu = Const.Menu.Manifold + "Colliders/";
            // ColliderImport
            public const string Import = Menu + "Create collider meshes";
            public const string Import256 = Menu + "Create 256 static collider meshes (single scene, select type)";
            public const int ImportPriority = 1;
            public const int Import256Priority = 101;
        }

        public static class GMA
        {
            public const string Menu = Const.Menu.Manifold + "GMA/";
            // GmaImport
            public const string ImportGma = Menu + "Import all models from Source Folder";
            public const string ImportGmaAllRegions = Menu + "Test - Import all models from all regions";
            public const int ImportGmaPriority = 1;
            public const int ImportGmaAllRegionsPriority = 101;
            // GmaTestMenuItems
            public const string LoadSaveToDisk = Menu + "Test - Load\\Save source folder GMAs to Disk (file output)";
            public const string LoadSaveToMemory = Menu + "Test - Load\\Save source folder GMAs in RAM (no file output)";
            public const int LoadSaveToDiskPriority = 102;
            public const int LoadSaveToMemoryPriority = 103;
        }

        public static class Stage
        {
            public const string Menu = Const.Menu.Manifold + "Scenes/";
            // SceneImportUtility
            public const string ImportAll = Menu + "Import All Stages";
            public const string ImportSingle = Menu + "Import Stage (Single)";
            public const string ImportSingleSelect = Menu + "Import Stage (Single, Select File)";
            public const int ImportAllPriority = 1;
            public const int ImportSinglePriority = 2;
            public const int ImportSingleSelectPriority = 3;
            // SceneExportUtility
            public const string ExportActiveScene = Menu + "Export (Active Scene)";
            public const int ExportActiveScenePriority = 101;
        }

    }
}