using GameCube.GX;
using GameCube.GFZ.Gma2;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ.Gma2
{
    public static class GmaImport
    {
        public static void Import(Gma gma, UnityEngine.Material defaultMaterial)
        {
            // Set destination folder name for models
            var filePath = Path.GetDirectoryName(gma.FileName);
            var fileName = Path.GetFileNameWithoutExtension(gma.FileName);
            // Create folder if it doesn't already exist
            var modelDestination = $"{filePath}/{fileName}/";
            // Ensure the folder path exists
            AssetDatabaseUtility.CreatePath(modelDestination);

            foreach (var model in gma.Models)
            {
                //var importTitle = $"Importing Model (/{totalModels}) Submesh Total ({submeshes})";
                var mesh = CreateSingleMeshFromModel(model, modelDestination, "TEMP: Importing Model");

                // HACK: Add a generic material to each model
                // In the future, generate materials for models
                var hackMaterials = new UnityEngine.Material[mesh.subMeshCount];
                for (int i = 0; i < hackMaterials.Length; i++)
                    hackMaterials[i] = defaultMaterial;

                // Construct path and name for prefab
                var prefabPath = $"{modelDestination}/pf_{model.Name}.prefab";
                // Create and store asset to Asset Database
                ImportUtility.CreatePrefabFromModel(mesh, hackMaterials, prefabPath);
            }
            ImportUtility.FinalizeAssetImport();
        }

        public static int[] GetTrianglesFromTriangleStrip(int numVerts, bool baseCCW)
        {
            // Construct triangles from GameCube GX TRIANGLE_STRIP
            // For one, note that we need to unwind the tristrip.
            // We can use the index to know if the indice is odd or even.
            // However, in GFZ, the winding for different display lists
            // inverts based on it's "index," so to speak.
            // To compensate, we need to XOR the odd/even value with whether
            // the base index of the strip is meant to be CCW or CW.

            const int vertexStride = 3;

            var nTriangles = numVerts - 2;
            int[] triangles = new int[nTriangles * vertexStride];
            for (int i = 0; i < nTriangles; i++)
            {
                var triIndex = i * vertexStride;
                var indexIsCW = (i % 2) > 0;
                var isCCW = (baseCCW ^ indexIsCW);

                if (isCCW)
                {
                    triangles[triIndex + 0] = i + 0;
                    triangles[triIndex + 1] = i + 1;
                    triangles[triIndex + 2] = i + 2;
                }
                else
                {
                    triangles[triIndex + 0] = i + 0;
                    triangles[triIndex + 1] = i + 2;
                    triangles[triIndex + 2] = i + 1;
                }
            }

            return triangles;
        }

        public static Mesh CreateSingleMeshFromModel(Model model, string path, string title = "Importing GCMF...")
        {
            // Count how many submeshes we will need to iterate through
            var numSubmeshes = CountSubmeshes(model);

            // Create base data for mesh
            var mesh = new Mesh();
            int submeshIndex = 0;
            var submeshDescriptors = new SubMeshDescriptor[numSubmeshes];

            // Go over each mesh in submeshes
            foreach (var submesh in model.Gcmf.Submeshes)
            {
                ImportUtility.ProgressBar<Gcmf>(submeshIndex, numSubmeshes, $"{path}{model.Name}");

                if (submesh.DisplayListCW is not null)
                {
                    var submeshDescriptor = CreateSubMesh(submesh.DisplayListCW, ref mesh, true);
                    submeshDescriptors[submeshIndex] = submeshDescriptor;
                    submeshIndex++;
                }

                if (submesh.DisplayListCCW is not null)
                {
                    var submeshDescriptor = CreateSubMesh(submesh.DisplayListCCW, ref mesh, true);
                    submeshDescriptors[submeshIndex] = submeshDescriptor;
                    submeshIndex++;
                }

                if (submesh.SkinnedDisplayListCW is not null)
                {
                    var submeshDescriptor = CreateSubMesh(submesh.SkinnedDisplayListCW, ref mesh, true);
                    submeshDescriptors[submeshIndex] = submeshDescriptor;
                    submeshIndex++;
                }

                if (submesh.SkinnedDisplayListCCW is not null)
                {
                    var submeshDescriptor = CreateSubMesh(submesh.SkinnedDisplayListCCW, ref mesh, true);
                    submeshDescriptors[submeshIndex] = submeshDescriptor;
                    submeshIndex++;
                }
            }

            // Set each submesh in the mesh
            mesh.subMeshCount = submeshDescriptors.Length;
            for (int i = 0; i < submeshDescriptors.Length; i++)
            {
                mesh.SetSubMesh(i, submeshDescriptors[i], MeshUpdateFlags.Default);
            }
            mesh.RecalculateBounds();

            // Name the model as it appears in the Asset Database
            // NOTE: this also has the pleasant side effect of preventing models being named
            // COM1, COM2, COM3, etc, which results in an error on Windows due to those being
            // reserved names for legacy systems.
            string name = $"mdl_{model.Name}.asset";
            AssetDatabase.CreateAsset(mesh, $"{path}/{name}");

            return mesh;
        }

        public static Vector3[] GetPositions(float3[] pos)
        {
            // Convert float3[] to Vector3[]
            var _pos = new Vector3[pos.Length];
            for (int i = 0; i < pos.Length; i++)
                _pos[i] = pos[i];
            return _pos;
        }

        public static Vector2[] HackUVs(float2[] uvs, int vertCount)
        {
            if (uvs.Length > 0)
            {
                // Convert float2[] to Vector2[]
                var _uvs = new Vector2[uvs.Length];
                for (int i = 0; i < uvs.Length; i++)
                    _uvs[i] = uvs[i];
                return _uvs;
            }
            else
            {
                // If no UV data, store floats as NaNs for removal later.
                var hackUVs = new Vector2[vertCount];
                for (int i = 0; i < hackUVs.Length; i++)
                    hackUVs[i] = Vector2.one * float.NaN;
                return hackUVs;
            }
        }

        public static Vector3[] HackNormals(float3[] normals, int vertCount)
        {
            // Normals will get recalculated based on polygon
            if (normals.Length > 0)
            {
                // Convert float3[] to Vector3[]
                var _normals = new Vector3[normals.Length];
                for (int i = 0; i < normals.Length; i++)
                    _normals[i] = normals[i];
                return _normals;
            }
            else
            {
                return new Vector3[vertCount];
            }
        }

        public static Color32[] HackColors(Color32[] colors, int vertCount)
        {
            // Looks like white, but is 0xFEFDFCFF, but becomes "magic"
            // value you can parse out since it likely* isn't used.
            // *TODO: confirm this colour is not used in the game.
            var defaultColor = new Color32(254, 253, 252, 255);

            if (colors.Length == 0)
            {
                colors = new Color32[vertCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = defaultColor;
                }
            }
            return colors;
        }

        public static SubMeshDescriptor CreateSubMesh(DisplayList displayList, ref Mesh mesh, bool isCCW)
        {
            var submesh = new SubMeshDescriptor();
            var nVerts = displayList.pos.Length;

            // New from this list/submesh
            var vertices = GetPositions(displayList.pos);
            var normals = HackNormals(displayList.nrm, nVerts);
            var uv1 = HackUVs(displayList.tex0, nVerts);
            var uv2 = HackUVs(displayList.tex1, nVerts);
            var uv3 = HackUVs(displayList.tex2, nVerts);
            var colors = HackColors(displayList.clr0, nVerts);
            var triangles = GetTrianglesFromTriangleStrip(vertices.Length, isCCW);

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
            var uv1Concat = mesh.uv.Concat(uv1).ToArray();
            var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
            var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
            var colorsConcat = mesh.colors32.Concat(colors).ToArray();
            //if (list.nbt != null)
            //    mesh.tangents = list.nbt;
            var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

            // Assign values to mesh
            mesh.vertices = verticesConcat;
            mesh.normals = normalsConcat;
            mesh.uv = uv1Concat;
            mesh.uv2 = uv2Concat;
            mesh.uv3 = uv3Concat;
            mesh.colors32 = colorsConcat;
            mesh.triangles = trianglesConcat;

            return submesh;
        }

        public static int CountModels(Gma gma, out int submeshes)
        {
            int numSubmeshes = 0;
            int numModels = 0;

            foreach (var model in gma.Models)
            {
                // Increment model count as model data exists
                numModels++;
                numSubmeshes += CountSubmeshes(model);

            }
            submeshes = numSubmeshes;
            return numModels;
        }

        public static int CountSubmeshes(Model model)
        {
            int numSubmeshes = 0;

            foreach (var submesh in model.Gcmf.Submeshes)
            {
                if (submesh.DisplayListCW is not null)
                    numSubmeshes++;
                if (submesh.DisplayListCCW is not null)
                    numSubmeshes++;
                if (submesh.SkinnedDisplayListCW is not null)
                    numSubmeshes++;
                if (submesh.SkinnedDisplayListCCW is not null)
                    numSubmeshes++;
            }

            return numSubmeshes;
        }

    }
}
