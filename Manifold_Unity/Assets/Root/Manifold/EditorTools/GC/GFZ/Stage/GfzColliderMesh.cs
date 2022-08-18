using GameCube.GFZ.Stage;
using System;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzColliderMesh : MonoBehaviour,
        IGfzConvertable<ColliderMesh>
    {
        [SerializeField] private MeshFilter colliderMesh;
        [SerializeField] private Color32 gizmosColor = new Color32(255, 64, 64, 128);
        [Header("Other Data")]
        [SerializeField] private uint unk_0x00; // flags
        [SerializeField] private float unk_0x04;
        [SerializeField] private float unk_0x08;
        [SerializeField] private float unk_0x0C;
        [SerializeField] private float unk_0x10;

        public MeshFilter ColliderMesh { get => colliderMesh; set => colliderMesh = value; }


        public ColliderMesh ExportGfz()
        {
            // Create triangles
            var triangles = ColliderUtility.CreateColliderTriangles(ColliderMesh.sharedMesh);
            // for now, empty
            var quads = new ColliderQuad[0];

            var colliderMesh = new ColliderMesh
            {
                Unk_0x00 = unk_0x00,
                Unk_0x04 = unk_0x04,
                Unk_0x08 = unk_0x08,
                Unk_0x0C = unk_0x0C,
                Unk_0x10 = unk_0x10,
                Tris = triangles,
                Quads = quads,
            };

            return colliderMesh;
        }

        public void ImportGfz(ColliderMesh colliderMesh)
        {
            unk_0x00 = colliderMesh.Unk_0x00;
            unk_0x04 = colliderMesh.Unk_0x04;
            unk_0x08 = colliderMesh.Unk_0x08;
            unk_0x0C = colliderMesh.Unk_0x0C;
            unk_0x10 = colliderMesh.Unk_0x10;
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (ColliderMesh == null)
                ColliderMesh = GetComponent<MeshFilter>();
        }

        public void OnDrawGizmosSelected()
        {
            if (colliderMesh == null || colliderMesh.sharedMesh == null)
                return;

            var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            DrawMesh(matrix);
        }

        public void DrawMesh(Transform transform)
            => DrawMesh(Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale));

        public void DrawMesh(Matrix4x4 matrix)
        {
            if (colliderMesh == null || colliderMesh.sharedMesh == null)
                return;

            var position = matrix.Position();
            var rotation = matrix.rotation;
            var scale = matrix.lossyScale;

            Gizmos.color = gizmosColor;
            for (int i = 0; i < colliderMesh.sharedMesh.subMeshCount; i++)
                Gizmos.DrawMesh(colliderMesh.sharedMesh, i, position, rotation, scale);
        }

    }

}
