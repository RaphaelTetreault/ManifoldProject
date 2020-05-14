//using StarkTools.IO;
//using System;
//using System.IO;
//using UnityEngine;

//[Serializable]
//public class LiveCameraStageEx : IFile
//{
//    #region MEMBERS

//    [Header("Livecam Stage")]
//    [SerializeField]
//    string name;

//    [SerializeField]
//    protected CameraPanSobj[] pans;

//    #endregion

//    #region PROPERTIES

//    public string FileName
//    {
//        get => name;
//        set => name = value;
//    }

//    public CameraPanSobj[] Pans => pans;

//    #endregion

//    public void SetCameraPans(CameraPanSobj[] pans)
//    {
//        this.pans = pans;
//    }
//}
