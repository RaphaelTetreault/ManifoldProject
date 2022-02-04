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

            // Displays the bezier in Scene view
            // TODO: new version of ShowPoint
            BezierPoint bezier0 = DisplayBezierPoint(0);
            //DisplayBezierPoint(0);

            for (int i = 1; i <= spline.CurveCount; i++)
            {
                BezierPoint bezier1 = DisplayBezierPoint(i);

                var p0 = handleTransform.TransformPoint(bezier0.point);
                var p1 = handleTransform.TransformPoint(bezier0.outTangent);
                var p2 = handleTransform.TransformPoint(bezier1.inTangent);
                var p3 = handleTransform.TransformPoint(bezier1.point);

                Handles.color = Color.grey;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);
                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);

                bezier0 = bezier1;
            }
            //ShowDirections();
        }

        public override void OnInspectorGUI()
        {
            spline = target as GfzBezierSplineSegment;

            //if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
            //{
            //    DrawSelectedPointInspector();
            //    //Repaint();
            //}

            if (GUILayout.Button("Add Curve"))
            {
                Undo.RecordObject(spline, $"Add curve to {nameof(GfzBezierSplineSegment)}");
                spline.AddCurve();
                EditorUtility.SetDirty(spline);
            }

            //
            DrawDefaultInspector();
            Repaint();
        }
        private void DrawSelectedPointInspector(Vector3 point)
        {
            //GUILayout.Label("Selected Point");

            //EditorGUI.BeginChangeCheck();
            ////
            //var point = spline.GetControlPoint(selectedIndex);
            //Vector3 position = EditorGUILayout.Vector3Field("Position", point);
            ////
            //if (EditorGUI.EndChangeCheck())
            //{
            //    Undo.RecordObject(spline, "Move Point");
            //    EditorUtility.SetDirty(spline);
            //    spline.SetControlPoint(selectedIndex, position);
            //}

            //// Let user change bezier mode between curves
            //EditorGUI.BeginChangeCheck();
            //BezierControlPointMode mode = (BezierControlPointMode)
            //    EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
            //if (EditorGUI.EndChangeCheck())
            //{
            //    Undo.RecordObject(spline, "Change Point Mode");
            //    spline.SetControlPointMode(selectedIndex, mode);
            //    EditorUtility.SetDirty(spline);
            //}
        }

        private enum SelectedPart
        {
            none, point, inTangent, outTangent
        }

        private int selectedIndex = -1;
        private SelectedPart selectedPart = SelectedPart.none;

        private BezierPoint DisplayBezierPoint(int index)
        {
            BezierPoint bezierPoint = spline.GetBezierPoint(index);
            var mode = bezierPoint.mode;
            var pointPosition = handleTransform.TransformPoint(bezierPoint.point);
            var inTangentPosition = handleTransform.TransformPoint(bezierPoint.inTangent);
            var outTangentPosition = handleTransform.TransformPoint(bezierPoint.outTangent);
            var color = modeColors[(int)mode];

            Handles.color = color;
            if(DoBezierHandle(ref pointPosition))
            {
                selectedIndex = index;
                selectedPart = SelectedPart.point;
                bezierPoint.point = pointPosition;
            }

            if (DoBezierHandle(ref inTangentPosition))
            {
                selectedIndex = index;
                selectedPart = SelectedPart.point;
                bezierPoint.inTangent = inTangentPosition;
                if (index != 0)
                {
                    //var prevBezier = spline.GetBezierPoint(index - 1);
                    //prevBezier.
                }
            }

            if (DoBezierHandle(ref outTangentPosition))
            {
                selectedIndex = index;
                selectedPart = SelectedPart.point;
                bezierPoint.outTangent = outTangentPosition;

            }

            return bezierPoint;
        }

        private bool DoBezierHandle(ref Vector3 position)
        {
            var size = HandleUtility.GetHandleSize(position);
            var isSelected = Handles.Button(position, handleRotation, handleSize * size, pickSize * size, Handles.DotHandleCap);

            if (isSelected)
            {
                //
                EditorGUI.BeginChangeCheck();
                position = Handles.DoPositionHandle(position, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, $"Move {nameof(GfzBezierSplineSegment)} point");
                    position = handleTransform.InverseTransformPoint(position);
                    EditorUtility.SetDirty(spline);
                }
            }

            return isSelected;
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
