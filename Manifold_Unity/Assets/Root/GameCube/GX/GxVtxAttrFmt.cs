using Manifold.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using GameCube.GFZ.GMA;

namespace GameCube.GX
{

    /// <summary>
    /// What would compromise a column in GX VAT - Vertex Attribute Table:::
    /// 
    /// GX Vertex Attribute Format
    /// </summary>
    [Serializable]
    public class GxVtxAttrFmt
    {
        // 2020-05-04 Raph: I worry this can have errors when if someone
        // accidentally modifies a value
        [SerializeField] public GxVtxAttr pos;
        [SerializeField] public GxVtxAttr nrm;
        [SerializeField] public GxVtxAttr nbt;
        [SerializeField] public GxVtxAttr clr0;
        [SerializeField] public GxVtxAttr clr1;
        [SerializeField] public GxVtxAttr tex0;
        [SerializeField] public GxVtxAttr tex1;
        [SerializeField] public GxVtxAttr tex2;
        [SerializeField] public GxVtxAttr tex3;
        [SerializeField] public GxVtxAttr tex4;
        [SerializeField] public GxVtxAttr tex5;
        [SerializeField] public GxVtxAttr tex6;
        [SerializeField] public GxVtxAttr tex7;

        public GxVtxAttr GetAttr(GXAttr attribute)
        {
            switch (attribute)
            {
                case GXAttr.GX_VA_POS: return pos;
                case GXAttr.GX_VA_NRM: return nrm;
                case GXAttr.GX_VA_NBT: return nbt;
                case GXAttr.GX_VA_CLR0: return clr0;
                case GXAttr.GX_VA_CLR1: return clr1;
                case GXAttr.GX_VA_TEX0: return tex0;
                case GXAttr.GX_VA_TEX1: return tex1;
                case GXAttr.GX_VA_TEX2: return tex2;
                case GXAttr.GX_VA_TEX3: return tex3;
                case GXAttr.GX_VA_TEX4: return tex4;
                case GXAttr.GX_VA_TEX5: return tex5;
                case GXAttr.GX_VA_TEX6: return tex6;
                case GXAttr.GX_VA_TEX7: return tex7;

                default:
                    throw new NotImplementedException();
            }
        }

        public GxVtxAttr GetAttr(GXAttributes attribute)
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
                    throw new NotImplementedException();
            }
        }

        public void SetAttr(GXAttr attribute, GxVtxAttr vertexAttribute)
        {
            switch (attribute)
            {
                case GXAttr.GX_VA_POS: pos = vertexAttribute; break;
                case GXAttr.GX_VA_NRM: nrm = vertexAttribute; break;
                case GXAttr.GX_VA_NBT: nbt = vertexAttribute; break;
                case GXAttr.GX_VA_CLR0: clr0 = vertexAttribute; break;
                case GXAttr.GX_VA_CLR1: clr1 = vertexAttribute; break;
                case GXAttr.GX_VA_TEX0: tex0 = vertexAttribute; break;
                case GXAttr.GX_VA_TEX1: tex1 = vertexAttribute; break;
                case GXAttr.GX_VA_TEX2: tex2 = vertexAttribute; break;
                case GXAttr.GX_VA_TEX3: tex3 = vertexAttribute; break;
                case GXAttr.GX_VA_TEX4: tex4 = vertexAttribute; break;
                case GXAttr.GX_VA_TEX5: tex5 = vertexAttribute; break;
                case GXAttr.GX_VA_TEX6: tex6 = vertexAttribute; break;
                case GXAttr.GX_VA_TEX7: tex7 = vertexAttribute; break;

                default:
                    throw new NotImplementedException();
            }
        }
    }

}