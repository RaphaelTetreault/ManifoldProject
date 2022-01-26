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
    }
}
