using StarkTools.IO;
using GameCube.GX;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.FZeroGX.GMA
{
    [Serializable]
    public class GcmfRenderData : IBinarySerializable, IBinaryAddressable
    {
        [Header("GCMF Render Data")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        #region MEMBERS

        // Perhaps find way to distinguish translucid mats from mats?
        [SerializeField] Material material;
        [SerializeField] FzgxDisplayList matDisplayObjectA;
        [SerializeField] FzgxDisplayList matDisplayObjectB;
        [SerializeField] SkinData skinData;
        [SerializeField] FzgxDisplayList skinDisplayObjectA;
        [SerializeField] FzgxDisplayList skinDisplayObjectB;

        #endregion

        #region PROPERTIES

        // Metadata
        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        public bool IsRenderSkinOrEffectiveAB
        {
            get
            {
                if (material is null)
                    return false;

                var isRenderSkinOrEffectiveA = (material.VertexRenderFlags & MatVertexRenderFlag_U8.RENDER_SKIN_OR_EFFECTIVE_A) != 0;
                var isRenderSkinOrEffectiveB = (material.VertexRenderFlags & MatVertexRenderFlag_U8.RENDER_SKIN_OR_EFFECTIVE_B) != 0;
                var isRenderSkinOrEffectiveAB = isRenderSkinOrEffectiveA && isRenderSkinOrEffectiveB;
                return isRenderSkinOrEffectiveAB;
            }
        }

        public Material Material => material;
        public FzgxDisplayList MatDisplayObjectA => matDisplayObjectA;
        public FzgxDisplayList MatDisplayObjectB => matDisplayObjectB;
        public SkinData SkinData => skinData;
        public FzgxDisplayList SkinDisplayObjectA => skinDisplayObjectA;
        public FzgxDisplayList SkinDisplayObjectB => skinDisplayObjectB;

        #endregion


        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref material, true);

            // MATERIAL
            //if (!isSkinOrEffective)
            if (IsRenderSkinOrEffectiveAB)
            {
                matDisplayObjectA = new FzgxDisplayList(material.MatDisplayListSize);
                matDisplayObjectB = new FzgxDisplayList(material.TlMatDisplayListSize);
                reader.ReadX(ref matDisplayObjectA, false);
                reader.ReadX(ref matDisplayObjectB, false);
            }

            // SKIN
            if (IsRenderSkinOrEffectiveAB)
            {
                reader.ReadX(ref skinData, true);
                skinDisplayObjectA = new FzgxDisplayList(skinData.VertexSize0);
                skinDisplayObjectB = new FzgxDisplayList(skinData.VertexSize1);
                reader.ReadX(ref skinDisplayObjectA, false);
                reader.ReadX(ref skinDisplayObjectB, false);
            }

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(material);

            // MATERIAL
            //if (!isSkinOrEffective)
            if (!IsRenderSkinOrEffectiveAB)
            {
                writer.WriteX(matDisplayObjectA);
                writer.WriteX(matDisplayObjectB);
            }

            // SKIN
            if (IsRenderSkinOrEffectiveAB)
            {
                writer.WriteX(skinData);
                writer.WriteX(skinDisplayObjectA);
                writer.WriteX(skinDisplayObjectB);
            }
        }
    }
}