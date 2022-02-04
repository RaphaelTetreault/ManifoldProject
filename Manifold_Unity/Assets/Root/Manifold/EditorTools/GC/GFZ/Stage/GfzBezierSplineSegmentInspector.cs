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

        // CONSIDER: make these public params on editor?
        private const int stepsPerCurve = 10;
        private const float directionScale = 50f;
        private const float handleSize = 0.06f;

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
            BezierPoint bezier0 = DisplayEditableBezierPoint(0);
            for (int i = 1; i <= spline.CurveCount; i++)
            {
                BezierPoint bezier1 = DisplayEditableBezierPoint(i);

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
            ShowDirections();
        }

        public override void OnInspectorGUI()
        {
            spline = target as GfzBezierSplineSegment;

            // Default Script field for MonoBehaviour components
            GuiSimple.DefaultScript(spline);

            //
            GuiSimple.Label("Global Fields", EditorStyles.boldLabel);
            {
                var buttonWidth = GUILayout.Width(128);
                // Widths Curves
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    var editedCurve = EditorGUILayout.CurveField("Width", spline.WidthCurve);
                    if (GUILayout.Button("Copy Bézier Widths", buttonWidth))
                    {
                        // do copy operation
                    }
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                    {
                        string undoMessage = $"Edit '{target.name}' {nameof(GfzBezierSplineSegment)} Width Curve";
                        Undo.RegisterCompleteObjectUndo(spline, undoMessage);
                        spline.WidthCurve = editedCurve;
                        EditorUtility.SetDirty(spline);
                    }
                }

                // Widths Curves
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    var editedCurve = EditorGUILayout.CurveField("Roll", spline.RollCurve);
                    if (GUILayout.Button("Copy Bézier Rolls", buttonWidth))
                    {
                        // do copy operation
                    }
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                    {
                        string undoMessage = $"Edit '{target.name}' {nameof(GfzBezierSplineSegment)} Roll Curve";
                        Undo.RegisterCompleteObjectUndo(spline, undoMessage);
                        spline.RollCurve = editedCurve;
                        EditorUtility.SetDirty(spline);
                    }
                }
            }

            // SHOW Array data and toolbar
            EditorGUILayout.Space();
            GuiSimple.Label("Bézier Points", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Bézier Point"))
            {
                Undo.RecordObject(spline, $"Add bézier point to {nameof(GfzBezierSplineSegment)}");
                spline.AddCurve();
                EditorUtility.SetDirty(spline);
            }
            int rows = spline.CurveCount / 10;
            for (int r = 0; r <= rows; r++)
            {
                int rowBaseCurr = (r + 0) * 10;
                int rowBaseNext = (r + 1) * 10;

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

            // SHOW bezier point preview/editor
            EditorGUILayout.Space();
            bool isIndexSelectable = selectedIndex >= 0 && selectedIndex <= spline.CurveCount;
            if (isIndexSelectable)
            {
                EditorGUI.indentLevel++;
                DrawSelectedPointInspector(selectedIndex);
                EditorGUI.indentLevel--;
            }
            else
            {
                const string msg = "No bézier control point selected. Select a control point in the Scene view or via the toolbar above.";
                EditorGUILayout.HelpBox(msg, MessageType.Info);
            }

            // OPTIONAL show raw data, allow re-ordering array data
            EditorGUILayout.Space();
            viewDefaultValues = EditorGUILayout.Foldout(viewDefaultValues, "View Reorderable Array", EditorStyles.foldoutHeader);
            if (viewDefaultValues)
            {
                const string msg = "This view is meant for re-ordering bézier points only! Modify data at your own risk.";
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
            BezierPoint bezier = spline.GetBezierPoint(index);
            var mode = bezier.tangentMode;
            var pointPosition = root.TransformPoint(bezier.position);
            var inTangentPosition = root.TransformPoint(bezier.inTangent);
            var outTangentPosition = root.TransformPoint(bezier.outTangent);
            var color = modeColors[(int)mode];

            bool isFirstPoint = index == 0;
            bool isLastPoint = index == spline.CurveCount;

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

            //// For all points except the last
            //if (index <= spline.CurveCount)
            //{
            //    var sceneCamera = SceneView.currentDrawingSceneView.camera;
            //    var size = HandleUtility.GetHandleSize(bezier.position);
            //    var labelPosition = bezier.position + sceneCamera.transform.right * size / 5;
            //    var label = $"{index}";
            //    Handles.color = Color.white;
            //    Handles.Label(labelPosition, label, EditorStyles.boldLabel);
            //}

            return bezier;
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
            float viewSize = handleSize * size;
            float pickSize = handleSize + 3; // +3 px
            var isSelected = Handles.Button(position, handleRotation, viewSize, pickSize, Handles.DotHandleCap);
            return isSelected;
        }

        private void ShowDirections()
        {
            Handles.color = Color.green;
            Vector3 point = spline.GetPoint(0f);
            Vector3 direction = spline.GetDirection(0f) * directionScale;
            Handles.DrawLine(point, point + direction);

            int numIters = stepsPerCurve * spline.CurveCount; 
            for (int i = 0; i <= numIters; i++)
            {
                float time = (float)i / numIters;
                point = spline.GetPoint(time);
                direction = spline.GetDirection(time);
                Handles.DrawLine(point, point + direction * directionScale);
            }
        }

    }
}
