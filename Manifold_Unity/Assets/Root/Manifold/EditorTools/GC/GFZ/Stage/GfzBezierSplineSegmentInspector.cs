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

        private const float splineThickness = lineThickness * 1.5f;
        private const float lineThickness = 2f;

        private static Color[] modeColors = {
            Color.white,
            Color.yellow,
            Color.cyan
        };

        private GfzBezierSplineSegment spline;
        private Transform root;
        private Quaternion handleRotation;

        private void OnSceneGUI()
        {
            // Set up editor variables
            spline = target as GfzBezierSplineSegment;
            root = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local
                ? root.rotation
                : Quaternion.identity;

            // Displays the bezier in Scene view
            // TODO: new version of ShowPoint
            BezierPoint bezier0 = DisplayEditableBezierPoint(0);
            //spline.SetControlPoint(0, bezier0);
            //DisplayBezierPoint(0);

            for (int i = 1; i <= spline.PointCount; i++)
            {
                BezierPoint bezier1 = DisplayEditableBezierPoint(i);
                //spline.SetControlPoint(i, bezier1);

                var p0 = root.TransformPoint(bezier0.position);
                var p1 = root.TransformPoint(bezier0.outTangent);
                var p2 = root.TransformPoint(bezier1.inTangent);
                var p3 = root.TransformPoint(bezier1.position);

                Handles.color = Color.grey;
                Handles.DrawLine(p0, p1, lineThickness);
                Handles.DrawDottedLine(p1, p2, 5f);
                Handles.DrawLine(p2, p3, lineThickness);
                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, splineThickness);

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

            int rows = spline.PointCount / 10;
            for (int r = 0; r <= rows; r++)
            {
                int rowBaseCurr = (r + 0) * 10;
                int rowBaseNext = (r + 1) * 10;

                // Generate a label for each toolbar cell
                List<string> labels = new List<string>();
                for (int c = rowBaseCurr; c < rowBaseNext; c++)
                {
                    if (c <= spline.PointCount)
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
                bool isValidIndex = result >= 0 && result <= spline.PointCount;
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
                const string msg = "No bézier control point selected. Select a control point in the Scene view or via the toolbar above.";
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

            var bezier = spline.GetBezierPoint(index);

            // POSITION
            EditorGUI.BeginChangeCheck();
            bezier.position = GuiSimple.Vector3(nameof(bezier.position), bezier.position);
            if (EditorGUI.EndChangeCheck())
            {
                var position = root.TransformPoint(bezier.position);
                EditPointPosition(index, position);
            }

            EditorGUILayout.Space();

            // MODE
            EditorGUI.BeginChangeCheck();
            bezier.tangentMode = GuiSimple.EnumPopup(nameof(bezier.tangentMode), bezier.tangentMode);
            if (EditorGUI.EndChangeCheck())
            {
                spline.SetBezierPoint(index, bezier);
            }

            // IN TANGENT
            EditorGUI.BeginChangeCheck();
            bezier.inTangent = GuiSimple.Vector3(nameof(bezier.inTangent), bezier.inTangent);
            if (EditorGUI.EndChangeCheck())
            {
                var inTangent = root.TransformPoint(bezier.inTangent);
                EditInTangent(index, inTangent);
            }

            // OUT TANGENT
            EditorGUI.BeginChangeCheck();
            bezier.outTangent = GuiSimple.Vector3(nameof(bezier.outTangent), bezier.outTangent);
            if (EditorGUI.EndChangeCheck())
            {
                var outTangent = root.TransformPoint(bezier.outTangent);
                EditOutTangent(index, outTangent);
            }

            EditorGUILayout.Space();

            // WIDTH
            EditorGUI.BeginChangeCheck();
            bezier.width = GuiSimple.Float(nameof(bezier.width), bezier.width);
            if (EditorGUI.EndChangeCheck())
            {
                spline.SetBezierPoint(index, bezier);
            }

            // ROLL
            EditorGUI.BeginChangeCheck();
            bezier.roll = GuiSimple.Float(nameof(bezier.roll), bezier.roll);
            if (EditorGUI.EndChangeCheck())
            {
                spline.SetBezierPoint(index, bezier);
            }
        }

        private enum SelectedPart
        {
            none, point, inTangent, outTangent
        }

        private int selectedIndex = -1;
        private SelectedPart selectedPart = SelectedPart.none;

        public void EditPointPosition(int index, Vector3 localPosition)
        {
            BezierPoint bezierPoint = spline.GetBezierPoint(index);

            // Compute new position relative to transform, get delta between positions
            var newPosition = localPosition; // pre-transformed
            var oldPosition = root.TransformPoint(bezierPoint.position);
            var positionDelta = newPosition - oldPosition;

            // Get tangents
            var inTangentPosition = root.TransformPoint(bezierPoint.inTangent);
            var outTangentPosition = root.TransformPoint(bezierPoint.outTangent);

            //
            string undoMessage = $"Move '{target.name}' {nameof(GfzBezierSplineSegment)}[{index}] point";
            Undo.RecordObject(spline, undoMessage);
            bezierPoint.position = root.InverseTransformPoint(newPosition);
            bezierPoint.inTangent = root.InverseTransformPoint(inTangentPosition + positionDelta);
            bezierPoint.outTangent = root.InverseTransformPoint(outTangentPosition + positionDelta);
            spline.SetBezierPoint(index, bezierPoint);
            EditorUtility.SetDirty(spline);
        }

        public void EditInTangent(int index, Vector3 localPosition)
        {
            BezierPoint bezierPoint = spline.GetBezierPoint(index);

            // 
            var inTangentPosition = localPosition;

            //
            string undoMessage = $"Move '{target.name}' {nameof(GfzBezierSplineSegment)}[{index}] point";
            Undo.RecordObject(spline, undoMessage);
            bezierPoint.inTangent = root.InverseTransformPoint(inTangentPosition);
            bezierPoint.outTangent = GetTangentFromMode(bezierPoint, bezierPoint.inTangent, bezierPoint.outTangent);
            spline.SetBezierPoint(index, bezierPoint);
            EditorUtility.SetDirty(spline);
        }

        public void EditOutTangent(int index, Vector3 localPosition)
        {
            BezierPoint bezierPoint = spline.GetBezierPoint(index);

            // 
            var outTangentPosition = localPosition;

            //
            string undoMessage = $"Move '{target.name}' {nameof(GfzBezierSplineSegment)}[{index}] point";
            Undo.RecordObject(spline, undoMessage);
            bezierPoint.outTangent = root.InverseTransformPoint(outTangentPosition);
            bezierPoint.inTangent = GetTangentFromMode(bezierPoint, bezierPoint.outTangent, bezierPoint.inTangent);
            spline.SetBezierPoint(index, bezierPoint);
            EditorUtility.SetDirty(spline);
        }

        private BezierPoint DisplayEditableBezierPoint(int index)
        {
            BezierPoint bezierPoint = spline.GetBezierPoint(index);
            var mode = bezierPoint.tangentMode;
            var pointPosition = root.TransformPoint(bezierPoint.position);
            var inTangentPosition = root.TransformPoint(bezierPoint.inTangent);
            var outTangentPosition = root.TransformPoint(bezierPoint.outTangent);
            var color = modeColors[(int)mode];

            bool isFirstPoint = index == 0;
            bool isLastPoint = index == spline.PointCount;

            Handles.color = color;
            bool pointSelected = DoBezierHandle(pointPosition);
            // Only visualize/edit in-tangent if not first, out-tangent if not last
            bool inTangentSelected = !isFirstPoint ? DoBezierHandle(inTangentPosition) : false;
            bool outTangentSelected = !isLastPoint ? DoBezierHandle(outTangentPosition) : false;
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
                if (EditorGUI.EndChangeCheck())
                {
                    EditPointPosition(index, result);
                }
            }

            if (bezierSelected && selectedPart == SelectedPart.inTangent)// && !isFirstPoint)
            {
                EditorGUI.BeginChangeCheck();
                inTangentPosition = Handles.DoPositionHandle(inTangentPosition, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    EditInTangent(index, inTangentPosition);
                }
            }

            if (bezierSelected && selectedPart == SelectedPart.outTangent)// && !isLastPoint)
            {
                EditorGUI.BeginChangeCheck();
                outTangentPosition = Handles.DoPositionHandle(outTangentPosition, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    EditOutTangent(index, outTangentPosition);
                }
            }

            return bezierPoint;
        }

        private Vector3 GetTangentFromMode(BezierPoint bezierPoint, Vector3 at, Vector3 bt)
        {
            BezierControlPointMode mode = bezierPoint.tangentMode;
            Vector3 point = bezierPoint.position;

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
        }

        private bool DoBezierHandle(Vector3 position)
        {
            var size = HandleUtility.GetHandleSize(position);
            var isSelected = Handles.Button(position, handleRotation, handleSize * size, pickSize * size, Handles.DotHandleCap);
            return isSelected;
        }

        private void ShowDirections()
        {
            Handles.color = Color.green;
            Vector3 point = spline.GetPoint(0f);
            Vector3 direction = spline.GetDirection(0f) * directionScale;
            Handles.DrawLine(point, point + direction);
            int steps = stepsPerCurve * spline.PointCount;
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
