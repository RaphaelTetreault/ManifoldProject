using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzStaticColliderMesh : MonoBehaviour
    {
        [field: SerializeField] public StaticColliderMeshPropertyFlags ColliderType { get; set; } = StaticColliderMeshPropertyFlags.dash;
        [field: SerializeField] public MeshFilter MeshFilter { get; protected set; }


        public ColliderTriangle[] CreateColliderTriangles()
        {
            var mesh = MeshFilter.sharedMesh;
            var triangles = new ColliderTriangle[mesh.triangles.Length / 3];
            var matrix = GetGfzMatrix();
            for (int i = 0; i < triangles.Length; i++)
            {
                int i0 = i * 3;
                int i1 = i0 + 1;
                int i2 = i0 + 2;
                int v0 = mesh.triangles[i0];
                int v1 = mesh.triangles[i1];
                int v2 = mesh.triangles[i2];

                var triangle = new ColliderTriangle();
                triangle.Vertex0 = matrix.MultiplyPoint(mesh.vertices[v0]);
                triangle.Vertex1 = matrix.MultiplyPoint(mesh.vertices[v1]);
                triangle.Vertex2 = matrix.MultiplyPoint(mesh.vertices[v2]);
                triangle.Update();

                triangles[i] = triangle;
            }

            return triangles;
        }

        public Matrix4x4 GetGfzMatrix()
        {
            // Create matrix
            var position = transform.position;
            var rotation = transform.rotation.eulerAngles;
            var scale = transform.lossyScale;
            ////
            position.z = -position.z;
            rotation.x = -rotation.x;
            rotation.y = -rotation.y;
            ////
            var matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
            return matrix;
        }


        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (MeshFilter == null)
                MeshFilter = GetComponent<MeshFilter>();
        }

    }
}
