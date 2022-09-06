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
    public enum GFZPathType
    {
        BEZIER_OLD,
        BEZIER,
        LINE,
        SPIRAL
    }

    public enum GFZRoadType
    {
        NORMAL,
        CIRCLE,
        CAPSULE
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
        public static void AddGfzPath(GFZPathType pathType, GameObject obj)
        {
            switch (pathType)
            {
                case GFZPathType.BEZIER_OLD:
                    Undo.AddComponent<GfzPathBezier>(obj);
                    break;
                case GFZPathType.BEZIER:
                    Undo.AddComponent<GfzPathFixedBezier>(obj);
                    break;
                case GFZPathType.LINE:
                    Undo.AddComponent<GfzPathLine>(obj);
                    break;
                case GFZPathType.SPIRAL:
                    Undo.AddComponent<GfzPathSpiral>(obj);
                    break;
            }

            return;
        }

        /// <summary>
        /// Aling to previous Generated PathSegment
        /// </summary>
        /// <param name="pathType"></param>
        /// <param name="track"></param>
        /// <param name="obj"></param>
        public static void AlignPrev(GFZPathType pathType, GfzTrack track, GameObject obj)
        {
            // Get last of GfzPathSegment
            GfzPathSegment prev = track.AllRoots[track.AllRoots.Length - 2];
            // Get last position, rotation, scale
            var hacTRS = prev.CreateHierarchichalAnimationCurveTRS(false);
            float maxTime = hacTRS.GetMaxTime();
            var matrix = hacTRS.AnimationCurveTRS.EvaluateMatrix(maxTime);

            switch (pathType)
            {
                case GFZPathType.BEZIER_OLD:
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
                case GFZPathType.BEZIER:
                    Undo.RecordObject(obj, $"Change FixedBezier {obj.name}");
                    GfzPathFixedBezier fixedBezier = obj.GetComponent<GfzPathFixedBezier>();
                    fixedBezier.SetControlPoints(matrix.Position(), matrix.rotation);
                    break;
                case GFZPathType.LINE:
                case GFZPathType.SPIRAL:
                    Undo.RecordObject(obj, $"Change Transform {obj.name}");
                    obj.transform.position = matrix.Position();
                    obj.transform.rotation = matrix.rotation;
                    break;
            }

            return;

        }

        /// <summary>
        /// Add GfzShape Component
        /// </summary>
        /// <param name="roadType"></param>
        /// <param name="obj"></param>
        public static void AddGfzShape(GFZRoadType roadType, GameObject obj)
        {
            switch (roadType)
            {
                case GFZRoadType.NORMAL:
                    Undo.AddComponent<GfzShapeRoad>(obj);
                    break;
                case GFZRoadType.CIRCLE:
                    Undo.AddComponent<GfzShapePipeCylinder>(obj);
                    break;
                case GFZRoadType.CAPSULE:
                    Undo.AddComponent<GfzShapeCapsule>(obj);
                    break;
            }

        }

        /// <summary>
        /// Generate GfzPathSegment Extended Component as child
        /// </summary>
        /// <param name="pathType"></param>
        /// <param name="roadType"></param>
        public static void GenerateGfzPath(GFZPathType pathType, GFZRoadType roadType)
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
                case GFZPathType.BEZIER_OLD:
                case GFZPathType.BEZIER:
                    GfzPathFixedBezier[] gfzPathFixedBeziers = GameObject.FindObjectsOfType<GfzPathFixedBezier>();
                    GfzPathBezier[] gfzPathBeziers = GameObject.FindObjectsOfType<GfzPathBezier>();
                    idx = gfzPathFixedBeziers.Length + gfzPathBeziers.Length;
                    pathName = $"{NamePathBezier} ({idx})";
                    break;
                case GFZPathType.LINE:
                    GfzPathLine[] gfzPathLines = GameObject.FindObjectsOfType<GfzPathLine>();
                    idx = gfzPathLines.Length;
                    pathName = $"{NamePathLine} ({idx})";
                    break;
                case GFZPathType.SPIRAL:
                    GfzPathSpiral[] gfzPathSpirals = GameObject.FindObjectsOfType<GfzPathSpiral>();
                    idx = gfzPathSpirals.Length;
                    pathName = $"{NamePathSpiral} ({idx})";
                    break;
            }
            GameObject pathSegment = new GameObject(pathName);

            string roadName = "";
            switch (roadType)
            {
                case GFZRoadType.NORMAL:
                    roadName = NameRoadNormal;
                    break;
                case GFZRoadType.CIRCLE:
                    roadName = NameRoadCylindePipe;
                    break;
                case GFZRoadType.CAPSULE:
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
        public static void AddPathBezierNormalRoad() => GenerateGfzPath(GFZPathType.BEZIER, GFZRoadType.NORMAL);

        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddLine, priority = 101)]
        public static void AddPathLineNormalRoad() => GenerateGfzPath(GFZPathType.LINE, GFZRoadType.NORMAL);

        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddSpiral, priority = 102)]
        public static void AddPathSprialNormalRoad() => GenerateGfzPath(GFZPathType.SPIRAL, GFZRoadType.NORMAL);

        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddBezierOld, priority = 999)]
        public static void AddPathBezierOldNormalRoad() => GenerateGfzPath(GFZPathType.BEZIER_OLD, GFZRoadType.NORMAL);


        // Circle Road
        [MenuItem(GfzMenuItems.TrackCreate.AddCircleRoad + GfzMenuItems.TrackCreate.AddBezier, priority = 100)]
        public static void AddPathBezierCircleRoad() => GenerateGfzPath(GFZPathType.BEZIER, GFZRoadType.CIRCLE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCircleRoad + GfzMenuItems.TrackCreate.AddLine, priority = 101)]
        public static void AddPathLineCircleRoad() => GenerateGfzPath(GFZPathType.LINE, GFZRoadType.CIRCLE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCircleRoad + GfzMenuItems.TrackCreate.AddSpiral, priority = 102)]
        public static void AddPathSprialCircleRoad() => GenerateGfzPath(GFZPathType.SPIRAL, GFZRoadType.CIRCLE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCircleRoad + GfzMenuItems.TrackCreate.AddBezierOld, priority = 999)]
        public static void AddPathBezierOldCircleRoad() => GenerateGfzPath(GFZPathType.BEZIER_OLD, GFZRoadType.CIRCLE);


        // Capsule Road
        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddBezier, priority = 100)]
        public static void AddPathBezierCapsuleRoad() => GenerateGfzPath(GFZPathType.BEZIER, GFZRoadType.CAPSULE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddLine, priority = 101)]
        public static void AddPathLineCapsuleRoad() => GenerateGfzPath(GFZPathType.LINE, GFZRoadType.CAPSULE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddSpiral, priority = 102)]
        public static void AddPathSprialCapsuleRoad() => GenerateGfzPath(GFZPathType.SPIRAL, GFZRoadType.CAPSULE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddBezierOld, priority = 999)]
        public static void AddPathBezierOldCapsuleRoad() => GenerateGfzPath(GFZPathType.BEZIER_OLD, GFZRoadType.CAPSULE);

    }
}
