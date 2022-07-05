using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzCheckpoint : MonoBehaviour
    {
        [field: SerializeField] public Mesh Mesh { get; set; }
        [field: SerializeField] public float GizmosSize { get; set; } = 25f;

        [field: Header("Debug")]
        [field: SerializeField, ReadOnlyGUI] public float CurveTimeStart { get; set; }
        [field: SerializeField, ReadOnlyGUI] public float CurveTimeEnd { get; set; }
        [field: SerializeField, ReadOnlyGUI] public float StartDistance { get; set; }
        [field: SerializeField, ReadOnlyGUI] public float EndDistance { get; set; }
        [field: SerializeField, ReadOnlyGUI] public float TrackWidth { get; set; }
        [field: SerializeField, ReadOnlyGUI] public bool ConnectToTrackIn{ get; set; }
        [field: SerializeField, ReadOnlyGUI] public bool ConnectToTrackOut{ get; set; }
        [field: SerializeField, ReadOnlyGUI] public GameCube.GFZ.Stage.Plane PlaneStart{ get; set; }
        [field: SerializeField, ReadOnlyGUI] public GameCube.GFZ.Stage.Plane PlaneEnd { get; set; }


        public void Init(Checkpoint checkpoint)
        {
            transform.position = checkpoint.PlaneStart.origin;
            transform.localRotation = Quaternion.LookRotation(checkpoint.PlaneStart.normal);
            // 
            CurveTimeStart = checkpoint.CurveTimeStart;
            CurveTimeEnd = checkpoint.CurveTimeEnd;
            StartDistance = checkpoint.StartDistance;
            EndDistance = checkpoint.EndDistance;
            TrackWidth = checkpoint.TrackWidth;
            ConnectToTrackIn = checkpoint.ConnectToTrackIn;
            ConnectToTrackOut = checkpoint.ConnectToTrackOut;
            PlaneStart = checkpoint.PlaneStart;
            PlaneEnd = checkpoint.PlaneEnd;
        }

        const float size = 5f;
        private void OnDrawGizmosSelected()
        {
            var from = PlaneStart.origin;
            var to = PlaneEnd.origin;
            var halfWidthFrom = TrackWidth;
            var scale = new Vector3(halfWidthFrom, halfWidthFrom, size);
            var forward = Quaternion.LookRotation(PlaneStart.normal);
            var backward = Quaternion.LookRotation(PlaneEnd.normal);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, from + PlaneStart.normal * GizmosSize);
            Gizmos.DrawMesh(Mesh, 0, from + PlaneStart.normal, forward, scale);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(to, to + PlaneEnd.normal * GizmosSize);
            Gizmos.DrawMesh(Mesh, 0, to + PlaneEnd.normal, backward, scale);
        }
    }
}
