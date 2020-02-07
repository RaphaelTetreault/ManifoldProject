using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class MultipleIOSobj : ScriptableObject
{
    [SerializeField] protected ImportSobj[] import;
    [SerializeField] protected ExportSobj[] export;

    public void DoAction()
    {
        foreach (var i in import)
            i.Import();

        foreach (var e in export)
            e.Export();
    }
}

[CustomEditor(typeof(MultipleIOSobj), true)]
public class MultipleIOSobj_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        var editorTarget = target as MultipleIOSobj;

        if (GUILayout.Button("Run Actions"))
        {
            editorTarget.DoAction();
        }
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }
}