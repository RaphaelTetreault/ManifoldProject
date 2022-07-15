using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzLinePath))]
    internal class GfzLinePathEditor : Editor
    {
        SerializedProperty endPositionX;
        SerializedProperty endPositionY;
        SerializedProperty endPositionZ;
        SerializedProperty startRotationY;
        SerializedProperty startRotationZ;
        SerializedProperty endRotationY;
        SerializedProperty endRotationZ;
        SerializedProperty scaleCurvesXYZ;
        SerializedProperty animationCurveTRS;

        void OnEnable()
        {
            endPositionX = serializedObject.FindProperty(nameof(endPositionX));
            endPositionY = serializedObject.FindProperty(nameof(endPositionY));
            endPositionZ = serializedObject.FindProperty(nameof(endPositionZ));
            startRotationY = serializedObject.FindProperty(nameof(startRotationY));
            startRotationZ = serializedObject.FindProperty(nameof(startRotationZ));
            endRotationY = serializedObject.FindProperty(nameof(endRotationY));
            endRotationZ = serializedObject.FindProperty(nameof(endRotationZ));
            scaleCurvesXYZ = serializedObject.FindProperty(nameof(scaleCurvesXYZ));
            animationCurveTRS = serializedObject.FindProperty(nameof(animationCurveTRS));
        }

        public override void OnInspectorGUI()
        {
            var line = target as GfzLinePath;

            if (GUILayout.Button("Generate TRS"))
                line.UpdateTRS();
            if (GUILayout.Button("Update Max Time"))
                line.UpdateMaxKeyTimes();
            if (GUILayout.Button("Debug tangents"))
                DebugTangents(line);

            //if (GUILayout.Button("Smooth Circle Tangent P.X"))
            //{
            //    var curve = line.AnimationCurveTRS.Position.y;
            //    var keys = line.SmoothCircleTangentFromPrevious(curve, 1);
            //    line.AnimationCurveTRS.Position.x.keys = keys;
            //}
            //if (GUILayout.Button("Smooth Circle Tangent P.Y"))
            //{
            //    var curve = line.AnimationCurveTRS.Position.y;
            //    var keys = line.SmoothCircleTangentToNext(curve, 0);
            //    line.AnimationCurveTRS.Position.y.keys = keys;
            //}

            base.OnInspectorGUI();
        }

        private void DebugTangents(GfzLinePath linePath)
        {
            int index = 0;
            foreach (var key in linePath.AnimationCurveTRS.Scale.x.keys)
                Debug.Log($"TRS {index++}: in: {key.inTangent} out: {key.outTangent}");

            index = 0;
            foreach (var key in linePath.ScaleCurvesXYZ.x.keys)
                Debug.Log($"Scale {index++}: in: {key.inTangent} out: {key.outTangent}");
        }

        private void OnSceneGUI()
        {
            var line = target as GfzLinePath;

            var hacTRS = line.CreateHierarchichalAnimationCurveTRS(false);
            float length = line.GetLineLength();
            int nSteps = (int)Mathf.Ceil(length / line.Step);
            var matrices = new Matrix4x4[nSteps+1];
            for (int i = 0; i <= nSteps; i++)
            {
                var time = i / (float)nSteps * length;
                matrices[i] = hacTRS.EvaluateHierarchyMatrix(time);
                // debug
                var p = matrices[i].Position();
                var r = matrices[i].rotation.eulerAngles;
                var s = matrices[i].lossyScale;
            }

            const float thickness = 5f;
            Vector3 left = Vector3.left * 0.5f;
            Vector3 right = Vector3.right * 0.5f;

            // Start and en lines
            {
                Handles.color = Color.white;
                var mtx0 = matrices[0];
                var mtxN = matrices[matrices.Length - 1];
                var sl = mtx0.MultiplyPoint(left);
                var sr = mtx0.MultiplyPoint(right);
                var el = mtxN.MultiplyPoint(left);
                var er = mtxN.MultiplyPoint(right);
                Handles.DrawLine(sl, sr, thickness);
                Handles.DrawLine(el, er, thickness);
            }

            for (int i = 0; i < matrices.Length - 1; i++)
            {
                var mtx0 = matrices[i + 0];
                var mtx1 = matrices[i + 1];
                var l0 = mtx0.MultiplyPoint(left);
                var l1 = mtx1.MultiplyPoint(left);
                var r0 = mtx0.MultiplyPoint(right);
                var r1 = mtx1.MultiplyPoint(right);
                Handles.color = Color.white;
                Handles.DrawLine(l0, r0, 1.25f);
                Handles.color = Color.green;
                Handles.DrawLine(l0, l1, thickness);
                Handles.color = Color.red;
                Handles.DrawLine(r0, r1, thickness);

            }
        }
    }
}
