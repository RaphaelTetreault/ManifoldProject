using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzCheckpoint : MonoBehaviour
    {
        [field: Header("Gizmos Debug Settings")]
        [field: SerializeField] public Mesh Mesh { get; set; }
        [field: SerializeField] public float GizmosLineLength { get; set; } = 25f;
        [field: SerializeField] public float GizmosMeshScaleXY { get; set; } = 1f;
        [field: SerializeField] public float GizmosMeshScaleZ { get; set; } = 5f;

        [field: Header("Checkpoint Data - Debug")]
        [field: SerializeField, ReadOnlyGUI] public float CurveTimeStart { get; private set; }
        [field: SerializeField, ReadOnlyGUI] public float CurveTimeEnd { get; private set; }
        [field: SerializeField, ReadOnlyGUI] public float StartDistance { get; private set; }
        [field: SerializeField, ReadOnlyGUI] public float EndDistance { get; private set; }
        [field: SerializeField, ReadOnlyGUI] public float TrackWidth { get; private set; }
        [field: SerializeField, ReadOnlyGUI] public bool ConnectToTrackIn{ get; private set; }
        [field: SerializeField, ReadOnlyGUI] public bool ConnectToTrackOut{ get; private set; }
        [field: SerializeField, ReadOnlyGUI] public GameCube.GFZ.Stage.Plane PlaneStart{ get; private set; }
        [field: SerializeField, ReadOnlyGUI] public GameCube.GFZ.Stage.Plane PlaneEnd { get; private set; }


        public void SetCheckpointData(Checkpoint checkpoint)
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

        private void OnDrawGizmosSelected()
        {
            var from = PlaneStart.origin;
            var to = PlaneEnd.origin;
            var halfWidthFrom = TrackWidth * GizmosMeshScaleXY;
            var scale = new Vector3(halfWidthFrom, halfWidthFrom, GizmosMeshScaleZ);
            var forward = Quaternion.LookRotation(PlaneStart.normal);
            var backward = Quaternion.LookRotation(PlaneEnd.normal);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, from + PlaneStart.normal * GizmosLineLength);
            Gizmos.DrawMesh(Mesh, 0, from + PlaneStart.normal, forward, scale);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(to, to + PlaneEnd.normal * GizmosLineLength);
            Gizmos.DrawMesh(Mesh, 0, to + PlaneEnd.normal, backward, scale);
        }
    }
}
