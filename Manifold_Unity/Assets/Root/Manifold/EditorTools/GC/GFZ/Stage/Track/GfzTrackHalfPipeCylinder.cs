using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzTrackHalfPipeCylinder : GfzTrackSegmentShapeNode
    {
        [Header("Half-Pipe / Half-Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;

        public PipeCylinderType Type
        {
            get => type;
            set => type = value;
        }

        public override TrackSegmentType TrackSegmentType => throw new System.NotImplementedException();

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            throw new System.NotImplementedException();
        }

        public override Gcmf CreateGcmf()
        {
            throw new System.NotImplementedException();
        }

        public override Mesh CreateMesh()
        {
            throw new System.NotImplementedException();
        }

        public override TrackSegment CreateTrackSegment()
        {
            throw new System.NotImplementedException();
        }

    }
}
