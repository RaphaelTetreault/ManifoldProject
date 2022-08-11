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


        SerializedProperty controlPoints;
        SerializedProperty selectedIndex;
        SerializedProperty animationCurveTRS;
        SerializedProperty autoGenerateTRS;
        Transform transform;

        private SelectMode selectMode = SelectMode.PositionHandle;

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
            Handles.color = Color.red;
            for (int i = 0; i < editorTarget.ControlPointsLength; i++)
            {
                var controlPoint = editorTarget.GetControlPoint(i);

                var position = WorldPosition(controlPoint);
                var orientation = WorldOrientation(controlPoint);
                bool isPressed = Handles.Button(position, orientation, 20f, 30f, Handles.DotHandleCap);
                if (isPressed)
                {
                    selectedIndex.intValue = i;
                    //Debug.Log(i);
                }
            }

            int index = selectedIndex.intValue;
            switch (selectMode)
            {
                case SelectMode.PositionHandle: PositionHandle(editorTarget, index); break;
                case SelectMode.RotationHandle: EulerRotationHandle(editorTarget, index); break;
            }
            CaptureEditorEvent();
            serializedObject.ApplyModifiedProperties();
        }

        private void CaptureEditorEvent()
        {
            Event e = Event.current;
            switch (e.type)
            {
                //case EventType.MouseDown:
                //    Debug.Log("MouseDown");
                //    break;

                //case EventType.MouseUp:
                //    Debug.Log("MouseUp");
                //    break;

                //case EventType.MouseDrag:
                //    Debug.Log("MouseDrag");
                //    break;

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
        /// Create a position handle for the current <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="index"></param>
        private void PositionHandle(GfzFixedBezierPath tool, int index)
        {
            var controlPoint = tool.GetControlPoint(index);
            Vector3 position = WorldPosition(controlPoint);
            Quaternion orientation = WorldOrientation(controlPoint);

            bool didUserMoveHandle = HandlesUtility.PositionHandle(position, orientation, out Vector3 editedPosition);
            if (didUserMoveHandle)
            {
                controlPoint.position = LocalPosition(editedPosition);
                tool.SetControlPoint(index, controlPoint);
            }
        }

        /// <summary>
        /// Create a euler rotation handle for the current <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="index"></param>
        private void EulerRotationHandle(GfzFixedBezierPath tool, int index)
        {
            var controlPoint = tool.GetControlPoint(index);
            Vector3 position = WorldPosition(controlPoint);
            Quaternion orientation = WorldOrientation(controlPoint);

            bool didUserMoveHandle = HandlesUtility.EulerRotationHandle(position, orientation, out Vector3 eulerDelta);
            if (didUserMoveHandle)
            {
                controlPoint.EulerOrientation += eulerDelta;
                tool.SetControlPoint(index, controlPoint);
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
