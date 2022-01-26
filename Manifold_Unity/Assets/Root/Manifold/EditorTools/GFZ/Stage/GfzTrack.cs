using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTrack : MonoBehaviour
    {
        [SerializeField] private GfzTrackSegment startSegment;
        [SerializeField] private GfzTrackSegment[] segments;


        public GfzTrackSegment StartSegment
        {
            get => startSegment;
            set => startSegment = value;
        }

        public GfzTrackSegment[] Segments
        {
            get => segments;
            set => segments = value;
        }
    }
}
