using GameCube.GX;
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

namespace GameCube.Games
{
    public static class FZeroGX
    {
        // TODO
        // pass GXAttrType at render time (where is index array?)

        private static readonly GxVertexAttributeFormat vaf0 = new GxVertexAttributeFormat()
        {
            pos = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_POS, GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_F32, 0),
            nrm = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_NRM, GXCompCnt_Rev2.GX_NRM_XYZ, GXCompType.GX_F32, 0),
            clr0 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_CLR0, GXCompCnt_Rev2.GX_CLR_RGBA, GXCompType.GX_F32, 0),
            tex0 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_TEX0, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32, 0),
            tex1 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_TEX1, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32, 0),
            tex2 = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_TEX2, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_F32, 0),
            nbt = new GxVertexAttribute(GXAttrType.GX_DIRECT, GXAttr.GX_VA_NBT, GXCompCnt_Rev2.GX_NRM_NBT, GXCompType.GX_F32, 0),
        };

        private static readonly GxVertexAttributeFormat vaf1 = new GxVertexAttributeFormat()
        {
            pos = new GxVertexAttribute(GXAttrType.GX_INDEX16, GXAttr.GX_VA_POS, GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16, 0),
            nrm = new GxVertexAttribute(GXAttrType.GX_INDEX16, GXAttr.GX_VA_NRM, GXCompCnt_Rev2.GX_POS_XYZ, GXCompType.GX_S16, 0),
            tex0 = new GxVertexAttribute(GXAttrType.GX_INDEX16, GXAttr.GX_VA_TEX0, GXCompCnt_Rev2.GX_TEX_ST, GXCompType.GX_S16, 0),
        };

        public static readonly GxVertexAttributeTable VAT = new GxVertexAttributeTable(vaf0, vaf1);
    }
}


namespace GameCube.FZeroGX.GMA
{
    [Serializable]
    public class GMA : IBinarySerializable, IFile
    {
        #region MEMBERS

        private const int kGmaHeaderSize = 8;
        public const uint kNullPtr = 0xFFFFFFFF; // or int -1

        [Header("GMA")]
        [SerializeField]
        string name;

        [SerializeField, Hex(8)]
        int gcmfCount;

        [SerializeField, Hex(8)]
        uint headerEndPosition;

        [SerializeField]
        GCMF[] gcmf;

        #endregion

        #region PROPERTIES

        public uint HeaderEndPosition => headerEndPosition;
        public int GcmfCount => gcmfCount;
        public int GcmfNamePtr => (0x08) + gcmfCount * 0x08;
        public GCMF[] GCMF => gcmf;
        public string FileName
        {
            get => name;
            set => name = value;
        }

        #endregion

        #region METHODS 

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref gcmfCount);
            reader.ReadX(ref headerEndPosition);

            gcmf = new GCMF[GcmfCount];
            for (int i = 0; i < gcmf.Length; i++)
            {
                uint gcmfRelativePtr = 0;
                uint nameRelativePtr = 0;
                reader.ReadX(ref gcmfRelativePtr);
                reader.ReadX(ref nameRelativePtr);

                // If 1st of 2 pointers is null, skip
                if (gcmfRelativePtr == kNullPtr)
                {
                    gcmf[i] = null;
                    continue;
                }

                var endPosition = reader.BaseStream.Position;

                // Get name of model
                var modelName = string.Empty;
                var modelNamePtr = nameRelativePtr + GcmfNamePtr;
                reader.BaseStream.Seek(modelNamePtr, SeekOrigin.Begin);
                reader.ReadXCString(ref modelName, Encoding.ASCII);

                // Init GCMF with file name and model name for debugging
                gcmf[i] = new GCMF();
                gcmf[i].ModelName = modelName;
                gcmf[i].FileName = FileName;

                // Load GCMF
                var gcmfPtr = gcmfRelativePtr + HeaderEndPosition;
                reader.BaseStream.Seek(gcmfPtr, SeekOrigin.Begin);
                reader.ReadX(ref gcmf[i], false);

                // reset stream for next entry
                reader.BaseStream.Seek(endPosition, SeekOrigin.Begin);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

