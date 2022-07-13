using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public abstract class GfzTrackSegmentShapeNode : GfzTrackSegmentNode
    {
        [field: Header("Mesh Properties")]
        [field: SerializeField] public MeshFilter MeshFilter { get; protected set; }
        [field: SerializeField] public MeshRenderer MeshRenderer { get; protected set; }
        [field: SerializeField] public Mesh Mesh { get; protected set; }
        [field: SerializeField] public UnityEngine.Material DefaultMaterial { get; protected set; }


        public abstract Mesh CreateMesh();
        public abstract Gcmf CreateGcmf();

        protected virtual void OnValidate()
        {
            if (MeshRenderer == null)
                MeshRenderer = GetComponent<MeshRenderer>();

            if (MeshFilter == null)
                MeshFilter = GetComponent<MeshFilter>();
        }

        public Mesh TristripsToMesh(Tristrip[] tristrips)
        {
            var mesh = new Mesh();
            var submeshes = SubmeshesFromTristrips(mesh, tristrips);
            mesh.SetSubMeshes(submeshes);
            return mesh;
        }

        public static SubMeshDescriptor[] SubmeshesFromTristrips(Mesh mesh, Tristrip[] tristrips)
        {
            var submeshes = new SubMeshDescriptor[tristrips.Length];
            for (int i = 0; i < submeshes.Length; i++)
            {
                var submesh = new SubMeshDescriptor();

                var tristrip = tristrips[i];
                var vertices = tristrip.positions;
                var normals = tristrip.normals;
                var triangles = tristrip.GetIndices(); // not offset

                // Build submesh
                submesh.baseVertex = mesh.vertexCount;
                submesh.firstVertex = mesh.vertexCount;
                submesh.indexCount = triangles.Length;
                submesh.indexStart = mesh.triangles.Length;
                submesh.topology = MeshTopology.Triangles;
                submesh.vertexCount = vertices.Length;

                // Append to mesh
                var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                var normalsConcat = mesh.normals.Concat(normals).ToArray();
                //var uv1Concat = mesh.uv.Concat(uv1).ToArray();
                //var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
                //var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
                var colorsConcat = mesh.colors32.Concat(tristrip.color0).ToArray();
                //if (list.nbt != null)
                //    mesh.tangents = list.nbt;
                var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

                // Assign values to mesh
                mesh.vertices = verticesConcat;
                mesh.normals = normalsConcat;
                //mesh.uv = uv1Concat;
                //mesh.uv2 = uv2Concat;
                //mesh.uv3 = uv3Concat;
                mesh.colors32 = colorsConcat;
                mesh.triangles = trianglesConcat;

                submeshes[i] = submesh;
            }
            return submeshes;
        }

        public void UpdateMesh()
        {
            Mesh = CreateMesh();

            if (MeshFilter != null)
                MeshFilter.mesh = Mesh;

            if (MeshRenderer != null)
            {
                int numTristrips = Mesh.subMeshCount;
                var materials = new UnityEngine.Material[numTristrips];
                for (int i = 0; i < materials.Length; i++)
                    materials[i] = DefaultMaterial;
                MeshRenderer.sharedMaterials = materials;
            }
        }
    }
}
