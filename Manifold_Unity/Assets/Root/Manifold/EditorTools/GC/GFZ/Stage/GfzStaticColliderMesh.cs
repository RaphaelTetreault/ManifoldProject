using GameCube.GFZ.Stage;
using System.Collections.Generic;
using Unity.Mathematics;
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

        public void CreateCollider(out ColliderTriangle[] triangles, out ColliderQuad[] quads)
        {
            triangles = CreateColliderTriangles();
            quads = new ColliderQuad[0];
        }

        public void CreateColliderOptimized(out ColliderTriangle[] triangles, out ColliderQuad[] quads)
        {
            var mesh = MeshFilter.sharedMesh;
            var trisList = new List<ColliderTriangle>();
            var quadsList = new List<ColliderQuad>();
            var matrix = GetGfzMatrix();
            int indexes = mesh.triangles.Length / 3;
            for (int i = 0; i < indexes; i++)
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
                trisList.Add(triangle);
            }

            // Reverse order, collapse triangles into quad when necessary
            for (int i = indexes-1; i > 0; i--)
            {
                var triangle0 = trisList[i-0];
                var triangle1 = trisList[i-1];
                float sameness = math.dot(triangle0.Normal, triangle1.Normal);
                // TODO: make sure triangles share 2/3 vertices
                if (sameness > 0.995f)
                {
                    var quad = new ColliderQuad();
                    // TODO: select correct vertices to collapse, ensure 4 verts make circle
                    quad.Vertex0 = triangle0.Vertex0;
                    quad.Vertex1 = triangle0.Vertex2;
                    quad.Vertex2 = triangle1.Vertex0;
                    quad.Vertex3 = triangle1.Vertex2;
                    quad.Update();
                    quadsList.Add(quad);

                    if (float.IsNaN(quad.EdgeNormal0.x))
                        throw new System.Exception("Bad assumption on quad collapse.");

                    trisList.Remove(triangle0);
                    trisList.Remove(triangle1);
                    i--;
                }
            }

            triangles = trisList.ToArray();
            quads = quadsList.ToArray();
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
