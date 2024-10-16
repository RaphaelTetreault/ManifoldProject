using System;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public enum PathType
    {
        BezierOld,
        Bezier,
        Line,
        Spiral
    }

    public enum ShapeType
    {
        Normal,
        CylinderPipe,
        Capsule,
        OpenPipeCylinder,
        SquareCorner,
        Modulated
    }

    public static class TrackCreateMenuItems
    {
        const string NamePathBezier = "bezier";
        const string NamePathLine = "line";
        const string NamePathSpiral = "spiral";

        const string NameShapeNormal = "road";
        const string NameShapeCylinderPipe = "circle";
        const string NameShapeCapsule = "capsule";
        const string NameShapeOpenPipeCylinder = "open-circle";
        const string NameShapeModulated = "modulated";
        const string NameShapeSquareCorner = "square";

        /// <summary>
        ///  Generate GfzTrack component
        /// </summary>
        [MenuItem(GfzMenuItems.CreateTrackSegment.CreateTrack, priority = GfzMenuItems.CreateTrackSegment.Priority.CreateTrack)]
        public static void GenerateTrack()
        {
            GameObject track = new GameObject("Track");
            Undo.RegisterCreatedObjectUndo(track, "Add Track");
            Undo.AddComponent<GfzTrack>(track);

            return;
        }

        /// <summary>
        /// Add GfzPathSegment extended component
        /// </summary>
        /// <param name="pathType"></param>
        /// <param name="obj"></param>
        public static void AddGfzPath(PathType pathType, GameObject obj)
        {
            switch (pathType)
            {
                case PathType.BezierOld:
                    Undo.AddComponent<GfzPathBezier>(obj);
                    break;
                case PathType.Bezier:
                    Undo.AddComponent<GfzPathFixedBezier>(obj);
                    break;
                case PathType.Line:
                    Undo.AddComponent<GfzPathLine>(obj);
                    break;
                case PathType.Spiral:
                    Undo.AddComponent<GfzPathSpiral>(obj);
                    break;
            }

            return;
        }

        /// <summary>
        /// Aling to previous generated PathSegment
        /// </summary>
        /// <param name="pathType"></param>
        /// <param name="track"></param>
        /// <param name="obj"></param>
        public static void AlignPrev(PathType pathType, GfzTrack track, GameObject obj)
        {
            // Get last of GfzPathSegment
            GfzPathSegment prev = track.AllRoots[track.AllRoots.Length - 2];
            // Get last position, rotation, scale
            var hacTRS = prev.CreateHierarchichalAnimationCurveTRS(false);
            float maxTime = hacTRS.GetMaxTime();
            var matrix = hacTRS.AnimationCurveTRS.EvaluateMatrix(maxTime);

            switch (pathType)
            {
                case PathType.BezierOld:
                    Undo.RecordObject(obj, $"Change Bezier {obj.name}");
                    GfzPathBezier bezier = obj.GetComponent<GfzPathBezier>();
                    // ControlPoint0
                    BezierPoint controlPoint = bezier.GetBezierPoint(0);
                    controlPoint.position = matrix.Position();
                    controlPoint.outTangent = matrix.Position() + (matrix.rotation * Vector3.forward * 100f);
                    controlPoint.roll = matrix.RotationEuler().z;
                    bezier.SetBezierPoint(0, controlPoint);
                    // ControlPoint1
                    controlPoint = bezier.GetBezierPoint(1);
                    controlPoint.position = matrix.Position() + (matrix.rotation * Vector3.forward * CourseConst.GoodAverageLength);
                    controlPoint.inTangent = matrix.Position() + (matrix.rotation * Vector3.forward * (CourseConst.GoodAverageLength - 100f));
                    controlPoint.roll = matrix.RotationEuler().z;
                    bezier.SetBezierPoint(1, controlPoint);
                    break;
                case PathType.Bezier:
                    Undo.RecordObject(obj, $"Change FixedBezier {obj.name}");
                    GfzPathFixedBezier fixedBezier = obj.GetComponent<GfzPathFixedBezier>();
                    fixedBezier.SetControlPoints(matrix.Position(), matrix.rotation);
                    break;
                case PathType.Line:
                case PathType.Spiral:
                    Undo.RecordObject(obj, $"Change Transform {obj.name}");
                    obj.transform.position = matrix.Position();
                    obj.transform.rotation = matrix.rotation;
                    break;
            }

            return;

        }

        /// <summary>
        /// Add GfzShape component
        /// </summary>
        /// <param name="shapeType"></param>
        /// <param name="obj"></param>
        public static void AddGfzShape(ShapeType shapeType, GameObject obj)
        {
            switch (shapeType)
            {
                case ShapeType.Normal:
                    Undo.AddComponent<GfzShapeRoad>(obj);
                    break;
                case ShapeType.CylinderPipe:
                    Undo.AddComponent<GfzShapePipeCylinder>(obj);
                    break;
                case ShapeType.Capsule:
                    Undo.AddComponent<GfzShapeCapsule>(obj);
                    break;
                case ShapeType.OpenPipeCylinder:
                    Undo.AddComponent<GfzShapeOpenPipeCylinder>(obj);
                    break;
                case ShapeType.Modulated:
                    Undo.AddComponent<GfzShapeRoadModulated>(obj);
                    break;
                case ShapeType.SquareCorner:
                    Undo.AddComponent<GfzShapeSquareCorner>(obj);
                    break;
            }

        }

        /// <summary>
        /// Generate GfzPathSegment extended component as child
        /// </summary>
        /// <param name="pathType"></param>
        /// <param name="shapeType"></param>
        public static void GenerateGfzPath(PathType pathType, ShapeType shapeType)
        {
            int idx = -1;
            GfzTrack track;

            try
            {
                track = GameObject.FindObjectOfType<GfzTrack>();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"GfzTrack object not exist.\r\nAuto Generate Track");
                Debug.Log($"Actual Exception: {e.Message}");
                GenerateTrack();
                track = GameObject.FindObjectOfType<GfzTrack>();
            }

            if (track == null)
            {
                Debug.LogWarning($"GfzTrack object not exist.\r\nAuto Generate Track");
                GenerateTrack();
                track = GameObject.FindObjectOfType<GfzTrack>();
            }

            // Generate GfzPathSegment GameObject
            string pathName = "";
            switch (pathType)
            {
                case PathType.BezierOld:
                case PathType.Bezier:
                    GfzPathFixedBezier[] gfzPathFixedBeziers = GameObject.FindObjectsOfType<GfzPathFixedBezier>();
                    GfzPathBezier[] gfzPathBeziers = GameObject.FindObjectsOfType<GfzPathBezier>();
                    idx = gfzPathFixedBeziers.Length + gfzPathBeziers.Length;
                    pathName = $"{NamePathBezier} ({idx})";
                    break;
                case PathType.Line:
                    GfzPathLine[] gfzPathLines = GameObject.FindObjectsOfType<GfzPathLine>();
                    idx = gfzPathLines.Length;
                    pathName = $"{NamePathLine} ({idx})";
                    break;
                case PathType.Spiral:
                    GfzPathSpiral[] gfzPathSpirals = GameObject.FindObjectsOfType<GfzPathSpiral>();
                    idx = gfzPathSpirals.Length;
                    pathName = $"{NamePathSpiral} ({idx})";
                    break;
            }
            GameObject pathSegment = new GameObject(pathName);

            string roadName = "";
            switch (shapeType)
            {
                case ShapeType.Normal:
                    roadName = NameShapeNormal;
                    break;
                case ShapeType.CylinderPipe:
                    roadName = NameShapeCylinderPipe;
                    break;
                case ShapeType.Capsule:
                    roadName = NameShapeCapsule;
                    break;
                case ShapeType.OpenPipeCylinder:
                    roadName = NameShapeOpenPipeCylinder;
                    break;
                case ShapeType.Modulated:
                    roadName = NameShapeModulated;
                    break;
                case ShapeType.SquareCorner:
                    roadName = NameShapeSquareCorner;
                    break;
            }
            GameObject roadShape = new GameObject(roadName);
            roadShape.transform.parent = pathSegment.transform;
            pathSegment.transform.parent = track.gameObject.transform;
            Undo.RegisterCreatedObjectUndo(pathSegment, $"Generate {pathName}");

            // Select Generated object
            Selection.activeGameObject = pathSegment;

            AddGfzPath(pathType, pathSegment);
            if (track.AllRoots.Length > 1)
            {
                AlignPrev(pathType, track, pathSegment);
            }
            AddGfzShape(shapeType, roadShape);

            return;
        }


        // Normal Road
        [MenuItem(GfzMenuItems.CreateTrackSegment.RoadBezier, priority = GfzMenuItems.CreateTrackSegment.Priority.RoadBezier)]
        public static void AddPathBezierNormalRoad() => GenerateGfzPath(PathType.Bezier, ShapeType.Normal);

        [MenuItem(GfzMenuItems.CreateTrackSegment.RoadLine, priority = GfzMenuItems.CreateTrackSegment.Priority.RoadLine)]
        public static void AddPathLineNormalRoad() => GenerateGfzPath(PathType.Line, ShapeType.Normal);

        [MenuItem(GfzMenuItems.CreateTrackSegment.RoadSpiral, priority = GfzMenuItems.CreateTrackSegment.Priority.RoadSpiral)]
        public static void AddPathSprialNormalRoad() => GenerateGfzPath(PathType.Spiral, ShapeType.Normal);

        //[MenuItem(GfzMenuItems.CreateTrackSegment.RoadBezier + "(legacy)", priority = 999)]
        public static void AddPathBezierOldNormalRoad() => GenerateGfzPath(PathType.BezierOld, ShapeType.Normal);


        // CylinderPipe Road
        [MenuItem(GfzMenuItems.CreateTrackSegment.PipeCylinderBezier, priority = GfzMenuItems.CreateTrackSegment.Priority.PipeCylinderBezier)]
        public static void AddPathBezierCylinderPipeRoad() => GenerateGfzPath(PathType.Bezier, ShapeType.CylinderPipe);

        [MenuItem(GfzMenuItems.CreateTrackSegment.PipeCylinderLine, priority = GfzMenuItems.CreateTrackSegment.Priority.PipeCylinderLine)]
        public static void AddPathLineCylinderPipeRoad() => GenerateGfzPath(PathType.Line, ShapeType.CylinderPipe);

        [MenuItem(GfzMenuItems.CreateTrackSegment.PipeCylinderSpiral, priority = GfzMenuItems.CreateTrackSegment.Priority.PipeCylinderSpiral)]
        public static void AddPathSprialCylinderPipeRoad() => GenerateGfzPath(PathType.Spiral, ShapeType.CylinderPipe);

        //[MenuItem(GfzMenuItems.CreateTrackSegment.PipeCylinderBezier + "(legacy)", priority = 999)]
        public static void AddPathBezierOldCylinderPipeRoad() => GenerateGfzPath(PathType.BezierOld, ShapeType.CylinderPipe);


        // OpenPipeCylinder Road
        [MenuItem(GfzMenuItems.CreateTrackSegment.OpenPipeCylinderBezier, priority = GfzMenuItems.CreateTrackSegment.Priority.OpenPipeCylinderBezier)]
        public static void AddPathBezierOpenPipeCylinderRoad() => GenerateGfzPath(PathType.Bezier, ShapeType.OpenPipeCylinder);

        [MenuItem(GfzMenuItems.CreateTrackSegment.OpenPipeCylinderLine, priority = GfzMenuItems.CreateTrackSegment.Priority.OpenPipeCylinderLine)]
        public static void AddPathLineOpenPipeCylinderRoad() => GenerateGfzPath(PathType.Line, ShapeType.OpenPipeCylinder);

        [MenuItem(GfzMenuItems.CreateTrackSegment.OpenPipeCylinderSpiral, priority = GfzMenuItems.CreateTrackSegment.Priority.OpenPipeCylinderSpiral)]
        public static void AddPathSprialOpenPipeCylinderRoad() => GenerateGfzPath(PathType.Spiral, ShapeType.OpenPipeCylinder);

        //[MenuItem(GfzMenuItems.CreateTrackSegment.OpenPipeCylinderBezier + "(legacy)", priority = 999)]
        public static void AddPathBezierOldOpenPipeCylinderRoad() => GenerateGfzPath(PathType.BezierOld, ShapeType.OpenPipeCylinder);


        // Capsule Road
        [MenuItem(GfzMenuItems.CreateTrackSegment.CapsuleBezier, priority = GfzMenuItems.CreateTrackSegment.Priority.CapsuleBezier)]
        public static void AddPathBezierCapsuleRoad() => GenerateGfzPath(PathType.Bezier, ShapeType.Capsule);

        [MenuItem(GfzMenuItems.CreateTrackSegment.CapsuleLine, priority = GfzMenuItems.CreateTrackSegment.Priority.CapsuleLine)]
        public static void AddPathLineCapsuleRoad() => GenerateGfzPath(PathType.Line, ShapeType.Capsule);

        [MenuItem(GfzMenuItems.CreateTrackSegment.CapsuleSpiral, priority = GfzMenuItems.CreateTrackSegment.Priority.CapsuleSpiral)]
        public static void AddPathSprialCapsuleRoad() => GenerateGfzPath(PathType.Spiral, ShapeType.Capsule);

        //[MenuItem(GfzMenuItems.CreateTrackSegment.CapsuleBezier + "(legacy)", priority = 999)]
        public static void AddPathBezierOldCapsuleRoad() => GenerateGfzPath(PathType.BezierOld, ShapeType.Capsule);


        // Modulated Road
        [MenuItem(GfzMenuItems.CreateTrackSegment.ModulatedBezier, priority = GfzMenuItems.CreateTrackSegment.Priority.ModulatedBezier)]
        public static void AddPathBezierModulatedRoad() => GenerateGfzPath(PathType.Bezier, ShapeType.Modulated);

        [MenuItem(GfzMenuItems.CreateTrackSegment.ModulatedLine, priority = GfzMenuItems.CreateTrackSegment.Priority.ModulatedLine)]
        public static void AddPathLineModulatedRoad() => GenerateGfzPath(PathType.Line, ShapeType.Modulated);

        [MenuItem(GfzMenuItems.CreateTrackSegment.ModulatedSpiral, priority = GfzMenuItems.CreateTrackSegment.Priority.ModulatedSpiral)]
        public static void AddPathSprialModulatedRoad() => GenerateGfzPath(PathType.Spiral, ShapeType.Modulated);

        //[MenuItem(GfzMenuItems.CreateTrackSegment.ModulatedBezier + "(legacy)", priority = 999)]
        public static void AddPathBezierOldModulatedRoad() => GenerateGfzPath(PathType.BezierOld, ShapeType.Modulated);


    }
}
