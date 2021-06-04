using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.GMA
{
    [Serializable]
    public class GcmfSubmesh : IBinarySerializable, IBinaryAddressable
    {

        #region FIELDS


        [Header("GCMF Submesh")]
        [SerializeField]
        private AddressRange addressRange;
        [SerializeField]
        private bool isSkinOrEffective;

        // Perhaps find way to distinguish translucid mats from mats?
        [SerializeField] Material material;
        [SerializeField] GfzDisplayList displayList0;
        [SerializeField] GfzDisplayList displayList1;

        [SerializeField] ExtraDisplayListHeader extraDisplayListHeader;
        [SerializeField] GfzDisplayList extraDisplayList0;
        [SerializeField] GfzDisplayList extraDisplayList1;


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
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

                var renderExtraDisplayList0 = (material.VertexRenderFlags & RenderDisplayListSetting.RENDER_EX_DISPLAY_LIST_0) != 0;
                var renderExtraDisplayList1 = (material.VertexRenderFlags & RenderDisplayListSetting.RENDER_EX_DISPLAY_LIST_1) != 0;
                var renderExtraDisplayList01 = renderExtraDisplayList0 && renderExtraDisplayList1;
                return renderExtraDisplayList01;
            }
        }

        public Material Material => material;

        public GfzDisplayList DisplayList0 => displayList0;

        public GfzDisplayList DisplayList1 => displayList1;

        public ExtraDisplayListHeader ExtraDisplayListHeader => extraDisplayListHeader;

        public GfzDisplayList ExtraDisplayList0 => extraDisplayList0;

        public GfzDisplayList ExtraDisplayList1 => extraDisplayList1;


        #endregion

        #region METHODS


        public GcmfSubmesh() { }

        public GcmfSubmesh(bool isSkinOrEffective)
        {
            this.isSkinOrEffective = isSkinOrEffective;
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref material, true);
                var attrFlags = material.VertexDescriptorFlags;

                if (!isSkinOrEffective)
                {
                    displayList0 = new GfzDisplayList(attrFlags, material.MatDisplayListSize);
                    displayList1 = new GfzDisplayList(attrFlags, material.TlMatDisplayListSize);
                    reader.ReadX(ref displayList0, false);
                    reader.ReadX(ref displayList1, false);

                    if (IsRenderExtraDisplayLists)
                    {
                        reader.ReadX(ref extraDisplayListHeader, true);
                        extraDisplayList0 = new GfzDisplayList(attrFlags, extraDisplayListHeader.VertexSize0);
                        extraDisplayList1 = new GfzDisplayList(attrFlags, extraDisplayListHeader.VertexSize1);
                        reader.ReadX(ref extraDisplayList0, false);
                        reader.ReadX(ref extraDisplayList1, false);
                    }
                }
            }
            this.RecordEndAddress(reader);
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


        #endregion

    }
}