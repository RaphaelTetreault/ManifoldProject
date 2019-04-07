using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MultiVis))]
public class MultiVisDrawer : PropertyDrawer
{
    public const int height = 4;

    public readonly string[] enums = {
         "0",  "1",  "2",  "3",  "4",  "5",  "6",  "7",
         "8",  "9", "10", "11", "12", "13", "14", "15",
        "16", "17", "18", "19", "20", "21", "22", "23",
        "24", "25", "26", "27", "28", "29", "30", "31",
    };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (attribute as MultiVis);
        var propInt = property.intValue;
        var propBytes = BitConverter.GetBytes(propInt);
        var pos = position;
        pos.height /= height;

        EditorGUI.PropertyField(pos, property, label);
        pos.y += pos.height;
        //pos.x += EditorGUIUtility.labelWidth;
        //pos.width -= EditorGUIUtility.labelWidth;

        EditorGUI.MaskField(pos, " ", property.intValue, enums);
        pos.y += pos.height;

        var text = propInt.ToString("X8");
        EditorGUI.TextField(pos, " ", text);
        pos.y += pos.height;

        var color = new Color32(propBytes[3], propBytes[2], propBytes[1], propBytes[0]);
        EditorGUI.ColorField(pos, " ", color);
        //pos.y += pos.height;
    }
}
