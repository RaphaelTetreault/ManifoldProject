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
            public const string ImportGmaSingleScene = Menu + "Import only scene models from Source Folder";
            public const string ImportGmaAllRegions = Menu + "Test - Import all models from all regions";
            public const int ImportGmaPriority = 1;
            public const int ImportGmaSingleScenePriority = 2;
            public const int ImportGmaAllRegionsPriority = 101;
            // GmaTestMenuItems
            public const string LoadSaveToDisk = Menu + "Test - Load\\Save source folder GMAs to Disk (file output)";
            public const string LoadSaveToMemory = Menu + "Test - Load\\Save source folder GMAs in RAM (no file output)";
            public const int LoadSaveToDiskPriority = 102;
            public const int LoadSaveToMemoryPriority = 103;
        }

        public static class Lz
        {
            public const string Menu = Const.Menu.Manifold + "LZ Compression Tools/";
            // LzMenuItems
            public const string DecompressFromSource = Menu + "Decompress all LZ files in source directory";
            public const string DecompressAllAvLz = Menu + "Decompress all LZ files in select directory tree";
            public const string DecompressSingleAvLz = Menu + "Decompress single LZ file";
            public const string CompressSingleFileGx = Menu + "Compress single file with AV LZ (GX)";
            public const string CompressSingleFileAx = Menu + "Compress single file with AV LZ (AX)";
            public const int DecompressFromSourcePriority = 0;
            public const int DecompressAllAvLzPriority = 1;
            public const int DecompressSingleAvLzPriority = 2;
            public const int CompressSingleFileGxPriority = 101;
            public const int CompressSingleFileAxPriority = 102;
        }

        public static class ProjectWindow
        {
            public const string Menu = Const.Menu.Manifold;
            // GfzProjectWindow
            public const string OpenNewWindow = Menu + "Open Settings Window";
            public const int OpenNewWindowPriority = 101;
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

        public static class TPL
        {
            public const string Menu = Const.Menu.Manifold + "TPL/";
            // SceneImportUtility
            public const string BuildHashReferenceObject = Menu + "Build hash reference objects";
            public const string ImportTexturesNoMipmips = Menu + "Import textures (no mipmaps, build hash reference objects)";
            public const string ImportTexturesWithMipmips = Menu + "Import textures with no mipmaps (build hash reference objects)";

            public static class Priority
            {
                public const int BuildHashReferenceObject = 1;
                public const int ImportTexturesNoMipmips = 2;
                public const int ImportTexturesWithMipmips = 3;
            }
        }

        public static class Materials
        {
            public const string Menu = Const.Menu.Manifold + "Materials/";
            // UnityMaterialTempaltes
            public const string CreateEditorMaterials = Menu + "Create editor materials from textures";

            public static class Priority
            {
                public const int CreateEditorMaterials = 1;
            }
        }

    }
}