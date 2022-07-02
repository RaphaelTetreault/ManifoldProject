using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzCheckpointGenerator : MonoBehaviour
    {
        [field: SerializeField] public GfzTrackSegment TrackSegment { get; private set; }
        [field: SerializeField] public Mesh GizmosMesh { get; private set; }

        private void OnValidate()
        {
            if (TrackSegment is null)
            {
                TrackSegment = GetComponent<GfzTrackSegment>();
            }
        }
    }
}
