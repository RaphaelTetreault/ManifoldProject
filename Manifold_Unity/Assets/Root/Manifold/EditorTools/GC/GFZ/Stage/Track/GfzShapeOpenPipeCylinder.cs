using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeOpenPipeCylinder : GfzShape
    {
        [Header("Half-Pipe / Half-Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;

        public PipeCylinderType Type
        {
            get => type;
            set => type = value;
        }

        public override ShapeID ShapeIdentifier
        {
            get
            {
                bool isPipe = type == PipeCylinderType.Pipe;

                if (isPipe)
                    return ShapeID.pipe;
                else
                    return ShapeID.cylinder;
            }
        }

        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            throw new System.NotImplementedException();
        }

        public override Tristrip[][] GetTristrips(bool isGfzCoordinateSpace)
        {
            throw new System.NotImplementedException();
        }

        public override TrackSegment CreateTrackSegment()
        {
            throw new System.NotImplementedException();
        }

        public override GcmfTemplate[] GetGcmfTemplates()
        {
            throw new System.NotImplementedException();
        }

        public override float GetMaxTime()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateTRS()
        {
            throw new System.NotImplementedException();
        }

    }
}
