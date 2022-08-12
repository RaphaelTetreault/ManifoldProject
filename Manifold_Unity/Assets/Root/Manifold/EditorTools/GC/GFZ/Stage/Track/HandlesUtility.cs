using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public static class HandlesUtility
    {
        public static bool PositionHandle(Vector3 position, Quaternion orientation, out Vector3 editedPosition)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 result = Handles.DoPositionHandle(position, orientation);
            bool didUserMoveHandle = EditorGUI.EndChangeCheck();

            editedPosition = didUserMoveHandle ? result : Vector3.zero;
            return didUserMoveHandle;
        }

        public static bool RotationHandle(Vector3 position, Quaternion orientation, out Quaternion editedOrientation)
        {
            EditorGUI.BeginChangeCheck();
            Quaternion result = Handles.DoRotationHandle(orientation, position);
            bool didUserMoveHandle = EditorGUI.EndChangeCheck();

            editedOrientation = didUserMoveHandle ? result : Quaternion.identity;
            return didUserMoveHandle;
        }
       
        public static bool EulerRotationHandle(Vector3 position, Quaternion orientation, out Vector3 eulerOrientationDelta)
        {
            eulerOrientationDelta = Vector3.zero;

            bool didUserMoveHandle = RotationHandle(position, orientation, out Quaternion editedOrientation);
            if (didUserMoveHandle)
            {
                eulerOrientationDelta = GetEulerDelta(orientation, editedOrientation);
            }
            return didUserMoveHandle;
        }

        private static float CleanRotation(float value)
        {
            bool wrapsPositive = value > +180;
            bool wrapsNegative = value < -180;

            if (wrapsPositive)
                value -= 360;
            if (wrapsNegative)
                value += 360;

            return value;
        }

        private static Vector3 CleanRotation(Vector3 vector3)
        {
            Vector3 cleanRotation = new Vector3(
                CleanRotation(vector3.x),
                CleanRotation(vector3.y),
                CleanRotation(vector3.z));

            // Maybe handle something whe we have two 180 values?

            return cleanRotation;
        }

        public static Vector3 GetEulerDelta(Quaternion orientation, Quaternion newOrientation)
        {
            Vector3 eulerOriginal = orientation.eulerAngles;
            Vector3 eulerEdited = newOrientation.eulerAngles;
            Vector3 eulerDelta = eulerEdited - eulerOriginal;
            Vector3 eulerDeltaClean = CleanRotation(eulerDelta);
            //Debug.Log($"og:{eulerOriginal}, edit:{eulerEdited}, delta:{eulerDelta}, clean:{eulerDeltaClean}");
            return eulerDeltaClean;
        }
    }
}
