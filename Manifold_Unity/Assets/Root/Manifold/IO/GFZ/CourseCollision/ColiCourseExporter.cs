using GameCube.GFZ;
using GameCube.GFZ.CourseCollision;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Manifold.IO.GFZ.CourseCollision
{

    [CreateAssetMenu(menuName = Const.Menu.GfzCourseCollision + "COLI Exporter")]
    public class ColiCourseExporter : ExecutableScriptableObject,
        IExportable
    {
        [Header("Export Settings")]
        [SerializeField]
        protected IOOption exportOptions = IOOption.selectedFiles;

        [SerializeField]
        protected ColiScene.SerializeFormat format = ColiScene.SerializeFormat.GX;

        [SerializeField, BrowseFolderField, Tooltip("Used for ExportOptions.ExportAllOfTypeInFolder")]
        protected string exportFrom = string.Empty;

        [SerializeField, BrowseFolderField]
        protected string exportTo;

        [SerializeField]
        protected bool allowOverwritingFiles = true;

        [SerializeField]
        protected bool preserveFolderStructure = true;

        [SerializeField]
        protected bool exportCompressed = false;

        [Header("Preferences")]
        [SerializeField]
        protected bool openFolderAfterExport = true;

        [Header("Exports")]
        [SerializeField]
        protected ColiSceneSobj[] exportSobjs;
        [SerializeField]
        protected UnityEditor.SceneAsset[] scenes;


        [Header("Testing")]
        [SerializeField]
        protected bool exportHandMade;
        [SerializeField]
        protected bool serializeVerbose = true;

        [Header("Export Overrides")]
        [SerializeField] protected bool exclude;

        public override string ExecuteText => "Export COLI_COURSE";

        public override void Execute() => Export();

        public void Export()
        {
            ColiCourseUtility.SerializeVerbose = serializeVerbose;

            // Construct ColiScene from Unity Scene "scenes"
            var coliScenes = new ColiScene[scenes.Length];

            // TODO: move to static script?
            // This way you can have editor export instead of through this dang scriptable object!

            for (int sceneIndex = 0; sceneIndex < coliScenes.Length; sceneIndex++)
            {
                // DEBUG
                const bool findInactive = true;
                const int width = 32;
                const string padding = "-";

                // 
                coliScenes[sceneIndex] = new ColiScene();

                // Breakout value
                var scene = scenes[sceneIndex];
                var coliScene = coliScenes[sceneIndex];

                // Set serialization format
                coliScene.Format = format;

                // Get path to scene object
                var scenePath = AssetDatabase.GetAssetPath(scene);
                // Open scene in Unity...
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                // ... so we can start doing the usual Unity methods to find stuff

                // Get scene parameters for general info
                var sceneParameters = FindObjectsOfType<GfzSceneParameters>();
                Assert.IsTrue(sceneParameters.Length == 1);
                var sceneParams = sceneParameters[0];

                // Start logging!
                var internalName = sceneParams.GetGfzInternalName();
                var displayName = sceneParams.GetGfzDisplayName();
                var dateTime = DateTime.Now;
                var timestamp = $"[{dateTime:yyyy-MM-dd}][{dateTime:HH-mm-ss}]";
                var logPath = Path.Combine(exportTo, $"{timestamp} {internalName} - {displayName}.txt");
                var log = new MarkdownTextLogger(logPath);

                // Write file and course title
                log.WriteHeading("FILE INFORMATION", padding, width);
                log.WriteLine($"File: {internalName}");
                log.WriteLine($"Name: {displayName}");
                log.WriteLine();

                // Validate venue and id
                {
                    bool isValidIndexForVenue = CourseUtility.GetVenue(sceneParams.courseIndex) == sceneParams.venue;
                    byte r = isValidIndexForVenue ? (byte)0 : (byte)255;
                    log.WriteLineColor($"{nameof(isValidIndexForVenue)} = {isValidIndexForVenue}", r, 0, 0);
                }

                // Get scene-wide parameters from SceneParameters
                {
                    // Set filename to what F-Zero GX/AX would use
                    coliScene.FileName = internalName;
                    coliScene.CourseName = sceneParams.courseName;
                    coliScene.Venue = sceneParams.venue;
                    coliScene.Author = sceneParams.author;
                    // Construct range from 2 parameters
                    coliScene.unkRange0x00 = new Range(sceneParams.rangeNear, sceneParams.rangeFar);
                    // Use functions to get form parameters
                    coliScene.fog = sceneParams.ToGfzFog();
                    coliScene.fogCurves = sceneParams.ToGfzFogCurves();

                    var fogMsg = sceneParams.exportCustomFog
                        ? $"{nameof(Fog)}: using custom fog."
                        : $"{nameof(Fog)}: using default fog for {sceneParams.venue}.";
                    log.WriteLine(fogMsg);

                    var fogCurvesMsg = sceneParams.exportCustomFog
                        ? $"{nameof(FogCurves)}: using custom fog curves."
                        : $"{nameof(FogCurves)}: using default fog curves for {sceneParams.venue}.";
                    log.WriteLine(fogCurvesMsg);
                    log.WriteLine();
                }

                // Static Collider Meshes
                {
                    coliScene.staticColliderMeshes = new StaticColliderMeshes(format);
                }

                // Triggers
                {
                    var arcadeCheckpointTriggers = FindObjectsOfType<GfzArcadeCheckpoint>(findInactive);
                    coliScene.arcadeCheckpointTriggers = GetGfzValues(arcadeCheckpointTriggers);

                    // This trigger type is a mess... Get all 3 representations, combine, assign.
                    // Collect all trigger types. They all get converted to the same GFZ base type.
                    var objectPaths = FindObjectsOfType<GfzObjectPath>(findInactive);
                    var storyCapsules = FindObjectsOfType<GfzStoryCapsule>(findInactive);
                    var unknownMetadataTriggers = FindObjectsOfType<GfzUnknownCourseMetadataTrigger>(findInactive);
                    // Make a list, add range for each type
                    var courseMetadataTriggers = new List<CourseMetadataTrigger>();
                    courseMetadataTriggers.AddRange(GetGfzValues(objectPaths));
                    courseMetadataTriggers.AddRange(GetGfzValues(storyCapsules));
                    courseMetadataTriggers.AddRange(GetGfzValues(unknownMetadataTriggers));
                    // Convert list to array, assign to ColiScene
                    coliScene.courseMetadataTriggers = courseMetadataTriggers.ToArray();

                    // TODO:
                    // story object triggers

                    // This trigger type is a mess... Get all 3 representations, combine, assign.
                    var unknownTriggers = FindObjectsOfType<GfzUnknownTrigger>(findInactive);
                    coliScene.unknownTriggers = GetGfzValues(unknownTriggers);

                    var unknownSolsTriggers = FindObjectsOfType<GfzUnknownSolsTrigger>(findInactive);
                    coliScene.unknownSolsTriggers = GetGfzValues(unknownSolsTriggers);

                    var visualEffectTriggers = FindObjectsOfType<GfzVisualEffectTrigger>(findInactive);
                    coliScene.visualEffectTriggers = GetGfzValues(visualEffectTriggers);


                    log.WriteLine("TRIGGERS");
                    log.WriteLineSummary(coliScene.arcadeCheckpointTriggers);
                    // Log. TODO: more granularity in type.
                    log.WriteLineSummary(coliScene.courseMetadataTriggers);
                    log.WriteLineSummary(coliScene.unknownTriggers);
                    log.WriteLineSummary(coliScene.unknownSolsTriggers);
                    log.WriteLineSummary(coliScene.visualEffectTriggers);
                    log.WriteLine();
                }

                // Scene Objects / Instances / References / Names
                {
                    var sceneObjects = FindObjectsOfType<GfzSceneObject>(findInactive);
                    coliScene.sceneObjects = new SceneObject[0];

                    // TODO: construct the actual objects...

                    log.WriteLine();
                    log.WriteLine("SCENE OBJECTS");
                    log.WriteLineSummary(coliScene.sceneObjects);
                    log.WriteLineSummary(coliScene.sceneOriginObjects);
                    log.WriteLineSummary(coliScene.sceneInstances);
                    log.WriteLineSummary(coliScene.sceneObjectReferences);
                    log.WriteLineSummary(coliScene.objectNames);
                    log.WriteLine();
                }

                // Track Data
                {
                    var controlPoints = FindObjectsOfType<GfzControlPoint>(findInactive);

                    //coliScene.

                    // Parse all CPs, get root nodes only?
                    // Serialization could then run from root through chicldren, respecting array pointer seriialization.

                    //
                    coliScene.trackCheckpointMatrix = new TrackCheckpointMatrix();

                    // TODO: this is temp data
                    coliScene.trackLength = new TrackLength();
                    coliScene.trackMinHeight = new TrackMinHeight();
                }
                Debug.LogWarning("TODO: define bounds");

                // Export the file
                var exported = ExportUtility.ExportSerializable(coliScene, exportTo, "", allowOverwritingFiles);
                log.WriteHeading("EXPORT", padding, width);
                log.WriteLine($"Exported file to path \"{exported}\"");
                log.WriteLine();

                // Write out all types and their address in file
                log.WriteHeading("SERIALIZATION SUMMARY", padding, width);
                log.WriteLine();
                //
                log.WriteAddress(coliScene.fog);
                log.WriteAddress(coliScene.fogCurves);
                log.WriteLine();
                //
                log.WriteAddress(coliScene.trackLength);
                log.WriteAddress(coliScene.trackMinHeight);
                log.WriteAddress(coliScene.trackCheckpointMatrix);
                log.WriteAddress(coliScene.trackNodes);
                log.WriteLine();
                // checkpoints
                // segments
                //
                log.WriteHeading("STATIC COLLISION", padding, width);
                log.WriteAddress(coliScene.staticColliderMeshes);
                log.WriteLine();
                log.WriteLine("TRIANGLES");
                log.WriteAddress(coliScene.staticColliderMeshes.colliderTriangles);
                log.WriteAddress(coliScene.staticColliderMeshes.triMeshIndexMatrices);
                foreach (var triIndexList in coliScene.staticColliderMeshes.triMeshIndexMatrices)
                    if (triIndexList != null)
                    log.WriteAddress(triIndexList.indexLists);
                log.WriteLine("QUADS");
                log.WriteAddress(coliScene.staticColliderMeshes.colliderQuads);
                log.WriteAddress(coliScene.staticColliderMeshes.quadMeshIndexMatrices);
                foreach (var quadIndexList in coliScene.staticColliderMeshes.quadMeshIndexMatrices)
                    if (quadIndexList != null)
                        log.WriteAddress(quadIndexList.indexLists);
                // triggers
                log.WriteAddress(coliScene.arcadeCheckpointTriggers);
                log.WriteAddress(coliScene.courseMetadataTriggers);
                log.WriteAddress(coliScene.storyObjectTriggers);
                log.WriteAddress(coliScene.unknownSolsTriggers);
                log.WriteAddress(coliScene.unknownTriggers);
                log.WriteAddress(coliScene.visualEffectTriggers);
                log.WriteLine();


                log.Close();
                OSUtility.OpenDirectory(openFolderAfterExport, exported);
                OSUtility.OpenDirectory(openFolderAfterExport, logPath);
            }

            //
            // NOTE: no "preserve structure"
            // TODO: make these export functions less long.
            var exportedFiles = ExportUtility.ExportSerializable(coliScenes, exportTo, "", allowOverwritingFiles);
            OSUtility.OpenDirectory(openFolderAfterExport, exportedFiles);
        }


        public static T[] GetGfzValues<T>(IGfzConvertable<T>[] unity)
        {
            var gfz = new T[unity.Length];
            for (int i = 0; i < unity.Length; i++)
                gfz[i] = unity[i].ExportGfz();

            return gfz;
        }
    }
}
