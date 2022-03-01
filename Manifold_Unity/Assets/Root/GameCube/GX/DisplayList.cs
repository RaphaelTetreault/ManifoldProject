using Manifold;
using Manifold.EditorTools;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;


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
        public GXColor[] clr0;
        public GXColor[] clr1;
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
        public VertexAttributeTable Vat { get => vat; set => vat = value; }
        public GXAttributes Attributes { get => attributes; }
        public DisplayCommand GxCommand { get => gxCommand; set => gxCommand = value; }
        public ushort VertexCount { get => count; set => count = value; }

        public DisplayList(GXAttributes attr, VertexAttributeTable vat)
        {
            this.attributes = attr;
            this.vat = vat;
        }


        private void ReadPOS(BinaryReader reader, VertexAttributeFormat fmt, int i)
        {
            pos[i] = GXUtility.ReadPos(reader, fmt.pos.NElements, fmt.pos.ComponentFormat, fmt.pos.NFracBits);
        }
        private void ReadNRM(BinaryReader reader, VertexAttributeFormat fmt, int i)
        {
            nrm[i] = GXUtility.ReadNormal(reader, fmt.nrm.NElements, fmt.nrm.ComponentFormat, fmt.nrm.NFracBits);
        }
        private void ReadNBT(BinaryReader reader, VertexAttributeFormat fmt, int i)
        {
            nbt[i] = new NormalBinormalTangent();
            nbt[i].normal = GXUtility.ReadNormal(reader, fmt.nbt.NElements, fmt.nbt.ComponentFormat, fmt.nbt.NFracBits);
            nbt[i].binormal = GXUtility.ReadNormal(reader, fmt.nbt.NElements, fmt.nbt.ComponentFormat, fmt.nbt.NFracBits);
            nbt[i].tangent = GXUtility.ReadNormal(reader, fmt.nbt.NElements, fmt.nbt.ComponentFormat, fmt.nbt.NFracBits);
        }
        private void ReadCLR(BinaryReader reader, VertexAttribute fmt_clr, int i, GXColor[] clr)
        {
            var color = new GXColor(fmt_clr.ComponentFormat);
            color.Deserialize(reader);
            clr[i] =  color;
            // GXUtility.ReadColor(reader, fmt_clr.ComponentFormat);
        }
        private void ReadTEX(BinaryReader reader, VertexAttribute fmt_tex, int i, float2[] tex)
        {
            tex[i] = GXUtility.ReadUV(reader, fmt_tex.NElements, fmt_tex.ComponentFormat, fmt_tex.NFracBits);
        }
        private void ReadMTXIDX(BinaryReader reader, int i, byte[] mtx_idx)
        {
            mtx_idx[i] = reader.ReadByte();
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                // Read primitive Data
                reader.ReadX(ref gxCommand);
                reader.ReadX(ref count);

                // Load vertex attribute format (VAF) from vertex attribute table (VAT)
                var fmt = vat[gxCommand];

                // Check each component type, see if it is used
                bool hasPNMTXIDX = attributes.HasFlag(GXAttributes.GX_VA_PNMTXIDX);
                bool hasTEX0MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX0MTXIDX);
                bool hasTEX1MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX1MTXIDX);
                bool hasTEX2MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX2MTXIDX);
                bool hasTEX3MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX3MTXIDX);
                bool hasTEX4MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX4MTXIDX);
                bool hasTEX5MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX5MTXIDX);
                bool hasTEX6MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX6MTXIDX);
                bool hasTEX7MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX7MTXIDX);
                bool hasPOS_MTX_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_POS_MTX_ARRAY);
                bool hasNRM_MTX_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_NRM_MTX_ARRAY);
                bool hasTEX_MTX_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_TEX_MTX_ARRAY);
                bool hasLIGHT_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_LIGHT_ARRAY);
                bool hasPOS = attributes.HasFlag(GXAttributes.GX_VA_POS);
                bool hasNRM = attributes.HasFlag(GXAttributes.GX_VA_NRM);
                bool hasNBT = attributes.HasFlag(GXAttributes.GX_VA_NBT);
                bool hasCLR0 = attributes.HasFlag(GXAttributes.GX_VA_CLR0);
                bool hasCLR1 = attributes.HasFlag(GXAttributes.GX_VA_CLR1);
                bool hasTEX0 = attributes.HasFlag(GXAttributes.GX_VA_TEX0);
                bool hasTEX1 = attributes.HasFlag(GXAttributes.GX_VA_TEX1);
                bool hasTEX2 = attributes.HasFlag(GXAttributes.GX_VA_TEX2);
                bool hasTEX3 = attributes.HasFlag(GXAttributes.GX_VA_TEX3);
                bool hasTEX4 = attributes.HasFlag(GXAttributes.GX_VA_TEX4);
                bool hasTEX5 = attributes.HasFlag(GXAttributes.GX_VA_TEX5);
                bool hasTEX6 = attributes.HasFlag(GXAttributes.GX_VA_TEX6);
                bool hasTEX7 = attributes.HasFlag(GXAttributes.GX_VA_TEX7);

                // INITIALIZE ARRAYS
                // Currently unsupported
                if (hasTEX0MTXIDX || hasTEX1MTXIDX || hasTEX2MTXIDX || hasTEX3MTXIDX ||
                    hasTEX4MTXIDX || hasTEX5MTXIDX || hasTEX6MTXIDX || hasTEX7MTXIDX ||
                    hasPOS_MTX_ARRAY || hasNRM_MTX_ARRAY || hasTEX_MTX_ARRAY || hasLIGHT_ARRAY)
                    throw new NotImplementedException("Unsupported GXAttributes flag");
                //
                pn_mtx_idx = hasPNMTXIDX ? new byte[count] : new byte[0];
                pos = hasPOS ? new float3[count] : new float3[0];
                nrm = hasNRM ? new float3[count] : new float3[0];
                nbt = hasNBT ? new NormalBinormalTangent[count] : new NormalBinormalTangent[0];
                clr0 = hasCLR0 ? new GXColor[count] : new GXColor[0];
                clr1 = hasCLR1 ? new GXColor[count] : new GXColor[0];
                tex0 = hasTEX0 ? new float2[count] : new float2[0];
                tex1 = hasTEX1 ? new float2[count] : new float2[0];
                tex2 = hasTEX2 ? new float2[count] : new float2[0];
                tex3 = hasTEX3 ? new float2[count] : new float2[0];
                tex4 = hasTEX4 ? new float2[count] : new float2[0];
                tex5 = hasTEX5 ? new float2[count] : new float2[0];
                tex6 = hasTEX6 ? new float2[count] : new float2[0];
                tex7 = hasTEX7 ? new float2[count] : new float2[0];

                // For each existing component, add a delegate of their function to a list, called in order
                var actions = new List<Action<int>>();
                if (hasPNMTXIDX) actions.Add((int i) => { ReadMTXIDX(reader, i, pn_mtx_idx); });
                if (hasTEX0MTXIDX) actions.Add((int i) => { ReadMTXIDX(reader, i, tex0_mtx_idx); });
                if (hasTEX1MTXIDX) actions.Add((int i) => { ReadMTXIDX(reader, i, tex1_mtx_idx); });
                if (hasTEX2MTXIDX) actions.Add((int i) => { ReadMTXIDX(reader, i, tex2_mtx_idx); });
                if (hasTEX3MTXIDX) actions.Add((int i) => { ReadMTXIDX(reader, i, tex3_mtx_idx); });
                if (hasTEX4MTXIDX) actions.Add((int i) => { ReadMTXIDX(reader, i, tex4_mtx_idx); });
                if (hasTEX5MTXIDX) actions.Add((int i) => { ReadMTXIDX(reader, i, tex5_mtx_idx); });
                if (hasTEX6MTXIDX) actions.Add((int i) => { ReadMTXIDX(reader, i, tex6_mtx_idx); });
                if (hasTEX7MTXIDX) actions.Add((int i) => { ReadMTXIDX(reader, i, tex7_mtx_idx); });
                if (hasPOS) actions.Add((int i) => { ReadPOS(reader, fmt, i); });
                if (hasNRM) actions.Add((int i) => { ReadNRM(reader, fmt, i); });
                if (hasNBT) actions.Add((int i) => { ReadNBT(reader, fmt, i); });
                if (hasCLR0) actions.Add((int i) => { ReadCLR(reader, fmt.clr0, i, clr0); });
                if (hasCLR1) actions.Add((int i) => { ReadCLR(reader, fmt.clr1, i, clr1); });
                if (hasTEX0) actions.Add((int i) => { ReadTEX(reader, fmt.tex0, i, tex0); });
                if (hasTEX1) actions.Add((int i) => { ReadTEX(reader, fmt.tex1, i, tex1); });
                if (hasTEX2) actions.Add((int i) => { ReadTEX(reader, fmt.tex2, i, tex2); });
                if (hasTEX3) actions.Add((int i) => { ReadTEX(reader, fmt.tex3, i, tex3); });
                if (hasTEX4) actions.Add((int i) => { ReadTEX(reader, fmt.tex4, i, tex4); });
                if (hasTEX5) actions.Add((int i) => { ReadTEX(reader, fmt.tex5, i, tex5); });
                if (hasTEX6) actions.Add((int i) => { ReadTEX(reader, fmt.tex6, i, tex6); });
                if (hasTEX7) actions.Add((int i) => { ReadTEX(reader, fmt.tex7, i, tex7); });

                // READ IN DATA
                for (int i = 0; i < count; i++)
                {
                    foreach (var action in actions)
                    {
                        action.Invoke(i);
                    }
                }
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            // Check each component type, see if it is used
            bool hasPNMTXIDX = attributes.HasFlag(GXAttributes.GX_VA_PNMTXIDX);
            bool hasTEX0MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX0MTXIDX);
            bool hasTEX1MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX1MTXIDX);
            bool hasTEX2MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX2MTXIDX);
            bool hasTEX3MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX3MTXIDX);
            bool hasTEX4MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX4MTXIDX);
            bool hasTEX5MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX5MTXIDX);
            bool hasTEX6MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX6MTXIDX);
            bool hasTEX7MTXIDX = attributes.HasFlag(GXAttributes.GX_VA_TEX7MTXIDX);
            bool hasPOS_MTX_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_POS_MTX_ARRAY);
            bool hasNRM_MTX_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_NRM_MTX_ARRAY);
            bool hasTEX_MTX_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_TEX_MTX_ARRAY);
            bool hasLIGHT_ARRAY = attributes.HasFlag(GXAttributes.GX_VA_LIGHT_ARRAY);
            bool hasPOS = attributes.HasFlag(GXAttributes.GX_VA_POS);
            bool hasNRM = attributes.HasFlag(GXAttributes.GX_VA_NRM);
            bool hasNBT = attributes.HasFlag(GXAttributes.GX_VA_NBT);
            bool hasCLR0 = attributes.HasFlag(GXAttributes.GX_VA_CLR0);
            bool hasCLR1 = attributes.HasFlag(GXAttributes.GX_VA_CLR1);
            bool hasTEX0 = attributes.HasFlag(GXAttributes.GX_VA_TEX0);
            bool hasTEX1 = attributes.HasFlag(GXAttributes.GX_VA_TEX1);
            bool hasTEX2 = attributes.HasFlag(GXAttributes.GX_VA_TEX2);
            bool hasTEX3 = attributes.HasFlag(GXAttributes.GX_VA_TEX3);
            bool hasTEX4 = attributes.HasFlag(GXAttributes.GX_VA_TEX4);
            bool hasTEX5 = attributes.HasFlag(GXAttributes.GX_VA_TEX5);
            bool hasTEX6 = attributes.HasFlag(GXAttributes.GX_VA_TEX6);
            bool hasTEX7 = attributes.HasFlag(GXAttributes.GX_VA_TEX7);

            this.RecordStartAddress(writer);
            {
                for (int i = 0; i < count; i++)
                {
                    if (hasPNMTXIDX) writer.WriteX(pn_mtx_idx[i]);
                    if (hasTEX0MTXIDX) writer.WriteX(tex0_mtx_idx[i]);
                    if (hasTEX1MTXIDX) writer.WriteX(tex1_mtx_idx[i]);
                    if (hasTEX2MTXIDX) writer.WriteX(tex2_mtx_idx[i]);
                    if (hasTEX3MTXIDX) writer.WriteX(tex3_mtx_idx[i]);
                    if (hasTEX4MTXIDX) writer.WriteX(tex4_mtx_idx[i]);
                    if (hasTEX5MTXIDX) writer.WriteX(tex5_mtx_idx[i]);
                    if (hasTEX6MTXIDX) writer.WriteX(tex6_mtx_idx[i]);
                    if (hasTEX7MTXIDX) writer.WriteX(tex7_mtx_idx[i]);
                    if (hasPOS) writer.WriteX(pos[i]);
                    if (hasNRM) writer.WriteX(nrm[i]);
                    if (hasNBT) writer.WriteX(nbt[i]);
                    if (hasCLR0) writer.WriteX(clr0[i]);
                    if (hasCLR1) writer.WriteX(clr1[i]);
                    if (hasTEX0) writer.WriteX(tex0[i]);
                    if (hasTEX1) writer.WriteX(tex1[i]);
                    if (hasTEX2) writer.WriteX(tex2[i]);
                    if (hasTEX3) writer.WriteX(tex3[i]);
                    if (hasTEX4) writer.WriteX(tex4[i]);
                    if (hasTEX5) writer.WriteX(tex5[i]);
                    if (hasTEX6) writer.WriteX(tex6[i]);
                    if (hasTEX7) writer.WriteX(tex7[i]);
                }
            }
            this.RecordEndAddress(writer);
        }

    }
}