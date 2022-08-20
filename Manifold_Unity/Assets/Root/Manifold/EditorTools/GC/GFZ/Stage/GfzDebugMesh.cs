using GameCube.GFZ.GMA;
using Manifold.EditorTools.GC.GFZ.TPL;
using Manifold.EditorTools.GC.GFZ.Stage.Track;
using GameCube.GFZ.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzDebugMesh : MonoBehaviour
    {
        [field: SerializeField] public MeshFilter MeshFilter { get; protected set; }
        [field: SerializeField] public Color32 Color { get; protected set; } = new Color32(255, 255, 255, 255);


        public Gcmf CreateGcmf(TplTextureContainer tpl)
        {
            var gcmfTemplate = GetGcmfTemplates();
            var tristrips = GetMesh();
            var gcmf = GcmfTemplate.CreateGcmf(gcmfTemplate, tristrips, tpl);
            return gcmf;
        }
        private Tristrip[][] GetMesh()
        {
            var tristrips = new List<Tristrip>();

            var mesh = MeshFilter.sharedMesh;
            var matrix = GetGfzMatrix();
            var triangles = new ColliderTriangle[mesh.triangles.Length / 3];
            for (int i = 0; i < triangles.Length; i++)
            {
                int i0 = i * 3;
                int i1 = i0 + 1;
                int i2 = i0 + 2;
                int v0 = mesh.triangles[i0];
                int v1 = mesh.triangles[i1];
                int v2 = mesh.triangles[i2];

                var tristrip = new Tristrip();
                tristrip.positions = new Vector3[]
                {
                    matrix.MultiplyPoint(mesh.vertices[v0]),
                    matrix.MultiplyPoint(mesh.vertices[v1]),
                    matrix.MultiplyPoint(mesh.vertices[v2]),
                };
                tristrip.normals = new Vector3[]
                {
                    matrix.rotation * TransformConverter.MirrorNormal(mesh.normals[v0]),
                    matrix.rotation * TransformConverter.MirrorNormal(mesh.normals[v1]),
                    matrix.rotation * TransformConverter.MirrorNormal(mesh.normals[v2]),
                };
                tristrip.color0 = new Color32[]
                {
                    Color,
                    Color,
                    Color,
                };
                tristrips.Add(tristrip);
            }
            var t = new Tristrip[1][];
            t[0] = tristrips.ToArray();
            return t;
        }
        private GcmfTemplate[] GetGcmfTemplates()
        {
            return new GcmfTemplate[] {
            GcmfTemplates.Debug.CreateLitVertexColored(),
            };
        }

        public Matrix4x4 GetGfzMatrix()
        {
            // Create matrix
            var position = TransformConverter.MirrorPosition(transform.position);
            var rotation = TransformConverter.MirrorRotation(transform.rotation);
            var scale = transform.lossyScale;
            var matrix = Matrix4x4.TRS(position, rotation, scale);
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
