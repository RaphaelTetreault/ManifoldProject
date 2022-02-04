using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [CustomEditor(typeof(GfzBezierSplineSegment))]
    public class GfzBezierSplineSegmentInspector : Editor
    {
        private const int stepsPerCurve = 10;
        private const float directionScale = 0.5f;
        private const float handleSize = 0.05f;
        private const float pickSize = handleSize * 1.5f;

        private static Color[] modeColors = {
            Color.white,
            Color.yellow,
            Color.cyan
        };

        private GfzBezierSplineSegment spline;
        private Transform handleTransform;
        private Quaternion handleRotation;

        private void OnSceneGUI()
        {
            // Set up editor variables
            spline = target as GfzBezierSplineSegment;
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local
                ? handleTransform.rotation
                : Quaternion.identity;

            //
            Vector3 p0 = ShowPoint(0);
            for (int i = 1; i < spline.ControlPointCount; i += 3)
            {
                Vector3 p1 = ShowPoint(i);
                Vector3 p2 = ShowPoint(i + 1);
                Vector3 p3 = ShowPoint(i + 2);

                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                p0 = p3;
            }
            ShowDirections();
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            spline = target as GfzBezierSplineSegment;

            if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
            {
                DrawSelectedPointInspector();
                //Repaint();
            }

            if (GUILayout.Button("Add Curve"))
            {
                Undo.RecordObject(spline, $"Add curve to {nameof(GfzBezierSplineSegment)}");
                spline.AddCurve();
                EditorUtility.SetDirty(spline);
            }

            Repaint();
        }
        private void DrawSelectedPointInspector()
        {
            GUILayout.Label("Selected Point");

            // Let use change Vector3 position of node
            EditorGUI.BeginChangeCheck();
            var point = spline.GetControlPoint(selectedIndex);
            Vector3 position = EditorGUILayout.Vector3Field("Position", point);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(selectedIndex, position);
            }

            // Let user change bezier mode between curves
            EditorGUI.BeginChangeCheck();
            BezierControlPointMode mode = (BezierControlPointMode)
                EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Change Point Mode");
                spline.SetControlPointMode(selectedIndex, mode);
                EditorUtility.SetDirty(spline);
            }
        }

        private int selectedIndex = -1;
        private Vector3 ShowPoint(int index)
        {
            var point = spline.GetControlPoint(index);
            var p = handleTransform.TransformPoint(point);

            //
            var modeIndex = spline.GetControlPointMode(index);
            Handles.color = modeColors[(int)modeIndex];
            var size = HandleUtility.GetHandleSize(p);
            if (Handles.Button(p, handleRotation, handleSize * size, pickSize * size, Handles.DotHandleCap))
            {
                selectedIndex = index;
            }

            //
            if (selectedIndex == index)
            {
                EditorGUI.BeginChangeCheck();
                p = Handles.DoPositionHandle(p, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, $"Move {nameof(GfzBezierSplineSegment)} point");
                    var position = handleTransform.InverseTransformPoint(p);
                    spline.SetControlPoint(index, position);
                    EditorUtility.SetDirty(spline);
                }
            }

            return p;
        }

        private void ShowDirections()
        {
            Handles.color = Color.green;
            Vector3 point = spline.GetPoint(0f);
            Vector3 direction = spline.GetDirection(0f) * directionScale;
            Handles.DrawLine(point, point + direction);
            int steps = stepsPerCurve * spline.CurveCount;
            for (int i = 1; i <= steps; i++)
            {
                var time = i / (float)steps;
                direction = spline.GetDirection(time) * directionScale;
                point = spline.GetPoint(time);
                Handles.DrawLine(point, point + direction);
            }
        }

    }
}
