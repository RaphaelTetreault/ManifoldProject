using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrackPipeCylinder : GfzSegmentShape
    {
        [Header("Pipe/Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;


        public PipeCylinderType Type
        {
            get => type;
            set => type = value;
        }

        public override Mesh[] GenerateMeshes()
        {
            throw new System.NotImplementedException();
        }

        public override TrackSegment GenerateTrackSegment()
        {
            throw new System.NotImplementedException();
        }

    }
}
