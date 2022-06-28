using Manifold.IO;
using Manifold.EditorTools.GC.GFZ.GMA;
using GameCube.GFZ.Stage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manifold.EditorTools.GC.GFZ.Stage.Colliders
{
    public static class ColliderImport
    {
        private const string materialsFolder = "Assets/Root/Manifold/EditorTools/GC/GFZ/Materials/";
        private const string triMaterialAsset = materialsFolder + "mat_ColliderTriangles.mat";
        private const string quadMaterialAsset = materialsFolder + "mat_ColliderQuads.mat";
        private static readonly string[] StaticColliderTypeMaterialAssets = new string[] {
            materialsFolder + "mat_ColiType00_Driveable.mat",
            materialsFolder + "mat_ColiType01_Recover.mat",
            materialsFolder + "mat_ColiType02_Wall.mat",
            materialsFolder + "mat_ColiType03_Dash.mat",
            materialsFolder + "mat_ColiType04_Jump.mat",
            materialsFolder + "mat_ColiType05_Ice.mat",
            materialsFolder + "mat_ColiType06_Dirt.mat",
            materialsFolder + "mat_ColiType07_Damage.mat",
            materialsFolder + "mat_ColiType08_OutOfBounds.mat",
            materialsFolder + "mat_ColiType09_DeathGround.mat",
            materialsFolder + "mat_ColiType10_Death1.mat",
            materialsFolder + "mat_ColiType11_Death2.mat",
            materialsFolder + "mat_ColiType12_Death3.mat",
            materialsFolder + "mat_ColiType13_Death4.mat",
        };

        // For object colliders
        private static Material triMaterial = null;
        private static Material quadMaterial = null;
        private static Material[] staticColliderTypeMaterials = new Material[14];

        private static Material TriMaterial
        {
            get
            {
                if (triMaterial == null)
                    triMaterial = AssetDatabase.LoadAssetAtPath<Material>(triMaterialAsset);
                return triMaterial;
            }
        }
        private static Material QuadMaterial
        {
            get
            {
                if (quadMaterial == null)
                    quadMaterial = AssetDatabase.LoadAssetAtPath<Material>(quadMaterialAsset);
                return triMaterial;
            }
        }
        private static Material[] StaticColliderTypeMaterials
        {
            get
            {
                for (int i = 0; i < staticColliderTypeMaterials.Length; i++)
                {
                    if (staticColliderTypeMaterials[i] is null)
                        staticColliderTypeMaterials[i] = AssetDatabase.LoadAssetAtPath<Material>(StaticColliderTypeMaterialAssets[i]);
                }
                return staticColliderTypeMaterials;
            }
        }



        [MenuItem(GfzMenuItems.Colliders.Import, priority = GfzMenuItems.Colliders.ImportPriority)]
        public static void Import()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceStageDirectory;
            var filePaths = Directory.GetFiles(inputPath, "COLI_COURSE???", SearchOption.TopDirectoryOnly);
            var sceneIterator = BinarySerializableIO.LoadFile<Scene>(filePaths);

            var outputPath = DirectoryUtility.GetTopDirectory(settings.SourceDirectory);
            var fullOutputPath = $"Assets/{outputPath}/stage/";
            AssetDatabaseUtility.CreateDirectory(fullOutputPath);

            foreach (Scene scene in sceneIterator)
            {
                CreateColliders(scene, fullOutputPath, settings.CreateColliderBackfaces);
            }
        }

        [MenuItem(GfzMenuItems.Colliders.Import256, priority = GfzMenuItems.Colliders.Import256Priority)]
        public static void Import256()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceStageDirectory;
            var outputPath = DirectoryUtility.GetTopDirectory(settings.SourceDirectory);
            var fullOutputPath = $"Assets/{outputPath}/stage/";
            AssetDatabaseUtility.CreateDirectory(fullOutputPath);

            var sceneIndex = settings.Collider256SceneIndex;
            var filePath = $"{inputPath}COLI_COURSE{sceneIndex:00}";
            var scene = BinarySerializableIO.LoadFile<Scene>(filePath);

            CreateStaticColliders256(scene, fullOutputPath, settings.CreateColliderBackfaces, settings.Collider256MeshType);

            ProgressBar.Clear();
        }

        public static void CreateColliders(Scene scene, string outputDirectory, bool createBackfaces)
        {
            // Get scene, assert validity
            Assert.IsTrue(scene.IsValidFile);

            // Todo: load materials here once
            //

            // Then these functions build the assets
            CreateObjectColliders(scene, outputDirectory, createBackfaces);
            CreateStaticColliders(scene, outputDirectory, createBackfaces);

            ProgressBar.Clear();
        }

        public static void CreateObjectColliders(Scene scene, string outputDirectory, bool createBackfaces)
        {
            int count = 0;
            int total = scene.dynamicSceneObjects.Length;
            foreach (var sceneObject in scene.dynamicSceneObjects)
            {
                count++;
                if (sceneObject.SceneObject.ColliderGeometryPtr.IsNotNull)
                {
                    string meshName = sceneObject.Name;
                    string outputPathBase = $"{outputDirectory}st{scene.CourseIndex:00}/";
                    ProgressBar.ShowIndexed(count, total, "Importing Object Colliders", $"st{scene.CourseIndex:00} {meshName}");

                    // Create mesh
                    var mesh = CreateObjectColliderMesh(sceneObject, createBackfaces);

                    // Save mesh to Asset Database
                    var assetPath = $"{outputPathBase}coli_{meshName}.asset";
                    AssetDatabase.CreateAsset(mesh, assetPath);

                    // Create mesh prefab
                    var materials = new Material[] { TriMaterial, QuadMaterial };
                    var prefabPath = $"{outputPathBase}/pf_{meshName}.prefab";
                    var prefab = GmaImport.CreatePrefabFromModel(mesh, materials, prefabPath);

                    // Edit then save again
                    var script = prefab.AddComponent<GfzObjectColliderMesh>();
                    script.ColliderMesh = script.GetComponent<MeshFilter>();
                    PrefabUtility.SavePrefabAsset(prefab);
                }
            }
        }

        public static void CreateStaticColliders(Scene scene, string outputDirectory, bool createBackfaces)
        {
            var meshes = CreateStaticColliderMeshes(scene, createBackfaces);
            for (int meshIndex = 0; meshIndex < meshes.Length; meshIndex++)
            {
                var mesh = meshes[meshIndex];
                var meshName = mesh.name;
                string outputPathBase = $"{outputDirectory}st{scene.CourseIndex:00}/";
                ProgressBar.ShowIndexed(meshIndex, meshes.Length, "Importing Static Colliders", $"st{scene.CourseIndex:00} {meshName}");

                // HACK: assign material to each static surface type mesh
                var materials = new Material[mesh.subMeshCount];
                for (int matIndex = 0; matIndex < materials.Length; matIndex++)
                {
                    materials[matIndex] = StaticColliderTypeMaterials[meshIndex];
                }

                // Save mesh to Asset Database.
                var meshPath = $"{outputPathBase}/coli_{meshName}_{(StaticColliderMeshProperty)meshIndex}.asset";
                AssetDatabase.CreateAsset(mesh, meshPath);

                // Create mesh prefab
                var prefabPath = $"{outputPathBase}/pf_{meshName}_{(StaticColliderMeshProperty)meshIndex}.prefab";
                var prefab = GmaImport.CreatePrefabFromModel(mesh, materials, prefabPath);
                // TODO: anything to tag prefab with?
            }
        }

        public static void CreateStaticColliders256(Scene scene, string outputDirectory, bool createBackfaces, StaticColliderMeshProperty type)
        {
            // The lesson I learned is that generating 14 * 256 meshes AND prefabs is a bad idea.
            string baseMeshName = $"st{scene.CourseIndex:00}_{(int)type:00}_{type}";
            var meshes = CreateStaticColliderMeshes256(scene, type, createBackfaces, baseMeshName);
            string outputPathBase = $"{outputDirectory}st{scene.CourseIndex:00}/{baseMeshName}";
            AssetDatabaseUtility.CreateDirectory(outputPathBase);

            var material = StaticColliderTypeMaterials[(int)type];
            for (int meshIndex = 0; meshIndex < meshes.Length; meshIndex++)
            {
                // get mesh
                var mesh = meshes[meshIndex];
                var materials = new Material[mesh.subMeshCount];
                for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
                    materials[subMeshIndex] = material;
                var meshName = mesh.name;

                var cancel = ProgressBar.ShowIndexed(meshIndex, meshes.Length, "Importing Static Colliders", $"st{scene.CourseIndex:00} {meshName}");
                if (cancel) break;

                // Save mesh to Asset Database
                var meshPath = $"{outputPathBase}/coli_{meshName}.asset";
                AssetDatabase.CreateAsset(mesh, meshPath);

                // Create mesh prefab
                var prefabPath = $"{outputPathBase}/pf_{meshName}.prefab";
                var prefab = GmaImport.CreatePrefabFromModel(mesh, materials, prefabPath);
            }
        }

        public static Mesh[] CreateStaticColliderMeshes256(Scene scene, StaticColliderMeshProperty meshSurfaceType, bool createBackfaces, string baseMeshName)
        {
            // Simplify access to tris/quads
            var scm = scene.staticColliderMeshManager;
            var colliderTriangles = scm.ColliderTris;
            var colliderQuads = scm.ColliderQuads;

            //
            const int nLists = StaticColliderMeshGrid.kListCount;
            var meshes = new Mesh[nLists];

            //
            int meshSurfaceTypeIndex = (int)meshSurfaceType;

            // Get triangle information for the current mesh type
            var triIndex16x16 = scm.TriMeshGrids[meshSurfaceTypeIndex];
            var triIndexLists = triIndex16x16.IndexLists;
            Assert.IsTrue(triIndexLists.Length == 0 || triIndexLists.Length == StaticColliderMeshGrid.kListCount);

            //
            var quadMeshIndexes = scm.QuadMeshGrids[meshSurfaceTypeIndex];
            var quadIndexLists = quadMeshIndexes.IndexLists;
            Assert.IsTrue(quadIndexLists.Length == 0 || quadIndexLists.Length == StaticColliderMeshGrid.kListCount);

            //
            for (int listIndex = 0; listIndex < nLists; listIndex++)
            {
                // Create base data for mesh for EACH mesh type (boost, heal, etc)
                var mesh = new Mesh()
                {
                    name = $"{baseMeshName}.{listIndex:000}_{meshSurfaceType}",
                };

                // Get triangle indexes, get traingles from array using indexes, create submesh, then assign it.
                var triIndexList = triIndexLists[listIndex];
                var colliderTriangleSubset = GetIndexes(colliderTriangles, triIndexList.Indexes);
                var trianglesSubmesh = CreateTriSubmeshForMesh(mesh, colliderTriangleSubset, createBackfaces);

                // Get quad indexes, get traingles from array using indexes, create submesh, then assign it.
                var quadIndexList = quadIndexLists[listIndex];
                var colliderQuadSubset = GetIndexes(colliderQuads, quadIndexList.Indexes);
                var quadSubmesh = CreateQuadSubmeshForMesh(mesh, colliderQuadSubset, createBackfaces);

                //
                var submeshes = new SubMeshDescriptor[]
                {
                        trianglesSubmesh,
                        quadSubmesh,
                };
                // Set each submesh in the mesh
                mesh.subMeshCount = submeshes.Length;
                for (int submeshIndex = 0; submeshIndex < submeshes.Length; submeshIndex++)
                {
                    mesh.SetSubMesh(submeshIndex, submeshes[submeshIndex], MeshUpdateFlags.Default);
                }

                // Compute other Mesh data
                mesh.RecalculateBounds();
                // Assign mesh to mesh array
                meshes[listIndex] = mesh;
            }

            return meshes;
        }


        /// <summary>
        /// Builds a mesh for each static collider surface type for the given <paramref name="scene"/>.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="createBackfaces"></param>
        /// <returns></returns>
        public static Mesh[] CreateStaticColliderMeshes(Scene scene, bool createBackfaces)
        {
            // Get number of mesh. AX and GX differ in count.
            var meshCount = scene.staticColliderMeshManager.SurfaceCount;
            var meshes = new Mesh[meshCount];

            // Simplify access to tris/quads
            var scm = scene.staticColliderMeshManager;
            var colliderTriangles = scm.ColliderTris;
            var colliderQuads = scm.ColliderQuads;

            // Remember which index is used for what
            const int triSubmeshIndex = 0;
            const int quadSubmeshIndex = 1;

            // Iterate over each table index
            for (int meshSurfaceType = 0; meshSurfaceType < meshCount; meshSurfaceType++)
            {
                // Create base data for mesh
                // This is done for EACH mesh type (boost, heal, etc)
                var mesh = new Mesh()
                {
                    name = $"st{scene.CourseIndex:00}_{meshSurfaceType:00}",
                };
                // One submesh for tris, one for quads
                var submeshes = new SubMeshDescriptor[2];

                // Create triangle mesh from unique triangles
                var triIndexLists = scm.TriMeshGrids[meshSurfaceType].IndexLists;
                var triUniqueIndexes = GetUniqueIndexes(triIndexLists);
                var colliderTrianglesSubset = GetIndexes(colliderTriangles, triUniqueIndexes);
                var trianglesSubmesh = CreateTriSubmeshForMesh(mesh, colliderTrianglesSubset, createBackfaces);
                submeshes[triSubmeshIndex] = trianglesSubmesh;

                // Create triangle mesh from unique quads (quads are interpreted as triangles)
                var quadIndexLists = scm.QuadMeshGrids[meshSurfaceType].IndexLists;
                var quadUniqueIndexes = GetUniqueIndexes(quadIndexLists);
                var colliderQuadsSubset = GetIndexes(colliderQuads, quadUniqueIndexes);
                var quadSubmesh = CreateQuadSubmeshForMesh(mesh, colliderQuadsSubset, createBackfaces);
                submeshes[quadSubmeshIndex] = quadSubmesh;

                // Set each submesh in the mesh
                mesh.subMeshCount = submeshes.Length;
                for (int submeshIndex = 0; submeshIndex < submeshes.Length; submeshIndex++)
                {
                    mesh.SetSubMesh(submeshIndex, submeshes[submeshIndex], MeshUpdateFlags.Default);
                }

                // Compute other Mesh data
                mesh.RecalculateBounds();
                // Assign mesh to mesh array
                meshes[meshSurfaceType] = mesh;
            }

            return meshes;
        }

        public static SubMeshDescriptor CreateTriSubmeshForMesh(Mesh mesh, ColliderTriangle[] triangles, bool addBackfaces)
        {
            int nTriangles = triangles is null ? 0 : triangles.Length;
            var allVertices = new List<Vector3>();
            var allNormals = new List<Vector3>();
            var allIndexes = new List<int>();

            // Iterate over each triangle
            for (int triIndex = 0; triIndex < nTriangles; triIndex++)
            {
                // Get base data
                var triangle = triangles[triIndex];
                var vertices = triangle.GetVerts();
                var vertTotal = vertices.Length;

                // Iterate over each vertex (thus, 3 times per triangle)
                for (int vertexIndex = 0; vertexIndex < vertTotal; vertexIndex++)
                {
                    allVertices.Add(vertices[vertexIndex]);
                    allIndexes.Add(triIndex * vertTotal + vertexIndex);
                    allNormals.Add(triangle.Normal);
                }
            }

            // Option to add backfacing triangles
            if (addBackfaces)
            {
                var backfaceIndexes = allIndexes.ToArray();
                Array.Reverse(backfaceIndexes);
                allIndexes.AddRange(backfaceIndexes);
            }

            // Build submesh
            var triSubmesh = new SubMeshDescriptor();
            triSubmesh.baseVertex = mesh.vertexCount;
            triSubmesh.firstVertex = mesh.vertexCount;
            triSubmesh.indexCount = allIndexes.Count;
            triSubmesh.indexStart = mesh.triangles.Length;
            triSubmesh.topology = MeshTopology.Triangles;
            triSubmesh.vertexCount = allVertices.Count;

            // Append to mesh
            var verticesConcat = mesh.vertices.Concat(allVertices).ToArray();
            var normalsConcat = mesh.normals.Concat(allNormals).ToArray();
            var trianglesConcat = mesh.triangles.Concat(allIndexes).ToArray();

            // Assign values to mesh
            mesh.vertices = verticesConcat;
            mesh.normals = normalsConcat;
            mesh.triangles = trianglesConcat;

            // Return submesh
            return triSubmesh;
        }

        public static SubMeshDescriptor CreateQuadSubmeshForMesh(Mesh mesh, ColliderQuad[] colliderQuads, bool addBackfaces)
        {
            int nColliderQuads = colliderQuads is null ? 0 : colliderQuads.Length;
            var allVertices = new List<Vector3>();
            var allNormals = new List<Vector3>();
            var allIndexes = new List<int>();

            // Iterate over each quad
            for (int quadIndex = 0; quadIndex < nColliderQuads; quadIndex++)
            {
                // Get base data
                var quad = colliderQuads[quadIndex];
                var vertices = quad.GetVerts();
                var vertTotal = 6; // 2 triangles, 3 verts each

                // Add vertices for quad as 2 triangles
                allVertices.Add(vertices[0]);
                allVertices.Add(vertices[1]);
                allVertices.Add(vertices[2]);
                allVertices.Add(vertices[2]);
                allVertices.Add(vertices[3]);
                allVertices.Add(vertices[0]);

                // Iterate over each vertex
                for (int vertexIndex = 0; vertexIndex < vertTotal; vertexIndex++)
                {
                    allIndexes.Add(quadIndex * vertTotal + vertexIndex);
                    allNormals.Add(quad.Normal);
                }
            }

            // Option to add backfacing triangles
            if (addBackfaces)
            {
                var backfaceIndexes = allIndexes.ToArray();
                Array.Reverse(backfaceIndexes);
                allIndexes.AddRange(backfaceIndexes);
            }

            // Build submesh
            var quadSubmesh = new SubMeshDescriptor();
            quadSubmesh.baseVertex = mesh.vertexCount;
            quadSubmesh.firstVertex = mesh.vertexCount;
            quadSubmesh.indexCount = allIndexes.Count;
            quadSubmesh.indexStart = mesh.triangles.Length;
            quadSubmesh.topology = MeshTopology.Triangles;
            quadSubmesh.vertexCount = allVertices.Count;

            //
            Assert.IsTrue(allVertices.Count == allNormals.Count, $"v:{allVertices.Count} != n:{allNormals.Count}");

            // Append to mesh
            var verticesConcat = mesh.vertices.Concat(allVertices).ToArray();
            var normalsConcat = mesh.normals.Concat(allNormals).ToArray();
            var trianglesConcat = mesh.triangles.Concat(allIndexes).ToArray();

            // Assign values to mesh
            mesh.vertices = verticesConcat;
            mesh.normals = normalsConcat;
            mesh.triangles = trianglesConcat;

            // 
            return quadSubmesh;
        }

        public static Mesh CreateObjectColliderMesh(SceneObjectDynamic sceneObject, bool createBackfaces)
        {
            // Get collider
            var colliderGeo = sceneObject.SceneObject.ColliderMesh;

            // Create base data for mesh
            var mesh = new Mesh();
            mesh.name = sceneObject.Name;

            // Get triangles and quads of this collider
            var trisSubmesh = CreateTriSubmeshForMesh(mesh, colliderGeo.Tris, createBackfaces);
            var quadsSubmesh = CreateQuadSubmeshForMesh(mesh, colliderGeo.Quads, createBackfaces);
            var submeshes = new SubMeshDescriptor[]
            {
                trisSubmesh,
                quadsSubmesh,
            };

            // Set each submesh in the mesh
            mesh.subMeshCount = submeshes.Length;
            for (int i = 0; i < submeshes.Length; i++)
            {
                mesh.SetSubMesh(i, submeshes[i], MeshUpdateFlags.Default);
            }

            // Compute other Mesh data
            mesh.RecalculateBounds();
            //mesh.RecalculateNormals();

            return mesh;
        }

        /// <summary>
        /// Retrieve all <paramref name="array"/> elements using the <paramref name="indexList"/> indexes provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="indexList"></param>
        /// <returns></returns>
        public static T[] GetIndexes<T>(T[] array, ushort[] indexList)
        {
            var indexedArray = new T[indexList.Length];
            for (int i = 0; i < indexList.Length; i++)
            {
                int index = indexList[i];
                indexedArray[i] = array[index];
            }
            return indexedArray;
        }

        /// <summary>
        /// Collects all unique indexes of triangle or quad list.
        /// </summary>
        /// <param name="indexLists"></param>
        /// <returns></returns>
        public static ushort[] GetUniqueIndexes(IndexList[] indexLists)
        {
            var list = new List<ushort>();
            foreach (var indexList in indexLists)
            {
                foreach (var index in indexList.Indexes)
                {
                    if (!list.Contains(index))
                    {
                        list.Add(index);
                    }
                }
            }
            return list.ToArray();
        }


    }
}
