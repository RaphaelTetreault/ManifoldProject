using GameCube.GX;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;


namespace GameCube.GFZ.Gma2
{
    internal class Submesh :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private Material material;
        private DisplayList[] displayListCW;
        private DisplayList[] displayListCCW;
        private SkinnedMeshDescriptor skinnedMeshDescriptor;
        private DisplayList[] skinnedDisplayListCW;
        private DisplayList[] skinnedDisplayListCCW;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public bool IsSkinOrEffective { get; set; }
        public Material Material { get => material; set => material = value; }
        public DisplayList[] DisplayListCW { get => displayListCW; set => displayListCW = value; }
        public DisplayList[] DisplayListCCW { get => displayListCCW; set => displayListCCW = value; }
        public DisplayList[] SkinnedDisplayListCW { get => skinnedDisplayListCW; set => skinnedDisplayListCW = value; }
        public DisplayList[] SkinnedDisplayListCCW { get => skinnedDisplayListCCW; set => skinnedDisplayListCCW = value; }
        internal SkinnedMeshDescriptor SkinnedMeshDescriptor { get => skinnedMeshDescriptor; set => skinnedMeshDescriptor = value; }

        private DisplayList[] ReadDisplayLists(BinaryReader reader, int endAddress)
        {
            var displayLists = new List<DisplayList>();

            var gxBegin = reader.ReadByte();
            Assert.IsTrue(gxBegin == 0);

            while (true)
            {
                // Reasons to stop reading display list data
                bool isAtEnd = reader.BaseStream.Position >= endAddress;
                bool isEndOfFile = reader.IsAtEndOfStream();
                bool isFifoPadding = reader.PeekUint16() == 0;
                if (isAtEnd || isEndOfFile || isFifoPadding)
                    break;

                var displayList = new DisplayList(material.GxAttributes, VertexAttributeTable.GfzVat);
                displayList.Deserialize(reader);
                displayLists.Add(displayList);
            }
            reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

            return displayLists.ToArray();
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Read material. It has important properties to resolve some of the next values.
                reader.ReadX(ref material, true);
                reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

                // TODO: re-implement this stuff
                if (IsSkinOrEffective)
                {
                    throw new NotImplementedException();
                }


                int endAddress = new Pointer(reader.BaseStream.Position).address;

                if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCW))
                {
                    endAddress += material.MaterialDisplayListSize;
                    displayListCW = ReadDisplayLists(reader, endAddress);
                }

                if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCCW))
                {
                    endAddress += material.TranslucidMaterialDisplayListSize;
                    displayListCCW = ReadDisplayLists(reader, endAddress);
                }


                ////
                //if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCW))
                //    displayListCW = displayLists.ToArray();
                //if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCCW))
                //    displayListCCW = displayLists.ToArray();

                //// Read display lists if present
                //if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCW))
                //{
                //    displayListCW = new DisplayList(material.GxAttributes, VertexAttributeTable.GfzVat);
                //    displayListCW.Deserialize(reader);
                //    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                //}

                //if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCCW))
                //{
                //    displayListCCW = new DisplayList(material.GxAttributes, VertexAttributeTable.GfzVat);
                //    displayListCCW.Deserialize(reader);
                //    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                //}

                //// Read skin descriptor and display lists if present
                //if (IsSkinOrEffective)
                //{
                //    reader.ReadX(ref skinnedMeshDescriptor, true);
                //    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

                //    if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSkinnedCW))
                //    {
                //        skinnedDisplayListCW = new DisplayList(material.GxAttributes, VertexAttributeTable.GfzVat);
                //        skinnedDisplayListCW.Deserialize(reader);
                //        reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                //    }

                //    if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSkinnedCCW))
                //    {
                //        skinnedDisplayListCCW = new DisplayList(material.GxAttributes, VertexAttributeTable.GfzVat);
                //        skinnedDisplayListCCW.Deserialize(reader);
                //        reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                //    }
                //}
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
        }

    }

}