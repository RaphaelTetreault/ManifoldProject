﻿using GameCube.GX;
using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// BobJrSr
// https://github.com/bobjrsenior/GxUtils/blob/master/GxUtils/LibGxFormat/Gma/GMAFormat.xml
// https://github.com/bobjrsenior/GxUtils/blob/master/GxUtils/LibGxFormat/Gma/GcmfMesh.cs

// My old docs
// http://www.gc-forever.com/forums/viewtopic.php?f=35&t=1897&p=26717&hilit=TPL#p26717
// http://www.gc-forever.com/forums/viewtopic.php?t=2311
// https://docs.google.com/spreadsheets/d/1WS9H2GcPGnjbOo7KS37tiySXDHRIHE3kcTbHLG0ICgc/edit#gid=0

// LZ resources
// https://www.google.com/search?q=lempel+ziv&rlz=1C1CHBF_enCA839CA839&oq=lempel+&aqs=chrome.0.0j69i57j0l4.2046j0j4&sourceid=chrome&ie=UTF-8

// noclip.website
// https://github.com/magcius/noclip.website/blob/master/src/metroid_prime/mrea.ts

// Dolphin (for patching files)
// https://forums.dolphin-emu.org/Thread-how-to-decompile-patch-a-gamecube-game



namespace GameCube.FZeroGX.GMA
{
    [Serializable]
    public class GMA : IBinarySerializable, IFile
    {
        private const int kGmaHeaderSize = 8;

        #region MEMBERS

        [Header("GMA")]
        [SerializeField]
        string name; // File Name

        [SerializeField, Hex("00", 8)]
        int gcmfCount;

        [SerializeField, Hex("04", 8)]
        int headerSize;

        [SerializeField]
        GcmfPointerPair[] gcmfPointerPairs;

        [SerializeField]
        GCMF[] gcmf;

        #endregion

        #region PROPERTIES

        public string FileName
        {
            get => name;
            set => name = value;
        }

        public GCMF[] GCMF
            => gcmf;

        public int GcmfCount
            => gcmfCount;

        public int GcmfDataBasePtr
            => headerSize;

        public int GcmfNameBasePtr
            => (0x08) + gcmfCount * 0x08;

        public GcmfPointerPair[] GcmfPointerPairs
            => gcmfPointerPairs;

        #endregion

        #region METHODS 

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref gcmfCount);
            reader.ReadX(ref headerSize);

            gcmf = new GCMF[gcmfCount];
            gcmfPointerPairs = new GcmfPointerPair[gcmfCount];
            for (int i = 0; i < gcmfPointerPairs.Length; i++)
            {
                reader.ReadX(ref gcmfPointerPairs[i], true);

                // If 1st of 2 pointers is null, skip
                if (gcmfPointerPairs[i].GcmfDataRelPtr == GcmfPointerPair.kNullPtr)
                    continue;

                // Record address we'll need to return to
                var jumpAddress = reader.BaseStream.Position;

                // Load name of model
                var modelName = string.Empty;
                var modelNamePtr = GcmfNameBasePtr + gcmfPointerPairs[i].GcmfNameRelPtr;
                reader.BaseStream.Seek(modelNamePtr, SeekOrigin.Begin);
                reader.ReadXCString(ref modelName, Encoding.ASCII);

                // Init GCMF with file name and model name for debugging
                gcmf[i] = new GCMF
                {
                    ModelName = modelName,
                    FileName = FileName
                };

                // Load GCMF data
                var gcmfPtr = GcmfDataBasePtr + gcmfPointerPairs[i].GcmfDataRelPtr;
                reader.BaseStream.Seek(gcmfPtr, SeekOrigin.Begin);
                reader.ReadX(ref gcmf[i], false);

                // reset stream for next entry
                reader.BaseStream.Seek(jumpAddress, SeekOrigin.Begin);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
