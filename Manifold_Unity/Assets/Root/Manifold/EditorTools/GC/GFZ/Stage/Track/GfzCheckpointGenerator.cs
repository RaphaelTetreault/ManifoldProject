using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzCheckpointGenerator : MonoBehaviour
    {
        private const string assetPath = "Assets/Root/Resources/normal-cylinder-16-hollowed.fbx";


        [field: Header("Checkpoint Config")]
        [field: SerializeField] public GfzSegmentNode TrackSegmentNode { get; private set; }
        [field: SerializeField, Min(5f)] public float MetersPerCheckpoint { get; private set; } = 100f;
        
        [field: Header("Debug")]
        [field: SerializeField] public Mesh GizmosMesh { get; private set; }
        [field: SerializeField] public bool IsGfzCoordinateSpace { get; private set; }

        private void Reset()
        {
            GizmosMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            OnValidate();
        }

        private void OnValidate()
        {
            if (TrackSegmentNode is null)
            {
                TrackSegmentNode = GetComponent<GfzSegmentNode>();
            }
        }
    }
}
