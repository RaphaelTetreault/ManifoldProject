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
    public class GcmfSubmesh : IBinarySerializable, IBinaryAddressable
    {
        [Header("GCMF Submesh")]
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;
        [SerializeField] bool isSkinOrEffective;

        #region MEMBERS

        // Perhaps find way to distinguish translucid mats from mats?
        [SerializeField] Material material;
        [SerializeField] FzgxDisplayList displayList0;
        [SerializeField] FzgxDisplayList displayList1;
        [SerializeField] ExtraDisplayListHeader extraDisplayListHeader;
        [SerializeField] FzgxDisplayList extraDisplayList0;
        [SerializeField] FzgxDisplayList extraDisplayList1;

        #endregion

        public GcmfSubmesh() { }

        public GcmfSubmesh(bool isSkinOrEffective)
        {
            this.isSkinOrEffective = isSkinOrEffective;
        }

        #region PROPERTIES

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

        public bool IsSkinOrEffective
        {
            get => isSkinOrEffective;
            set => isSkinOrEffective = value;
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
        public FzgxDisplayList DisplayList0 => displayList0;
        public FzgxDisplayList DisplayList1 => displayList1;
        public ExtraDisplayListHeader ExtraDisplayListHeader => extraDisplayListHeader;
        public FzgxDisplayList ExtraDisplayList0 => extraDisplayList0;
        public FzgxDisplayList ExtraDisplayList1 => extraDisplayList1;

        #endregion


        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref material, true);

            if (!isSkinOrEffective)
            {
                displayList0 = new FzgxDisplayList(material.MatDisplayListSize);
                displayList1 = new FzgxDisplayList(material.TlMatDisplayListSize);
                reader.ReadX(ref displayList0, false);
                reader.ReadX(ref displayList1, false);

                if (IsRenderExtraDisplayLists)
                {
                    reader.ReadX(ref extraDisplayListHeader, true);
                    extraDisplayList0 = new FzgxDisplayList(extraDisplayListHeader.VertexSize0);
                    extraDisplayList1 = new FzgxDisplayList(extraDisplayListHeader.VertexSize1);
                    reader.ReadX(ref extraDisplayList0, false);
                    reader.ReadX(ref extraDisplayList1, false);
                }
            }

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(material);

            if (!isSkinOrEffective)
            {
                writer.WriteX(displayList0);
                writer.WriteX(displayList1);

                if (IsRenderExtraDisplayLists)
                {
                    writer.WriteX(extraDisplayListHeader);
                    writer.WriteX(extraDisplayList0);
                    writer.WriteX(extraDisplayList1);
                }
            }
        }

    }
}