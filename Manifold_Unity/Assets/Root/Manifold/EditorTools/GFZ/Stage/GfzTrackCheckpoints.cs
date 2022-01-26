using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrackCheckpoints : MonoBehaviour
    {
        [SerializeField] private GfzTrackSegment segment;

        public GfzTrackSegment Segment
        {
            get => segment;
            set => segment = value;
        }

        public TrackCheckpoint[] GetCheckpoints()
        {
            // TODO: make 100% sure you feed checkpoints in order!
            throw new System.NotImplementedException();
        }
    }
}
