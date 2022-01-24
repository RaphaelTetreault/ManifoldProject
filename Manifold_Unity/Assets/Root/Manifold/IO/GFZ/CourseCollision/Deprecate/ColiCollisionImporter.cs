using GameCube.GFZ.CourseCollision;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manifold.IO.GFZ.CourseCollision
{
    [CreateAssetMenu(menuName = Const.Menu.GfzCourseCollision + "COLI Collision Importer")]
    public class ColiCollisionImporter : ExecutableScriptableObject,
        IImportable
    {
        // FIELDS
        [Header("Import Settings")]
        [SerializeField, BrowseFolderField()]
        protected string importFrom;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importTo;

        [SerializeField]
        protected IOOption importOption = IOOption.selectedFiles;

        [Header("Mesh Generation Options")]
        [SerializeField]
        protected bool usePrecomputes = false;
        [SerializeField]
        protected bool createBackfaces = true;
        [SerializeField]
        protected bool createMesh256OfType = false;
        [SerializeField]
        protected StaticColliderMeshProperty type = StaticColliderMeshProperty.recover;


        [Header("Mesh Materials")]
        [SerializeField]
        protected Material triMaterial;

        [SerializeField]
        protected Material quadMaterial;

        [SerializeField]
        protected Material[] triMaterials;

        [SerializeField]
        protected Material[] quadMaterials;

        [Header("Import Files")]
        [SerializeField] protected ColiSceneSobj[] sceneSobjs;


        public override string ExecuteText => "Import COLI Object Collision";

        public override void Execute() => Import();

        public void Import()
        {
            sceneSobjs = AssetDatabaseUtility.GetSobjByOption(sceneSobjs, importOption, importFrom);

            foreach (var sceneSobj in sceneSobjs)
            {
                var scene = sceneSobj.Value;

                // Create object-based collider meshes
                {
                    int total = scene.dynamicSceneObjects.Length;
                    int count = 0;
                    foreach (var sceneObject in scene.dynamicSceneObjects)
                    {
                        count++;

                        if (sceneObject.sceneObject.colliderGeometryPtr.IsNotNullPointer)
                        {
                            var meshName = sceneObject.Name;
                            ImportUtility.ProgressBar<SceneObject>(count, total, $"st{sceneSobj.Value.ID:00} {meshName}");

                            // Create mesh
                            var mesh = CreateObjectColliderMesh(sceneObject, createBackfaces, usePrecomputes);

                            // Save mesh to Asset Database
                            var assetPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/coli_{meshName}.asset";
                            AssetDatabase.CreateAsset(mesh, assetPath);

                            // Refresh instance reference
                            // IMPORTANT! Sometimes reference gets lost.
                            mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

                            // Create mesh prefab
                            var materials = new Material[] { triMaterial, quadMaterial };
                            var prefabPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/pf_{meshName}.prefab";
                            var prefab = ImportUtility.CreatePrefabFromModel(mesh, materials, prefabPath);

                            // Edit then save again
                            var script = prefab.AddComponent<GfzObjectColliderMesh>();
                            script.ColliderMesh = script.GetComponent<MeshFilter>();
                            PrefabUtility.SavePrefabAsset(prefab);
                        }
                    }
                }

                // Create static scene colliders
                {
                    var meshes = CreateStaticColliderMeshes(sceneSobj, createBackfaces);
                    int total = meshes.Length;
                    int count = 0;

                    for (int meshIndex = 0; meshIndex < meshes.Length; meshIndex++)
                    {
                        var mesh = meshes[meshIndex];

                        var half = mesh.subMeshCount / 2;
                        var materials = new Material[mesh.subMeshCount];
                        for (int matIndex = 0; matIndex < materials.Length; matIndex++)
                        {
                            materials[matIndex] = matIndex < half
                                ? triMaterials[meshIndex]
                                : quadMaterials[meshIndex];
                        }

                        count++;
                        var meshName = mesh.name;
                        ImportUtility.ProgressBar<SceneObject>(count, total, $"st{sceneSobj.Value.ID:00} {meshName}");

                        // Save mesh to Asset Database
                        var meshPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/coli_{meshName}.asset";
                        if (AssetDatabase.LoadAssetAtPath<Mesh>(meshPath) != null)
                            AssetDatabase.DeleteAsset(meshPath);
                        AssetDatabase.CreateAsset(mesh, meshPath);

                        // Refresh instance reference
                        // IMPORTANT! Sometimes reference gets lost.
                        mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);

                        // Create mesh prefab
                        var prefabPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/pf_{meshName}.prefab";
                        var prefab = ImportUtility.CreatePrefabFromModel(mesh, materials, prefabPath);
                    }
                }

                // The lesson I learned is that generating 14 * 256 meshes AND prefabs is a bad idea.
                //The current format create 256 per run.
                if (createMesh256OfType)
                {
                    var meshes = CreateStaticColliderMeshes256(sceneSobj, type, createBackfaces);
                    int total = meshes.Length;
                    int count = 0;

                    var material = triMaterials[(int)type];

                    for (int meshIndex = 0; meshIndex < meshes.Length; meshIndex++)
                    {
                        // get mesh
                        var mesh = meshes[meshIndex];
                        var materials = new Material[mesh.subMeshCount];
                        for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
                             materials[subMeshIndex] = material;

                        count++;
                        var meshName = mesh.name;
                        ImportUtility.ProgressBar<SceneObject>(count, total, $"st{sceneSobj.Value.ID:00} {meshName}");

                        // Save mesh to Asset Database
                        var meshPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/coli_{meshName}.asset";
                        if (AssetDatabase.LoadAssetAtPath<Mesh>(meshPath) != null)
                            AssetDatabase.DeleteAsset(meshPath);
                        AssetDatabase.CreateAsset(mesh, meshPath);

                        // Refresh instance reference
                        // IMPORTANT! Sometimes reference gets lost.
                        mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);

                        // Create mesh prefab
                        var prefabPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/pf_{meshName}.prefab";
                        var prefab = ImportUtility.CreatePrefabFromModel(mesh, materials, prefabPath);
                    }
                }
            }

            ImportUtility.ProgressBar<SceneObject>(1, 1, $"Saving assets...");
            ImportUtility.FinalizeAssetImport();
        }

        public static Mesh[] CreateStaticColliderMeshes256(ColiSceneSobj sceneSobj, StaticColliderMeshProperty meshSurfaceType, bool createBackfaces)
        {
            // Get scene, assert validity
            var scene = sceneSobj.Value;
            Assert.IsTrue(scene.IsValidFile);

            // Simplify access to tris/quads
            var scm = scene.staticColliderMap;
            var colliderTriangles = scm.colliderTris;
            var colliderQuads = scm.colliderQuads;

            //
            const int nLists = StaticColliderMeshMatrix.kListCount;
            var meshes = new Mesh[nLists];

            //
            int meshSurfaceTypeIndex = (int)meshSurfaceType;

            // Get triangle information for the current mesh type
            var triIndex16x16 = scm.triMeshMatrices[meshSurfaceTypeIndex];
            var triIndexLists = triIndex16x16.indexLists;
            Assert.IsTrue(triIndexLists.Length == 0 || triIndexLists.Length == StaticColliderMeshMatrix.kListCount);

            //
            var quadMeshIndexes = scm.quadMeshMatrices[meshSurfaceTypeIndex];
            var quadIndexLists = quadMeshIndexes.indexLists;
            Assert.IsTrue(quadIndexLists.Length == 0 || quadIndexLists.Length == StaticColliderMeshMatrix.kListCount);

            //
            for (int listIndex = 0; listIndex < nLists; listIndex++)
            {
                // Create base data for mesh for EACH mesh type (boost, heal, etc)
                var mesh = new Mesh()
                {
                    name = $"st{scene.ID:00}_{meshSurfaceTypeIndex:00}.{listIndex:000}_{meshSurfaceType}",
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

        public static Mesh[] CreateStaticColliderMeshes(ColiSceneSobj sceneSobj, bool createBackfaces)
        {
            // Get scene, assert validity
            var scene = sceneSobj.Value;
            Assert.IsTrue(scene.IsValidFile);

            // Get number of mesh. AX and GX differ in count.
            var meshCount = scene.staticColliderMap.SurfaceCount;
            var meshes = new Mesh[meshCount];

            // Simplify access to tris/quads
            var scm = scene.staticColliderMap;
            var colliderTriangles = scm.colliderTris;
            var colliderQuads = scm.colliderQuads;

            //
            const int triSubmeshIndex = 0;
            const int quadSubmeshIndex = 1;

            // Iterate over each table index
            for (int meshSurfaceType = 0; meshSurfaceType < meshCount; meshSurfaceType++)
            {
                // Create base data for mesh for EACH mesh type (boost, heal, etc)
                var mesh = new Mesh()
                {
                    name = $"st{scene.ID:00}_{meshSurfaceType:00}_{(StaticColliderMeshProperty)meshSurfaceType}",
                };
                // one submesh for tris, one for quads
                var submeshes = new SubMeshDescriptor[2];

                // Create triangle mesh from unique triangles
                var triIndexLists = scm.triMeshMatrices[meshSurfaceType].indexLists;
                var triUniqueIndexes = GetUniqueIndexes(triIndexLists);
                var colliderTrianglesSubset = GetIndexes(colliderTriangles, triUniqueIndexes);
                var trianglesSubmesh = CreateTriSubmeshForMesh(mesh, colliderTrianglesSubset, createBackfaces);
                submeshes[triSubmeshIndex] = trianglesSubmesh;

                // Create triangle mesh from unique quads (quads are interpreted as triangles)
                var quadIndexLists = scm.quadMeshMatrices[meshSurfaceType].indexLists;
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

        public static SubMeshDescriptor CreateTriSubmeshForMesh(Mesh mesh, ColliderTriangle[] triangles, bool addBackfaces = false, bool usePrecompute = false)
        {
            var allVertices = new List<Vector3>();
            var allNormals = new List<Vector3>();
            var allIndexes = new List<int>();

            // Iterate over each triangle
            for (int triIndex = 0; triIndex < triangles.Length; triIndex++)
            {
                // Get base data
                var triangle = triangles[triIndex];
                var vertices = usePrecompute ? triangle.GetPrecomputes() : triangle.GetVerts();
                var vertTotal = vertices.Length;

                // Iterate over each vertex (thus, 3 times per triangle)
                for (int vertexIndex = 0; vertexIndex < vertTotal; vertexIndex++)
                {
                    allVertices.Add(vertices[vertexIndex]);
                    allIndexes.Add(triIndex * vertTotal + vertexIndex);
                    allNormals.Add(triangle.normal);
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

        public static SubMeshDescriptor CreateQuadSubmeshForMesh(Mesh mesh, ColliderQuad[] colliderQuads, bool addBackfaces = false, bool usePrecompute = false)
        {
            var allVertices = new List<Vector3>();
            var allNormals = new List<Vector3>();
            var allIndexes = new List<int>();

            // Iterate over each quad
            for (int quadIndex = 0; quadIndex < colliderQuads.Length; quadIndex++)
            {
                // Get base data
                var quad = colliderQuads[quadIndex];
                var vertTotal = 6; // 2 triangles, 3 verts each
                var verts = usePrecompute ? quad.GetPrecomputes() : quad.GetVerts();

                // Add vertices for quad as 2 triangles
                allVertices.Add(verts[0]);
                allVertices.Add(verts[1]);
                allVertices.Add(verts[2]);
                allVertices.Add(verts[2]);
                allVertices.Add(verts[3]);
                allVertices.Add(verts[0]);

                // Iterate over each vertex
                for (int vertexIndex = 0; vertexIndex < vertTotal; vertexIndex++)
                {
                    allIndexes.Add(quadIndex * vertTotal + vertexIndex);
                    allNormals.Add(quad.normal);
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

        public Mesh CreateObjectColliderMesh(SceneObjectDynamic sceneObject, bool createBackfaces, bool usePrecomputes)
        {
            var colliderGeo = sceneObject.sceneObject.colliderGeometry;

            // Create base data for mesh
            var mesh = new Mesh();
            mesh.name = sceneObject.Name;

            var trisSubmesh = CreateTriSubmeshForMesh(mesh, colliderGeo.tris, createBackfaces, usePrecomputes);
            var quadsSubmesh = CreateQuadSubmeshForMesh(mesh, colliderGeo.quads, createBackfaces, usePrecomputes);
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


    }
}