using StarkTools.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnalyzerSobj : ScriptableObject
{
    [Header("File Read")]
    [SerializeField]
    protected FileStreamSettings Read = FileStreamSettings.Read;

    #region PROPERTIES

    public virtual string ButtonText
    {
        get
        {
            return $"Analyze {TypeName}";
        }
    }

    public abstract string ProcessMessage
    {
        get;
    }

    public virtual string HelpBoxMessage
    {
        get { return string.Empty; }
    }

    public abstract string TypeName
    {
        get;
    }

    #endregion

    #region METHODS 

    public abstract void Analyze();

    #endregion
}
