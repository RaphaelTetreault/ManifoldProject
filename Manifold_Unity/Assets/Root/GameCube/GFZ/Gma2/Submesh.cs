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
        private DisplayListDescriptor displayListDescriptor;
        private UnkSubmeshType unknown;
        private DisplayList[] displayListCW;
        private DisplayList[] displayListCCW;
        private DisplayListDescriptor secondaryDisplayListDescriptor;
        private DisplayList[] secondaryDisplayListCW;
        private DisplayList[] secondaryDisplayListCCW;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public bool IsSkinOrEffective { get; set; }
        public Material Material { get => material; set => material = value; }
        internal DisplayListDescriptor DisplayListDescriptor { get => displayListDescriptor; set => displayListDescriptor = value; }
        public DisplayList[] DisplayListCW { get => displayListCW; set => displayListCW = value; }
        public DisplayList[] DisplayListCCW { get => displayListCCW; set => displayListCCW = value; }
        public DisplayListDescriptor SecondaryMeshDescriptor { get => secondaryDisplayListDescriptor; set => secondaryDisplayListDescriptor = value; }
        public DisplayList[] SecondaryDisplayListCW { get => secondaryDisplayListCW; set => secondaryDisplayListCW = value; }
        public DisplayList[] SecondaryDisplayListCCW { get => secondaryDisplayListCCW; set => secondaryDisplayListCCW = value; }

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
                //bool isFifoPadding = reader.PeekUint8() == 0;
                if (isAtEnd || isEndOfFile || reader.PeekUint8() == 0)
                    break;

                var displayList = new DisplayList(material.VertexAttributes, VertexAttributeTable.GfzVat);
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
                //
                reader.ReadX(ref material, true);
                reader.ReadX(ref displayListDescriptor, true);
                reader.ReadX(ref unknown, true);
                reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

                int endAddress = new Pointer(reader.BaseStream.Position).address;

                // TODO: re-implement this stuff
                if (IsSkinOrEffective)
                {
                    UnityEngine.Debug.LogWarning("Skipping IsSkin/IsEffective");
                    return;
                    //throw new NotImplementedException();
                }

                if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCW))
                {
                    endAddress += displayListDescriptor.MaterialSize;
                    displayListCW = ReadDisplayLists(reader, endAddress);
                }

                if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCCW))
                {
                    endAddress += displayListDescriptor.TranslucidMaterialSize;
                    displayListCCW = ReadDisplayLists(reader, endAddress);
                }

                if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSecondaryCW) ||
                    material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSecondaryCCW))
                {
                    reader.ReadX(ref secondaryDisplayListDescriptor, true);
                    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                    endAddress = new Pointer(reader.BaseStream.Position).address;

                    if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSecondaryCW))
                    {
                        endAddress += secondaryDisplayListDescriptor.MaterialSize;
                        displayListCCW = ReadDisplayLists(reader, endAddress);
                    }

                    if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSecondaryCCW))
                    {
                        endAddress += secondaryDisplayListDescriptor.TranslucidMaterialSize;
                        displayListCCW = ReadDisplayLists(reader, endAddress);
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
        }

    }

}