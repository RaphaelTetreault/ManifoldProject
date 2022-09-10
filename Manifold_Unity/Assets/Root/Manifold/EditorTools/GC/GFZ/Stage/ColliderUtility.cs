using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// Utility class for handling GFZ collision data.
    /// </summary>
    public static  class ColliderUtility
    {
        // TODO: use mesh's submesh layers? Identify triangles and quads

        public static ColliderTriangle[] CreateColliderTriangles(Mesh mesh)
            => CreateColliderTriangles(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one), mesh);

        public static ColliderTriangle[] CreateColliderTriangles(Transform transform, Mesh mesh)
            => CreateColliderTriangles(GetGfzMatrix(transform), mesh);

        public static ColliderTriangle[] CreateColliderTriangles(Matrix4x4 matrix, Mesh mesh)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                var submesh = mesh.GetSubMesh(i);
                //Debug.Log(submesh.topology);
            }

            var triangles = new ColliderTriangle[mesh.triangles.Length / 3];
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

        public static Matrix4x4 GetGfzMatrix(Transform transform)
        {
            var position = TransformConverter.MirrorPosition(transform.position);
            var rotation = TransformConverter.MirrorRotation(transform.rotation);
            var scale = transform.lossyScale;
            var matrix = Matrix4x4.TRS(position, rotation, scale);
            return matrix;
        }
    }
}
