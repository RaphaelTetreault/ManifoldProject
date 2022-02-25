using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

// temp for Color32
using UnityEngine;


namespace GameCube.GX
{
    [Serializable]
    public class DisplayList :
        IBinaryAddressable,
        IBinarySerializable
    {
        //
        private VertexAttributeTable vat;
        private GXAttributes attributes;
        
        //
        private DisplayCommand gxCommand;
        private ushort count;

        // Matrix index
        // TODO: confirm byte. Could be u16?
        public byte[] pn_mtx_idx;
        public byte[] tex0_mtx_idx;
        public byte[] tex1_mtx_idx;
        public byte[] tex2_mtx_idx;
        public byte[] tex3_mtx_idx;
        public byte[] tex4_mtx_idx;
        public byte[] tex5_mtx_idx;
        public byte[] tex6_mtx_idx;
        public byte[] tex7_mtx_idx;
        // Standard vertex data
        public float3[] pos;
        public float3[] nrm;
        public NormalBinormalTangent[] nbt;
        public Color32[] clr0;
        public Color32[] clr1;
        public float2[] tex0;
        public float2[] tex1;
        public float2[] tex2;
        public float2[] tex3;
        public float2[] tex4;
        public float2[] tex5;
        public float2[] tex6;
        public float2[] tex7;

        // TODO
        // pos mtx array
        // nrm mtx array
        // tex mtx array
        // light array


        public AddressRange AddressRange { get; set; }



        public DisplayList(GXAttributes attr, VertexAttributeTable vat)
        {
            this.attributes = attr;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Read primitive Data
                reader.ReadX(ref gxCommand, true);
                reader.ReadX(ref count);

                // Load vertex attribute format (VAF) from vertex attribute table (VAT)
                var vatIndex = gxCommand.VertexFormatIndex;
                var vaf = vat.GxVtxAttrFmts[vatIndex];

                //mtx
                pn_mtx_idx = (attributes & GXAttributes.GX_VA_PNMTXIDX) != 0
                    ? new byte[count] : new byte[0];

                if ((attributes & GXAttributes.GX_VA_TEX0MTXIDX) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_TEX1MTXIDX) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_TEX2MTXIDX) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_TEX3MTXIDX) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_TEX4MTXIDX) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_TEX5MTXIDX) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_TEX6MTXIDX) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_TEX7MTXIDX) != 0)
                    throw new NotImplementedException();

                if ((attributes & GXAttributes.GX_VA_POS_MTX_ARRAY) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_NRM_MTX_ARRAY) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_TEX_MTX_ARRAY) != 0)
                    throw new NotImplementedException();
                if ((attributes & GXAttributes.GX_VA_LIGHT_ARRAY) != 0)
                    throw new NotImplementedException();

                // Pos
                pos = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_POS)
                    ? new float3[count] : new float3[0];
                // Normal
                nrm = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_NRM)
                    ? new float3[count] : new float3[0];
                nbt = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_NBT)
                    ? new NormalBinormalTangent[count] : new NormalBinormalTangent[0];
                // Color
                clr0 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_CLR0)
                    ? new Color32[count] : new Color32[0];
                clr1 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_CLR1)
                    ? new Color32[count] : new Color32[0];
                // Tex
                tex0 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_TEX0)
                    ? new float2[count] : new float2[0];
                tex1 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_TEX1)
                    ? new float2[count] : new float2[0];
                tex2 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_TEX2)
                    ? new float2[count] : new float2[0];
                tex3 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_TEX3)
                    ? new float2[count] : new float2[0];
                tex4 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_TEX4)
                    ? new float2[count] : new float2[0];
                tex5 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_TEX5)
                    ? new float2[count] : new float2[0];
                tex6 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_TEX6)
                    ? new float2[count] : new float2[0];
                tex7 = vat.HasAttr(gxCommand, attributes & GXAttributes.GX_VA_TEX7)
                    ? new float2[count] : new float2[0];

                // TODO: replace IFs with list of functions, construct list, then iterate?
                for (int i = 0; i < count; i++)
                {
                    if (pn_mtx_idx.Length > 0)
                        reader.ReadX(ref pn_mtx_idx[i]);

                    if (pos.Length > 0)
                    {
                        var fmt = vaf.pos;
                        pos[i] = GXUtility.ReadPos(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }

                    if (nrm.Length > 0)
                    {
                        var fmt = vaf.nrm;
                        nrm[i] = GXUtility.ReadNormal(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }
                    if (nbt.Length > 0)
                    {
                        var fmt = vaf.nbt;
                        nbt[i].normal = GXUtility.ReadNormal(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                        nbt[i].binormal = GXUtility.ReadNormal(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                        nbt[i].tangent = GXUtility.ReadNormal(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }

                    if (clr0.Length > 0)
                    {
                        var fmt = vaf.clr0;
                        clr0[i] = GXUtility.ReadColor(reader, fmt.ComponentFormat);
                    }
                    if (clr1.Length > 0)
                    {
                        var fmt = vaf.clr1;
                        clr1[i] = GXUtility.ReadColor(reader, fmt.ComponentFormat);
                    }

                    if (tex0.Length > 0)
                    {
                        var fmt = vaf.tex0;
                        tex0[i] = GXUtility.ReadUV(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }
                    if (tex1.Length > 0)
                    {
                        var fmt = vaf.tex1;
                        tex1[i] = GXUtility.ReadUV(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }
                    if (tex2.Length > 0)
                    {
                        var fmt = vaf.tex2;
                        tex2[i] = GXUtility.ReadUV(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }
                    if (tex3.Length > 0)
                    {
                        var fmt = vaf.tex3;
                        tex3[i] = GXUtility.ReadUV(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }
                    if (tex4.Length > 0)
                    {
                        var fmt = vaf.tex4;
                        tex4[i] = GXUtility.ReadUV(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }
                    if (tex5.Length > 0)
                    {
                        var fmt = vaf.tex5;
                        tex5[i] = GXUtility.ReadUV(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }
                    if (tex6.Length > 0)
                    {
                        var fmt = vaf.tex6;
                        tex6[i] = GXUtility.ReadUV(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }
                    if (tex7.Length > 0)
                    {
                        var fmt = vaf.tex7;
                        tex7[i] = GXUtility.ReadUV(reader, fmt.NElements, fmt.ComponentFormat, fmt.NFracBits);
                    }
                }
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}