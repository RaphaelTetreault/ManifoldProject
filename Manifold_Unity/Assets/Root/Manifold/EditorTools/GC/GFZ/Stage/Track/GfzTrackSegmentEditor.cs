using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzTrackSegment))]
    public class GfzTrackSegmentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var trackSegment = target as GfzTrackSegment;

            GuiSimple.Label("Generation");

            if (GUILayout.Button("Generate Animation Curve TRS"))
                GenerateAnimationCurvesTRS(trackSegment);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Generate Debug Checkpoints"))
                    GenerateCheckpointDebug(trackSegment);
                GUI.color = Color.red;
                if (GUILayout.Button("Delete Debug Checkpoints"))
                    DeleteCheckpointDebug(trackSegment);
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            base.OnInspectorGUI();
        }

        public void GenerateCheckpointDebug(GfzTrackSegment trackSegment)
        {
            var checkpoints = trackSegment.CreateCheckpoints(false);

            int index = 0;
            foreach (var checkpoint in checkpoints)
            {
                var gobj = new GameObject($"Checkpoint[{index++}]");
                gobj.transform.parent = trackSegment.transform;
                var script = gobj.AddComponent<GfzCheckpoint>();
                script.Init(checkpoint);
            }
        }

        public void DeleteCheckpointDebug(GfzTrackSegment trackSegment)
        {
            var checkpoints = trackSegment.GetComponentsInChildren<GfzCheckpoint>();
            foreach (var checkpoint in checkpoints)
            {
                DestroyImmediate(checkpoint.gameObject);
            }
        }

        public void GenerateAnimationCurvesTRS(GfzTrackSegment trackSegment)
        {
            string undoMessage = $"{trackSegment.name}: generate animation TRS";
            Undo.RegisterCompleteObjectUndo(trackSegment, undoMessage);
            trackSegment.GenerateAnimationCurves();
            EditorUtility.SetDirty(trackSegment);
        }
    }
}
