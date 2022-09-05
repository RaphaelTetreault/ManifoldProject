using GameCube.GFZ.TPL;
using Manifold.IO;
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

        /// <summary>
        /// Add GfzTrack Component as child.
        /// </summary>
        [MenuItem(GfzMenuItems.TrackCreate.AddTrack, priority = 1)]
        public static void AddTrack()
        {
            GameObject track = new GameObject("Track");
            //Undo.RegisterCreatedObjectUndo(track, "Add Track");

            track.AddComponent<GfzTrack>();

            return;
        }

        // Add GfzShape Component as child.
        public static void AddGfzShape(Transform parent, GFZRoadType roadType)
        {
            string objName = "";
            switch (roadType)
            {
                case GFZRoadType.NORMAL:
                    objName = "road";
                    break;
                case GFZRoadType.CIRCLE:
                    objName = "circle";
                    break;
                case GFZRoadType.CAPSULE:
                    objName = "capsule";
                    break;
            }

            GameObject obj = new GameObject(objName);
            //Undo.RegisterCreatedObjectUndo(obj, $"Add {objName}");
            //EditorUtility.SetDirty(obj);

            obj.transform.parent = parent;

            switch (roadType)
            {
                case GFZRoadType.NORMAL:
                    obj.AddComponent<GfzShapeRoad>();
                    break;
                case GFZRoadType.CIRCLE:
                    obj.AddComponent<GfzShapePipeCylinder>();
                    break;
                case GFZRoadType.CAPSULE:
                    obj.AddComponent<GfzShapeCapsule>();
                    break;
            }

        }

        // Add GfzPathSegment Component as child.
        public static void AddGfzPath(GFZPathType pathType, GFZRoadType roadType)
        {
            int idx = -1; // TODO: GET index

            GameObject track = GameObject.FindObjectOfType<GfzTrack>().gameObject;

            if (track == null)
            {
                return;
            }


            string objName = "";
            switch (pathType)
            {
                case GFZPathType.BEZIER_OLD:
                case GFZPathType.BEZIER:
                    GfzPathFixedBezier[] gfzPathFixedBeziers = GameObject.FindObjectsOfType<GfzPathFixedBezier>();
                    GfzPathBezier[] gfzPathBeziers = GameObject.FindObjectsOfType<GfzPathBezier>();
                    idx = gfzPathFixedBeziers.Length + gfzPathBeziers.Length;
                    objName = $"bezier ({idx})";
                    break;
                case GFZPathType.LINE:
                    GfzPathLine[] gfzPathLines = GameObject.FindObjectsOfType<GfzPathLine>();
                    idx = gfzPathLines.Length;
                    objName = $"ilne ({idx})";
                    break;
                case GFZPathType.SPIRAL:
                    GfzPathSpiral[] gfzPathSpirals = GameObject.FindObjectsOfType<GfzPathSpiral>();
                    idx = gfzPathSpirals.Length;
                    objName = $"spiral ({idx})";
                    break;
            }


            GameObject obj = new GameObject(objName);
            //Undo.RegisterCreatedObjectUndo(obj, $"Add Path");
            //EditorUtility.SetDirty(obj);

            //Undo.RecordObject(track, $"set {objName} as child of Track");
            obj.transform.parent = track.transform;
            //EditorUtility.SetDirty(track);

            //Undo.RecordObject(track, $"AddComponent {pathType}");
            switch (pathType)
            {
                case GFZPathType.BEZIER_OLD:
                    obj.AddComponent<GfzPathBezier>();
                    break;
                case GFZPathType.BEZIER:
                    obj.AddComponent<GfzPathFixedBezier>();
                    break;
                case GFZPathType.LINE:
                    obj.AddComponent<GfzPathLine>();
                    break;
                case GFZPathType.SPIRAL:
                    obj.AddComponent<GfzPathSpiral>();
                    break;
            }


            AddGfzShape(obj.transform, roadType);

            //EditorUtility.SetDirty(track);

            return;
        }

        // Normal Road
        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddBezier, priority = 100)]
        public static void AddPathBezierNormalRoad() => AddGfzPath(GFZPathType.BEZIER, GFZRoadType.NORMAL);

        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddLine, priority = 101)]
        public static void AddPathLineNormalRoad() => AddGfzPath(GFZPathType.LINE, GFZRoadType.NORMAL);

        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddSpiral, priority = 102)]
        public static void AddPathSprialNormalRoad() => AddGfzPath(GFZPathType.SPIRAL, GFZRoadType.NORMAL);

        [MenuItem(GfzMenuItems.TrackCreate.AddNormalRoad + GfzMenuItems.TrackCreate.AddBezierOld, priority = 999)]
        public static void AddPathBezierOldNormalRoad() => AddGfzPath(GFZPathType.BEZIER_OLD, GFZRoadType.NORMAL);


        // Circle Road
        [MenuItem(GfzMenuItems.TrackCreate.AddCircleRoad + GfzMenuItems.TrackCreate.AddBezier, priority = 100)]
        public static void AddPathBezierCircleRoad() => AddGfzPath(GFZPathType.BEZIER, GFZRoadType.CIRCLE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCircleRoad + GfzMenuItems.TrackCreate.AddLine, priority = 101)]
        public static void AddPathLineCircleRoad() => AddGfzPath(GFZPathType.LINE, GFZRoadType.CIRCLE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCircleRoad + GfzMenuItems.TrackCreate.AddSpiral, priority = 102)]
        public static void AddPathSprialCircleRoad() => AddGfzPath(GFZPathType.SPIRAL, GFZRoadType.CIRCLE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCircleRoad + GfzMenuItems.TrackCreate.AddBezierOld, priority = 999)]
        public static void AddPathBezierOldCircleRoad() => AddGfzPath(GFZPathType.BEZIER_OLD, GFZRoadType.CIRCLE);


        // Capsule Road
        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddBezier, priority = 100)]
        public static void AddPathBezierCapsuleRoad() => AddGfzPath(GFZPathType.BEZIER, GFZRoadType.CAPSULE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddLine, priority = 101)]
        public static void AddPathLineCapsuleRoad() => AddGfzPath(GFZPathType.LINE, GFZRoadType.CAPSULE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddSpiral, priority = 102)]
        public static void AddPathSprialCapsuleRoad() => AddGfzPath(GFZPathType.SPIRAL, GFZRoadType.CAPSULE);

        [MenuItem(GfzMenuItems.TrackCreate.AddCapsuleRoad + GfzMenuItems.TrackCreate.AddBezierOld, priority = 999)]
        public static void AddPathBezierOldCapsuleRoad() => AddGfzPath(GFZPathType.BEZIER_OLD, GFZRoadType.CAPSULE);

    }
}
