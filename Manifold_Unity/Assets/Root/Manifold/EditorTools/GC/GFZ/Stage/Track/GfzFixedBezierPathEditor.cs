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

                var position = controlPoint.position;
                var orientation = controlPoint.Orientation;
                bool isPressed = Handles.Button(position, orientation, 20f, 30f, Handles.DotHandleCap);
                if (isPressed)
                {
                    selectedIndex.intValue = i;
                    Debug.Log(i);
                }

                if (i == selectedIndex.intValue)
                {
                    switch (selectMode)
                    {
                        case SelectMode.PositionHandle: PositionHandle(editorTarget, i); break;
                        case SelectMode.RotationHandle: RotationHandle(editorTarget, i); break;
                    }
                }

                CaptureEvent();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CaptureEvent()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    Debug.Log("MouseDown");
                    break;

                case EventType.MouseUp:
                    Debug.Log("MouseUp");
                    break;

                case EventType.MouseDrag:
                    Debug.Log("MouseDrag");
                    break;

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

        private bool PositionHandle(Vector3 position, Quaternion orientation, out Vector3 editedPosition)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 result = Handles.DoPositionHandle(position, orientation);
            bool didUserMoveHandle = EditorGUI.EndChangeCheck();

            editedPosition = didUserMoveHandle ? result : Vector3.zero;
            return didUserMoveHandle;
        }
        private bool PositionHandle(FixedBezierPoint controlPoint, out Vector3 editedPosition)
            => PositionHandle(controlPoint.position, controlPoint.Orientation, out editedPosition);
        private void PositionHandle(GfzFixedBezierPath tool, int index)
        {
            var controlPoint = tool.GetControlPoint(index);
            bool didUserMoveHandle = PositionHandle(controlPoint, out Vector3 editedPosition);
            if (didUserMoveHandle)
            {
                controlPoint.position = editedPosition;
                tool.SetControlPoint(index, controlPoint);
            }
        }

        private bool RotationHandle(Vector3 position, Quaternion orientation, out Quaternion editedOrientation)
        {
            EditorGUI.BeginChangeCheck();
            Quaternion result = Handles.DoRotationHandle(orientation, position);
            bool didUserMoveHandle = EditorGUI.EndChangeCheck();

            editedOrientation = didUserMoveHandle ? result : Quaternion.identity;
            return didUserMoveHandle;
        }
        private bool RotationHandle(FixedBezierPoint controlPoint, out Quaternion editedOrientation)
            => RotationHandle(controlPoint.position, controlPoint.Orientation, out editedOrientation);
        private void RotationHandle(GfzFixedBezierPath tool, int index)
        {
            var controlPoint = tool.GetControlPoint(index);
            bool didUserMoveHandle = RotationHandle(controlPoint, out Quaternion editedOrientation);
            if (didUserMoveHandle)
            {
                Vector3 eulerOriginal = controlPoint.Orientation.eulerAngles;
                Vector3 eulerEdited = editedOrientation.eulerAngles;
                Vector3 eulerDelta = eulerEdited - eulerOriginal;
                Vector3 eulerDeltaClean = CleanRotation(eulerDelta);
                //Debug.Log($"og {eulerOriginal} :: edit {eulerEdited} :: delta {eulerDelta} :: {eulerDeltaClean}");
                controlPoint.EulerOrientation += eulerDeltaClean;
                tool.SetControlPoint(index, controlPoint);
            }
        }

        private float CleanRotation(float value)
        {
            bool wrapsPositive = value > +180;
            bool wrapsNegative = value < -180;
            if (wrapsPositive)
                value -= 360;
            if (wrapsNegative)
                value += 360;
            return value;
        }
        private Vector3 CleanRotation(Vector3 vector3)
        {
            Vector3 cleanRotation = new Vector3(
                CleanRotation(vector3.x),
                CleanRotation(vector3.y),
                CleanRotation(vector3.z));
            return cleanRotation;
        }

        private void AssignToolVisibility()
        {
            // https://forum.unity.com/threads/hiding-default-transform-handles.86760/
            int selectedIndex = this.selectedIndex.intValue;
            bool hideUnityTransformHandle = selectedIndex > 0;
            Tools.hidden = hideUnityTransformHandle;
        }
    }
}
