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
        [SerializeField]
        private bool viewDefaultValues = false;


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
            //spline.SetControlPoint(0, bezier0);
            //DisplayBezierPoint(0);

            for (int i = 1; i <= spline.CurveCount; i++)
            {
                BezierPoint bezier1 = DisplayBezierPoint(i);
                //spline.SetControlPoint(i, bezier1);

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

            GuiSimple.DefaultScript(spline);

            // TODO: display the currently active bezier point
            //       allow editing array, but leave it separate


            if (GUILayout.Button("Add Bézier Point"))
            {
                Undo.RecordObject(spline, $"Add bézier point to {nameof(GfzBezierSplineSegment)}");
                spline.AddCurve();
                EditorUtility.SetDirty(spline);
            }

            int rows = spline.CurveCount / 10;
            for (int r = 0; r <= rows; r++)
            {
                int rowBaseCurr = (r+0) * 10;
                int rowBaseNext = (r+1) * 10;

                // Generate a label for each toolbar cell
                List<string> labels = new List<string>();
                for (int c = rowBaseCurr; c < rowBaseNext; c++)
                {
                    if (c <= spline.CurveCount)
                    {
                        labels.Add(c.ToString());
                    }
                    else
                    {
                        labels.Add(string.Empty);
                    }
                }

                // See if selected index applies to this row
                bool withinRowLower = selectedIndex >= rowBaseCurr;
                bool withinRowUpper = selectedIndex < rowBaseNext;
                // Get index, -1 if not for this row
                int index = withinRowLower && withinRowUpper ? selectedIndex : -1;
                // Display toolbar, get index returned (-1 if non-applicable row)
                int result = GUILayout.Toolbar(index, labels.ToArray());
                result += rowBaseCurr;

                // Set index if valid. If invalid row, we have result = -1, so this doesn't run.
                bool isValidIndex = result >= 0 && result <= spline.CurveCount;
                if (isValidIndex)
                {
                    selectedIndex = result;
                }
            }

            EditorGUILayout.Space();
            


            bool isIndexSelectable = selectedIndex >= 0 && selectedIndex < spline.ControlPointCount;
            if (isIndexSelectable)
            {
                EditorGUI.indentLevel++;
                DrawSelectedPointInspector(selectedIndex);
                EditorGUI.indentLevel--;
                //Repaint();
            }
            else
            {
                const string msg = "No bézier control point selected.";
                EditorGUILayout.HelpBox(msg, MessageType.Info);
            }


            EditorGUILayout.Space();
            viewDefaultValues = EditorGUILayout.Foldout(viewDefaultValues, "View Reorderable Array", EditorStyles.foldoutHeader);
            if (viewDefaultValues)
            {
                const string msg = "This view is meant for re-order bézier points only! Modify other data at your own risk.";
                EditorGUILayout.HelpBox(msg, MessageType.Warning);
                DrawDefaultInspector();
            }

            //
            Repaint();
        }
        private void DrawSelectedPointInspector(int index)
        {
            GUILayout.Label($"Bézier Point [{index}]", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            var bezier = spline.GetBezierPoint(index);
            bezier.mode = GuiSimple.EnumPopup(nameof(bezier.point), bezier.mode);
            bezier.point = GuiSimple.Vector3(nameof(bezier.point), bezier.point);
            bezier.inTangent = GuiSimple.Vector3(nameof(bezier.inTangent), bezier.inTangent);
            bezier.outTangent = GuiSimple.Vector3(nameof(bezier.outTangent), bezier.outTangent);
            bezier.width = GuiSimple.Float(nameof(bezier.width), bezier.width);
            bezier.roll = GuiSimple.Float(nameof(bezier.roll), bezier.roll);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetBezierPoint(index, bezier);
            }
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
            bool pointSelected = DoBezierHandle(pointPosition);
            bool inTangentSelected = DoBezierHandle(inTangentPosition);
            bool outTangentSelected = DoBezierHandle(outTangentPosition);
            bool bezierSelected = index == selectedIndex;

            if (pointSelected || inTangentSelected || outTangentSelected)
                selectedIndex = index;

            if (pointSelected)
                selectedPart = SelectedPart.point;
            if (inTangentSelected)
                selectedPart = SelectedPart.inTangent;
            if (outTangentSelected)
                selectedPart = SelectedPart.outTangent;

            if (bezierSelected && selectedPart == SelectedPart.point)
            {
                EditorGUI.BeginChangeCheck();
                var result = Handles.DoPositionHandle(pointPosition, handleRotation);
                var delta = result - pointPosition;
                pointPosition = result;
                if (EditorGUI.EndChangeCheck())
                {
                    string undoMessage = $"Move '{target.name}' {nameof(GfzBezierSplineSegment)}[{index}] point";
                    Undo.RecordObject(spline, undoMessage);
                    bezierPoint.point = handleTransform.InverseTransformPoint(pointPosition);
                    bezierPoint.inTangent = handleTransform.InverseTransformPoint(inTangentPosition + delta);
                    bezierPoint.outTangent = handleTransform.InverseTransformPoint(outTangentPosition + delta);
                    spline.SetBezierPoint(index, bezierPoint);
                    EditorUtility.SetDirty(spline);
                }
            }

            if (bezierSelected && selectedPart == SelectedPart.inTangent)
            {
                EditorGUI.BeginChangeCheck();
                inTangentPosition = Handles.DoPositionHandle(inTangentPosition, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    string undoMessage = $"Move '{target.name}' {nameof(GfzBezierSplineSegment)}[{index}] point";
                    Undo.RecordObject(spline, undoMessage);
                    bezierPoint.inTangent = handleTransform.InverseTransformPoint(inTangentPosition);
                    bezierPoint.outTangent = GetTangentFromMode(bezierPoint, bezierPoint.inTangent, bezierPoint.outTangent);
                    spline.SetBezierPoint(index, bezierPoint);
                    EditorUtility.SetDirty(spline);
                }
            }

            if (bezierSelected && selectedPart == SelectedPart.outTangent)
            {
                EditorGUI.BeginChangeCheck();
                outTangentPosition = Handles.DoPositionHandle(outTangentPosition, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    string undoMessage = $"Move '{target.name}' {nameof(GfzBezierSplineSegment)}[{index}] point";
                    Undo.RecordObject(spline, undoMessage);
                    bezierPoint.outTangent = handleTransform.InverseTransformPoint(outTangentPosition);
                    bezierPoint.inTangent = GetTangentFromMode(bezierPoint, bezierPoint.outTangent, bezierPoint.inTangent);
                    spline.SetBezierPoint(index, bezierPoint);
                    EditorUtility.SetDirty(spline);
                }
            }

            return bezierPoint;
        }


        private Vector3 GetTangentFromMode(BezierPoint bezierPoint, Vector3 at, Vector3 bt)
        {
            BezierControlPointMode mode = bezierPoint.mode;
            Vector3 point = bezierPoint.point;

            switch (mode)
            {
                case BezierControlPointMode.Mirrored:
                    {
                        // Direction from tangent to point
                        var direction = point - at;
                        // Direction added to point, mirror
                        var tangent = direction + point;
                        return tangent;
                    }

                case BezierControlPointMode.Aligned:
                    {
                        // Direction from tangent to point
                        var direction = point - at;
                        //
                        var magnitude = (point - bt).magnitude;
                        // Direction added to point, mirror
                        var tangent = direction.normalized * magnitude + point;
                        return tangent;
                    }

                case BezierControlPointMode.Free:
                    {
                        // Do nothing
                        return bt;
                    }

                default:
                    throw new NotImplementedException();
            }


            if (bezierPoint.mode == BezierControlPointMode.Mirrored)
            {
                // Direction from inTangent to point
                var direction = bezierPoint.point - bezierPoint.inTangent;
                // Direction added to point = outTangent
                bezierPoint.outTangent = direction + bezierPoint.point;
            }
        }


        private bool DoBezierHandle(Vector3 position)
        {
            var size = HandleUtility.GetHandleSize(position);
            var isSelected = Handles.Button(position, handleRotation, handleSize * size, pickSize * size, Handles.DotHandleCap);

            //if (isSelected)
            //{
            //    //
            //    EditorGUI.BeginChangeCheck();
            //    position = Handles.DoPositionHandle(position, handleRotation);
            //    if (EditorGUI.EndChangeCheck())
            //    {
            //        Undo.RecordObject(spline, $"Move {nameof(GfzBezierSplineSegment)} point");
            //        position = handleTransform.InverseTransformPoint(position);
            //        EditorUtility.SetDirty(spline);
            //    }
            //}

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
