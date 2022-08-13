using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzPathFixedBezier))]
    internal sealed class GfzPathFixedBezierEditor : GfzPathEditor
    {
        internal enum SelectMode
        {
            PositionHandle,
            RotationHandle,
        }
        private readonly Color32 HandlesButtonColor = new Color32(255, 0, 0, 196);
        private readonly Color32 HandlesButtonColorSelected = new Color32(255, 0, 0, 128);

        SerializedProperty controlPoints;
        SerializedProperty selectedIndex;
        SerializedProperty animationCurveTRS;
        SerializedProperty keysBetweenControlPoints;
        SerializedProperty autoGenerateTRS;
        Transform transform;

        private static SelectMode selectMode = SelectMode.PositionHandle;

        private bool IsValidIndex(int selectedIndex)
        {
            int nControlPoints = controlPoints.arraySize;
            bool isValidIndex = selectedIndex >= 0 && selectedIndex < nControlPoints;
            return isValidIndex;
        }

        void OnEnable()
        {
            transform = (target as GfzPathFixedBezier).transform;
            controlPoints = serializedObject.FindProperty(nameof(controlPoints));
            selectedIndex = serializedObject.FindProperty(nameof(selectedIndex));
            animationCurveTRS = serializedObject.FindProperty(nameof(animationCurveTRS));
            keysBetweenControlPoints = serializedObject.FindProperty(nameof(keysBetweenControlPoints));
            autoGenerateTRS = serializedObject.FindProperty(nameof(autoGenerateTRS));

            // Make sure to call after getting serialized properties
            AssignToolVisibility();
        }

        void OnDisable()
        {
            // Re-enable Unity QWERT handles
            Tools.hidden = false;
        }


        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            var editorTarget = target as GfzPathFixedBezier;
            int index = selectedIndex.intValue;

            DrawDefaults(editorTarget);
            EditorGUILayout.Separator();
            DrawControlPointSelect(editorTarget, index);
            EditorGUILayout.Separator();
            // Clamp in case we deleted a control point and index becomes invalid
            index = Mathf.Clamp(index, -1, editorTarget.DistancesBetweenLength);
            DrawCurrentControlPoint(editorTarget, index);
            EditorGUILayout.Separator();
            DrawAnimationData();

            AssignToolVisibility();
        }

        private void DrawButtonFields(GfzPathFixedBezier editorTarget, int index)
        {
            EditorGUILayout.BeginHorizontal();
            DrawButtonInsertBefore(editorTarget, index);
            DrawButtonInsertAfter(editorTarget, index);
            DrawButtonDelete(editorTarget, index);
            EditorGUILayout.EndHorizontal();
        }
        private void DrawButtonInsertBefore(GfzPathFixedBezier editorTarget, int index)
        {
            if (!GUILayout.Button("Insert Before"))
                return;

            int indexPrev = index - 1;
            bool indexPrevIsValid = editorTarget.IsValidIndex(indexPrev);
            if (indexPrevIsValid)
            {
                ActionInsertBetween(editorTarget, indexPrev, index);
            }
            else
            {
                ActionInsertBefore(editorTarget, index);
            }
        }
        private void DrawButtonInsertAfter(GfzPathFixedBezier editorTarget, int index)
        {
            if (!GUILayout.Button("Insert After"))
                return;

            int indexNext = index + 1;
            bool indexIsValid = editorTarget.IsValidIndex(index);
            bool indexNextIsValid = editorTarget.IsValidIndex(indexNext);

            if (indexNextIsValid && indexIsValid)
            {
                ActionInsertBetween(editorTarget, index, indexNext);
            }
            else
            {
                ActionInsertAfter(editorTarget, index);
            }

            serializedObject.Update();
            selectedIndex.intValue += 1;
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawButtonDelete(GfzPathFixedBezier editorTarget, int index)
        {
            bool canDeleteControlPoints = editorTarget.ControlPointsLength > 2;
            GUI.color = canDeleteControlPoints
                ? new Color32(255, 128, 128, 255)
                : Color.grey;

            if (GUILayout.Button("Delete"))
            {
                ActionDelete(editorTarget, index);
            }
            GUI.color = Color.white;
        }
        private void ActionInsertBetween(GfzPathFixedBezier editorTarget, int index0, int index1)
        {
            string undoMessage = $"Insert bezier point between {index0} and {index1}";
            Undo.RecordObject(editorTarget, undoMessage);
            {
                editorTarget.InsertBetween(index0, index1);
            }
            EditorUtility.SetDirty(editorTarget);
        }
        private void ActionInsertBefore(GfzPathFixedBezier editorTarget, int index)
        {
            string undoMessage = $"Insert bezier point before {index}";
            Undo.RecordObject(editorTarget, undoMessage);
            {
                var controlPoint = editorTarget.GetControlPoint(index);
                Vector3 offset = controlPoint.Orientation * Vector3.back * 500f;
                controlPoint.position += offset;
                editorTarget.InsertBefore(index, controlPoint);
            }
            EditorUtility.SetDirty(editorTarget);
        }
        private void ActionInsertAfter(GfzPathFixedBezier editorTarget, int index)
        {
            string undoMessage = $"Insert bezier point before {index}";
            Undo.RecordObject(editorTarget, undoMessage);
            {
                var controlPoint = editorTarget.GetControlPoint(index);
                Vector3 offset = controlPoint.Orientation * Vector3.forward * 500f;
                controlPoint.position += offset;
                editorTarget.InsertAfter(index, controlPoint);
            }
            EditorUtility.SetDirty(editorTarget);
        }
        private void DeselectControlPoint()
        {
            if (GUILayout.Button("Edit Transform"))
            {
                selectedIndex.intValue = -1;
            }
        }

        private void ActionDelete(GfzPathFixedBezier editorTarget, int index)
        {
            string undoMessage = $"Delete bezier point {index}";
            Undo.RecordObject(editorTarget, undoMessage);
            {
                editorTarget.RemoveAt(index);
                editorTarget.SelectedIndex--;
            }
            EditorUtility.SetDirty(editorTarget);
        }

        private void DrawCurrentControlPoint(GfzPathFixedBezier editorTarget, int index)
        {
            EditorGUILayout.LabelField($"Bezier Control Point", EditorStyles.boldLabel);

            bool isInvalidIndex = !editorTarget.IsValidIndex(index);
            if (isInvalidIndex)
            {
                EditorGUILayout.HelpBox("Select a node to view properties.", MessageType.None);
                return;
            }

            DrawCurrentControlPointPosition(editorTarget, index);
            DrawCurrentControlPointOrientation(editorTarget, index);
            DrawCurrentControlPointScale(editorTarget, index);
            DrawScaleKeyFlags(editorTarget, index);
        }
        private void DrawCurrentControlPointPosition(GfzPathFixedBezier editorTarget, int index)
        {
            var controlPoint = editorTarget.GetControlPoint(index);
            Vector3 position = WorldPosition(controlPoint);

            EditorGUI.BeginChangeCheck();
            Vector3 result = GuiSimple.Vector3(nameof(controlPoint.position), position);
            if (EditorGUI.EndChangeCheck())
            {
                string undoMessage = $"Change bezier point {index} position";
                Undo.RecordObject(editorTarget, undoMessage);
                {
                    controlPoint.position = LocalPosition(result);
                    editorTarget.SetControlPoint(index, controlPoint);
                }
                EditorUtility.SetDirty(editorTarget);
            }
        }
        private void DrawCurrentControlPointOrientation(GfzPathFixedBezier editorTarget, int index)
        {
            var controlPoint = editorTarget.GetControlPoint(index);
            Vector3 baseOrientation = transform.rotation.eulerAngles;
            Vector3 orientation = controlPoint.EulerOrientation + baseOrientation;

            EditorGUI.BeginChangeCheck();
            Vector3 result = GuiSimple.Vector3(nameof(controlPoint.EulerOrientation), orientation);
            if (EditorGUI.EndChangeCheck())
            {
                string undoMessage = $"Change bezier point {index} euler orientation";
                Undo.RecordObject(editorTarget, undoMessage);
                {
                    controlPoint.EulerOrientation = result - baseOrientation;
                    editorTarget.SetControlPoint(index, controlPoint);
                }
                EditorUtility.SetDirty(editorTarget);
            }
        }
        private void DrawCurrentControlPointScale(GfzPathFixedBezier editorTarget, int index)
        {
            var controlPoint = editorTarget.GetControlPoint(index);
            Vector2 scale = controlPoint.scale;

            EditorGUI.BeginChangeCheck();
            Vector2 result = GuiSimple.Vector2(nameof(scale), scale);
            if (EditorGUI.EndChangeCheck())
            {
                string undoMessage = $"Change bezier point {index} euler orientation";
                Undo.RecordObject(editorTarget, undoMessage);
                {
                    controlPoint.scale = result;
                    editorTarget.SetControlPoint(index, controlPoint);
                }
                EditorUtility.SetDirty(editorTarget);
            }
        }

        private void DrawScaleKeyFlags(GfzPathFixedBezier editorTarget, int index)
        {
            var controlPoint = editorTarget.GetControlPoint(index);
            string name = nameof(controlPoint.keyScale);
            bool changed = GuiSimple.Bool2(name, controlPoint.keyScale, out bool2 edited);
            if (changed)
            {
                string undoMessage = $"Change bezier point {index} scale keying";
                Undo.RecordObject(editorTarget, undoMessage);
                {
                    controlPoint.keyScale = edited;
                    editorTarget.SetControlPoint(index, controlPoint);
                }
                EditorUtility.SetDirty(editorTarget);
            }
        }
        private void DrawAnimationData()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Animation Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoGenerateTRS);
            EditorGUILayout.PropertyField(keysBetweenControlPoints);
            EditorGUILayout.PropertyField(animationCurveTRS);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawControlPointSelect(GfzPathFixedBezier editorTarget, int index)
        {
            serializedObject.Update();
            EditorGUILayout.LabelField($"Selected Bezier Control Point {index}", EditorStyles.boldLabel);
            DeselectControlPoint();
            DrawButtonFields(editorTarget, index);
            DrawIndexToolbar(editorTarget, index);
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawIndexToolbar(GfzPathFixedBezier editorTarget, int index)
        {
            int nControlPoints = editorTarget.ControlPointsLength;
            int rows = Mathf.CeilToInt(nControlPoints / 10f);
            for (int r = 0; r < rows; r++)
            {
                int rowBaseCurr = (r + 0) * 10;
                int rowBaseNext = (r + 1) * 10;

                // Generate a label for each toolbar cell
                List<string> labels = new List<string>(10);
                for (int c = rowBaseCurr; c < rowBaseNext; c++)
                {
                    bool isValidLabelIndex = editorTarget.IsValidIndex(c);
                    string label = isValidLabelIndex ? c.ToString() : string.Empty;
                    labels.Add(label);
                }

                // See if selected index applies to this row
                bool withinRowLower = index >= rowBaseCurr;
                bool withinRowUpper = index < rowBaseNext;
                bool isInRange = withinRowLower && withinRowUpper;
                // Get index in range 0-9, -1 if not for this row
                int indexForThisRow = isInRange ? index % 10 : -1;
                // Display toolbar, get index returned (-1 if non-applicable row)
                int selectedIndex = GUILayout.Toolbar(indexForThisRow, labels.ToArray());

                // only continue if we whould
                if (selectedIndex < 0)
                    continue;

                // Get true index
                selectedIndex += rowBaseCurr;

                // Set index if valid. If invalid row, we have result = -1, so this doesn't run.
                bool isValidIndex = editorTarget.IsValidIndex(selectedIndex);
                bool valueChanged = index != selectedIndex;
                if (isValidIndex && valueChanged)
                {
                    this.selectedIndex.intValue = selectedIndex;
                    GUI.FocusControl(null);
                }
            }
        }

        #endregion

        #region OnSceneGUI

        private void OnSceneGUI()
        {
            var editorTarget = target as GfzPathFixedBezier;
            int index = selectedIndex.intValue;

            DrawBezierPath(editorTarget, index);

            bool isInvalidIndex = !editorTarget.IsValidIndex(selectedIndex.intValue);
            if (isInvalidIndex)
                return;

            //
            serializedObject.Update();
            CaptureHandleClicked(editorTarget, index);
            CaptureHandleMove(editorTarget, index);
            CaptureKeyboardEvents(editorTarget, index);
            serializedObject.ApplyModifiedProperties();

            //editorTarget.InvokeUpdates();
        }

        private void CaptureKeyboardEvents(GfzPathFixedBezier editorTarget, int index)
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.W)
                    {
                        selectMode = SelectMode.PositionHandle;
                    }
                    if (e.keyCode == KeyCode.E)
                    {
                        selectMode = SelectMode.RotationHandle;
                    }
                    if (e.keyCode == KeyCode.Backspace)
                    {
                        ActionDelete(editorTarget, index);
                    }
                    if (e.keyCode == KeyCode.Plus || e.keyCode == KeyCode.KeypadPlus || e.keyCode == KeyCode.Equals)
                    {
                        editorTarget.IncrementSelectedIndex();
                    }
                    if (e.keyCode == KeyCode.Minus || e.keyCode == KeyCode.KeypadMinus)
                    {
                        editorTarget.DecrementSelectedIndex();
                    }
                    break;
            }
        }
        private void CaptureHandleClicked(GfzPathFixedBezier editorTarget, int index)
        {
            for (int i = 0; i < editorTarget.ControlPointsLength; i++)
            {
                var controlPoint = editorTarget.GetControlPoint(i);
                var position = WorldPosition(controlPoint);
                var orientation = WorldOrientation(controlPoint);

                // if selected, skip drawing button
                bool isSelected = i == index;
                if (isSelected)
                    continue;

                Handles.color = selectedIndex.intValue == i
                    ? HandlesButtonColorSelected
                    : HandlesButtonColor;

                bool isPressed = Handles.Button(position, orientation, 20f, 30f, Handles.DotHandleCap);
                if (isPressed)
                {
                    string undoMessage = $"Select bezier point {i}";
                    Undo.RecordObject(editorTarget, undoMessage);
                    {
                        selectedIndex.intValue = i;
                    }
                    EditorUtility.SetDirty(editorTarget);
                }
            }
        }
        private void CaptureHandleMove(GfzPathFixedBezier editorTarget, int index)
        {
            switch (selectMode)
            {
                case SelectMode.PositionHandle:
                    PositionHandle(editorTarget, index);
                    break;

                case SelectMode.RotationHandle:
                    EulerRotationHandle(editorTarget, index);
                    break;
            }
        }
        private void DrawBezierPath(GfzPathFixedBezier editorTarget, int index)
        {
            Color32 colorInactive = new Color32(128, 64, 64, 196);
            Color32 colorActive = Color.red;
            Vector3 cameraForward = SceneView.GetAllSceneCameras()[0].transform.forward;

            // Iterate on in-between-beziers rather than on control points
            for (int i = 0; i < editorTarget.ControlPointsLength - 1; i++)
            {
                var controlPoint0 = editorTarget.GetControlPoint(i + 0);
                var controlPoint1 = editorTarget.GetControlPoint(i + 1);
                Vector3 start = WorldPosition(controlPoint0);
                Vector3 end = WorldPosition(controlPoint1);
                Vector3 startTangent = transform.TransformPoint(controlPoint0.OutTangentPosition); // maybe add helper?
                Vector3 endTangent = transform.TransformPoint(controlPoint1.InTangentPosition); // maybe add helper?
                Handles.DrawBezier(start, end, startTangent, endTangent, Color.black, null, 5f);

                bool isInSelected = i == index - 1;
                bool isOutSelected = i == index;
                Handles.color = isOutSelected ? colorActive : colorInactive;
                Handles.DrawLine(start, startTangent, 3f);
                Handles.color = isInSelected ? colorActive : colorInactive;
                Handles.DrawLine(end, endTangent, 3f);

                // Draw circle where handle/button would normally be
                if (isOutSelected)
                {
                    Handles.color = colorActive;
                    Handles.DrawSolidDisc(start, cameraForward, 15f);
                }

                // Dotted line between tangents
                Handles.color = Color.black;
                Handles.DrawDottedLine(startTangent, endTangent, 3f);

                // Circle at midway point
                Vector3 center = Bezier.GetPoint(start, startTangent, endTangent, end, 0.5f);
                Handles.DrawSolidDisc(center, cameraForward, 10f);
            }
        }

        private Vector3 WorldPosition(FixedBezierPoint controlPoint)
        {
            var worldPosition = transform.TransformPoint(controlPoint.position);
            return worldPosition;
        }
        private Vector3 LocalPosition(Vector3 position)
        {
            var localPosition = transform.InverseTransformPoint(position);
            return localPosition;
        }

        private Quaternion WorldOrientation(FixedBezierPoint controlPoint)
        {
            var worldOrientation = transform.rotation * controlPoint.Orientation;
            return worldOrientation;
        }
        private Quaternion LocalOrientation(Quaternion orientation)
        {
            var localOrientation = Quaternion.Inverse(transform.rotation) * orientation;
            return localOrientation;
        }

        /// <summary>
        /// Create a position handle for the current <paramref name="editorTarget"/>.
        /// </summary>
        /// <param name="editorTarget"></param>
        /// <param name="index"></param>
        private void PositionHandle(GfzPathFixedBezier editorTarget, int index)
        {
            var controlPoint = editorTarget.GetControlPoint(index);
            Vector3 position = WorldPosition(controlPoint);
            Quaternion orientation = WorldOrientation(controlPoint);

            bool didUserMoveHandle = HandlesUtility.PositionHandle(position, orientation, out Vector3 editedPosition);
            if (didUserMoveHandle)
            {
                string undoMessage = $"Move bezier point {index}";
                Undo.RecordObject(editorTarget, undoMessage);
                {
                    controlPoint.position = LocalPosition(editedPosition);
                    editorTarget.SetControlPoint(index, controlPoint);
                    editorTarget.UpdateLinearDistanceTouchingControlPoint(index);
                    editorTarget.UpdateCurveDistanceTouchingControlPoint(index);
                }
                EditorUtility.SetDirty(editorTarget);
            }
        }

        /// <summary>
        /// Create a euler rotation handle for the current <paramref name="editorTarget"/>.
        /// </summary>
        /// <param name="editorTarget"></param>
        /// <param name="index"></param>
        private void EulerRotationHandle(GfzPathFixedBezier editorTarget, int index)
        {
            var controlPoint = editorTarget.GetControlPoint(index);
            Vector3 position = WorldPosition(controlPoint);
            Quaternion orientation = WorldOrientation(controlPoint);

            bool didUserMoveHandle = HandlesUtility.EulerRotationHandle(position, orientation, out Vector3 eulerDelta);
            if (didUserMoveHandle)
            {
                string undoMessage = $"Rotate bezier point {index}";
                Undo.RecordObject(editorTarget, undoMessage);
                {
                    controlPoint.EulerOrientation += eulerDelta;
                    editorTarget.SetControlPoint(index, controlPoint);
                }
                EditorUtility.SetDirty(editorTarget);
            }
        }

        /// <summary>
        /// Hides the Transform component editor gizmos when selecting a control point.
        /// </summary>
        private void AssignToolVisibility()
        {
            // https://forum.unity.com/threads/hiding-default-transform-handles.86760/
            int selectedIndex = this.selectedIndex.intValue;
            bool hideUnityTransformHandle = IsValidIndex(selectedIndex);
            Tools.hidden = hideUnityTransformHandle;
        }

        #endregion

    }
}
