using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public abstract class GfzSegmentShape : GfzSegmentNode
    {
        [field: SerializeField, ReadOnlyGUI] public MeshDisplay MeshDisplay { get; protected set; }

        public abstract Mesh CreateMesh();
        public abstract Gcmf CreateGcmf(out GcmfTemplate[] gcmfTemplates, TplTextureContainer tpl);

        public override float GetMaxTime()
        {
            return GetRoot().GetMaxTime();
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
                var colors = tristrip.color0.IsNullOrEmpty() ? ArrayUtility.DefaultArray(new Color32(255, 255, 255, 255), tristrip.VertexCount) : tristrip.color0;
                var triangles = tristrip.GetIndices(); // not offset
                //var uv1 = tristrip.uv0 == null ? new Vector2[vertices.Length] : tristrip.uv0;
                //var uv2 = tristrip.uv1;
                //var uv3 = tristrip.uv2;

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
                var colorsConcat = mesh.colors32.Concat(colors).ToArray();
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

        public Tristrip[] CombinedTristrips(Tristrip[][] tristripsCollection)
        {
            var allTristrips = new List<Tristrip>();
            foreach (var tristrips in tristripsCollection)
                allTristrips.AddRange(tristrips);

            return allTristrips.ToArray();
        }


        public void UpdateMesh()
        {
            var mesh = CreateMesh();
            MeshDisplay.UpdateMesh(mesh);

            // Do no offset mesh display - mesh coords are in world space.
            var scale = transform.lossyScale;
            MeshDisplay.transform.localScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
            MeshDisplay.transform.position = Vector3.zero;
            MeshDisplay.transform.rotation = Quaternion.identity;
        }

        public override void InvokeUpdates()
        {
            ValidateMeshDisplay();
            base.InvokeUpdates();
        }

        public void ValidateMeshDisplay()
        {
            // If null, see if we have a child already. Important check on Reset().
            if (MeshDisplay == null)
                MeshDisplay = GetComponentInChildren<MeshDisplay>();

            // If not, it is still null, so make an instance.
            if (MeshDisplay == null)
                CreateMeshDisplay();
        }

        private void CreateMeshDisplay()
        {
            var meshDisplayGobj = new GameObject("Mesh Display");
            meshDisplayGobj.transform.parent = this.transform;

            var meshDisplay = meshDisplayGobj.AddComponent<MeshDisplay>();
            MeshDisplay = meshDisplay;

            // Hide this object in inspector
            meshDisplay.SetHideGameObjectInEditor(true);
        }

        private void OnDrawGizmosSelected()
        {
            var pos = CreateHierarchichalAnimationCurveTRS(false).EvaluateHierarchyPosition(0);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 10f);
        }


    }
}
