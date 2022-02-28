using GameCube.GX;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    public class Submesh :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        private GcmfAttributes attributes;

        // FIELDS
        private Material material;
        private DisplayListDescriptor displayListDescriptor;
        private UnkSubmeshType unknown;
        private DisplayList[] primaryDisplayListOpaque;
        private DisplayList[] primaryDisplayListTranslucid;
        private DisplayListDescriptor secondaryDisplayListDescriptor;
        private DisplayList[] secondaryDisplayListOpaque;
        private DisplayList[] secondaryDisplayListTranslucid;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        /// <summary>
        /// A copy of the GCMF attributes of the parent GCMF class.
        /// </summary>
        public GcmfAttributes Attributes { get => attributes; set => attributes = value; }
        public bool IsSkinnedModel
        {
            get => attributes.HasFlag(GcmfAttributes.isSkinModel);
        }
        public bool IsPhysicsDrivenModel
        {
            get => attributes.HasFlag(GcmfAttributes.isEffectiveModel);
        }
        public bool IsStitchingModel
        {
            get => attributes.HasFlag(GcmfAttributes.isStitchingModel);
        }
        public bool Is16bitModel
        {
            get => attributes.HasFlag(GcmfAttributes.is16Bit);
        }

        public bool RenderPrimaryOpaque
        {
            get => material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderPrimaryOpaque);
        }
        public bool RenderPrimaryTranslucid
        {
            get => material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderPrimaryTranslucid);
        }
        public bool RenderSecondaryOpaque
        {
            get => material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSecondaryOpaque);
        }
        public bool RenderSecondaryTranslucid
        {
            get => material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSecondaryTranslucid);
        }

        public Material Material { get => material; set => material = value; }
        public DisplayListDescriptor DisplayListDescriptor { get => displayListDescriptor; set => displayListDescriptor = value; }
        public DisplayList[] DisplayListCW { get => primaryDisplayListOpaque; set => primaryDisplayListOpaque = value; }
        public DisplayList[] DisplayListCCW { get => primaryDisplayListTranslucid; set => primaryDisplayListTranslucid = value; }
        public DisplayListDescriptor SecondaryMeshDescriptor { get => secondaryDisplayListDescriptor; set => secondaryDisplayListDescriptor = value; }
        public DisplayList[] SecondaryDisplayListCW { get => secondaryDisplayListOpaque; set => secondaryDisplayListOpaque = value; }
        public DisplayList[] SecondaryDisplayListCCW { get => secondaryDisplayListTranslucid; set => secondaryDisplayListTranslucid = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                //
                reader.ReadX(ref material, true);
                reader.ReadX(ref displayListDescriptor, true);
                reader.ReadX(ref unknown, true);
                reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

                int endAddress = new Pointer(reader.BaseStream.Position).address;

                // If the GCMF this submesh is a part of has either of these attributes,
                // then it does not actually store any display lists (at least, not in
                // in a GX/GPU useable format). While some of the data /should/ be placed
                // in a DisplayList, it is easier to manage in their own container classes.
                // These "skinned" containers reside in the GCMF structure.
                if (IsSkinnedModel || IsPhysicsDrivenModel)
                {
                    this.RecordEndAddress(reader);
                    return;
                }

                if (RenderPrimaryOpaque)
                {
                    endAddress += displayListDescriptor.OpaqueMaterialSize;
                    primaryDisplayListOpaque = ReadDisplayLists(reader, endAddress);
                }

                if (RenderPrimaryTranslucid)
                {
                    endAddress += displayListDescriptor.TranslucidMaterialSize;
                    primaryDisplayListTranslucid = ReadDisplayLists(reader, endAddress);
                }

                if (RenderSecondaryOpaque || RenderSecondaryTranslucid)
                {
                    reader.ReadX(ref secondaryDisplayListDescriptor, true);
                    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                    endAddress = new Pointer(reader.BaseStream.Position).address;

                    if (RenderPrimaryOpaque)
                    {
                        endAddress += secondaryDisplayListDescriptor.OpaqueMaterialSize;
                        secondaryDisplayListOpaque = ReadDisplayLists(reader, endAddress);
                    }

                    if (RenderSecondaryTranslucid)
                    {
                        endAddress += secondaryDisplayListDescriptor.TranslucidMaterialSize;
                        secondaryDisplayListTranslucid = ReadDisplayLists(reader, endAddress);
                    }
                }
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                //writer.WriteX();
            }
            this.RecordEndAddress(writer);
            {
                throw new NotImplementedException();
            }
        }

        private DisplayList[] ReadDisplayLists(BinaryReader reader, int endAddress)
        {
            var displayLists = new List<DisplayList>();

            var gxNOP = reader.ReadByte();
            Assert.IsTrue(gxNOP == 0);

            while (!reader.IsAtEndOfStream())
            {
                // Reasons to stop reading display list data
                bool isAtEnd = reader.BaseStream.Position >= endAddress;
                bool isFifoPadding = reader.PeekUint8() == 0;
                if (isAtEnd || isFifoPadding)
                    break;

                var displayList = new DisplayList(material.VertexAttributes, VertexAttributeTable.GfzVat);
                displayList.Deserialize(reader);
                displayLists.Add(displayList);
            }
            reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

            return displayLists.ToArray();
        }

    }

}