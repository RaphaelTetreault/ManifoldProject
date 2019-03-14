using StarkTools.IO;
using UnityEngine;

public abstract class ExportSobj : ScriptableObject
{
    [Header("File Write")]
    [SerializeField]
    protected FileStreamSettings write = FileStreamSettings.Write;

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