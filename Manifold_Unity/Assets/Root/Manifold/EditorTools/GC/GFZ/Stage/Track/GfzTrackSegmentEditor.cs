//using System;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace Manifold.EditorTools.GC.GFZ.Stage.Track
//{
//    [CustomEditor(typeof(GfzTrackSegment))]
//    public class GfzTrackSegmentEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            var trackSegment = target as GfzTrackSegment;

//            GuiSimple.Label("Generation");
//            if (GUILayout.Button("Generate Animation Curve TRS"))
//                GenerateAnimationCurvesTRS(trackSegment);
//            EditorGUILayout.Separator();

//            base.OnInspectorGUI();
//        }

//        public void GenerateAnimationCurvesTRS(GfzTrackSegment trackSegment)
//        {
//            string undoMessage = $"{trackSegment.name}: generate animation TRS";
//            Undo.RegisterCompleteObjectUndo(trackSegment, undoMessage);
//            trackSegment.GenerateAnimationCurves();
//            EditorUtility.SetDirty(trackSegment);
//        }
//    }
//}
