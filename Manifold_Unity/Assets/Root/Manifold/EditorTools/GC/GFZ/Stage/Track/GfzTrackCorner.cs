using GameCube.GFZ.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzTrackCorner : GfzTrackShape
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
        public float RailHeight { get => railHeight; set => railHeight = value; }


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
