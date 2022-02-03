using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [CustomEditor(typeof(GfzBezierSplineSegment))]
    public class GfzBezierSplineSegmentInspector : Editor
    {
        private const int steps = 10;
        private const float directionScale = 0.5f;

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
            for (int i = 1; i < spline.points.Length; i += 3)
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

            //Handles.color = Color.grey;
            //Handles.DrawLine(p0, p1);
            //Handles.DrawLine(p1, p2);
            //Handles.DrawLine(p2, p3);

            ////Handles.color = Color.white;
            //ShowDirections();
            //Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            spline = target as GfzBezierSplineSegment;
            if (GUILayout.Button("Add Curve"))
            {
                Undo.RecordObject(spline, $"Add curve to {nameof(GfzBezierSplineSegment)}");
                spline.AddCurve();
                EditorUtility.SetDirty(spline);
            }
        }
        private Vector3 ShowPoint(int index)
        {
            var point = spline.points[index];
            var p = handleTransform.TransformPoint(point.position);
            EditorGUI.BeginChangeCheck();
            p = Handles.DoPositionHandle(p, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Move {nameof(GfzBezierSplineSegment)} point");
                spline.points[index].position = handleTransform.InverseTransformPoint(p);
                EditorUtility.SetDirty(spline);
            }

            return p;
        }

        private void ShowDirections()
        {
            Handles.color = Color.green;
            Vector3 point = spline.GetPoint(0f);
            Vector3 direction = spline.GetDirection(0f) * directionScale;
            Handles.DrawLine(point, point + direction);
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
