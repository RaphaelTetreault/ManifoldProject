using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzFixedBezierPath))]
    internal sealed class GfzFixedBezierPathEditor : GfzPathEditor
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
            controlPoints = serializedObject.FindProperty(nameof(controlPoints));
            selectedIndex = serializedObject.FindProperty(nameof(selectedIndex));
            transform = (target as GfzFixedBezierPath).transform;

            animationCurveTRS = serializedObject.FindProperty(nameof(animationCurveTRS));
            autoGenerateTRS = serializedObject.FindProperty(nameof(autoGenerateTRS));

            // Make sure to call after getting serialized properties
            AssignToolVisibility();
        }

        void OnDisable()
        {
            // Re-enable Unity QWERT handles
            Tools.hidden = false;
        }


        public override void OnInspectorGUI()
        {
            var editorTarget = target as GfzFixedBezierPath;

            DrawDefaults(editorTarget);
            //
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            var editorTarget = target as GfzFixedBezierPath;

            serializedObject.Update();
            {
                CaptureHandleClicked(editorTarget);
                CaptureHandleMove(editorTarget);
                CaptureKeyboardEvents();
                DrawBezierPath(editorTarget);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void CaptureKeyboardEvents()
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
                    break;
            }
        }
        private void CaptureHandleClicked(GfzFixedBezierPath editorTarget)
        {
            for (int i = 0; i < editorTarget.ControlPointsLength; i++)
            {
                var controlPoint = editorTarget.GetControlPoint(i);
                var position = WorldPosition(controlPoint);
                var orientation = WorldOrientation(controlPoint);

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
                        //Debug.Log(i);
                    }
                    EditorUtility.SetDirty(editorTarget);
                }
            }
        }
        private void CaptureHandleMove(GfzFixedBezierPath editorTarget)
        {
            int index = selectedIndex.intValue;
            switch (selectMode)
            {
                case SelectMode.PositionHandle: PositionHandle(editorTarget, index); break;
                case SelectMode.RotationHandle: EulerRotationHandle(editorTarget, index); break;
            }
        }
        private void DrawBezierPath(GfzFixedBezierPath editorTarget)
        {
            // Iterate on in-between-beziers rather than on control points
            Handles.color = Color.grey;
            for (int i = 0; i < editorTarget.ControlPointsLength - 1; i++)
            {
                var controlPoint0 = editorTarget.GetControlPoint(i + 0);
                var controlPoint1 = editorTarget.GetControlPoint(i + 1);
                Vector3 start = WorldPosition(controlPoint0);
                Vector3 end = WorldPosition(controlPoint1);
                Vector3 startTangent = transform.TransformPoint(controlPoint0.OutTangentPosition); // maybe add helper?
                Vector3 endTangent = transform.TransformPoint(controlPoint1.InTangentPosition); // maybe add helper?
                Handles.DrawBezier(start, end, startTangent, endTangent, Color.black, null, 5f);

                Handles.DrawLine(start, startTangent, 3f);
                Handles.DrawLine(end, endTangent, 3f);
                Handles.DrawDottedLine(startTangent, endTangent, 3f);
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
        private void PositionHandle(GfzFixedBezierPath editorTarget, int index)
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
        private void EulerRotationHandle(GfzFixedBezierPath editorTarget, int index)
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
    }
}
