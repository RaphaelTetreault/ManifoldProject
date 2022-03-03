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
        private DisplayList[] primaryDisplayListsOpaque;
        private DisplayList[] primaryDisplayListsTranslucid;
        private DisplayListDescriptor secondaryDisplayListDescriptor;
        private DisplayList[] secondaryDisplayListsOpaque;
        private DisplayList[] secondaryDisplayListsTranslucid;


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
        public DisplayList[] PrimaryDisplayListsOpaque { get => primaryDisplayListsOpaque; set => primaryDisplayListsOpaque = value; }
        public DisplayList[] PrimaryDisplayListsTranslucid { get => primaryDisplayListsTranslucid; set => primaryDisplayListsTranslucid = value; }
        public DisplayListDescriptor SecondaryMeshDescriptor { get => secondaryDisplayListDescriptor; set => secondaryDisplayListDescriptor = value; }
        public DisplayList[] SecondaryDisplayListsOpaque { get => secondaryDisplayListsOpaque; set => secondaryDisplayListsOpaque = value; }
        public DisplayList[] SecondaryDisplayListsTranslucid { get => secondaryDisplayListsTranslucid; set => secondaryDisplayListsTranslucid = value; }


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
                    primaryDisplayListsOpaque = ReadDisplayLists(reader, endAddress);
                }

                if (RenderPrimaryTranslucid)
                {
                    endAddress += displayListDescriptor.TranslucidMaterialSize;
                    primaryDisplayListsTranslucid = ReadDisplayLists(reader, endAddress);
                }

                if (RenderSecondary)
                {
                    reader.ReadX(ref secondaryDisplayListDescriptor);
                    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                    endAddress = new Pointer(reader.BaseStream.Position).address;

                    if (RenderSecondaryOpaque)
                    {
                        endAddress += secondaryDisplayListDescriptor.OpaqueMaterialSize;
                        secondaryDisplayListsOpaque = ReadDisplayLists(reader, endAddress);
                    }

                    if (RenderSecondaryTranslucid)
                    {
                        endAddress += secondaryDisplayListDescriptor.TranslucidMaterialSize;
                        secondaryDisplayListsTranslucid = ReadDisplayLists(reader, endAddress);
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
                    (primaryDisplayListsOpaque is null ? 0 : DisplayListRenderFlags.renderPrimaryOpaque) |
                    (primaryDisplayListsTranslucid is null ? 0 : DisplayListRenderFlags.renderPrimaryTranslucid) |
                    (secondaryDisplayListsOpaque is null ? 0 : DisplayListRenderFlags.renderSecondaryOpaque) |
                    (secondaryDisplayListsTranslucid is null ? 0 : DisplayListRenderFlags.renderSecondaryTranslucid);

                // Compute and store sizes
                displayListDescriptor.OpaqueMaterialSize = DisplayListsSizeOnDisk(primaryDisplayListsOpaque);
                displayListDescriptor.TranslucidMaterialSize = DisplayListsSizeOnDisk(primaryDisplayListsTranslucid);
                if (RenderSecondary)
                {
                    secondaryDisplayListDescriptor.OpaqueMaterialSize = DisplayListsSizeOnDisk(secondaryDisplayListsOpaque);
                    secondaryDisplayListDescriptor.TranslucidMaterialSize = DisplayListsSizeOnDisk(secondaryDisplayListsTranslucid);
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
                    WriteDisplayLists(writer, primaryDisplayListsOpaque, out displayListAddressRange);
                    Assert.IsTrue(displayListAddressRange.Size == displayListDescriptor.OpaqueMaterialSize, $"Serialized: {displayListAddressRange.Size},  {displayListDescriptor.OpaqueMaterialSize}");
                }

                if (RenderPrimaryTranslucid)
                {
                    WriteDisplayLists(writer, primaryDisplayListsTranslucid, out displayListAddressRange);
                    Assert.IsTrue(displayListAddressRange.Size == displayListDescriptor.TranslucidMaterialSize, $"Serialized: {displayListAddressRange.Size},  {displayListDescriptor.TranslucidMaterialSize}");
                }


                if (RenderSecondary)
                {
                    writer.WriteX(secondaryDisplayListDescriptor);
                    writer.AlignTo(GXUtility.GX_FIFO_ALIGN);

                    if (RenderSecondaryOpaque)
                    {
                        WriteDisplayLists(writer, secondaryDisplayListsOpaque, out displayListAddressRange);
                        Assert.IsTrue(displayListAddressRange.Size == secondaryDisplayListDescriptor.OpaqueMaterialSize);
                    }

                    if (RenderSecondaryTranslucid)
                    {
                        WriteDisplayLists(writer, secondaryDisplayListsTranslucid, out displayListAddressRange);
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

        private int DisplayListsSizeOnDisk(DisplayList[] displayLists)
        {
            int size = 0;

            if (displayLists is null || displayLists.Length == 0)
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

                // Add +1 for size of GXCommand
                // Add +2 for size of count (uint16)
                size += 3;
                size += sizeOfVertex * displayList.VertexCount;
            }

            // Add +1 for size of GX_NOP
            size += 1;

            // Add padding size if necessary
            var remainder = size % GXUtility.GX_FIFO_ALIGN;
            if (remainder > 0)
                size += GXUtility.GX_FIFO_ALIGN - remainder;


            return size;
        }

    }

}