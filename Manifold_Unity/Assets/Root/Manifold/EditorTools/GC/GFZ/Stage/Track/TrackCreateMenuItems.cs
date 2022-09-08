using GameCube.GFZ.TPL;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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

    public enum RoadType
    {
        Normal,
        CylinderPipe,
        Capsule
    }

    public static class TrackCreateMenuItems
    {
        const string NamePathBezier = "bezier";
        const string NamePathLine = "line";
        const string NamePathSpiral = "spiral";

        const string NameRoadNormal = "road";
        const string NameRoadCylindePipe = "circle";
        const string NameRoadCupsule = "capsule";

        /// <summary>
        ///  Generate GfzTrack component
        /// </summary>
        [MenuItem(GfzMenuItems.TrackCreate.AddTrack, priority = 1)]
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
        /// <param name="roadType"></param>
        /// <param name="obj"></param>
        public static void AddGfzShape(RoadType roadType, GameObject obj)
        {
            switch (roadType)
            {
                case RoadType.Normal:
                    Undo.AddComponent<GfzShapeRoad>(obj);
                    break;
                case RoadType.CylinderPipe:
                    Undo.AddComponent<GfzShapePipeCylinder>(obj);
                    break;
                case RoadType.Capsule:
                    Undo.AddComponent<GfzShapeCapsule>(obj);
                    break;
            }

        }

        /// <summary>
        /// Generate GfzPathSegment extended component as child
        /// </summary>
        /// <param name="pathType"></param>
        /// <param name="roadType"></param>
        public static void GenerateGfzPath(PathType pathType, RoadType roadType)
        {
            int idx = -1;
            GfzTrack track;

            try
            {
                track = GameObject.FindObjectOfType<GfzTrack>();
            } catch (Exception e) {
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
            switch (roadType)
            {
                case RoadType.Normal:
                    roadName = NameRoadNormal;
                    break;
                case RoadType.CylinderPipe:
                    roadName = NameRoadCylindePipe;
                    break;
                case RoadType.Capsule:
                    roadName = NameRoadCupsule;
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
            AddGfzShape(roadType, roadShape);

            return;
        }


        // Normal Road
        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddBezier, priority = 100)]
        public static void AddPathBezierNormalRoad() => GenerateGfzPath(PathType.Bezier, RoadType.Normal);

        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddLine, priority = 101)]
        public static void AddPathLineNormalRoad() => GenerateGfzPath(PathType.Line, RoadType.Normal);

        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddSpiral, priority = 102)]
        public static void AddPathSprialNormalRoad() => GenerateGfzPath(PathType.Spiral, RoadType.Normal);

        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddBezierOld, priority = 999)]
        public static void AddPathBezierOldNormalRoad() => GenerateGfzPath(PathType.BezierOld, RoadType.Normal);


        // CylinderPipe Road
        [MenuItem(GfzMenuItems.TrackCreate.AddCylinderPipeRoad + GfzMenuItems.TrackCreate.AddBezier, priority = 100)]
        public static void AddPathBezierCylinderPipeRoad() => GenerateGfzPath(PathType.Bezier, RoadType.CylinderPipe);

        [MenuItem(GfzMenuItems.TrackCreate.AddCylinderPipeRoad + GfzMenuItems.TrackCreate.AddLine, priority = 101)]
        public static void AddPathLineCylinderPipeRoad() => GenerateGfzPath(PathType.Line, RoadType.CylinderPipe);

        [MenuItem(GfzMenuItems.TrackCreate.AddCylinderPipeRoad + GfzMenuItems.TrackCreate.AddSpiral, priority = 102)]
        public static void AddPathSprialCylinderPipeRoad() => GenerateGfzPath(PathType.Spiral, RoadType.CylinderPipe);

        [MenuItem(GfzMenuItems.TrackCreate.AddCylinderPipeRoad + GfzMenuItems.TrackCreate.AddBezierOld, priority = 999)]
        public static void AddPathBezierOldCylinderPipeRoad() => GenerateGfzPath(PathType.BezierOld, RoadType.CylinderPipe);


        // Capsule Road
        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddBezier, priority = 100)]
        public static void AddPathBezierCapsuleRoad() => GenerateGfzPath(PathType.Bezier, RoadType.Capsule);

        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddLine, priority = 101)]
        public static void AddPathLineCapsuleRoad() => GenerateGfzPath(PathType.Line, RoadType.Capsule);

        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddSpiral, priority = 102)]
        public static void AddPathSprialCapsuleRoad() => GenerateGfzPath(PathType.Spiral, RoadType.Capsule);

        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddBezierOld, priority = 999)]
        public static void AddPathBezierOldCapsuleRoad() => GenerateGfzPath(PathType.BezierOld, RoadType.Capsule);

    }
}
