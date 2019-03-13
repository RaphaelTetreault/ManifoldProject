using StarkTools.IO;
using System.IO;
using UnityEngine;
using UnityEditor;

public abstract class ImportSobj<T> : ImportSobj
        where T : ScriptableObject, IBinarySerializable
{
    [SerializeField]
    //[BrowseFolderField]
    protected string sourceFile;

    public override void Import()
    {
        using (var fileStream = File.Open(sourceFile, Read.mode, Read.access, Read.share))
        {
            using (var reader = new BinaryReader(fileStream))
            {
                var fileName = Path.GetFileNameWithoutExtension(sourceFile);
                var sobj = CreateFromBinary<T>(fileName, reader);
            }
        }
    }
}

public abstract class ImportSobj : ScriptableObject
{
    [Header("File Read")]
    [SerializeField]
    protected FileStreamSettings Read = FileStreamSettings.Read;

    [Header("Import Settings")]
    [SerializeField]
    //[BrowseFolderField("FZGX_StageEditor_UnityProject/")]
    protected string destinationDir;

    public virtual string ButtonText
    {
        get
        {
            return "Import";
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

    public abstract void Import();

    public TSobj Create<TSobj>(string fileName)
        where TSobj : ScriptableObject
    {
        var sobj = CreateInstance<TSobj>();
        var filePath = $"{destinationDir}/{fileName}.asset";
        AssetDatabase.CreateAsset(sobj, filePath);
        return sobj;
    }

    public TSobj CreateFromBinary<TSobj>(string fileName, BinaryReader reader)
        where TSobj : ScriptableObject, IBinarySerializable
    {
        var sobj = CreateInstance<TSobj>();
        var filePath = $"Assets/{destinationDir}/{fileName}.asset";
        AssetDatabase.CreateAsset(sobj, filePath);
        AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
        sobj.Deserialize(reader);
        return sobj;
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomEditor(typeof(ImportSobj), true)]
    public class ImportSobj_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            var editorTarget = target as ImportSobj;

            if (GUILayout.Button(editorTarget.ButtonText))
            {
                editorTarget.Import();

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