using Manifold;
using System;

namespace GameCube.GX
{
    // GameCube VAT
    [Serializable]
    public class VertexAttributeTable
    {
        // FIELDS
        private VertexAttributeFormat[] gxVertexAttributeFormats = new VertexAttributeFormat[8];

        // INDEXERS
        public VertexAttributeFormat this[int i]
        {
            get => gxVertexAttributeFormats[i];
        }
        public VertexAttributeFormat this[VertexFormat vertexFormat]
        {
            get => gxVertexAttributeFormats[(byte)vertexFormat];
        }
        public VertexAttributeFormat this[DisplayCommand displayCommand]
        {
            get => gxVertexAttributeFormats[displayCommand.VertexFormatIndex];
        }


        public VertexAttributeTable(params VertexAttributeFormat[] formats)
        {
            if (formats.Length > 8)
                throw new ArgumentOutOfRangeException();

            // Update formats
            for (int i = 0; i < formats.Length; i++)
                gxVertexAttributeFormats[i] = formats[i];

            // Clear old refs
            for (int i = formats.Length; i < gxVertexAttributeFormats.Length; i++)
                gxVertexAttributeFormats[i] = null;
        }

        public bool VatHasAttr(DisplayCommand gxCmd, Attribute attribute)
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