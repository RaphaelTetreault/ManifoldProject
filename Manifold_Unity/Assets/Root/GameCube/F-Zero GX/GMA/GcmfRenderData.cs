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
        [SerializeField] bool isSkinOrEffective;

        #region MEMBERS

        // Perhaps find way to distinguish translucid mats from mats?
        [SerializeField] Material material;
        [SerializeField] FzgxDisplayList matDisplayObjectA;
        [SerializeField] FzgxDisplayList matDisplayObjectB;
        [SerializeField] SkinData skinData;
        [SerializeField] FzgxDisplayList skinDisplayObjectA;
        [SerializeField] FzgxDisplayList skinDisplayObjectB;

        #endregion

        public GcmfRenderData() { }

        public GcmfRenderData(bool isSkinOrEffective)
        {
            this.isSkinOrEffective = isSkinOrEffective;
        }

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

        public bool IsRenderExtraDisplayLists
        {
            get
            {
                if (material is null)
                    return false;

                var renderExtraDisplayList0 = (material.VertexRenderFlags & MatVertexRenderFlag_U8.RENDER_EX_DISPLAY_LIST_0) != 0;
                var renderExtraDisplayList1 = (material.VertexRenderFlags & MatVertexRenderFlag_U8.RENDER_EX_DISPLAY_LIST_1) != 0;
                var renderExtraDisplayList01 = renderExtraDisplayList0 && renderExtraDisplayList1;
                return renderExtraDisplayList01;
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

            if (!isSkinOrEffective)
            {
                matDisplayObjectA = new FzgxDisplayList(material.MatDisplayListSize);
                matDisplayObjectB = new FzgxDisplayList(material.TlMatDisplayListSize);
                reader.ReadX(ref matDisplayObjectA, false);
                reader.ReadX(ref matDisplayObjectB, false);

                if (IsRenderExtraDisplayLists)
                {
                    reader.ReadX(ref skinData, true);
                    skinDisplayObjectA = new FzgxDisplayList(skinData.VertexSize0);
                    skinDisplayObjectB = new FzgxDisplayList(skinData.VertexSize1);
                    reader.ReadX(ref skinDisplayObjectA, false);
                    reader.ReadX(ref skinDisplayObjectB, false);
                }
            }

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(material);

            if (!isSkinOrEffective)
            {
                writer.WriteX(matDisplayObjectA);
                writer.WriteX(matDisplayObjectB);

                if (IsRenderExtraDisplayLists)
                {
                    writer.WriteX(skinData);
                    writer.WriteX(skinDisplayObjectA);
                    writer.WriteX(skinDisplayObjectB);
                }
            }
        }

    }
}