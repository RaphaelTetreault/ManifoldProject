using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class TempMeshVis : MonoBehaviour
    {
        public GfzTrackSegment trackSegment;
        public bool enableDebug = true;
        public int nTritrips = 4;
        public float maxStep = 10f;

        private void OnDrawGizmos()
        {
            if (!enableDebug)
                return;
            if (trackSegment is null)
                return;

            // Sample left and right vertices
            var endpointA = new Vector3(-0.5f, 0, 0);
            var endpointB = new Vector3(+0.5f, 0, 0);
            var vertices = new Vector3[nTritrips+1];
            for (int i = 0; i < vertices.Length; i++)
            {
                float percentage = i / (float)nTritrips;
                vertices[i] = Vector3.Lerp(endpointA, endpointB, percentage);
            }

            //var vertices = new Vector3[] { new Vector3(0.5f, 0, 0), new Vector3(0,0,0), new Vector3(-0.5f, 0, 0) };
            var matrices = TrackGeoGenerator.GenerateMtxIntervals(trackSegment, maxStep);
            var tristrips = TrackGeoGenerator.GenerateTristrips(matrices, vertices);

            foreach (var tristrip in tristrips)
            {
                tristrip.normals = new Vector3[tristrip.positions.Length];
                for (int i = 0; i < tristrip.normals.Length; i++)
                    tristrip.normals[i] = new Vector3(0, 1, 0);
            }

            var mesh = new Mesh();
            var submeshes = SubmeshesFromTristrips(mesh, tristrips);
            mesh.SetSubMeshes(submeshes);

            Gizmos.color = Color.blue;
            for (int i = 0; i < mesh.subMeshCount; i++)
                Gizmos.DrawMesh(mesh, i);

            //Gizmos.color = Color.red;
            //for (int t = 0; t < tristrips.Length; t++)
            //{
            //    var tristrip = tristrips[t];
            //    for (int i = 0; i < tristrip.TrianglesCount; i++)
            //    {
            //        var v0 = tristrip.positions[i + 0];
            //        var v1 = tristrip.positions[i + 1];
            //        var v2 = tristrip.positions[i + 2];
            //        var v3 = tristrip.positions[i + 3];
            //        Gizmos.DrawLine(v0, v1);
            //        Gizmos.DrawLine(v0, v2);
            //        Gizmos.DrawLine(v1, v2);
            //        Gizmos.DrawLine(v1, v3);
            //        Gizmos.DrawLine(v2, v3);
            //    }
            //}
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
                //var colorsConcat = mesh.colors32.Concat(colors).ToArray();
                //if (list.nbt != null)
                //    mesh.tangents = list.nbt;
                var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

                // Assign values to mesh
                mesh.vertices = verticesConcat;
                mesh.normals = normalsConcat;
                //mesh.uv = uv1Concat;
                //mesh.uv2 = uv2Concat;
                //mesh.uv3 = uv3Concat;
                //mesh.colors32 = colorsConcat;
                mesh.triangles = trianglesConcat;

                submeshes[i] = submesh;
            }
            return submeshes;
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
