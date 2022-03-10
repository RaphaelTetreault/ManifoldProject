using GameCube.GX;
using Manifold;
using Manifold.IO;
using System.Collections.Generic;
using System.IO;

namespace GameCube.GFZ.GMA
{
    public class Submesh :
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONSTANTS
        /// <summary>
        /// GameCube GPU No-Operation opcode
        /// </summary>
        public const byte GX_NOP = 0x00;

        // METADATA
        private GcmfAttributes attributes;

        // FIELDS
        private Material material;
        private DisplayListDescriptor primaryDisplayListDescriptor;
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
        public bool Is16bitModel => attributes.HasFlag(GcmfAttributes.is16Bit);
        public bool IsPhysicsDrivenModel => attributes.HasFlag(GcmfAttributes.isEffectiveModel);
        public bool IsSkinnedModel => attributes.HasFlag(GcmfAttributes.isSkinModel);
        public bool IsStitchingModel => attributes.HasFlag(GcmfAttributes.isStitchingModel);
        public Material Material { get => material; set => material = value; }
        public DisplayListDescriptor PrimaryDisplayListDescriptor { get => primaryDisplayListDescriptor; set => primaryDisplayListDescriptor = value; }
        public DisplayList[] PrimaryDisplayListsOpaque { get => primaryDisplayListsOpaque; set => primaryDisplayListsOpaque = value; }
        public DisplayList[] PrimaryDisplayListsTranslucid { get => primaryDisplayListsTranslucid; set => primaryDisplayListsTranslucid = value; }
        public bool RenderPrimaryOpaque => material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderPrimaryOpaque);
        public bool RenderPrimaryTranslucid => material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderPrimaryTranslucid);
        public bool RenderSecondary => RenderSecondaryOpaque || RenderSecondaryTranslucid;
        public bool RenderSecondaryOpaque => material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSecondaryOpaque);
        public bool RenderSecondaryTranslucid => material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSecondaryTranslucid);
        public DisplayListDescriptor SecondaryDisplayListDescriptor { get => secondaryDisplayListDescriptor; set => secondaryDisplayListDescriptor = value; }
        public DisplayList[] SecondaryDisplayListsOpaque { get => secondaryDisplayListsOpaque; set => secondaryDisplayListsOpaque = value; }
        public DisplayList[] SecondaryDisplayListsTranslucid { get => secondaryDisplayListsTranslucid; set => secondaryDisplayListsTranslucid = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                //
                reader.ReadX(ref material);
                reader.ReadX(ref primaryDisplayListDescriptor);
                reader.ReadX(ref unknown);
                reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

                int endAddress = reader.GetPositionAsPointer().address;

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
                    endAddress += primaryDisplayListDescriptor.OpaqueMaterialSize;
                    primaryDisplayListsOpaque = ReadDisplayLists(reader, endAddress);
                }

                if (RenderPrimaryTranslucid)
                {
                    endAddress += primaryDisplayListDescriptor.TranslucidMaterialSize;
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
            // Reset the render flags based on instance data
            material.DisplayListRenderFlags =
                (primaryDisplayListsOpaque.IsNullOrEmpty() ? 0 : DisplayListRenderFlags.renderPrimaryOpaque) |
                (primaryDisplayListsTranslucid.IsNullOrEmpty() ? 0 : DisplayListRenderFlags.renderPrimaryTranslucid) |
                (secondaryDisplayListsOpaque.IsNullOrEmpty() ? 0 : DisplayListRenderFlags.renderSecondaryOpaque) |
                (secondaryDisplayListsTranslucid.IsNullOrEmpty() ? 0 : DisplayListRenderFlags.renderSecondaryTranslucid);

            // Temp variables to store ranges that display lists are serialized at, used to get size on disk
            var pdlOpaque = new AddressRange();
            var pdlTranslucid = new AddressRange();
            var sdlOpaque = new AddressRange();
            var sdlTranslucid = new AddressRange();

            this.RecordStartAddress(writer);
            {
                writer.WriteX(material);
                writer.WriteX(primaryDisplayListDescriptor);
                writer.WriteX(unknown);
                writer.WriteAlignment(GXUtility.GX_FIFO_ALIGN);

                if (RenderPrimaryOpaque)
                    WriteDisplayLists(writer, primaryDisplayListsOpaque, out pdlOpaque);

                if (RenderPrimaryTranslucid)
                    WriteDisplayLists(writer, primaryDisplayListsTranslucid, out pdlTranslucid);


                if (RenderSecondary)
                {
                    writer.WriteX(secondaryDisplayListDescriptor);
                    writer.WriteAlignment(GXUtility.GX_FIFO_ALIGN);

                    if (RenderSecondaryOpaque)
                        WriteDisplayLists(writer, secondaryDisplayListsOpaque, out sdlOpaque);

                    if (RenderSecondaryTranslucid)
                        WriteDisplayLists(writer, secondaryDisplayListsTranslucid, out sdlTranslucid);
                }

            }
            this.RecordEndAddress(writer);
            {
                // Now that we know the size of the display lists, update values and reserialize
                primaryDisplayListDescriptor.OpaqueMaterialSize = pdlOpaque.Size;
                primaryDisplayListDescriptor.TranslucidMaterialSize = pdlTranslucid.Size;
                writer.JumpToAddress(primaryDisplayListDescriptor.AddressRange.startAddress);
                writer.WriteX(primaryDisplayListDescriptor);

                if (RenderSecondary)
                {
                    secondaryDisplayListDescriptor.OpaqueMaterialSize = sdlOpaque.Size;
                    secondaryDisplayListDescriptor.TranslucidMaterialSize = sdlTranslucid.Size;
                    writer.JumpToAddress(secondaryDisplayListDescriptor.AddressRange.startAddress);
                    writer.WriteX(secondaryDisplayListDescriptor);
                }
            }
            this.SetWriterToEndAddress(writer);
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

                var displayList = new DisplayList(material.VertexAttributes, GfzGX.VAT);
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
                writer.WriteAlignment(GXUtility.GX_FIFO_ALIGN);
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