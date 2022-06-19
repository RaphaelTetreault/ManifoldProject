// TODO:
// - Implement data for all vertex data, ie: NBT, etc. Review what is used for GFZ.

using GameCube.GX;
using GameCube.GFZ.GMA;
using Manifold.IO;
using System;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ.GMA
{
    public static class GmaImport
    {
        private const string MaterialsFolder = "Assets/Root/Manifold/EditorTools/GC/GFZ/Materials/";
        private const string DefaultMaterialAssetPath = MaterialsFolder + "mat_VertexColor.mat";


        [MenuItem(GfzMenuItems.ImportGma, priority = GfzMenuItems.ImportGmaPriority)]
        public static void ImportGma()
        {
            var settings = GfzProjectWindow.GetSettings();
            var rootDirectory = settings.SourceDirectory;
            ImportGma(rootDirectory);
        }

        [MenuItem(GfzMenuItems.ImportGmaAllRegions, priority = GfzMenuItems.ImportGmaAllRegionsPriority)]
        public static void ImportGmaAllRegions()
        {
            var settings = GfzProjectWindow.GetSettings();
            var testDirectories = settings.GetTestRootDirectories();
            foreach (var dir in testDirectories)
                ImportGma(dir);
        }

        public static void ImportGma(string rootDirectory)
        {
            var filePaths = Directory.GetFiles(rootDirectory, "*.gma", SearchOption.AllDirectories);
            var gmas = BinarySerializableIO.LoadFile<Gma>(filePaths);

            // Go up a directory
            var relativeRoot = DirectoryUtility.GoUpDirectory(rootDirectory, 1);

            int index = 0;
            foreach (var gma in gmas)
            {
                // Get relative path to asset, get folder, create folder in Assets if necessary
                var filePath = filePaths[index++].Remove(0, relativeRoot.Length);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var directory = Path.GetDirectoryName($"Assets/{filePath}");
                var cleanDirectory = directory.Replace('\\', '/');
                var outputDirectory = $"{cleanDirectory}/{fileName}";
                AssetDatabaseUtility.CreateDirectory(outputDirectory);

                foreach (var model in gma.Models)
                {
                    // Get mesh and materials
                    var mesh = CreateSingleMeshFromModel(model, outputDirectory, "Importing Model");
                    var tempMaterial = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(DefaultMaterialAssetPath);
                    var meshMaterials = HackApplyDefaultMaterial(mesh, tempMaterial);

                    // Construct prefab path, save to asset database
                    var prefabPath = $"{outputDirectory}/pf_{model.Name}.prefab";
                    CreatePrefabFromModel(mesh, meshMaterials, prefabPath);
                }
            }

            ProgressBar.Clear();
        }

        public static UnityEngine.Material[] HackApplyDefaultMaterial(Mesh mesh, UnityEngine.Material defaultMaterial)
        {
            // HACK: Add a generic material to each model submesh
            // In the future, generate materials for models from GMA
            var hackMaterials = new UnityEngine.Material[mesh.subMeshCount];
            for (int i = 0; i < hackMaterials.Length; i++)
                hackMaterials[i] = defaultMaterial;

            return hackMaterials;
        }

        public static GameObject CreatePrefabFromModel(Mesh mesh, UnityEngine.Material[] meshMaterials, string assetPath)
        {
            // Create new GameObject (ends up in current scene)
            var tempObject = new GameObject();

            // Add mesh components, assign mesh
            var meshRenderer = tempObject.AddComponent<MeshRenderer>();
            var meshFilter = tempObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshRenderer.sharedMaterials = meshMaterials;

            // Save to Asset Database
            var prefab = PrefabUtility.SaveAsPrefabAsset(tempObject, assetPath);
            // Remove asset from scene
            UnityEngine.Object.DestroyImmediate(tempObject);

            return prefab;
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

        private static void SetTrianglesCounterClockwise(int[] triangles, int triIndex, int i)
        {
            triangles[triIndex + 0] = i + 0;
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = i + 2;
        }
        private static void SetTrianglesClockwise(int[] triangles, int triIndex, int i)
        {
            triangles[triIndex + 0] = i + 0;
            triangles[triIndex + 1] = i + 2;
            triangles[triIndex + 2] = i + 1;
        }

        delegate void SetTriangleIndexes(int[] triangleIndexes, int baseTriangleIndex, int triangleIndex);
        public static int[] GetTrianglesFromTriangleStrip2(int numVerts, bool baseCCW)
        {
            // Construct triangles from GameCube GX TRIANGLE_STRIP
            // For one, note that we need to unwind the tristrip.
            // We can use the index to know if the indice is odd or even.
            // However, in GFZ, the winding for different display lists
            // inverts based on it's "index," so to speak.
            // To compensate, we need to XOR the odd/even value with whether
            // the base index of the strip is meant to be CCW or CW.

            const int vertexStride = 3;
            int nTriangles = numVerts - 2;
            int[] triangles = new int[nTriangles * vertexStride];

            // Create array of functions to set rtriangles based on index
            SetTriangleIndexes[] setTris = new SetTriangleIndexes[]
            {
                baseCCW ? SetTrianglesCounterClockwise : SetTrianglesClockwise,
                baseCCW ? SetTrianglesClockwise : SetTrianglesCounterClockwise,
            };

            // Foreach triangle
            for (int i = 0; i < nTriangles; i++)
            {
                int baseTriangleIndex = i * vertexStride;
                setTris[0].Invoke(triangles, baseTriangleIndex + 0, i + 0);
                setTris[1].Invoke(triangles, baseTriangleIndex + 1, i + 1);
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
                var cancel = ProgressBar.ShowIndexed(submeshIndex, numSubmeshes, "Importing Model's Submesh", $"{path}/{model.Name}");
                if (cancel)
                {
                    ProgressBar.Clear();
                    throw new Exception("Aborted import.");
                }

                if (submesh.PrimaryDisplayListsOpaque is not null)
                {
                    foreach (var displayList in submesh.PrimaryDisplayListsOpaque)
                    {
                        var submeshDescriptor = CreateSubMesh(displayList, mesh, true);
                        submeshDescriptors[submeshIndex] = submeshDescriptor;
                        submeshIndex++;
                    }
                }

                if (submesh.PrimaryDisplayListsTranslucid is not null)
                {
                    foreach (var displayList in submesh.PrimaryDisplayListsTranslucid)
                    {
                        var submeshDescriptor = CreateSubMesh(displayList, mesh, false);
                        submeshDescriptors[submeshIndex] = submeshDescriptor;
                        submeshIndex++;
                    }
                }

                if (submesh.SecondaryDisplayListsOpaque is not null)
                {
                    foreach (var displayList in submesh.SecondaryDisplayListsOpaque)
                    {
                        var submeshDescriptor = CreateSubMesh(displayList, mesh, true);
                        submeshDescriptors[submeshIndex] = submeshDescriptor;
                        submeshIndex++;
                    }
                }

                if (submesh.SecondaryDisplayListsTranslucid is not null)
                {
                    foreach (var displayList in submesh.SecondaryDisplayListsTranslucid)
                    {
                        var submeshDescriptor = CreateSubMesh(displayList, mesh, false);
                        submeshDescriptors[submeshIndex] = submeshDescriptor;
                        submeshIndex++;
                    }
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
            string outputPath = $"{path}/{name}";
            AssetDatabase.CreateAsset(mesh, outputPath);

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

        public static Vector2[] GetUVs(float2[] uvs, int vertCount)
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

        public static Vector3[] GetNormals(float3[] normals, int vertCount)
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

        public static Color32[] GetColors(GXColor[] gxColors, int vertCount)
        {
            // Looks like white, but is 0xFEFDFCFF, but becomes "magic"
            // value you can parse out since it likely* isn't used.
            // *TODO: confirm this colour is not used in the game.
            var defaultColor = new Color32(254, 253, 252, 255);
            var colors = new Color32[vertCount];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = (gxColors.Length > 0)
                    ? new Color32(gxColors[i].R, gxColors[i].G, gxColors[i].B, gxColors[i].A)
                    : defaultColor;
            }

            return colors;
        }

        public static SubMeshDescriptor CreateSubMesh(DisplayList displayList, Mesh mesh, bool isCCW)
        {
            var submesh = new SubMeshDescriptor();
            var nVerts = displayList.pos.Length;

            // New from this list/submesh
            var vertices = GetPositions(displayList.pos);
            var normals = GetNormals(displayList.nrm, nVerts);
            var uv1 = GetUVs(displayList.tex0, nVerts);
            var uv2 = GetUVs(displayList.tex1, nVerts);
            var uv3 = GetUVs(displayList.tex2, nVerts);
            var colors = GetColors(displayList.clr0, nVerts);
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
                if (submesh.PrimaryDisplayListsOpaque is not null)
                    numSubmeshes += submesh.PrimaryDisplayListsOpaque.Length;
                if (submesh.PrimaryDisplayListsTranslucid is not null)
                    numSubmeshes += submesh.PrimaryDisplayListsTranslucid.Length;
                if (submesh.SecondaryDisplayListsOpaque is not null)
                    numSubmeshes += submesh.SecondaryDisplayListsOpaque.Length;
                if (submesh.SecondaryDisplayListsTranslucid is not null)
                    numSubmeshes += submesh.SecondaryDisplayListsTranslucid.Length;
            }

            return numSubmeshes;
        }

    }
}
