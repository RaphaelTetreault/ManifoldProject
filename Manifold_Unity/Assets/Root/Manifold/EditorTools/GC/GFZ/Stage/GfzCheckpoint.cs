using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzCheckpoint : MonoBehaviour
    {
        private Checkpoint Checkpoint { get; set; }
        [field: SerializeField] public Mesh Mesh { get; set; }
        [field: SerializeField] public float GizmosSize { get; set; } = 25f;

        [field: Header("Debug")]
        [field: SerializeField, ReadOnlyGUI] public float StartDistance { get; set; }
        [field: SerializeField, ReadOnlyGUI] public float EndDistance { get; set; }
        [field: SerializeField, ReadOnlyGUI] public bool ConnectToTrackIn{ get; set; }
        [field: SerializeField, ReadOnlyGUI] public bool ConnectToTrackOut{ get; set; }


        public void Init(Checkpoint checkpoint)
        {
            Checkpoint = checkpoint;
            transform.position = checkpoint.PlaneStart.origin;
            transform.localRotation = Quaternion.LookRotation(checkpoint.PlaneStart.normal);
            // debug
            StartDistance = checkpoint.StartDistance;
            EndDistance = checkpoint.EndDistance;
            ConnectToTrackIn = checkpoint.ConnectToTrackIn;
            ConnectToTrackOut = checkpoint.ConnectToTrackOut;
        }

        const float size = 5f;
        private void OnDrawGizmosSelected()
        {
            var from = Checkpoint.PlaneStart.origin;
            var to = Checkpoint.PlaneEnd.origin;
            var halfWidthFrom = Checkpoint.TrackWidth;
            var scale = new Vector3(halfWidthFrom, halfWidthFrom, size);
            var forward = Quaternion.LookRotation(Checkpoint.PlaneStart.normal);
            var backward = Quaternion.LookRotation(Checkpoint.PlaneEnd.normal);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, from + Checkpoint.PlaneStart.normal * GizmosSize);
            Gizmos.DrawMesh(Mesh, 0, from + Checkpoint.PlaneStart.normal, forward, scale);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(to, to + Checkpoint.PlaneEnd.normal * GizmosSize);
            Gizmos.DrawMesh(Mesh, 0, to + Checkpoint.PlaneEnd.normal, backward, scale);
        }
    }
}
