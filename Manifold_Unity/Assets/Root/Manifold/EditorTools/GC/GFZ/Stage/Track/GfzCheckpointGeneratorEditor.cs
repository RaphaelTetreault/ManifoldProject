﻿using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GfzCheckpointGenerator))]
    public class GfzCheckpointGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var trackSegment = target as GfzCheckpointGenerator;

            base.OnInspectorGUI();
            //GuiSimple.Label("Generation", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Debug Checkpoints"))
                GenerateCheckpointDebug(trackSegment);
            GUI.color = Color.red;
            if (GUILayout.Button("Delete Debug Checkpoints"))
                DeleteCheckpointDebug(trackSegment);
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }

        public void GenerateCheckpointDebug(GfzCheckpointGenerator checkpointGenerator)
        {
            DeleteCheckpointDebug(checkpointGenerator);

            var checkpoints = CheckpointUtility.CreateCheckpoints(checkpointGenerator, checkpointGenerator.IsGfzCoordinateSpace);

            int index = 0;
            foreach (var checkpoint in checkpoints)
            {
                var gobj = new GameObject($"Checkpoint[{index++}]");
                gobj.transform.parent = checkpointGenerator.transform;
                var script = gobj.AddComponent<GfzCheckpoint>();
                script.SetCheckpointData(checkpoint);
                script.Mesh = checkpointGenerator.GizmosMesh;
            }
        }

        public void DeleteCheckpointDebug(GfzCheckpointGenerator trackSegment)
        {
            var checkpoints = trackSegment.GetComponentsInChildren<GfzCheckpoint>();
            foreach (var checkpoint in checkpoints)
            {
                DestroyImmediate(checkpoint.gameObject);
            }
        }

    }
}
