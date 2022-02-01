using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrackCorner : GfzSegmentShape
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


        public CornerTurnDirection TurnDirection
        {
            get => cornerDirection;
            set => cornerDirection = value;
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
