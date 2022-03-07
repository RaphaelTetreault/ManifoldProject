using GameCube.GFZ.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzCheckpoint : MonoBehaviour
    {
        [SerializeField] private float gizmosSize = 25f;
        [SerializeField] private Checkpoint checkpoint;

        public void Init(Checkpoint checkpoint)
        {
            this.checkpoint = checkpoint;
            this.transform.position = checkpoint.PlaneStart.origin;
            this.transform.localRotation = Quaternion.LookRotation(checkpoint.PlaneStart.normal);
        }


        private void OnDrawGizmosSelected()
        {
            var mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Root/Resources/normal-cylinder-16-hollowed.fbx");

            var from = checkpoint.PlaneStart.origin;
            var to = checkpoint.PlaneEnd.origin;
            var halfWidth = checkpoint.TrackWidth / 2f;
            var scaleFrom = new Vector3(halfWidth, halfWidth, 1f);
            var scaleTo = scaleFrom * .9f;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, from + checkpoint.PlaneStart.normal * gizmosSize);
            Gizmos.DrawMesh(mesh, 0, from, Quaternion.LookRotation(checkpoint.PlaneStart.normal), scaleFrom);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(to, to + checkpoint.PlaneEnd.normal * gizmosSize);
            Gizmos.DrawWireMesh(mesh, 0, to, Quaternion.LookRotation(checkpoint.PlaneEnd.normal), scaleTo);
        }
    }
}
