using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [CustomEditor(typeof(Line))]
    public class LineInspector : Editor
    {
        private void OnSceneGUI()
        {
            var line = target as Line;
            var handleTransform = line.transform;
            var handleRotation = Tools.pivotRotation == PivotRotation.Local
                ? handleTransform.rotation
                : Quaternion.identity;
            var p0 = handleTransform.TransformPoint(line.p0.position);
            var p1 = handleTransform.TransformPoint(line.p1.position);

            // POINT 0
            EditorGUI.BeginChangeCheck();
            p0 = Handles.DoPositionHandle(p0, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(line, "Move Point");
                EditorUtility.SetDirty(line);
                line.p0.position = handleTransform.InverseTransformPoint(p0);
            }

            // POINT 1v
            EditorGUI.BeginChangeCheck();
            p1 = Handles.DoPositionHandle(p1, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(line, "Move Point");
                EditorUtility.SetDirty(line);
                line.p1.position = handleTransform.InverseTransformPoint(p1);
            }

            Handles.color = Color.white;
            Handles.DrawLine(p0, p1);
        }
    }
}
