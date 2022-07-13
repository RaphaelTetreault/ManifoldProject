using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzCheckpointGenerator : MonoBehaviour
    {
        [field: SerializeField] public GfzTrackSegmentNode TrackSegment { get; private set; }
        [field: SerializeField] public Mesh GizmosMesh { get; private set; }
        [field: SerializeField] public bool GenGfz { get; private set; }

        private void OnValidate()
        {
            if (TrackSegment is null)
            {
                TrackSegment = GetComponent<GfzTrackSegmentNode>();
            }
        }
    }
}
