using GameCube.GFZ.Stage;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzColliderMesh : MonoBehaviour,
        IGfzConvertable<ColliderMesh>
    {
        [SerializeField] private Mesh colliderMesh;
        [SerializeField] private Color32 gizmosColor = new Color32(255, 64, 64, 128);
        [Header("Other Data")]
        [UnityEngine.Serialization.FormerlySerializedAs("unk_0x00")]
        [SerializeField] private ColliderMeshType collidertype; // flags: 13=wall, 15=mine
        [SerializeField] private GameCube.GFZ.BoundingSphere boundingSphere;

        public Mesh ColliderMesh { get => colliderMesh; set => colliderMesh = value; }



        public bool IsReferenceEquivilent(GfzColliderMesh other)
        {
            bool isNull = other == null;
            if (isNull)
                return false;

            bool sameMesh = this.ColliderMesh == other.ColliderMesh;
            bool sameFlags = this.collidertype == other.collidertype;
            bool isReferenceEquivilent = sameMesh & sameFlags;
            return isReferenceEquivilent;
        }

        public ColliderMesh ExportGfz()
        {
            if (ColliderMesh == null)
                return null;

            // Create triangles, quads (quads currently just empty)
            var triangles = ColliderUtility.CreateColliderTriangles(ColliderMesh);
            var quads = new ColliderQuad[0];

            // Compute bounding sphere
            var vertices = new List<float3>();
            foreach (var triangle in triangles)
                vertices.AddRange(triangle.GetVertices());
            foreach (var quad in quads)
                vertices.AddRange(quad.GetVertices());
            boundingSphere = GameCube.GFZ.BoundingSphere.CreateBoundingSphereFromPoints(vertices, vertices.Count);
            boundingSphere.radius += 1f; // seems to be slightly padded

            var colliderMesh = new ColliderMesh
            {
                ColliderType = (uint)collidertype,
                BoundingSphere = boundingSphere,
                Tris = triangles,
                Quads = quads,
            };

            return colliderMesh;
        }

        public void ImportGfz(ColliderMesh colliderMesh)
        {
            collidertype = (EnumFlags32)colliderMesh.ColliderType;
            boundingSphere = colliderMesh.BoundingSphere;
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (ColliderMesh == null)
            {
                var meshFilter = GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    ColliderMesh = meshFilter.sharedMesh;
                }
            }
        }

        public void OnDrawGizmosSelected()
        {
            var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            DrawMesh(matrix);
        }

        public void DrawMesh(Transform transform)
            => DrawMesh(Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale));

        public void DrawMesh(Matrix4x4 matrix)
        {
            if (colliderMesh == null)
                return;

            var position = matrix.Position();
            var rotation = matrix.rotation;
            var scale = matrix.lossyScale;

            Gizmos.color = gizmosColor;
            for (int i = 0; i < colliderMesh.subMeshCount; i++)
                Gizmos.DrawMesh(colliderMesh, i, position, rotation, scale);
        }

    }

}
