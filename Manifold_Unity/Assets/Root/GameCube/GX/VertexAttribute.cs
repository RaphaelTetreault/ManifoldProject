using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GX
{
    [Serializable]
    public class VertexAttribute
    {
        [SerializeField] public bool enabled;
        [SerializeField] public ComponentCount nElements;
        [SerializeField] public ComponentType componentFormat;
        [SerializeField] public int nFracBits;

        public VertexAttribute(ComponentCount nElements, ComponentType format, int nFracBits = 0)
        {
            // Assert that we aren't shifting more bits than we have
            if (format == ComponentType.GX_S8 | format == ComponentType.GX_U8)
                Assert.IsTrue(nFracBits < 8);
            if (format == ComponentType.GX_S16 | format == ComponentType.GX_U16)
                Assert.IsTrue(nFracBits < 16);

            this.enabled = true;
            this.nElements = nElements;
            this.componentFormat = format;
            this.nFracBits = nFracBits;
        }
    }
}