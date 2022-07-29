using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    internal abstract class GfzPathEditor : Editor
    {
        [SerializeField] private static bool nodeInfoFoldout = false;

        protected void DrawDefaults(GfzPathSegment rootNode)
        {
            GuiSimple.DefaultScript("Script", rootNode);
            nodeInfoFoldout = EditorGUILayout.Foldout(nodeInfoFoldout, "Node Info");
            if (nodeInfoFoldout)
            {
                EditorGUI.indentLevel++;
                GUI.enabled = false;
                EditorGUILayout.ObjectField(nameof(rootNode.Prev), rootNode.Prev, typeof(GfzPathSegment), false);
                EditorGUILayout.ObjectField(nameof(rootNode.Next), rootNode.Next, typeof(GfzPathSegment), false);
                GuiSimple.Float("Segment Length", rootNode.GetSegmentLength());
                GUI.enabled = true;
                EditorGUILayout.Vector3Field(nameof(rootNode.StartPosition), rootNode.StartPosition);
                EditorGUILayout.Vector3Field(nameof(rootNode.EndPosition), rootNode.EndPosition);
                EditorGUI.indentLevel--;
            }
        }

    }
}
