using GameCube.GFZ.CourseCollision;
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

        public override void InitTrackSegment()
        {
            throw new System.NotImplementedException();
        }

        public override float GetSegmentLength()
        {
            throw new System.NotImplementedException();
        }

    }
}
