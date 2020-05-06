using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using GameCube.FZeroGX.GMA;
using UnityEngine.Rendering;
using System.Linq;

public class TempGmaViewer : MonoBehaviour
{
    [SerializeField] protected GMASobj gma;
    [SerializeField] protected Color posColor = Color.white;
    [SerializeField] protected Color nrmColor = Color.white;
    [SerializeField] protected Mesh[] meshes;

    public GMA GMA => gma.value;

    private void OnDrawGizmos()
    {
        if (gma == null)
            return;

        var up = Vector3.up * 10f;
        foreach (var mesh in meshes)
        {
            Gizmos.color = posColor;
            for (int i = 0; i < mesh.vertexCount - 1; i++)
            {
                var next = i + 1;

                Gizmos.DrawLine(mesh.vertices[i], mesh.vertices[next]);
            }

            Gizmos.color = nrmColor;
            for (int i = 0; i < mesh.vertexCount - 1; i++)
            {
                Gizmos.DrawLine(mesh.vertices[i], mesh.vertices[i] + mesh.normals[i]);
            }
        }

    }

    private void Start()
    {
        MakeOldest();
        MakeOld();
        MakeMesh();
    }

    public void MakeMesh()
    {
        if (gma == null)
            return;

        const string folder = "temp";

        // Count how many submeshes we will need to iterate through
        var numSubmeshes = 0;
        foreach (var gcmfMesh in GMA.GCMF[0].Submeshes)
        {
            numSubmeshes += gcmfMesh.DisplayList0.GxDisplayLists.Length;
        }

        // Future considerations:
        // Include disp1, etc.

        var mesh = new Mesh();
        int submeshIndex = 0;
        var submeshes = new SubMeshDescriptor[numSubmeshes];

        var gcmf = gma.value.GCMF[0];
        {
            foreach (var gcmfMesh in gcmf.Submeshes)
            {
                EditorUtility.DisplayProgressBar("Mesh Test", gcmf.ModelName, 0f);

                foreach (var list in gcmfMesh.DisplayList0.GxDisplayLists)
                {
                    // New from this submesh
                    var vertices = list.pos;
                    var normals = list.nrm;
                    var uv1 = list.tex0;
                    var uv2 = list.tex1;
                    var uv3 = list.tex2;
                    var colors = list.clr0;
                    var triangles = GetTriangleFromTriangleStrip(vertices.Length);

                    var idx = submeshIndex;

                    //submesh.baseVertex = ;
                    //submesh.bounds = ;
                    submeshes[idx].firstVertex = mesh.vertexCount;
                    submeshes[idx].indexCount = triangles.Length;
                    submeshes[idx].indexStart = mesh.vertexCount;
                    submeshes[idx].topology = MeshTopology.Triangles;
                    submeshes[idx].vertexCount = vertices.Length;

                    // Append to mesh
                    var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                    var normalsConcat = mesh.normals.Concat(normals).ToArray();
                    var uv1Concat = mesh.uv.Concat(uv1).ToArray();
                    var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
                    var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
                    var colorsConcat = mesh.colors32.Concat(colors).ToArray();
                    //if (list.nbt != null)
                    //    mesh.tangents = list.nbt;
                    var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

                    mesh.vertices = verticesConcat;
                    mesh.normals = normalsConcat;
                    mesh.uv = uv1Concat;
                    mesh.uv2 = uv2Concat;
                    mesh.uv3 = uv3Concat;
                    mesh.colors32 = colorsConcat;
                    mesh.triangles = trianglesConcat;

                    //mesh.subMeshCount += 2;
                    //mesh.SetSubMesh(submeshIndex, submeshes[i, MeshUpdateFlags.Default);
                    submeshIndex++;
                }
            }
            mesh.subMeshCount = submeshes.Length;
            for (int i = 0; i < submeshes.Length; i++)
            {
                mesh.SetSubMesh(i, submeshes[i], MeshUpdateFlags.Default);
            }
            string name = $"{gcmf.ModelName}.asset";
            AssetDatabase.CreateAsset(mesh, $"Assets/{folder}/{name}");
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }
    public void MakeOld()
    {
        if (gma == null)
            return;

        const string folder = "temp";
        //if (!AssetDatabase.IsValidFolder(folder))
        //{
        //    AssetDatabase.CreateFolder("Assets", folder);
        //}

        //int objectIndex = 0;
        //foreach (var gcmf in gma.value.GCMF)
        var gcmf = gma.value.GCMF[0];
        {
            var gcmfIdx = 0;
            foreach (var gcmfMesh in gcmf.Submeshes)
            {
                EditorUtility.DisplayProgressBar("Mesh Test", gcmf.ModelName, 0f);

                var mesh = new Mesh();
                int submeshIndex = 0;
                var count = gcmfMesh.DisplayList0.GxDisplayLists.Length;
                var submeshes = new SubMeshDescriptor[count];

                foreach (var list in gcmfMesh.DisplayList0.GxDisplayLists)
                {
                    // New from this submesh
                    var vertices = list.pos;
                    var normals = list.nrm;
                    var uv1 = list.tex0;
                    var uv2 = list.tex1;
                    var uv3 = list.tex2;
                    var colors = list.clr0;
                    var triangles = GetTriangleFromTriangleStrip(vertices.Length);

                    var idx = submeshIndex;

                    //submesh.baseVertex = ;
                    //submesh.bounds = ;
                    submeshes[idx].firstVertex = mesh.vertexCount;
                    submeshes[idx].indexCount = triangles.Length;
                    submeshes[idx].indexStart = mesh.vertexCount;
                    submeshes[idx].topology = MeshTopology.Triangles;
                    submeshes[idx].vertexCount = vertices.Length;

                    // Append to mesh
                    var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                    var normalsConcat = mesh.normals.Concat(normals).ToArray();
                    var uv1Concat = mesh.uv.Concat(uv1).ToArray();
                    var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
                    var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
                    var colorsConcat = mesh.colors32.Concat(colors).ToArray();
                    //if (list.nbt != null)
                    //    mesh.tangents = list.nbt;
                    var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

                    mesh.vertices = verticesConcat;
                    mesh.normals = normalsConcat;
                    mesh.uv = uv1Concat;
                    mesh.uv2 = uv2Concat;
                    mesh.uv3 = uv3Concat;
                    mesh.colors32 = colorsConcat;
                    mesh.triangles = trianglesConcat;

                    //mesh.subMeshCount += 2;
                    //mesh.SetSubMesh(submeshIndex, submeshes[i, MeshUpdateFlags.Default);
                    submeshIndex++;
                }
                mesh.subMeshCount = submeshes.Length;
                for (int i = 0; i < submeshes.Length; i++)
                {
                    mesh.SetSubMesh(i, submeshes[i], MeshUpdateFlags.Default);
                }

                string name = $"{gcmf.ModelName} {gcmfIdx}.asset";
                AssetDatabase.CreateAsset(mesh, $"Assets/{folder}/{name}");
                gcmfIdx++;
            }
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }
    public void MakeOldest()
    {
        if (gma == null)
            return;

        const string folder = "temp";
        //if (!AssetDatabase.IsValidFolder(folder))
        //{
        //    AssetDatabase.CreateFolder("Assets", folder);
        //}

        //int objectIndex = 0;
        //foreach (var gcmf in gma.value.GCMF)
        var gcmf = gma.value.GCMF[0];
        {
            var gcmfIdx = 0;
            foreach (var gcmfMesh in gcmf.Submeshes)
            {
                EditorUtility.DisplayProgressBar("Mesh Test", gcmf.ModelName, 0f);

                var subIndex = 0;
                foreach (var list in gcmfMesh.DisplayList0.GxDisplayLists)
                {
                    var mesh = new Mesh();

                    // New from this submesh
                    var vertices = list.pos;
                    var normals = list.nrm;
                    var uv1 = list.tex0;
                    var uv2 = list.tex1;
                    var uv3 = list.tex2;
                    var colors = list.clr0;
                    var triangles = GetTriangleFromTriangleStrip(vertices.Length);

                    // Append to mesh
                    var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                    var normalsConcat = mesh.normals.Concat(normals).ToArray();
                    var uv1Concat = mesh.uv.Concat(uv1).ToArray();
                    var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
                    var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
                    var colorsConcat = mesh.colors32.Concat(colors).ToArray();
                    //if (list.nbt != null)
                    //    mesh.tangents = list.nbt;
                    var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

                    mesh.vertices = verticesConcat;
                    mesh.normals = normalsConcat;
                    mesh.uv = uv1Concat;
                    mesh.uv2 = uv2Concat;
                    mesh.uv3 = uv3Concat;
                    mesh.colors32 = colorsConcat;
                    mesh.triangles = trianglesConcat;

                    string name = $"{gcmf.ModelName} {gcmfIdx}-{subIndex}.asset";
                    AssetDatabase.CreateAsset(mesh, $"Assets/{folder}/{name}");
                    subIndex++;
                }
                gcmfIdx++;
            }
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    public static int[] GetTriangleFromTriangleStrip(int numVerts)
    {
        // Construct triangles from GameCube GX TRIANLE_STRIP

        var nTriangles = numVerts - 2;
        int[] triangles = new int[nTriangles * 3];
        for (int i = 0; i < nTriangles; i++)
        {
            if ((i & 1) == 0)
            {
                var @base = i * 3;
                triangles[@base + 0] = i + 0;
                triangles[@base + 1] = i + 1;
                triangles[@base + 2] = i + 2;
            }
            else
            {
                var triIdx = i * 3;
                triangles[triIdx + 0] = i + 0;
                triangles[triIdx + 1] = i + 2;
                triangles[triIdx + 2] = i + 1;
            }
        }
        return triangles;
    }
}
