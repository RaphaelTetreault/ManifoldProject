using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class LiveCameraStage : IBinarySerializable, IFile
{
    #region MEMBERS

    /// <summary>
    /// Filename. This variable is called name to enable Unity to
    /// display this name in the Inspector.
    /// </summary>
    [Header("Livecam Stage")]
    [SerializeField]
    string name;

    [SerializeField]
    protected CameraPan[] pans;

    #endregion

    #region PROPERTIES

    public string FileName
    {
        get => name;
        set => name = value;
    }

    public CameraPan[] Pans => pans;

    #endregion

    #region METHODS

    public void Deserialize(BinaryReader reader)
    {
        var nPans = (int)(reader.BaseStream.Length / CameraPan.kSizeBytes);

        reader.ReadX(ref pans, nPans, true);
    }

    public void Serialize(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }

    #endregion
}
