using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrackPipeCylinder : GfzTrackSegment
    {
        [Header("Pipe/Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;


        public PipeCylinderType Type
        {
            get => type;
            set => type = value;
        }

    }
}
