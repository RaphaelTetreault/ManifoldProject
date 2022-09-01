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

        public enum ShapeID
        {
            road,
            pipe,
            cylinder,
            embed,
        }
        public abstract ShapeID ShapeIdentifier { get; }


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
            // To keep track of which vertex/index we're at in the mesh
            int vertexBase = 0;
            int indexesBase = 0;

            // Count all vertices and indexes used for this mesh
            int allVertexCount = 0;
            int allIndexesCount = 0;
            foreach (var tristrip in tristrips)
            {
                allVertexCount += tristrip.VertexCount;
                allIndexesCount += tristrip.TriangleIndexesCount;
            }

            // Init arrays for all of that data
            var allPositions = new Vector3[allVertexCount];
            var allNormals = new Vector3[allVertexCount];
            var allColors = new Color32[allVertexCount];
            var allTex0 = new Vector2[allVertexCount];
            var allTex1 = new Vector2[allVertexCount];
            var allTex2 = new Vector2[allVertexCount];
            var allIndices = new int[allIndexesCount];

            var submeshDescriptors = new SubMeshDescriptor[tristrips.Length];
            for (int i = 0; i < submeshDescriptors.Length; i++)
            {
                var submeshDescriptor = new SubMeshDescriptor();

                // Get tristrip and relevant info
                var tristrip = tristrips[i];
                int vertexCount = tristrip.VertexCount;
                int indexesCount = tristrip.TriangleIndexesCount;

                // Initi data for submesh. Missing data is replaced with defaults.
                var positions = tristrip.positions;
                var normals = tristrip.normals;
                var color0 = tristrip.color0.IsNullOrEmpty() ? ArrayUtility.DefaultArray(new Color32(255, 255, 255, 255), tristrip.VertexCount) : tristrip.color0;
                var tex0 = tristrip.tex0 == null ? new Vector2[positions.Length] : tristrip.tex0;
                var tex1 = tristrip.tex1 == null ? new Vector2[positions.Length] : tristrip.tex1;
                var tex2 = tristrip.tex2 == null ? new Vector2[positions.Length] : tristrip.tex2;
                var indices = tristrip.GetIndices();

                // Set submesh base vertex and base index information
                submeshDescriptor.baseVertex = vertexBase;
                submeshDescriptor.firstVertex = vertexBase;
                submeshDescriptor.indexCount = indexesCount;
                submeshDescriptor.indexStart = indexesBase;
                submeshDescriptor.topology = MeshTopology.Triangles;
                submeshDescriptor.vertexCount = vertexCount;
                
                // Copy all the data to the full mesh arrays
                positions.CopyTo(allPositions, vertexBase);
                normals.CopyTo(allNormals, vertexBase);
                tex0.CopyTo(allTex0, vertexBase);
                tex1.CopyTo(allTex1, vertexBase);
                tex2.CopyTo(allTex2, vertexBase);
                color0.CopyTo(allColors, vertexBase);
                indices.CopyTo(allIndices, indexesBase);

                // Increment base values
                vertexBase += vertexCount;
                indexesBase += indexesCount;

                // Assign submesh descriptor
                submeshDescriptors[i] = submeshDescriptor;
            }

            // Finally, assign complete data to mesh
            mesh.vertices = allPositions;
            mesh.normals = allNormals;
            mesh.uv = allTex0;
            mesh.uv2 = allTex1;
            mesh.uv3 = allTex2;
            mesh.colors32 = allColors;
            mesh.triangles = allIndices;

            return submeshDescriptors;
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
