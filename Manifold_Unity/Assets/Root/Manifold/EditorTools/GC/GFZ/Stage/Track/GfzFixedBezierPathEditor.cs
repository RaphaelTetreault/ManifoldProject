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
                        case SelectMode.RotationHandle: EulerRotationHandle(editorTarget, i); break;
                    }
                }
                CaptureEditorEvent();
            }
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

        /// <summary>
        /// Maps <paramref name="controlPoint"/> to underlying function.
        /// </summary>
        /// <param name="controlPoint"></param>
        /// <param name="editedPosition"></param>
        /// <returns></returns>
        private bool PositionHandle(FixedBezierPoint controlPoint, out Vector3 editedPosition)
            => HandlesUtility.PositionHandle(controlPoint.position, controlPoint.Orientation, out editedPosition);
       
        /// <summary>
        /// Create a position handle for the current <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="index"></param>
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

        /// <summary>
        /// Maps <paramref name="controlPoint"/> to underlying function.
        /// </summary>
        /// <param name="controlPoint"></param>
        /// <param name="eulerDelta"></param>
        /// <returns></returns>
        private bool EulerRotationHandle(FixedBezierPoint controlPoint, out Vector3 eulerDelta)
            => HandlesUtility.EulerRotationHandle(controlPoint.position, controlPoint.Orientation, out eulerDelta);

        /// <summary>
        /// Create a euler rotation handle for the current <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="index"></param>
        private void EulerRotationHandle(GfzFixedBezierPath tool, int index)
        {
            var controlPoint = tool.GetControlPoint(index);
            bool didUserMoveHandle = EulerRotationHandle(controlPoint, out Vector3 eulerDelta);
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
            bool hideUnityTransformHandle = selectedIndex > 0;
            Tools.hidden = hideUnityTransformHandle;
        }
    }
}
