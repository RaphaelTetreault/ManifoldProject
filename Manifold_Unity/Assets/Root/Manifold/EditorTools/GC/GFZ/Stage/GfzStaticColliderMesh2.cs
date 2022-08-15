using GameCube.GFZ.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzStaticColliderMesh2 : MonoBehaviour
    {
        [field: SerializeField] public StaticColliderMeshProperty ColliderType { get; set; } = StaticColliderMeshProperty.dash;
        [field: SerializeField] public MeshFilter MeshFilter { get; protected set; }


        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (MeshFilter == null)
                MeshFilter = GetComponent<MeshFilter>();
        }

        public ColliderTriangle[] CreateColliderTriangles()
        {
            var mesh = MeshFilter.sharedMesh;
            var triangles = new ColliderTriangle[mesh.triangles.Length / 3];

            var position = transform.position;
            var rotation = transform.rotation.eulerAngles;
            var scale = transform.lossyScale;
            ////
            position.z = -position.z;
            rotation.x = -rotation.x;
            rotation.y = -rotation.y;
            ////
            var matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);

            for (int i = 0; i < triangles.Length; i++)
            {
                int i0 = i * 3;
                int i1 = i0 + 1;
                int i2 = i0 + 2;
                int v0 = mesh.triangles[i0];
                int v1 = mesh.triangles[i1];
                int v2 = mesh.triangles[i2];

                var triangle = new ColliderTriangle();
                var normal = mesh.normals[i];
                normal.z = -normal.z;
                triangle.Normal = normal;
                triangle.Vertex0 = matrix.MultiplyPoint(mesh.vertices[v0]);
                triangle.Vertex1 = matrix.MultiplyPoint(mesh.vertices[v1]);
                triangle.Vertex2 = matrix.MultiplyPoint(mesh.vertices[v2]);
                triangle.UpdatePlaneDistance();
                triangles[i] = triangle;
            }

            return triangles;
        }
    }
}
