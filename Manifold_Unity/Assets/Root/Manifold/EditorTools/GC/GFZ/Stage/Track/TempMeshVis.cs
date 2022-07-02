using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class TempMeshVis : MonoBehaviour
    {
        public GfzTrackSegment trackSegment;
        public bool enableDebug = true;
        public int widthSamples = 4;
        public float minDistance = 10f;

        private void OnDrawGizmos()
        {
            if (!enableDebug)
                return;
            if (trackSegment is null)
                return;

            var tempStrips = TrackGeoGenerator.GenerateTristrips(trackSegment, minDistance, null);
            var mesh = new Mesh();
            var submeshes = CreateSubMesh(tempStrips, mesh);
            foreach (var submesh in submeshes)
                mesh.SetSubMesh(0, submesh, MeshUpdateFlags.Default);

            Gizmos.color = Color.white;
            for (int i = 0; i < mesh.subMeshCount; i++)
                Gizmos.DrawMesh(mesh, i);

            Gizmos.color = Color.red;
            int stride = widthSamples + 1;
            int maxVertIter = mesh.vertexCount - stride;
            for (int vi = 0; vi < maxVertIter; vi += stride)
            {
                var v0 = mesh.vertices[vi];
                var v1 = mesh.vertices[vi + 1];
                var v2 = mesh.vertices[vi + stride];
                var v3 = mesh.vertices[vi + 1 + stride];
                Gizmos.DrawLine(v0, v1);
                Gizmos.DrawLine(v0, v2);
                Gizmos.DrawLine(v1, v2);
                Gizmos.DrawLine(v1, v3);
                //Gizmos.DrawLine(v2, v3);
            }
        }

        public static SubMeshDescriptor[] CreateSubMesh(Tristrip[] tristrips, Mesh mesh)
        {
            var submeshes = new SubMeshDescriptor[tristrips.Length];
            for (int i = 0; i < submeshes.Length; i++)
            {
                var submesh = new SubMeshDescriptor();
                var indexes = tristrips[i];

                // Build submesh. Init with current position in mesh.
                submesh.baseVertex = mesh.vertexCount;
                submesh.firstVertex = mesh.vertexCount;
                submesh.indexCount = mesh.triangles.Length;
                submesh.indexStart = mesh.triangles.Length;
                submesh.topology = MeshTopology.Triangles;
                submesh.vertexCount = mesh.vertices.Length;

                // Append to mesh
                //var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                //var normalsConcat = mesh.normals.Concat(normals).ToArray();
                //var uv1Concat = mesh.uv.Concat(uv1).ToArray();
                //var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
                //var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
                //var colorsConcat = mesh.colors32.Concat(colors).ToArray();
                ////if (list.nbt != null)
                ////    mesh.tangents = list.nbt;
                //var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

                //// Assign values to mesh
                //mesh.vertices = verticesConcat;
                //mesh.normals = normalsConcat;
                //mesh.uv = uv1Concat;
                //mesh.uv2 = uv2Concat;
                //mesh.uv3 = uv3Concat;
                //mesh.colors32 = colorsConcat;
                //mesh.triangles = trianglesConcat;

                submeshes[i] = submesh;
            }
            throw new System.NotImplementedException();
            //return submeshes;
        }

        //public static SubMeshDescriptor CreateSubMesh(TrackMesh tempMesh, Mesh mesh)
        //{
        //    var submesh = new SubMeshDescriptor();

        //    // New from this list/submesh
        //    //var vertices = tempMesh.vertexes;
        //    //var normals = GetNormals(displayList.nrm, nVerts);
        //    //var uv1 = GetUVs(displayList.tex0, nVerts);
        //    //var uv2 = GetUVs(displayList.tex1, nVerts);
        //    //var uv3 = GetUVs(displayList.tex2, nVerts);
        //    //var colors = GetColors(displayList.clr0, nVerts);
        //    //var triangles = GetTrianglesFromTriangleStrip(vertices.Length, isCCW);

        //    // Build submesh
        //    submesh.baseVertex = 0;// mesh.vertexCount;
        //    submesh.firstVertex = 0;// mesh.vertexCount;
        //    submesh.indexCount = tempMesh.indexes.Length;// triangles.Length;
        //    submesh.indexStart = 0;// mesh.triangles.Length;
        //    submesh.topology = MeshTopology.Triangles;
        //    submesh.vertexCount = tempMesh.vertexes.Length;// vertices.Length;

        //    // Append to mesh
        //    //var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
        //    //var normalsConcat = mesh.normals.Concat(normals).ToArray();
        //    //var uv1Concat = mesh.uv.Concat(uv1).ToArray();
        //    //var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
        //    //var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
        //    //var colorsConcat = mesh.colors32.Concat(colors).ToArray();
        //    ////if (list.nbt != null)
        //    ////    mesh.tangents = list.nbt;
        //    //var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

        //    //// Assign values to mesh
        //    //mesh.vertices = verticesConcat;
        //    //mesh.normals = normalsConcat;
        //    //mesh.uv = uv1Concat;
        //    //mesh.uv2 = uv2Concat;
        //    //mesh.uv3 = uv3Concat;
        //    //mesh.colors32 = colorsConcat;
        //    //mesh.triangles = trianglesConcat;

        //    mesh.vertices = tempMesh.vertexes;
        //    mesh.normals = tempMesh.normals;
        //    mesh.triangles = tempMesh.indexes;

        //    return submesh;
        //}
    }
}
