using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzBezierSplineSegment))]
    public class GfzBezierSplineSegmentInspector : Editor
    {
        private static readonly Color splineColor = Color.white;
        private static readonly Color[] modeColors = {
            new Color32( 95, 223, 255, 255), // cyan-ish blue
            new Color32(223, 223,   0, 255), // slightly subdued yellow
            new Color32(255,  31, 127, 255), // near hot pink
        };

        private enum SelectedPart
        {
            none,
            point,
            inTangent,
            outTangent
        }

        private int selectedIndex = -1;
        private SelectedPart selectedPart = SelectedPart.none;
        private GfzBezierSplineSegment spline;
        private Transform root;
        private Quaternion handleRotation;

        private bool settingsTabFoldout = false;

        SerializedProperty widthsCurve;
        SerializedProperty heightsCurve;
        SerializedProperty rollsCurve;
        SerializedProperty viewDirection;
        SerializedProperty viewDirectionArrowsPerCurve;
        SerializedProperty viewDirectionScale;
        SerializedProperty bezierHandleSize;
        SerializedProperty splineThickness;
        SerializedProperty outterLineThickness;

        void OnEnable()
        {
            widthsCurve = serializedObject.FindProperty(nameof(widthsCurve));
            heightsCurve = serializedObject.FindProperty(nameof(heightsCurve));
            rollsCurve = serializedObject.FindProperty(nameof(rollsCurve));

            viewDirection = serializedObject.FindProperty(nameof(viewDirection));
            viewDirectionArrowsPerCurve = serializedObject.FindProperty(nameof(viewDirectionArrowsPerCurve));
            viewDirectionScale = serializedObject.FindProperty(nameof(viewDirectionScale));
            bezierHandleSize = serializedObject.FindProperty(nameof(bezierHandleSize));
            splineThickness = serializedObject.FindProperty(nameof(splineThickness));
            outterLineThickness = serializedObject.FindProperty(nameof(outterLineThickness));
        }

        private void OnSceneGUI()
        {
            // Set up editor variables
            spline = target as GfzBezierSplineSegment;
            root = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local
                ? root.rotation
                : Quaternion.identity;

            // Draw these first so they are on bottom of draw order
            if (spline.ViewDirection)
            {
                ShowDirections();
            }

            // Displays the bezier in Scene view
            BezierPoint bezier0 = DisplayEditableBezierPoint(0);
            for (int i = 1; i <= spline.CurveCount; i++)
            {
                BezierPoint bezier1 = DisplayEditableBezierPoint(i);

                var p0 = root.TransformPoint(bezier0.position);
                var p1 = root.TransformPoint(bezier0.outTangent);
                var p2 = root.TransformPoint(bezier1.inTangent);
                var p3 = root.TransformPoint(bezier1.position);

                var baseMtx = root.localToWorldMatrix;
                const int iters = 64;
                for (int p = 0; p < iters; p++)
                {
                    var time0 = (float)(p + 0) / iters;
                    var time1 = (float)(p + 1) / iters;
                    var mtx0 = baseMtx * spline.GetMatrix(time0);
                    var mtx1 = baseMtx * spline.GetMatrix(time1);
                    var l0 = mtx0.MultiplyPoint(Vector3.left / 2f);
                    var l1 = mtx1.MultiplyPoint(Vector3.left / 2f);
                    var r0 = mtx0.MultiplyPoint(Vector3.right / 2f);
                    var r1 = mtx1.MultiplyPoint(Vector3.right / 2f);

                    Handles.color = splineColor;
                    Handles.DrawLine(l0, l1, spline.OutterLineThickness);
                    Handles.color = Color.red;
                    Handles.DrawLine(r0, r1, spline.OutterLineThickness);
                }

                //
                Handles.DrawBezier(p0, p3, p1, p2, splineColor, null, spline.SplineThickness);
                //
                Handles.color = splineColor * 0.5f;
                Handles.DrawDottedLine(p1, p2, 5f);
                //
                Handles.color = GetBezierColor(bezier0, IsSelected(i - 1));
                Handles.DrawLine(p0, p1, spline.OutterLineThickness);
                //
                Handles.color = GetBezierColor(bezier1, IsSelected(i));
                Handles.DrawLine(p2, p3, spline.OutterLineThickness);

                bezier0 = bezier1;
            }
        }

        public override void OnInspectorGUI()
        {
            spline = target as GfzBezierSplineSegment;
            var undoPrefix = $"'{target.name}'({nameof(GfzBezierSplineSegment)}):";

            // Some properties are modified via PropertyField. Set up for those values.
            serializedObject.Update();


            // Default Script field for MonoBehaviour components
            GuiSimple.DefaultScript(spline);

            // SETTINGS
            settingsTabFoldout = EditorGUILayout.Foldout(settingsTabFoldout, "Settings", EditorStyles.foldoutHeader);
            //GuiSimple.Label("Editor Settings", EditorStyles.boldLabel);
            if (settingsTabFoldout)
            {
                EditorGUILayout.PropertyField(bezierHandleSize);
                EditorGUILayout.PropertyField(splineThickness);
                EditorGUILayout.PropertyField(outterLineThickness);
                EditorGUILayout.PropertyField(viewDirection);
                EditorGUILayout.PropertyField(viewDirectionScale);
                EditorGUILayout.PropertyField(viewDirectionArrowsPerCurve);
            }
            EditorGUILayout.Space();

            // GLOBAL SCRIPT FIELDS 
            GuiSimple.Label("Global Fields", EditorStyles.boldLabel);
            {
                var buttonWidth = GUILayout.Width(96);

                // LOOP
                {
                    EditorGUI.BeginChangeCheck();
                    bool isLoop = GuiSimple.Bool(nameof(spline.IsLoop), spline.IsLoop);
                    if (EditorGUI.EndChangeCheck())
                    {
                        string undoMessage = $"Set spline.IsLoop to '{isLoop}'.";
                        Undo.RegisterCompleteObjectUndo(spline, undoMessage);
                        spline.SetLoop(isLoop);
                        EditorUtility.SetDirty(spline);
                    }
                }

                // WIDTH
                GUILayout.BeginHorizontal();
                {
                    // Animation curve
                    EditorGUILayout.PropertyField(widthsCurve);
                    // Button & undo handling
                    if (GUILayout.Button("Reset Widths", buttonWidth))
                    {
                        string undoMessage = $"Reset {nameof(spline.WidthsCurve)} to bezier curve values.";
                        Undo.RecordObject(spline, undoMessage);
                        spline.WidthsCurve = spline.CreateWidthsCurve();
                        EditorUtility.SetDirty(spline);
                    }
                }
                GUILayout.EndHorizontal();

                // HEIGHT
                GUILayout.BeginHorizontal();
                {
                    // Animation curve
                    EditorGUILayout.PropertyField(heightsCurve);
                    // Button & undo handling
                    if (GUILayout.Button("Reset Heights", buttonWidth))
                    {
                        string undoMessage = $"Reset {nameof(spline.HeightsCurve)} to bezier curve values.";
                        Undo.RecordObject(spline, undoMessage);
                        spline.HeightsCurve = spline.CreateHeightsCurve();
                        EditorUtility.SetDirty(spline);
                    }
                }
                GUILayout.EndHorizontal();

                // ROLL
                GUILayout.BeginHorizontal();
                {
                    // Animation curve
                    EditorGUILayout.PropertyField(rollsCurve);
                    // Button & undo handling
                    if (GUILayout.Button("Reset Rolls", buttonWidth))
                    {
                        string undoMessage = $"Reset {nameof(spline.RollsCurve)} to bezier curve values.";
                        Undo.RecordObject(spline, undoMessage);
                        spline.RollsCurve = spline.CreateRollsCurve();
                        EditorUtility.SetDirty(spline);
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();

            // BEZIER POINTS EDITOR
            GuiSimple.Label("Bézier Points", EditorStyles.boldLabel);
            {
                GUILayout.BeginHorizontal();
                bool isValid = selectedIndex >= 0 && selectedIndex < spline.PointsCount;
                GUI.enabled = isValid;

                if (GUILayout.Button($"Insert Before [{selectedIndex}]"))
                {
                    Undo.RecordObject(spline, $"Add bézier point at {selectedIndex}");
                    spline.InsertBefore(selectedIndex);
                    selectedPart = SelectedPart.point;
                    EditorUtility.SetDirty(spline);
                }

                if (GUILayout.Button($"Insert After [{selectedIndex}]"))
                {
                    Undo.RecordObject(spline, $"Add bézier point at {spline.PointsCount}");
                    spline.InsertAfter(selectedIndex + 1);
                    selectedPart = SelectedPart.point;
                    EditorUtility.SetDirty(spline);
                    selectedIndex++;
                }

                GUI.color = new Color32(255, 160, 160, 255);
                if (GUILayout.Button($"Delete [{selectedIndex}]"))
                {
                    Undo.RecordObject(spline, $"Delete bézier point {selectedIndex}");
                    spline.RemovePoint(selectedIndex);
                    selectedPart = SelectedPart.point;
                    EditorUtility.SetDirty(spline);
                    selectedIndex--;
                }
                GUI.color = Color.white;
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                //
                DrawIndexToolbar();
            }
            EditorGUILayout.Space();

            // SELECTED BEZIER POINT PREVIEW
            {
                bool isIndexSelectable = selectedIndex >= 0 && selectedIndex <= spline.CurveCount;
                if (isIndexSelectable)
                {
                    EditorGUI.indentLevel++;
                    DrawSelectedBezierPointInspector(selectedIndex);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    const string msg = "No bézier control point selected. Select a control point in the Scene view or via the toolbar above.";
                    EditorGUILayout.HelpBox(msg, MessageType.Info);
                }
            }

            // Uncomment for debugging
            //DrawDefaultInspector();
            // Save values modified through PropertyField()
            serializedObject.ApplyModifiedProperties();
            //
            //Repaint();
        }

        private void DrawSelectedBezierPointInspector(int index)
        {
            GUILayout.Label($"Bézier Point [{index}]", EditorStyles.boldLabel);

            var bezier = spline.GetBezierPoint(index);

            // POSITION
            EditorGUI.BeginChangeCheck();
            bezier.position = GuiSimple.Vector3(nameof(bezier.position), bezier.position);
            if (EditorGUI.EndChangeCheck())
            {
                var position = root.TransformPoint(bezier.position);
                EditBezierPointPosition(index, position);
            }

            // MODE
            EditorGUI.BeginChangeCheck();
            bezier.tangentMode = GuiSimple.EnumPopup(nameof(bezier.tangentMode), bezier.tangentMode);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] mode");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
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

            // WIDTH
            EditorGUI.BeginChangeCheck();
            bezier.width = GuiSimple.Float(nameof(bezier.width), bezier.width);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] width");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }

            // HEIGHT
            EditorGUI.BeginChangeCheck();
            bezier.height = GuiSimple.Float(nameof(bezier.height), bezier.height);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] height");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }

            // ROLL
            EditorGUI.BeginChangeCheck();
            bezier.roll = GuiSimple.Float(nameof(bezier.roll), bezier.roll);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] roll");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }
        }

        private void DrawIndexToolbar()
        {
            int isLoopedCountOffset = spline.IsLoop ? 1 : 0;
            int rows = (spline.CurveCount - isLoopedCountOffset) / 10;
            for (int r = 0; r <= rows; r++)
            {
                int rowBaseCurr = (r + 0) * 10;
                int rowBaseNext = (r + 1) * 10;

                // Generate a label for each toolbar cell
                List<string> labels = new List<string>();
                for (int c = rowBaseCurr; c < rowBaseNext; c++)
                {
                    if (c <= spline.LoopCurveCount)
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
                bool isValidIndex = result >= 0 && result <= spline.LoopCurveCount;
                if (isValidIndex)
                {
                    selectedIndex = result;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        public void EditBezierPointPosition(int index, Vector3 localPosition)
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
            string undoMessage = $"Move bézier point [{index}] position";
            Undo.RecordObject(spline, undoMessage);
            bezierPoint.position = root.InverseTransformPoint(newPosition);
            bezierPoint.inTangent = root.InverseTransformPoint(inTangentPosition + positionDelta);
            bezierPoint.outTangent = root.InverseTransformPoint(outTangentPosition + positionDelta);
            spline.SetBezierPoint(index, bezierPoint);
            ConnectEndsIfLoop(index, bezierPoint);
            EditorUtility.SetDirty(spline);
        }

        public void EditInTangent(int index, Vector3 localPosition)
        {
            BezierPoint bezierPoint = spline.GetBezierPoint(index);

            // 
            var inTangentPosition = localPosition;

            //
            string undoMessage = $"Move bézier point [{index}] in-tangent";
            Undo.RecordObject(spline, undoMessage);
            bezierPoint.inTangent = root.InverseTransformPoint(inTangentPosition);
            bezierPoint.outTangent = GetTangentFromMode(bezierPoint, bezierPoint.inTangent, bezierPoint.outTangent);
            spline.SetBezierPoint(index, bezierPoint);
            ConnectEndsIfLoop(index, bezierPoint);
            EditorUtility.SetDirty(spline);
        }

        public void EditOutTangent(int index, Vector3 localPosition)
        {
            BezierPoint bezierPoint = spline.GetBezierPoint(index);

            // 
            var outTangentPosition = localPosition;

            //
            string undoMessage = $"Move bézier point [{index}] out-tangent";
            Undo.RecordObject(spline, undoMessage);
            bezierPoint.outTangent = root.InverseTransformPoint(outTangentPosition);
            bezierPoint.inTangent = GetTangentFromMode(bezierPoint, bezierPoint.outTangent, bezierPoint.inTangent);
            spline.SetBezierPoint(index, bezierPoint);
            ConnectEndsIfLoop(index, bezierPoint);
            EditorUtility.SetDirty(spline);
        }

        public void ConnectEndsIfLoop(int index, BezierPoint bezierPoint)
        {
            if (spline.IsLoop)
            {
                bool isFirstPoint = index == 0;
                bool isLastPoint = index == spline.CurveCount;
                if (isFirstPoint)
                {
                    spline.SetBezierPoint(spline.CurveCount, bezierPoint);
                }
                else if (isLastPoint)
                {
                    spline.SetBezierPoint(0, bezierPoint);
                }
            }
        }

        private BezierPoint DisplayEditableBezierPoint(int index)
        {
            BezierPoint bezier = spline.GetBezierPoint(index);
            var mode = bezier.tangentMode;
            var pointPosition = root.TransformPoint(bezier.position);
            var inTangentPosition = root.TransformPoint(bezier.inTangent);
            var outTangentPosition = root.TransformPoint(bezier.outTangent);

            // Draw position handle for all elements except the last point in a loop
            bool doPosition = !spline.IsLoop || index < spline.CurveCount;
            // Draw the first intangent if:
            // We are not the first point when not a loop OR
            // We are any index other than the final one when in a loop
            bool doFirstInTangent =
                (index > 0 && !spline.IsLoop) ||
                (spline.IsLoop && index < spline.CurveCount);
            // Draw the last outTangent if not last point
            bool doLastOutTangent = (index < spline.CurveCount);

            Handles.color = GetBezierColor(bezier, IsSelected(index));
            bool pointSelected = doPosition ? DoBezierHandle(pointPosition, 1.75f) : false;
            // Only visualize/edit in-tangent if not first, out-tangent if not last
            bool inTangentSelected = doFirstInTangent ? DoBezierHandle(inTangentPosition) : false;
            bool outTangentSelected = doLastOutTangent ? DoBezierHandle(outTangentPosition) : false;
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
                    EditBezierPointPosition(index, result);
                }
            }

            if (bezierSelected && selectedPart == SelectedPart.inTangent)
            {
                EditorGUI.BeginChangeCheck();
                inTangentPosition = Handles.DoPositionHandle(inTangentPosition, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    EditInTangent(index, inTangentPosition);
                }
            }

            if (bezierSelected && selectedPart == SelectedPart.outTangent)
            {
                EditorGUI.BeginChangeCheck();
                outTangentPosition = Handles.DoPositionHandle(outTangentPosition, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    EditOutTangent(index, outTangentPosition);
                }
            }

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

        private bool DoBezierHandle(Vector3 position, float scaleMultiplier = 1f)
        {
            var size = HandleUtility.GetHandleSize(position);
            float viewSize = spline.BezierHandleSize * size * scaleMultiplier;
            float pickSize = viewSize;
            var isSelected = Handles.Button(position, handleRotation, viewSize, pickSize, Handles.DotHandleCap);
            return isSelected;
        }

        private void ShowDirections()
        {
            Handles.color = splineColor;
            int curves = spline.CurveCount;
            int steps = spline.ViewDirectionArrowsPerCurve + 1;
            for (int i = 0; i < curves; i++)
            {
                for (int j = 1; j < steps; j++)
                {
                    var t = (float)j / steps; 
                    var point = spline.GetPositionRelative(t, i);
                    var direction = spline.GetDirection(t, i);
                    var orientation = Quaternion.LookRotation(direction);
                    var size = spline.ViewDirectionScale;
                    Handles.ArrowHandleCap(-1, point, orientation, size, EventType.Repaint);
                }

            }

        }

        public bool IsSelected(int index)
        {
            return selectedIndex == index;
        }

        public Color32 GetBezierColor(BezierPoint bezier, bool isSelected)
        {
            var c = modeColors[(int)bezier.tangentMode];
            return isSelected ? c : new Color(c.r, c.g, c.b, 0.4f);
        }


    }
}
