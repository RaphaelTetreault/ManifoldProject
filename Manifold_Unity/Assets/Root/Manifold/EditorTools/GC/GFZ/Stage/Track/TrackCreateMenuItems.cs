﻿using GameCube.GFZ.TPL;
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
        /// <summary>
        /// Add GfzTrack Component
        /// </summary>
        [MenuItem(GfzMenuItems.TrackCreate.AddTrack, priority = 1)]
        public static void AddTrack()
        {
            GameObject track = new GameObject("Track");
            Undo.RegisterCreatedObjectUndo(track, "Add Track");

            Undo.AddComponent<GfzTrack>(track);
            //track.AddComponent<GfzTrack>();

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
        /// Add GfzPathSegment extended Component
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
        /// Generate GfzPathSegment Extended Component as child
        /// </summary>
        /// <param name="pathType"></param>
        /// <param name="roadType"></param>
        public static void GenerateGfzPath(GFZPathType pathType, GFZRoadType roadType)
        {
            int idx = -1;
            GameObject track;

            try
            {
                track = GameObject.FindObjectOfType<GfzTrack>().gameObject;
            } catch(Exception e) {
                string[] menuPaths = GfzMenuItems.TrackCreate.AddTrack.Split("/");
                Debug.LogError($"GfzTrack object not exist.\r\nGenerate Track by {menuPaths[0]} > {menuPaths[1]} > {menuPaths[2]}");
                Debug.Log(e.Message);
                return;
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
                    pathName = $"bezier ({idx})";
                    break;
                case GFZPathType.LINE:
                    GfzPathLine[] gfzPathLines = GameObject.FindObjectsOfType<GfzPathLine>();
                    idx = gfzPathLines.Length;
                    pathName = $"ilne ({idx})";
                    break;
                case GFZPathType.SPIRAL:
                    GfzPathSpiral[] gfzPathSpirals = GameObject.FindObjectsOfType<GfzPathSpiral>();
                    idx = gfzPathSpirals.Length;
                    pathName = $"spiral ({idx})";
                    break;
            }
            GameObject pathSegment = new GameObject(pathName);

            string roadName = "";
            switch (roadType)
            {
                case GFZRoadType.NORMAL:
                    roadName = "road";
                    break;
                case GFZRoadType.CIRCLE:
                    roadName = "circle";
                    break;
                case GFZRoadType.CAPSULE:
                    roadName = "capsule";
                    break;
            }
            GameObject roadShape = new GameObject(roadName);
            roadShape.transform.parent = pathSegment.transform;
            pathSegment.transform.parent = track.transform;
            Undo.RegisterCreatedObjectUndo(pathSegment, $"Generate {pathName}");

            AddGfzShape(roadType, roadShape);
            AddGfzPath(pathType, pathSegment);

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


        [MenuItem(GfzMenuItems.TrackCreate.Menu + "Example/Create GameObject")]
        static void CreateGameObject()
        {
            // Create GameObject hierarchy.
            GameObject go = new GameObject("my GameObject");
            GameObject child = new GameObject();
            go.transform.position = new Vector3(5, 5, 5);
            child.transform.parent = go.transform;

            // Register root object for undo.
            Undo.RegisterCreatedObjectUndo(go, "Create object");
        }

    }
}
