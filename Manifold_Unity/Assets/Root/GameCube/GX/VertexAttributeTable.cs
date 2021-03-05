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
    // GameCube VAT
    [Serializable]
    public class VertexAttributeTable
    {
        [SerializeField] VertexAttributeFormat[] gxVertexAttributeFormats = new VertexAttributeFormat[8];

        public VertexAttributeFormat[] GxVtxAttrFmts
            => gxVertexAttributeFormats;

        public VertexAttributeTable(params VertexAttributeFormat[] formats)
        {
            if (formats.Length > 8)
                throw new ArgumentOutOfRangeException();

            // Update formats
            for (int i = 0; i < formats.Length; i++)
                GxVtxAttrFmts[i] = formats[i];

            // Clear old refs
            for (int i = formats.Length; i < GxVtxAttrFmts.Length; i++)
                GxVtxAttrFmts[i] = null;
        }

        public bool VatHasAttr(DisplayCommand gxCmd, GXAttr attribute)
        {
            Assert.IsTrue((byte)gxCmd.VertexFormat < 8);

            if (attribute == 0)
            {
                return false;
            }
            else
            {
                var vatIndex = (int)gxCmd.VertexFormat;
                var attr = gxVertexAttributeFormats[vatIndex].GetAttr(attribute);
                return attr != null;
            }
        }

        public bool HasAttr(DisplayCommand gxCmd, GXAttributes attribute)
        {
            Assert.IsTrue((byte)gxCmd.VertexFormat < 8);

            if (attribute == 0)
            {
                return false;
            }
            else
            {
                var vatIndex = (int)gxCmd.VertexFormat;
                var attr = gxVertexAttributeFormats[vatIndex].GetAttr(attribute);
                return attr != null;
            }
        }

    }

}