using StarkTools.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnalyzerSobj : ScriptableObject
{
    [Header("File IO")]
    [SerializeField]
    protected FileStreamSettings read = FileStreamSettings.Read;
    [SerializeField]
    protected FileStreamSettings write = FileStreamSettings.Write;

    #region PROPERTIES

    public virtual string ButtonText => $"Analyze {TypeName}";

    public virtual string ProcessMessage => string.Empty;

    public virtual string HelpBoxMessage => string.Empty;

    public virtual string TypeName => GetType().Name;

    #endregion

    #region METHODS 

    public abstract void Analyze();

    #endregion
}
