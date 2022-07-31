using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeOpenPipeCylinder : GfzSegmentShape
    {
        [Header("Half-Pipe / Half-Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;

        public PipeCylinderType Type
        {
            get => type;
            set => type = value;
        }

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            throw new System.NotImplementedException();
        }

        public override Gcmf CreateGcmf(out GcmfTemplate[] gcmfTemplates, ref Dictionary<string, ushort> textureHashesToIndex)
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
