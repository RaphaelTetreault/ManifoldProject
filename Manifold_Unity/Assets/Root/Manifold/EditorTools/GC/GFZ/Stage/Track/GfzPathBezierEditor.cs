using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzPathBezier))]
    internal class GfzPathBezierEditor : GfzPathEditor
    {
        private static readonly Color splineColor = Color.white;
        private static readonly Color[] modeColors = {
            new Color32(223, 223,   0, 255), // slightly subdued yellow
            new Color32(255,  64, 127, 255), // near hot pink
            new Color32(  0, 255, 255, 255), // cyan-ish blue
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
        private GfzPathBezier spline;
        private Transform root;
        private Quaternion handleRotation;

        private bool settingsTabFoldout = false;

        SerializedProperty viewDirection;
        SerializedProperty viewDirectionArrowsPerCurve;
        SerializedProperty viewDirectionScale;
        SerializedProperty bezierHandleSize;
        SerializedProperty splineThickness;
        SerializedProperty outterLineThickness;
        SerializedProperty animationCurveTRS;
        SerializedProperty autoGenerateTRS;

        private bool IsValidIndex(int selectedIndex)
        {
            bool isValidIndex = selectedIndex >= 0 && selectedIndex < spline.PointsCount;
            return isValidIndex;
        }



        void OnEnable()
        {
            viewDirection = serializedObject.FindProperty(nameof(viewDirection));
            viewDirectionArrowsPerCurve = serializedObject.FindProperty(nameof(viewDirectionArrowsPerCurve));
            viewDirectionScale = serializedObject.FindProperty(nameof(viewDirectionScale));
            bezierHandleSize = serializedObject.FindProperty(nameof(bezierHandleSize));
            splineThickness = serializedObject.FindProperty(nameof(splineThickness));
            outterLineThickness = serializedObject.FindProperty(nameof(outterLineThickness));

            animationCurveTRS = serializedObject.FindProperty(nameof(animationCurveTRS));
            autoGenerateTRS = serializedObject.FindProperty(nameof(autoGenerateTRS));
        }

        private void OnSceneGUI()
        {
            // Set up editor variables
            spline = target as GfzPathBezier;
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
            spline = target as GfzPathBezier;
            var undoPrefix = $"'{target.name}'({nameof(GfzPathBezier)}):";

            // Some properties are modified via PropertyField. Set up for those values.
            serializedObject.Update();

            //
            DrawDefaults(spline);

            // TRS
            GuiSimple.Label("Animation Curve TRS", EditorStyles.boldLabel);
            {
                EditorGUILayout.PropertyField(autoGenerateTRS);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"Generate Animation Curve TRS"))
                {
                    var objects = new List<GfzSegmentNode>();
                    var shapes = spline.GetShapeNodes();
                    objects.AddRange(shapes);
                    objects.Add(spline);
                    var objectsArray = objects.ToArray();

                    Undo.RecordObjects(objectsArray, $"Create TRS");
                    spline.UpdateAnimationCurveTRS();
                    spline.UpdateShapeNodeMeshes(shapes);
                    foreach (var obj in objectsArray)
                        EditorUtility.SetDirty(obj);
                }

                if (GUILayout.Button($"Update Child Meshes"))
                {
                    var objects = new List<GfzSegmentNode>();
                    var shapes = spline.GetShapeNodes();
                    objects.AddRange(shapes);
                    var objectsArray = objects.ToArray();

                    Undo.RecordObjects(objectsArray, $"Create TRS");
                    spline.UpdateShapeNodeMeshes(shapes);
                    foreach (var obj in objectsArray)
                        EditorUtility.SetDirty(obj);
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(animationCurveTRS);
            }
            EditorGUILayout.Space();

            // BEZIER POINTS EDITOR
            GuiSimple.Label("Bézier Points", EditorStyles.boldLabel);
            {
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

                GUILayout.BeginHorizontal();
                {
                    var buttonWidth = GUILayout.Width(GuiSimple.GetElementsWidth(3));

                    GUI.enabled = IsValidIndex(selectedIndex);
                    if (GUILayout.Button($"Insert Before [{selectedIndex}]", buttonWidth))
                    {
                        Undo.RecordObject(spline, $"Add bézier point at {selectedIndex}");
                        spline.InsertBefore(selectedIndex);
                        selectedPart = SelectedPart.point;
                        EditorUtility.SetDirty(spline);
                    }

                    if (GUILayout.Button($"Insert After [{selectedIndex}]", buttonWidth))
                    {
                        Undo.RecordObject(spline, $"Add bézier point at {spline.PointsCount}");
                        spline.InsertAfter(selectedIndex + 1);
                        selectedPart = SelectedPart.point;
                        EditorUtility.SetDirty(spline);
                        selectedIndex++;
                    }

                    GUI.color = new Color32(255, 160, 160, 255);
                    if (GUILayout.Button($"Delete [{selectedIndex}]", buttonWidth))
                    {
                        Undo.RecordObject(spline, $"Delete bézier point {selectedIndex}");
                        spline.RemovePoint(selectedIndex);
                        selectedPart = SelectedPart.point;
                        EditorUtility.SetDirty(spline);
                        selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, int.MaxValue);
                    }
                    GUI.color = Color.white;
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();

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

            // Uncomment for debugging
            //DrawDefaultInspector();
            // Save values modified through PropertyField()
            serializedObject.ApplyModifiedProperties();
            //
            Repaint();
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

            EditorGUILayout.Separator();
            GUILayout.Label($"Bézier Tangents", EditorStyles.boldLabel);
            // MODE
            EditorGUI.BeginChangeCheck();
            bezier.tangentMode = GuiSimple.EnumPopup(nameof(bezier.tangentMode), bezier.tangentMode);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier in tangent [{selectedIndex}]'s mode");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }
            EditorGUILayout.Separator();
            DrawInTangent(bezier, index);
            EditorGUILayout.Separator();
            DrawOutTangent(bezier, index);
            EditorGUILayout.Separator();
            EditorGUI.indentLevel--;

            // WIDTH
            EditorGUILayout.Space();
            GuiSimple.Label("Scale.X", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            bezier.width = GuiSimple.Float(nameof(bezier.width), bezier.width);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] width");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }
            EditorGUI.BeginChangeCheck();
            bezier.widthTangentMode = GuiSimple.EnumPopup(nameof(bezier.widthTangentMode), bezier.widthTangentMode);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] width tangent mode");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }
            EditorGUI.indentLevel--;

            // HEIGHT
            EditorGUILayout.Space();
            GuiSimple.Label("Scale.Y", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            bezier.height = GuiSimple.Float(nameof(bezier.height), bezier.height);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] height");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }
            EditorGUI.BeginChangeCheck();
            bezier.heightTangentMode = GuiSimple.EnumPopup(nameof(bezier.heightTangentMode), bezier.heightTangentMode);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] height tangent mode");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }
            EditorGUI.indentLevel--;

            // ROLL
            EditorGUILayout.Space();
            GuiSimple.Label("Rotation.Z", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            bezier.roll = GuiSimple.Float(nameof(bezier.roll), bezier.roll);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] roll tangent mode");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }
            EditorGUI.BeginChangeCheck();
            bezier.rollTangentMode = GuiSimple.EnumPopup(nameof(bezier.rollTangentMode), bezier.rollTangentMode);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, $"Set bézier point [{selectedIndex}] roll tangent mode");
                spline.SetBezierPoint(index, bezier);
                EditorUtility.SetDirty(spline);
            }
            //EditorGUI.indentLevel--;
        }

        private void DrawInTangent(BezierPoint bezier, int index)
        {
            // POSITION
            EditorGUI.BeginChangeCheck();
            bezier.inTangent = GuiSimple.Vector3(nameof(bezier.inTangent), bezier.inTangent);
            if (EditorGUI.EndChangeCheck())
            {
                var inTangent = root.TransformPoint(bezier.inTangent);
                EditInTangent(index, inTangent);
            }

            // LENGTH
            EditorGUI.BeginChangeCheck();
            var inTangentLength = GuiSimple.Float(nameof(bezier.inTangent) + "Length", bezier.InTangentLocal.magnitude);
            if (EditorGUI.EndChangeCheck())
            {
                var inTangentLocal = bezier.InTangentLocal.normalized * inTangentLength;
                var inTangent = root.TransformPoint(bezier.position + inTangentLocal);
                EditInTangent(index, inTangent);
            }

            DrawWeights(index - 1);
            DrawAlignBezierTangents(index - 1, "Align with Previous");
        }

        private void DrawOutTangent(BezierPoint bezier, int index)
        {
            // POSITION
            EditorGUI.BeginChangeCheck();
            bezier.outTangent = GuiSimple.Vector3(nameof(bezier.outTangent), bezier.outTangent);
            if (EditorGUI.EndChangeCheck())
            {
                var outTangent = root.TransformPoint(bezier.outTangent);
                EditOutTangent(index, outTangent);
            }

            // LENGTH
            EditorGUI.BeginChangeCheck();
            var outTangentLength = GuiSimple.Float(nameof(bezier.outTangent) + "Length", bezier.OutTangentLocal.magnitude);
            if (EditorGUI.EndChangeCheck())
            {
                var outTangentLocal = bezier.OutTangentLocal.normalized * outTangentLength;
                var outTangent = root.TransformPoint(bezier.position + outTangentLocal);
                EditOutTangent(index, outTangent);
            }

            DrawWeights(index + 0);
            DrawAlignBezierTangents(index + 0, "Align with Next");
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
                bool valueChanged = index != result;
                if (isValidIndex && valueChanged)
                {
                    selectedIndex = result;
                    EditorUtility.SetDirty(target);
                    GUI.FocusControl(null);
                }
            }
        }

        private void DrawWeights(int selectedIndex)
        {
            const float oneQuarter = 1 / 4f;
            const float oneThird = 1 / 3f;
            const float oneHalf = 1 / 2f;

            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Auto Weight");
                GUI.enabled = IsValidIndex(selectedIndex) && IsValidIndex(selectedIndex + 1);
                if (GUILayout.Button($"1/4"))
                    SetBezierWeight(selectedIndex, selectedIndex + 1, oneQuarter, oneQuarter);
                if (GUILayout.Button($"1/3"))
                    SetBezierWeight(selectedIndex, selectedIndex + 1, oneThird, oneThird);
                if (GUILayout.Button($"1/2"))
                    SetBezierWeight(selectedIndex, selectedIndex + 1, oneHalf, oneHalf);
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
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

        public void EditInTangent(int index, Vector3 localPosition, bool doRecord = true)
        {
            BezierPoint bezierPoint = spline.GetBezierPoint(index);
            var inTangentPosition = localPosition;

            //
            string undoMessage = $"Move bézier point [{index}] in-tangent";
            if (doRecord)
                Undo.RecordObject(spline, undoMessage);

            bezierPoint.inTangent = root.InverseTransformPoint(inTangentPosition);
            bezierPoint.outTangent = GetTangentFromMode(bezierPoint, bezierPoint.inTangent, bezierPoint.outTangent);
            spline.SetBezierPoint(index, bezierPoint);
            ConnectEndsIfLoop(index, bezierPoint);

            if (doRecord)
                EditorUtility.SetDirty(spline);
        }

        public void EditOutTangent(int index, Vector3 localPosition, bool doRecord = true)
        {
            BezierPoint bezierPoint = spline.GetBezierPoint(index);
            var outTangentPosition = localPosition;

            string undoMessage = $"Move bézier point [{index}] out-tangent";
            if (doRecord)
                Undo.RecordObject(spline, undoMessage);

            bezierPoint.outTangent = root.InverseTransformPoint(outTangentPosition);
            bezierPoint.inTangent = GetTangentFromMode(bezierPoint, bezierPoint.outTangent, bezierPoint.inTangent);
            spline.SetBezierPoint(index, bezierPoint);
            ConnectEndsIfLoop(index, bezierPoint);

            if (doRecord)
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
            //var mode = bezier.tangentMode;
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

        public void SetBezierWeight(int index0, int index1, float weight0, float weight1)
        {
            bool invalidMin = index0 < 0;
            bool invalidMax = index1 >= spline.PointsCount;
            if (invalidMin || invalidMax)
                return;

            BezierPoint bezierPoint0 = spline.GetBezierPoint(index0);
            BezierPoint bezierPoint1 = spline.GetBezierPoint(index1);

            // Set tangent mode to aligned if it was set to mirrored
            var mode0 = bezierPoint0.tangentMode;
            var mode1 = bezierPoint1.tangentMode;

            bezierPoint0.tangentMode = mode0 == BezierControlPointMode.Mirrored ? BezierControlPointMode.Aligned : mode0;
            bezierPoint1.tangentMode = mode1 == BezierControlPointMode.Mirrored ? BezierControlPointMode.Aligned : mode1;

            // 
            var distanceBetween = Vector3.Distance(bezierPoint0.position, bezierPoint1.position);
            bezierPoint0.outTangent = weight0 * distanceBetween * bezierPoint0.OutTangentLocal.normalized + bezierPoint0.position;
            bezierPoint1.inTangent = weight1 * distanceBetween * bezierPoint1.InTangentLocal.normalized + bezierPoint1.position;

            //
            string undoMessage = $"Set weight of tangets {index0} and {index1}";
            Undo.RecordObject(spline, undoMessage);
            {
                spline.SetBezierPoint(index0, bezierPoint0);
                spline.SetBezierPoint(index1, bezierPoint1);
                ConnectEndsIfLoop(index0, bezierPoint0);
                ConnectEndsIfLoop(index1, bezierPoint1);
            }
            EditorUtility.SetDirty(spline);
        }

        public void AlignBezierTangents(int index0, int index1)
        {
            var bezier0 = spline.GetBezierPoint(index0);
            var bezier1 = spline.GetBezierPoint(index1);
            Vector3 direction = (bezier1.position - bezier0.position).normalized;
            Vector3 outTangent0 = direction * bezier0.OutTangentLocal.magnitude;
            Vector3 inTangent1 = -direction * bezier1.InTangentLocal.magnitude;

            Vector3 basePosition = spline.GetPosition();
            outTangent0 += basePosition + bezier0.position;
            inTangent1 += basePosition + bezier1.position;

            string undoMessage = $"Align bézier tangents {index0} and {index1}";
            Undo.RecordObject(spline, undoMessage);
            {
                EditOutTangent(index0, outTangent0, false);
                EditInTangent(index1, inTangent1, false);
            }
            EditorUtility.SetDirty(spline);
        }

        private void DrawAlignBezierTangents(int selectedIndex, string buttonLabel)
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Auto Align");
                GUI.enabled = IsValidIndex(selectedIndex) && IsValidIndex(selectedIndex + 1);
                if (GUILayout.Button(buttonLabel))
                {
                    AlignBezierTangents(selectedIndex, selectedIndex + 1);
                }
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
        }

    }
}
