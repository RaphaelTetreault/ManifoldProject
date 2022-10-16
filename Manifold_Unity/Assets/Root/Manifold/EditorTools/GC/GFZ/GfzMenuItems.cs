namespace Manifold.EditorTools.GC.GFZ
{
    public static class GfzMenuItems
    {
        public static class BasePriority
        {
            public const int CreateTrackSegment = 0;
            public const int Stage = 20;
            public const int LZ = 202;
            public const int GMA = 204;
            public const int TPL = 206;
            public const int Colliders = 208;
            public const int Materials = 210;
            public const int ProjectWindow = 900;

        }


        public static class Colliders
        {
            public const string Menu = Const.Menu.Manifold + "Colliders/";
            // ColliderImport
            public const string Import = Menu + "Create collider meshes";
            public const string Import256 = Menu + "Create 256 static collider meshes (single scene, select type)";

            public class Priority
            {
                public const int Import = BasePriority.Colliders + 1;
                public const int Import256 = BasePriority.Colliders + 101;
            }
        }

        public static class GMA
        {
            public const string Menu = Const.Menu.Manifold + "GMA/";
            // GmaImport
            public const string ImportGma = Menu + "Import all models from Source Folder";
            public const string ImportGmaSingleScene = Menu + "Import only scene models from Source Folder";
            public const string ImportGmaAllRegions = Menu + "Test - Import all models from all regions";
            // GmaTestMenuItems
            public const string LoadSaveToDisk = Menu + "Test - Load\\Save source folder GMAs to Disk (file output)";
            public const string LoadSaveToMemory = Menu + "Test - Load\\Save source folder GMAs in RAM (no file output)";

            public class Priority
            {                // GmaImport
                public const int ImportGma = BasePriority.GMA + 1;
                public const int ImportGmaSingleScene = BasePriority.GMA + 2;
                public const int ImportGmaAllRegions = BasePriority.GMA + 101;
                // GmaTestMenuItems
                public const int LoadSaveToDisk = BasePriority.GMA + 102;
                public const int LoadSaveToMemory = BasePriority.GMA + 103;
            }
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

            public class Priority
            {
                public const int DecompressFromSource = BasePriority.LZ + 1;
                public const int DecompressAllAvLz = BasePriority.LZ + 2;
                public const int DecompressSingleAvLz = BasePriority.LZ + 3;
                public const int CompressSingleFileGx = BasePriority.LZ + 101;
                public const int CompressSingleFileAx = BasePriority.LZ + 102;
            }
        }

        public static class ProjectWindow
        {
            public const string Menu = Const.Menu.Manifold;
            // GfzProjectWindow
            public const string OpenNewWindow = Menu + "Open Settings Window";

            public class Priority
            {
                public const int OpenNewWindow = BasePriority.ProjectWindow;
            }

        }

        public static class Stage
        {
            public const string Menu = Const.Menu.Manifold + "Scenes/";
            // SceneImportUtility
            public const string ImportAll = Menu + "Import All Stages";
            public const string ImportSingle = Menu + "Import Stage (Single)";
            public const string ImportSingleSelect = Menu + "Import Stage (Single, Select File)";
            public const string TestPatchEnemyLine = Menu + "Test Patch Enemy Line";
            public const string DecryptEnemyLine = Menu + "Decrypt Enemy Line";
            // SceneExportUtility
            public const string ExportActiveScene = Menu + "Export (Active Scene)";
            // MeshDisplay
            public const string DeleteOldMeshDisplay = Menu + "Delete old MeshDisplays";

            public static class Priority
            {
                //
                public const int ExportActiveScene = BasePriority.Stage + 1;
                public const int ImportAll = BasePriority.Stage + 2;
                public const int ImportSingle = BasePriority.Stage + 3;
                public const int ImportSingleSelect = BasePriority.Stage + 4;
                //
                public const int DeleteOldMeshDisplay = BasePriority.Stage + 100;
                //
                public const int TestPatchEnemyLine = BasePriority.Stage + 111;
                public const int TestDecryptEnemyLine = BasePriority.Stage + 112;
            }
        }

        public static class TPL
        {
            public const string Menu = Const.Menu.Manifold + "TPL/";
            // SceneImportUtility
            public const string BuildHashReferenceObject = Menu + "Build hash reference objects";
            public const string ImportTexturesNoMipmips = Menu + "Import textures (no mipmaps, build hash reference objects)";
            public const string ImportTexturesWithMipmips = Menu + "Import textures with mipmaps (build hash reference objects)";

            public static class Priority
            {
                public const int BuildHashReferenceObject = BasePriority.TPL + 1;
                public const int ImportTexturesNoMipmips = BasePriority.TPL + 2;
                public const int ImportTexturesWithMipmips = BasePriority.TPL + 3;
            }
        }

        public static class Materials
        {
            public const string Menu = Const.Menu.Manifold + "Materials/";
            // UnityMaterialTempaltes
            public const string CreateEditorMaterials = Menu + "Create editor materials from textures";

            public static class Priority
            {
                public const int CreateEditorMaterials = BasePriority.Materials + 1;
            }
        }

        public static class CreateTrackSegment
        {
            public const string Menu = Const.Menu.Manifold + "Create Track Segment/";
            // Track manager object
            public const string CreateTrack = Menu + "Create Track";
            // Shapes
            private const string RoadShape = Menu + "Road/";
            private const string PipeCylinderShape = Menu + "Pipe or Cylinder/";
            private const string OpenPipeCylinderShape = Menu + "Pipe or Cylinder (Open)/";
            private const string CapsuleShape = Menu + "Capsule/";
            // Paths
            private const string BezierPath = "Beziér Path";
            private const string LinePath = "Line Path";
            private const string SpiralPath = "Spiral Path";
            // Road + Path
            public const string RoadBezier = RoadShape + BezierPath;
            public const string RoadLine = RoadShape + LinePath;
            public const string RoadSpiral = RoadShape + SpiralPath;
            //
            public const string PipeCylinderBezier = PipeCylinderShape + BezierPath;
            public const string PipeCylinderLine = PipeCylinderShape + LinePath;
            public const string PipeCylinderSpiral = PipeCylinderShape + SpiralPath;
            //
            public const string OpenPipeCylinderBezier = OpenPipeCylinderShape + BezierPath;
            public const string OpenPipeCylinderLine = OpenPipeCylinderShape + LinePath;
            public const string OpenPipeCylinderSpiral = OpenPipeCylinderShape + SpiralPath;
            //
            public const string CapsuleBezier = CapsuleShape + BezierPath;
            public const string CapsuleLine = CapsuleShape + LinePath;
            public const string CapsuleSpiral = CapsuleShape + SpiralPath;
            // Embeds
            public const string PropertyEmbed = Menu + "Property Embed";

            public static class Priority
            {
                public const int PriorityBase = BasePriority.CreateTrackSegment;
                public const int RoadBezier = PriorityBase + 1;
                public const int RoadLine = PriorityBase + 2;
                public const int RoadSpiral = PriorityBase + 3;
                //
                private const int PipeCylinderBase = RoadSpiral;
                public const int PipeCylinderBezier = PipeCylinderBase + 1;
                public const int PipeCylinderLine = PipeCylinderBase + 2;
                public const int PipeCylinderSpiral = PipeCylinderBase + 3;
                //
                private const int OpenPipeCylinderBase = PipeCylinderSpiral;
                public const int OpenPipeCylinderBezier = OpenPipeCylinderBase + 1;
                public const int OpenPipeCylinderLine = OpenPipeCylinderBase + 2;
                public const int OpenPipeCylinderSpiral = OpenPipeCylinderBase + 3;
                //
                private const int CapsuleBase = OpenPipeCylinderSpiral;
                public const int CapsuleBezier = CapsuleBase + 1;
                public const int CapsuleLine = CapsuleBase + 2;
                public const int CapsuleSpiral = CapsuleBase + 3;
                //
                public const int CreateTrack = PriorityBase + 100;
            }
        }

    }
}