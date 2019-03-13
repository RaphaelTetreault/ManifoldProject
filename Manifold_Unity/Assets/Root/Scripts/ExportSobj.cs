using StarkTools.IO;
using System.IO;
using UnityEngine;

public abstract class ExportSobj<T> : ExportSobj
    where T : ScriptableObject, IBinarySerializable
{
    [SerializeField]
    protected T value;

    public override void Export()
    {
        // Get file without .asset
        var fileName = Path.GetFileNameWithoutExtension(value.name);
        var outputFilePath = $"{destinationDir}/{fileName}.{OutputFileExtension}";
        outputFilePath = UnityPathUtility.EnforceSystemSeparators(outputFilePath);

        using (var fileStream = File.Open(outputFilePath, write.mode, write.access, write.share))
        {
            using (var writer = new BinaryWriter(fileStream))
            {
                writer.WriteX(value);
            }
        }
    }
}

public abstract class ExportSobj : ScriptableObject
{
    [Header("File Write")]
    [SerializeField]
    protected FileStreamSettings write = FileStreamSettings.Write;

    [Header("Export Settings")]
    [SerializeField]
    //[BrowseFolderField("FZGX_StageEditor_UnityProject/")]
    protected string destinationDir;

    public virtual string ButtonText
    {
        get
        {
            return "Export";
        }
    }
    public abstract string ProcessMessage
    {
        get;
    }
    public abstract string HelpBoxMessage
    {
        get;
    }
    public abstract string OutputFileExtension
    {
        get;
    }

    public abstract void Export();
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomEditor(typeof(ExportSobj), true)]
    public class ExportSobj_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            var editorTarget = target as ExportSobj;

            if (GUILayout.Button(editorTarget.ButtonText))
            {
                editorTarget.Export();

                if (!string.IsNullOrEmpty(editorTarget.ProcessMessage))
                    Debug.Log(editorTarget.ProcessMessage);
            }
            if (!string.IsNullOrEmpty(editorTarget.HelpBoxMessage))
                EditorGUILayout.HelpBox(editorTarget.HelpBoxMessage, MessageType.Info);
            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}
#endif