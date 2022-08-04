using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [CustomEditor(typeof(GfzPropertyEmbed))]
    internal class GfzPropertyEmbedEditor : Editor
    {
        SerializedProperty type;
        SerializedProperty widthDivisions;
        SerializedProperty lengthDistance;
        SerializedProperty from;
        SerializedProperty to;
        SerializedProperty widthCurve;
        SerializedProperty offsetCurve;
        SerializedProperty autoGenerateTRS;
        SerializedProperty animationCurveTRS;

        // Tex/mesh options
        SerializedProperty includeTrimLeft;
        SerializedProperty includeTrimRight;
        SerializedProperty includeTrimStart;
        SerializedProperty includeTrimEnd;
        SerializedProperty repeatFlashingUV;

        void OnEnable()
        {
            type = serializedObject.FindProperty(nameof(type));
            widthDivisions = serializedObject.FindProperty(nameof(widthDivisions));
            lengthDistance = serializedObject.FindProperty(nameof(lengthDistance));
            from = serializedObject.FindProperty(nameof(from));
            to = serializedObject.FindProperty(nameof(to));
            widthCurve = serializedObject.FindProperty(nameof(widthCurve));
            offsetCurve = serializedObject.FindProperty(nameof(offsetCurve));
            autoGenerateTRS = serializedObject.FindProperty(nameof(autoGenerateTRS));
            animationCurveTRS = serializedObject.FindProperty(nameof(animationCurveTRS));

            includeTrimLeft = serializedObject.FindProperty(nameof(includeTrimLeft));
            includeTrimRight = serializedObject.FindProperty(nameof(includeTrimRight));
            includeTrimStart = serializedObject.FindProperty(nameof(includeTrimStart));
            includeTrimEnd = serializedObject.FindProperty(nameof(includeTrimEnd));
            repeatFlashingUV = serializedObject.FindProperty(nameof(repeatFlashingUV));
        }

        public override void OnInspectorGUI()
        {
            var embed = target as GfzPropertyEmbed;

            serializedObject.Update();
            {
                GuiSimple.DefaultScript("Script", embed);
                EditorGUILayout.Separator();
                DrawSourceFields(embed);
            }
            serializedObject.ApplyModifiedProperties();

            // Uncomment if you need to debug something
            //base.OnInspectorGUI();
        }

        public void DrawSourceFields(GfzPropertyEmbed embed)
        {
            GuiSimple.Label("Property Embed", EditorStyles.boldLabel);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(type);
                DrawFromToRange();
                DrawFromTo();
                EditorGUILayout.PropertyField(widthCurve);
                EditorGUILayout.PropertyField(offsetCurve);
                DrawOffsetShortcuts(embed);
                EditorGUI.indentLevel--;
            }
            GuiSimple.Label("Mesh", EditorStyles.boldLabel);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(widthDivisions);
                EditorGUILayout.PropertyField(lengthDistance);
                EditorGUILayout.PropertyField(includeTrimLeft);
                EditorGUILayout.PropertyField(includeTrimRight);
                EditorGUILayout.PropertyField(includeTrimStart);
                EditorGUILayout.PropertyField(includeTrimEnd);
                EditorGUILayout.PropertyField(repeatFlashingUV);
                EditorGUI.indentLevel--;
            }
        }

        public void DrawOffsetShortcuts(GfzPropertyEmbed embed)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Justify Offset");
            if (GUILayout.Button(" Left "))
            {
                Undo.RecordObject(embed, $"Justify embed left");
                //embed.SetOffsets(-1f);
                embed.SetOffsets(GfzPropertyEmbed.Jusification.Left);
                EditorUtility.SetDirty(embed);
            }
            if (GUILayout.Button("Center"))
            {
                Undo.RecordObject(embed, $"Justify embed center");
                embed.SetOffsets(GfzPropertyEmbed.Jusification.Center);
                EditorUtility.SetDirty(embed);
            }
            if (GUILayout.Button("Right "))
            {
                Undo.RecordObject(embed, $"Justify embed right");
                embed.SetOffsets(GfzPropertyEmbed.Jusification.Right);
                EditorUtility.SetDirty(embed);
            }
            GUILayout.EndHorizontal();
        }

        public void DrawFromToRange()
        {
            // slider
            float start = from.floatValue;
            float end = to.floatValue;
            EditorGUILayout.MinMaxSlider("Embed Range", ref start, ref end, 0f, 1f);

            // Only allow if range is non-zero
            float delta = end - start;
            bool isValid = delta > 0f;
            if (isValid)
            {
                from.floatValue = start;
                to.floatValue = end;
            }
        }

        public void DrawFromTo()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Range");

            // FROM
            GUILayout.Label("From");
            EditorGUI.BeginChangeCheck();
            float newFrom = EditorGUILayout.FloatField(from.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                bool isValid = newFrom < to.floatValue;
                if (isValid)
                {
                    from.floatValue = Mathf.Clamp01(newFrom);
                }
            }

            // TO
            GUILayout.Label("To");
            EditorGUI.BeginChangeCheck();
            float newTo = EditorGUILayout.FloatField(to.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                bool isValid = from.floatValue < newTo;
                if (isValid)
                {
                    to.floatValue = Mathf.Clamp01(newTo);
                }
            }

            GUILayout.EndHorizontal();
        }

    }
}
