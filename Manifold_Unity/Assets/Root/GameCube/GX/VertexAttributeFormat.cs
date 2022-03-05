using System;

namespace GameCube.GX
{

    /// <summary>
    /// What would comprise a column in GX VAT - Vertex Attribute Table
    /// 
    /// GX Vertex Attribute Format
    /// </summary>
    [Serializable]
    public class VertexAttributeFormat
    {
        // 2020-05-04 Raph: I worry this can have errors when/if someone
        // accidentally modifies a value
        public VertexAttribute pos;
        public VertexAttribute nrm;
        public VertexAttribute nbt;
        public VertexAttribute clr0;
        public VertexAttribute clr1;
        public VertexAttribute tex0;
        public VertexAttribute tex1;
        public VertexAttribute tex2;
        public VertexAttribute tex3;
        public VertexAttribute tex4;
        public VertexAttribute tex5;
        public VertexAttribute tex6;
        public VertexAttribute tex7;

        public VertexAttribute GetAttr(Attribute attribute)
        {
            switch (attribute)
            {
                case Attribute.GX_VA_POS: return pos;
                case Attribute.GX_VA_NRM: return nrm;
                case Attribute.GX_VA_NBT: return nbt;
                case Attribute.GX_VA_CLR0: return clr0;
                case Attribute.GX_VA_CLR1: return clr1;
                case Attribute.GX_VA_TEX0: return tex0;
                case Attribute.GX_VA_TEX1: return tex1;
                case Attribute.GX_VA_TEX2: return tex2;
                case Attribute.GX_VA_TEX3: return tex3;
                case Attribute.GX_VA_TEX4: return tex4;
                case Attribute.GX_VA_TEX5: return tex5;
                case Attribute.GX_VA_TEX6: return tex6;
                case Attribute.GX_VA_TEX7: return tex7;

                default:
                    throw new ArgumentException();
            }
        }

        public VertexAttribute GetAttr(GXAttributes attribute)
        {
            switch (attribute)
            {
                case GXAttributes.GX_VA_POS: return pos;
                case GXAttributes.GX_VA_NRM: return nrm;
                case GXAttributes.GX_VA_NBT: return nbt;
                case GXAttributes.GX_VA_CLR0: return clr0;
                case GXAttributes.GX_VA_CLR1: return clr1;
                case GXAttributes.GX_VA_TEX0: return tex0;
                case GXAttributes.GX_VA_TEX1: return tex1;
                case GXAttributes.GX_VA_TEX2: return tex2;
                case GXAttributes.GX_VA_TEX3: return tex3;
                case GXAttributes.GX_VA_TEX4: return tex4;
                case GXAttributes.GX_VA_TEX5: return tex5;
                case GXAttributes.GX_VA_TEX6: return tex6;
                case GXAttributes.GX_VA_TEX7: return tex7;

                default:
                    throw new ArgumentException();
            }
        }

        public void SetAttr(Attribute attribute, VertexAttribute vertexAttribute)
        {
            switch (attribute)
            {
                case Attribute.GX_VA_POS: pos = vertexAttribute; break;
                case Attribute.GX_VA_NRM: nrm = vertexAttribute; break;
                case Attribute.GX_VA_NBT: nbt = vertexAttribute; break;
                case Attribute.GX_VA_CLR0: clr0 = vertexAttribute; break;
                case Attribute.GX_VA_CLR1: clr1 = vertexAttribute; break;
                case Attribute.GX_VA_TEX0: tex0 = vertexAttribute; break;
                case Attribute.GX_VA_TEX1: tex1 = vertexAttribute; break;
                case Attribute.GX_VA_TEX2: tex2 = vertexAttribute; break;
                case Attribute.GX_VA_TEX3: tex3 = vertexAttribute; break;
                case Attribute.GX_VA_TEX4: tex4 = vertexAttribute; break;
                case Attribute.GX_VA_TEX5: tex5 = vertexAttribute; break;
                case Attribute.GX_VA_TEX6: tex6 = vertexAttribute; break;
                case Attribute.GX_VA_TEX7: tex7 = vertexAttribute; break;

                default:
                    throw new ArgumentException();
            }
        }
    }

}