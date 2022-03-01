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
        // CONSTANTS
        public const byte GX_NOP = 0x00;

        // METADATA
        private GcmfAttributes attributes;
        private AddressRange displayListAddressRange;

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
        public bool RenderSecondary
        {
            get => RenderSecondaryOpaque || RenderSecondaryTranslucid;
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
                reader.ReadX(ref material);
                reader.ReadX(ref displayListDescriptor);
                reader.ReadX(ref unknown);
                reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

                int endAddress = reader.GetPositionAsPointer().Address;

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

                if (RenderSecondary)
                {
                    reader.ReadX(ref secondaryDisplayListDescriptor);
                    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                    endAddress = new Pointer(reader.BaseStream.Position).address;

                    if (RenderSecondaryOpaque)
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
            {
                // Reset the render flags based on instance data
                material.DisplayListRenderFlags =
                    (primaryDisplayListOpaque is null ? 0 : DisplayListRenderFlags.renderPrimaryOpaque) |
                    (primaryDisplayListTranslucid is null ? 0 : DisplayListRenderFlags.renderPrimaryTranslucid) |
                    (secondaryDisplayListOpaque is null ? 0 : DisplayListRenderFlags.renderSecondaryOpaque) |
                    (secondaryDisplayListTranslucid is null ? 0 : DisplayListRenderFlags.renderSecondaryTranslucid);

                // Compute and store sizes
                displayListDescriptor.OpaqueMaterialSize = DisplayListSizeOnDisk(primaryDisplayListOpaque);
                displayListDescriptor.TranslucidMaterialSize = DisplayListSizeOnDisk(primaryDisplayListTranslucid);
                if (RenderSecondary)
                {
                    secondaryDisplayListDescriptor.OpaqueMaterialSize = DisplayListSizeOnDisk(secondaryDisplayListOpaque);
                    secondaryDisplayListDescriptor.TranslucidMaterialSize = DisplayListSizeOnDisk(secondaryDisplayListTranslucid);
                }
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(material);
                writer.WriteX(displayListDescriptor);
                writer.WriteX(unknown);
                writer.AlignTo(GXUtility.GX_FIFO_ALIGN);

                if (RenderPrimaryOpaque)
                {
                    WriteDisplayLists(writer, primaryDisplayListOpaque, out displayListAddressRange);
                    Assert.IsTrue(displayListAddressRange.Size == displayListDescriptor.OpaqueMaterialSize, $"Serialized: {displayListAddressRange.Size},  {displayListDescriptor.TranslucidMaterialSize}");
                }

                if (RenderPrimaryTranslucid)
                {
                    WriteDisplayLists(writer, primaryDisplayListTranslucid, out displayListAddressRange);
                    Assert.IsTrue(displayListAddressRange.Size == displayListDescriptor.TranslucidMaterialSize, $"Serialized: {displayListAddressRange.Size},  {displayListDescriptor.TranslucidMaterialSize}");
                }


                if (RenderSecondary)
                {
                    writer.WriteX(secondaryDisplayListDescriptor);
                    writer.AlignTo(GXUtility.GX_FIFO_ALIGN);

                    if (RenderSecondaryOpaque)
                    {
                        WriteDisplayLists(writer, secondaryDisplayListOpaque, out displayListAddressRange);
                        Assert.IsTrue(displayListAddressRange.Size == secondaryDisplayListDescriptor.OpaqueMaterialSize);
                    }

                    if (RenderSecondaryTranslucid)
                    {
                        WriteDisplayLists(writer, secondaryDisplayListTranslucid, out displayListAddressRange);
                        Assert.IsTrue(displayListAddressRange.Size == secondaryDisplayListDescriptor.TranslucidMaterialSize);
                    }
                }

            }
            this.RecordEndAddress(writer);
        }

        private DisplayList[] ReadDisplayLists(BinaryReader reader, int endAddress)
        {
            var displayLists = new List<DisplayList>();

            var gxNOP = reader.ReadByte();
            Assert.IsTrue(gxNOP == GX_NOP);

            while (!reader.IsAtEndOfStream())
            {
                // Reasons to stop reading display list data
                bool isAtEnd = reader.BaseStream.Position >= endAddress;
                bool isFifoPadding = reader.PeekUInt8() == 0;
                if (isAtEnd || isFifoPadding)
                    break;

                var displayList = new DisplayList(material.VertexAttributes, VertexAttributeTable.GfzVat);
                displayList.Deserialize(reader);
                displayLists.Add(displayList);
            }
            reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

            return displayLists.ToArray();
        }

        private void WriteDisplayLists(BinaryWriter writer, DisplayList[] displayLists, out AddressRange addressRange)
        {
            addressRange = new AddressRange();
            addressRange.RecordStartAddress(writer);
            {
                writer.WriteX(GX_NOP);
                writer.WriteX(displayLists);
                writer.AlignTo(GXUtility.GX_FIFO_ALIGN);
            }
            addressRange.RecordEndAddress(writer);
        }

        private int DisplayListSizeOnDisk(DisplayList[] displayLists)
        {
            int size = 0;

            if (displayLists is null || displayLists.Length > 0)
                return size;

            // Get GX properties about display list
            var gxAttributes = displayLists[0].Attributes;
            var formatIndex = displayLists[0].GxCommand.VertexFormatIndex;
            var format = displayLists[0].Vat[formatIndex];

            // Compute size of all display lists
            int sizeOfVertex = GXUtility.GetGxVertexSize(gxAttributes, format);
            foreach (var displayList in displayLists)
            {
                // Every display list should have the same properties
                Assert.IsTrue(gxAttributes == displayList.Attributes);
                Assert.IsTrue(formatIndex == displayList.GxCommand.VertexFormatIndex);

                size += sizeOfVertex * displayList.VertexCount;
            }

            // Add +1 for size of GX_NOP
            size++;

            // Add padding size if necessary
            var remainder = size % GXUtility.GX_FIFO_ALIGN;
            if (remainder > 0)
                size += remainder;


            return size;
        }

    }

}