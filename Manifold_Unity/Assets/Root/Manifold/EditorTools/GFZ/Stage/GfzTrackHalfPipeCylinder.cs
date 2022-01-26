using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrackHalfPipeCylinder : GfzTrackSegment
    {
        [Header("Half-Pipe / Half-Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;

        public PipeCylinderType Type
        {
            get => type;
            set => type = value;
        }

        public override TrackSegment ExportGfz()
        {
            throw new System.NotImplementedException();
        }

        public override void ImportGfz(TrackSegment value)
        {
            throw new System.NotImplementedException();
        }

    }
}
