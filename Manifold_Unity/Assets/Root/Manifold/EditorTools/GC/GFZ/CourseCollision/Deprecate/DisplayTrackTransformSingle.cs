using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCube.GFZ.Stage;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class DisplayTrackTransformSingle : MonoBehaviour
    {
        [SerializeField]
        public float size = 1f;
        [SerializeField]
        public int depth;

        public bool useFizedSize = true;

        public static Color[] gizmosColor = new Color[]
        {
            new Color32 (255,   0,   0, 128),
            new Color32 (255, 255,   0, 128),
            new Color32 (  0, 255,   0, 128),
            new Color32 (  0, 255, 255, 128),
            new Color32 (  0,   0, 255, 128),
            new Color32 (255,   0, 255, 128),
            new Color32 (  0,   0,   0, 128),
        };


        private void OnDrawGizmos()
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            //

            var size = this.size + depth;

            //
            var position = transform.position;
            var rotation = transform.rotation;
            var scale = useFizedSize ? Vector3.one : transform.lossyScale;
            //
            Gizmos.color = gizmosColor[depth];
            Gizmos.DrawMesh(mesh, 0, position, rotation, scale * size);
            Gizmos.DrawWireMesh(mesh, 0, position, rotation, scale * size);

            foreach (var child in transform.GetChildren())
            {
                Gizmos.DrawLine(transform.position, child.position);
            }
        }

    }
}
