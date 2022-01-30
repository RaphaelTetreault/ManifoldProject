using GameCube.GFZ.CourseCollision;
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
        [SerializeField] private Checkpoint checkpoint;

        public void Init(Checkpoint checkpoint)
        {
            this.checkpoint = checkpoint;
            this.transform.position = checkpoint.planeStart.origin;
            this.transform.localRotation = Quaternion.LookRotation(checkpoint.planeStart.normal);
        }


        private void OnDrawGizmosSelected()
        {
            var mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Root/Resources/normal-cylinder-16-hollowed.fbx");

            var from = checkpoint.planeStart.origin;
            var to = checkpoint.planeEnd.origin;
            var halfWidth = checkpoint.trackWidth / 2f;
            var scaleFrom = new Vector3(halfWidth, halfWidth, 1f);
            var scaleTo = scaleFrom * .9f;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, from + checkpoint.planeStart.normal * 15f);
            Gizmos.DrawMesh(mesh, 0, from, Quaternion.LookRotation(checkpoint.planeStart.normal), scaleFrom);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(to, to + checkpoint.planeEnd.normal * 15f);
            Gizmos.DrawWireMesh(mesh, 0, to, Quaternion.LookRotation(checkpoint.planeEnd.normal), scaleTo);
        }
    }
}
