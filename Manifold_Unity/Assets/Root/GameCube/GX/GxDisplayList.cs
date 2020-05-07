using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using GameCube.FZeroGX;
using GameCube.FZeroGX.GMA;

namespace GameCube.GX
{
    [Serializable]
    public class GxDisplayList : IBinarySerializable, IBinaryAddressable
    {
        [Header("GX Vertex Page")]
        [SerializeField, Hex(8)] long startAddress;
        [SerializeField, Hex(8)] long endAddress;

        private GXAttrFlag_U32 attr;
        public GxDisplayCommand gxCmd;
        public ushort count;

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
        public Vector3[] pos;
        public Vector3[] nrm;
        public VectorNBT[] nbt;
        public Color32[] clr0;
        public Color32[] clr1;
        public Vector2[] tex0;
        public Vector2[] tex1;
        public Vector2[] tex2;
        public Vector2[] tex3;
        public Vector2[] tex4;
        public Vector2[] tex5;
        public Vector2[] tex6;
        public Vector2[] tex7;

        // TODO
        // pos mtx array
        // nrm mtx array
        // tex mtx array
        // light array

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

        public GxDisplayList(GXAttrFlag_U32 attr)
        {
            this.attr = attr;
        }

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref gxCmd, true);
            reader.ReadX(ref count);

            var vatIdx = (byte)gxCmd.VertexFormat;
            var vat = FzgxGxVat.FzgxVAT;
            var vaf = vat.GxVtxAttrFmts[vatIdx];

            //mtx
            pn_mtx_idx = (attr & GXAttrFlag_U32.GX_VA_PNMTXIDX) != 0
                ? new byte[count] : new byte[0];

            if ((attr & GXAttrFlag_U32.GX_VA_TEX0MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_TEX1MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_TEX2MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_TEX3MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_TEX4MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_TEX5MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_TEX6MTXIDX) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_TEX7MTXIDX) != 0)
                throw new NotImplementedException();

            if ((attr & GXAttrFlag_U32.GX_VA_POS_MTX_ARRAY) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_NRM_MTX_ARRAY) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_TEX_MTX_ARRAY) != 0)
                throw new NotImplementedException();
            if ((attr & GXAttrFlag_U32.GX_VA_LIGHT_ARRAY) != 0)
                throw new NotImplementedException();

            // Pos
            pos = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_POS)
                ? new Vector3[count] : new Vector3[0];
            // Normal
            nrm = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_NRM)
                ? new Vector3[count] : new Vector3[0];
            nbt = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_NBT)
                ? new VectorNBT[count] : new VectorNBT[0];
            // Color
            clr0 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_CLR0)
                ? new Color32[count] : new Color32[0];
            clr1 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_CLR1)
                ? new Color32[count] : new Color32[0];
            // Tex
            tex0 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_TEX0)
                ? new Vector2[count] : new Vector2[0];
            tex1 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_TEX1)
                ? new Vector2[count] : new Vector2[0];
            tex2 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_TEX2)
                ? new Vector2[count] : new Vector2[0];
            tex3 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_TEX3)
                ? new Vector2[count] : new Vector2[0];
            tex4 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_TEX4)
                ? new Vector2[count] : new Vector2[0];
            tex5 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_TEX5)
                ? new Vector2[count] : new Vector2[0];
            tex6 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_TEX6)
                ? new Vector2[count] : new Vector2[0];
            tex7 = vat.HasAttr(gxCmd, attr & GXAttrFlag_U32.GX_VA_TEX7)
                ? new Vector2[count] : new Vector2[0];


            for (int i = 0; i < count; i++)
            {
                if (pn_mtx_idx.Length > 0)
                    reader.ReadX(ref pn_mtx_idx[i]);


                if (pos.Length > 0)
                {
                    var fmt = vaf.pos;
                    pos[i] = GxUtility.ReadPos(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                }

                if (nrm.Length > 0)
                {
                    var fmt = vaf.nrm;
                    nrm[i] = GxUtility.ReadNormal(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                }
                if (nbt.Length > 0)
                {
                    var fmt = vaf.nbt;
                    nbt[i].normal = GxUtility.ReadNormal(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                    nbt[i].binormal = GxUtility.ReadNormal(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                    nbt[i].tangent = GxUtility.ReadNormal(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                }

                if (clr0.Length > 0)
                {
                    var fmt = vaf.clr0;
                    clr0[i] = GxUtility.ReadColorComponent(reader, fmt.componentFormat);
                }
                if (clr1.Length > 0)
                {
                    var fmt = vaf.clr1;
                    clr1[i] = GxUtility.ReadColorComponent(reader, fmt.componentFormat);

                }

                if (tex0.Length > 0)
                {
                    var fmt = vaf.tex0;
                    tex0[i] = GxUtility.ReadGxTextureST(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                }
                if (tex1.Length > 0)
                {
                    var fmt = vaf.tex1;
                    tex1[i] = GxUtility.ReadGxTextureST(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);

                }
                if (tex2.Length > 0)
                {
                    var fmt = vaf.tex2;
                    tex2[i] = GxUtility.ReadGxTextureST(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                }
                if (tex3.Length > 0)
                {
                    var fmt = vaf.tex3;
                    tex3[i] = GxUtility.ReadGxTextureST(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);

                }
                if (tex4.Length > 0)
                {
                    var fmt = vaf.tex4;
                    tex4[i] = GxUtility.ReadGxTextureST(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);

                }
                if (tex5.Length > 0)
                {
                    var fmt = vaf.tex5;
                    tex5[i] = GxUtility.ReadGxTextureST(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                }
                if (tex6.Length > 0)
                {
                    var fmt = vaf.tex6;
                    tex6[i] = GxUtility.ReadGxTextureST(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                }
                if (tex7.Length > 0)
                {
                    var fmt = vaf.tex7;
                    tex7[i] = GxUtility.ReadGxTextureST(reader, fmt.nElements, fmt.componentFormat, fmt.nFracBits);
                }
            }

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }

    [Serializable]
    public struct VectorNBT //: IBinarySerializable
    {
        public Vector3 normal;
        public Vector3 binormal;
        public Vector3 tangent;

        //public void Deserialize(BinaryReader reader)
        //{
        //    reader.ReadX(ref normal);
        //    reader.ReadX(ref binormal);
        //    reader.ReadX(ref tangent);
        //    // this requires parameters!
        //    throw new NotImplementedException();
        //}

        //public void Serialize(BinaryWriter writer)
        //{
        //    writer.WriteX(binormal);
        //    writer.WriteX(tangent);
        //}
    }
}