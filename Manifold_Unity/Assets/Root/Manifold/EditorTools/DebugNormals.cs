using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools
{
    public class DebugNormals : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public Color32 color = Color.blue;
        public float length = 5f;

        private void OnDrawGizmosSelected()
        {
            if (meshFilter == null || meshFilter.sharedMesh == null)
                return;

            var matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.color = color;
            var mesh = meshFilter.sharedMesh;
            var normals = mesh.normals;
            var vertices = mesh.vertices;
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                //var submesh = mesh.GetSubMesh(i);
                int[] indexes = mesh.GetIndices(i);
                for (int j = 0; j < indexes.Length; j++)
                {
                    int index = indexes[j];
                    var vertex = matrix.MultiplyPoint(vertices[index]);
                    var normal = matrix.rotation * normals[index] * length;
                    Gizmos.DrawLine(vertex, vertex + normal);
                }
            }
        }

        private void OnValidate()
        {
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
        }
    }
}
