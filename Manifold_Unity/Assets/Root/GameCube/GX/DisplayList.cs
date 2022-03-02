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
        public byte[] pn_mtx_idx;
        public byte[] tex0_mtx_idx;
        public byte[] tex1_mtx_idx;
        public byte[] tex2_mtx_idx;
        public byte[] tex3_mtx_idx;
        public byte[] tex4_mtx_idx;
        public byte[] tex5_mtx_idx;
        public byte[] tex6_mtx_idx;
        public byte[] tex7_mtx_idx;
        public float3[] pos;
        public float3[] nrm; // normal
        public float3[] bnm; // binormal
        public float3[] tan; // tangent
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
        public GXAttributes Attributes { get => attributes; set => attributes = value; }
        public DisplayCommand GxCommand { get => gxCommand; set => gxCommand = value; }
        public ushort VertexCount { get => count; set => count = value; }

        public DisplayList(GXAttributes attr, VertexAttributeTable vat)
        {
            this.attributes = attr;
            this.vat = vat;
        }


        private void ReadPOS(BinaryReader reader, VertexAttributeFormat fmt, int i)
        {
            pos[i] = GXUtility.ReadPos(reader, fmt.pos.NElements, fmt.pos.ComponentType, fmt.pos.NFracBits);
        }
        private void ReadNRM(BinaryReader reader, VertexAttributeFormat fmt, int i)
        {
            nrm[i] = GXUtility.ReadNormal(reader, fmt.nrm.NElements, fmt.nrm.ComponentType, fmt.nrm.NFracBits);
        }
        private void ReadNBT(BinaryReader reader, VertexAttributeFormat fmt, int i)
        {
            nrm[i] = GXUtility.ReadNormal(reader, fmt.nbt.NElements, fmt.nbt.ComponentType, fmt.nbt.NFracBits);
            bnm[i] = GXUtility.ReadNormal(reader, fmt.nbt.NElements, fmt.nbt.ComponentType, fmt.nbt.NFracBits);
            tan[i] = GXUtility.ReadNormal(reader, fmt.nbt.NElements, fmt.nbt.ComponentType, fmt.nbt.NFracBits);
        }
        private void ReadCLR(BinaryReader reader, VertexAttribute va, int i, GXColor[] clr)
        {
            var color = new GXColor(va.ComponentType);
            color.Deserialize(reader);
            clr[i] = color;
        }
        private void ReadTEX(BinaryReader reader, VertexAttribute va, int i, float2[] tex)
        {
            tex[i] = GXUtility.ReadUV(reader, va.NElements, va.ComponentType, va.NFracBits);
        }
        private void ReadMTXIDX(BinaryReader reader, int i, byte[] mtx_idx)
        {
            reader.ReadX(ref mtx_idx[i]);
        }


        private void WritePOS(BinaryWriter writer, VertexAttributeFormat fmt, int i)
        {
            var va = fmt.pos;
            GXUtility.WritePosition(writer, pos[i], va.NElements, va.ComponentType, va.NFracBits);
        }
        private void WriteNRM(BinaryWriter writer, VertexAttributeFormat fmt, int i)
        {
            var va = fmt.nrm;
            GXUtility.WriteNormal(writer, nrm[i], va.NElements, va.ComponentType, va.NFracBits);
        }
        private void WriteNBT(BinaryWriter writer, VertexAttributeFormat fmt, int i)
        {
            var va = fmt.nbt;
            GXUtility.WriteNormal(writer, nrm[i], va.NElements, va.ComponentType, va.NFracBits);
            GXUtility.WriteNormal(writer, bnm[i], va.NElements, va.ComponentType, va.NFracBits);
            GXUtility.WriteNormal(writer, tan[i], va.NElements, va.ComponentType, va.NFracBits);
        }
        private void WriteCLR(BinaryWriter writer, VertexAttribute va, int i, GXColor[] clr)
        {
            clr[i].ComponentType = va.ComponentType;
            clr[i].Serialize(writer);
        }
        private void WriteTEX(BinaryWriter writer, VertexAttribute va, int i, float2[] tex)
        {
            GXUtility.WriteUV(writer, tex[i], va.NElements, va.ComponentType, va.NFracBits);
        }
        private void WriteMTXIDX(BinaryWriter writer, int i, byte[] mtx_idx)
        {
            writer.WriteX(mtx_idx[i]);
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
                // Currently unsupported. TEX#MTXIDX should work but is untested.
                if (hasTEX0MTXIDX || hasTEX1MTXIDX || hasTEX2MTXIDX || hasTEX3MTXIDX ||
                    hasTEX4MTXIDX || hasTEX5MTXIDX || hasTEX6MTXIDX || hasTEX7MTXIDX ||
                    hasPOS_MTX_ARRAY || hasNRM_MTX_ARRAY || hasTEX_MTX_ARRAY || hasLIGHT_ARRAY)
                    throw new NotImplementedException("Unsupported GXAttributes flag");

                //
                Assert.IsTrue(hasNRM ^ hasNBT, "Data has both NRM and NBT. NRM data will be overwritten by NBT.NRM");

                //
                pn_mtx_idx = hasPNMTXIDX ? new byte[count] : new byte[0];
                pos = hasPOS ? new float3[count] : new float3[0];
                nrm = hasNRM || hasNBT ? new float3[count] : new float3[0];
                bnm = hasNBT ? new float3[count] : new float3[0];
                tan = hasNBT ? new float3[count] : new float3[0];
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
                var deserializeComponents = new List<Action<int>>(32);
                if (hasPNMTXIDX) deserializeComponents.Add((int i) => { ReadMTXIDX(reader, i, pn_mtx_idx); });
                if (hasTEX0MTXIDX) deserializeComponents.Add((int i) => { ReadMTXIDX(reader, i, tex0_mtx_idx); });
                if (hasTEX1MTXIDX) deserializeComponents.Add((int i) => { ReadMTXIDX(reader, i, tex1_mtx_idx); });
                if (hasTEX2MTXIDX) deserializeComponents.Add((int i) => { ReadMTXIDX(reader, i, tex2_mtx_idx); });
                if (hasTEX3MTXIDX) deserializeComponents.Add((int i) => { ReadMTXIDX(reader, i, tex3_mtx_idx); });
                if (hasTEX4MTXIDX) deserializeComponents.Add((int i) => { ReadMTXIDX(reader, i, tex4_mtx_idx); });
                if (hasTEX5MTXIDX) deserializeComponents.Add((int i) => { ReadMTXIDX(reader, i, tex5_mtx_idx); });
                if (hasTEX6MTXIDX) deserializeComponents.Add((int i) => { ReadMTXIDX(reader, i, tex6_mtx_idx); });
                if (hasTEX7MTXIDX) deserializeComponents.Add((int i) => { ReadMTXIDX(reader, i, tex7_mtx_idx); });
                if (hasPOS) deserializeComponents.Add((int i) => { ReadPOS(reader, fmt, i); });
                if (hasNRM) deserializeComponents.Add((int i) => { ReadNRM(reader, fmt, i); });
                if (hasNBT) deserializeComponents.Add((int i) => { ReadNBT(reader, fmt, i); });
                if (hasCLR0) deserializeComponents.Add((int i) => { ReadCLR(reader, fmt.clr0, i, clr0); });
                if (hasCLR1) deserializeComponents.Add((int i) => { ReadCLR(reader, fmt.clr1, i, clr1); });
                if (hasTEX0) deserializeComponents.Add((int i) => { ReadTEX(reader, fmt.tex0, i, tex0); });
                if (hasTEX1) deserializeComponents.Add((int i) => { ReadTEX(reader, fmt.tex1, i, tex1); });
                if (hasTEX2) deserializeComponents.Add((int i) => { ReadTEX(reader, fmt.tex2, i, tex2); });
                if (hasTEX3) deserializeComponents.Add((int i) => { ReadTEX(reader, fmt.tex3, i, tex3); });
                if (hasTEX4) deserializeComponents.Add((int i) => { ReadTEX(reader, fmt.tex4, i, tex4); });
                if (hasTEX5) deserializeComponents.Add((int i) => { ReadTEX(reader, fmt.tex5, i, tex5); });
                if (hasTEX6) deserializeComponents.Add((int i) => { ReadTEX(reader, fmt.tex6, i, tex6); });
                if (hasTEX7) deserializeComponents.Add((int i) => { ReadTEX(reader, fmt.tex7, i, tex7); });

                // READ IN DATA
                for (int i = 0; i < count; i++)
                {
                    foreach (var deserializeComponent in deserializeComponents)
                    {
                        deserializeComponent.Invoke(i);
                    }
                }
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            // Build a GXAttributes based on the component arrays that are non-zero length
            attributes = ComponentsToGXAttributes();

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

            var fmt = vat[gxCommand];

            // For each existing component, add a delegate of their function to a list, called in order
            var serializeComponents = new List<Action<int>>(32);
            if (hasPNMTXIDX) serializeComponents.Add((int i) => { WriteMTXIDX(writer, i, pn_mtx_idx); });
            if (hasTEX0MTXIDX) serializeComponents.Add((int i) => { WriteMTXIDX(writer, i, tex0_mtx_idx); });
            if (hasTEX1MTXIDX) serializeComponents.Add((int i) => { WriteMTXIDX(writer, i, tex1_mtx_idx); });
            if (hasTEX2MTXIDX) serializeComponents.Add((int i) => { WriteMTXIDX(writer, i, tex2_mtx_idx); });
            if (hasTEX3MTXIDX) serializeComponents.Add((int i) => { WriteMTXIDX(writer, i, tex3_mtx_idx); });
            if (hasTEX4MTXIDX) serializeComponents.Add((int i) => { WriteMTXIDX(writer, i, tex4_mtx_idx); });
            if (hasTEX5MTXIDX) serializeComponents.Add((int i) => { WriteMTXIDX(writer, i, tex5_mtx_idx); });
            if (hasTEX6MTXIDX) serializeComponents.Add((int i) => { WriteMTXIDX(writer, i, tex6_mtx_idx); });
            if (hasTEX7MTXIDX) serializeComponents.Add((int i) => { WriteMTXIDX(writer, i, tex7_mtx_idx); });
            if (hasPOS) serializeComponents.Add((int i) => { WritePOS(writer, fmt, i); });
            if (hasNRM) serializeComponents.Add((int i) => { WriteNRM(writer, fmt, i); });
            if (hasNBT) serializeComponents.Add((int i) => { WriteNBT(writer, fmt, i); });
            if (hasCLR0) serializeComponents.Add((int i) => { WriteCLR(writer, fmt.clr0, i, clr0); });
            if (hasCLR1) serializeComponents.Add((int i) => { WriteCLR(writer, fmt.clr1, i, clr1); });
            if (hasTEX0) serializeComponents.Add((int i) => { WriteTEX(writer, fmt.tex0, i, tex0); });
            if (hasTEX1) serializeComponents.Add((int i) => { WriteTEX(writer, fmt.tex1, i, tex1); });
            if (hasTEX2) serializeComponents.Add((int i) => { WriteTEX(writer, fmt.tex2, i, tex2); });
            if (hasTEX3) serializeComponents.Add((int i) => { WriteTEX(writer, fmt.tex3, i, tex3); });
            if (hasTEX4) serializeComponents.Add((int i) => { WriteTEX(writer, fmt.tex4, i, tex4); });
            if (hasTEX5) serializeComponents.Add((int i) => { WriteTEX(writer, fmt.tex5, i, tex5); });
            if (hasTEX6) serializeComponents.Add((int i) => { WriteTEX(writer, fmt.tex6, i, tex6); });
            if (hasTEX7) serializeComponents.Add((int i) => { WriteTEX(writer, fmt.tex7, i, tex7); });

            this.RecordStartAddress(writer);
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var serializeComponent in serializeComponents)
                    {
                        serializeComponent.Invoke(i);
                    }
                }
            }
            this.RecordEndAddress(writer);
        }


        public GXAttributes ComponentsToGXAttributes()
        {
            GXAttributes attributes = 0;
            if (pn_mtx_idx.Length > 0) attributes |= GXAttributes.GX_VA_PNMTXIDX;
            if (tex0_mtx_idx.Length > 0) attributes |= GXAttributes.GX_VA_TEX0MTXIDX;
            if (tex1_mtx_idx.Length > 0) attributes |= GXAttributes.GX_VA_TEX1MTXIDX;
            if (tex2_mtx_idx.Length > 0) attributes |= GXAttributes.GX_VA_TEX2MTXIDX;
            if (tex3_mtx_idx.Length > 0) attributes |= GXAttributes.GX_VA_TEX3MTXIDX;
            if (tex4_mtx_idx.Length > 0) attributes |= GXAttributes.GX_VA_TEX4MTXIDX;
            if (tex5_mtx_idx.Length > 0) attributes |= GXAttributes.GX_VA_TEX5MTXIDX;
            if (tex6_mtx_idx.Length > 0) attributes |= GXAttributes.GX_VA_TEX6MTXIDX;
            if (tex7_mtx_idx.Length > 0) attributes |= GXAttributes.GX_VA_TEX7MTXIDX;
            //if (.Length > 0) attributes |= GXAttributes.GX_VA_POS_MTX_ARRAY;
            //if (.Length > 0) attributes |= GXAttributes.GX_VA_NRM_MTX_ARRAY;
            //if (.Length > 0) attributes |= GXAttributes.GX_VA_TEX_MTX_ARRAY;
            //if (.Length > 0) attributes |= GXAttributes.GX_VA_LIGHT_ARRAY;
            if (pos.Length > 0) attributes |= GXAttributes.GX_VA_POS;
            /**/ if (bnm.Length > 0) attributes |= GXAttributes.GX_VA_NBT; // NBT if bnm or tan are non-zero length
            else if (nrm.Length > 0) attributes |= GXAttributes.GX_VA_NRM; // Otherwise, check if nrm is non-zero length
            if (clr0.Length > 0) attributes |= GXAttributes.GX_VA_CLR0;
            if (clr1.Length > 0) attributes |= GXAttributes.GX_VA_CLR1;
            if (tex0.Length > 0) attributes |= GXAttributes.GX_VA_TEX0;
            if (tex1.Length > 0) attributes |= GXAttributes.GX_VA_TEX1;
            if (tex2.Length > 0) attributes |= GXAttributes.GX_VA_TEX2;
            if (tex3.Length > 0) attributes |= GXAttributes.GX_VA_TEX3;
            if (tex4.Length > 0) attributes |= GXAttributes.GX_VA_TEX4;
            if (tex5.Length > 0) attributes |= GXAttributes.GX_VA_TEX5;
            if (tex6.Length > 0) attributes |= GXAttributes.GX_VA_TEX6;
            if (tex7.Length > 0) attributes |= GXAttributes.GX_VA_TEX7;

            return attributes;
        }
    }
}