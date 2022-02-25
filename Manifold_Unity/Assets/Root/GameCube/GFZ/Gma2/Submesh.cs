using GameCube.GX;
using Manifold;
using Manifold.IO;
using System.IO;


namespace GameCube.GFZ.Gma2
{
    internal class Submesh :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private Material material;
        private DisplayList displayListCW;
        private DisplayList displayListCCW;
        private SkinnedMeshDescriptor skinnedMeshDescriptor;
        private DisplayList skinnedDisplayListCW;
        private DisplayList skinnedDisplayListCCW;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public bool IsSkinOrEffective { get; set; }

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Read material. It has important properties to resolve some of the next values.
                reader.ReadX(ref material, true);
                reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

                // Read display lists if present
                if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCW))
                {
                    displayListCW = new DisplayList(material.GxAttributes, VertexAttributeTable.GfzVat);
                    displayListCW.Deserialize(reader);
                    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                }

                if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderCCW))
                {
                    displayListCCW = new DisplayList(material.GxAttributes, VertexAttributeTable.GfzVat);
                    displayListCCW.Deserialize(reader);
                    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                }

                // Read skin descriptor and display lists if present
                if (IsSkinOrEffective)
                {
                    reader.ReadX(ref skinnedMeshDescriptor, true);
                    reader.AlignTo(GXUtility.GX_FIFO_ALIGN);

                    if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSkinnedCW))
                    {
                        skinnedDisplayListCW = new DisplayList(material.GxAttributes, VertexAttributeTable.GfzVat);
                        skinnedDisplayListCW.Deserialize(reader);
                        reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
                    }

                    if (material.DisplayListRenderFlags.HasFlag(DisplayListRenderFlags.renderSkinnedCCW))
                    {
                        skinnedDisplayListCCW = new DisplayList(material.GxAttributes, VertexAttributeTable.GfzVat);
                        skinnedDisplayListCCW.Deserialize(reader);
                        reader.AlignTo(GXUtility.GX_FIFO_ALIGN);
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