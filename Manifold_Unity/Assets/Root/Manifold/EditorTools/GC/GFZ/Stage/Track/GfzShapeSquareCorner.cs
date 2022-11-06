using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeSquareCorner : GfzShape
    {
        public enum CornerTurnDirection
        {
            left = 1,
            right = 2,
        }

        [Header("Corner Properties")]
        [SerializeField]
        private CornerTurnDirection cornerDirection = CornerTurnDirection.right;

        [Min(0f)]
        [SerializeField]
        private float railHeight = 5f;


        public override ShapeID ShapeIdentifier => ShapeID.road;
        public override EndcapMode EndcapModeIn => throw new System.NotImplementedException();
        public override EndcapMode EndcapModeOut => throw new System.NotImplementedException();

        public CornerTurnDirection TurnDirection
        {
            get => cornerDirection;
            set => cornerDirection = value;
        }
        public float RailHeight { get => railHeight; set => railHeight = value; }


        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            throw new System.NotImplementedException();
        }

        public override float GetMaxTime()
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

        public override void UpdateTRS()
        {
            throw new System.NotImplementedException();
        }

    }
}