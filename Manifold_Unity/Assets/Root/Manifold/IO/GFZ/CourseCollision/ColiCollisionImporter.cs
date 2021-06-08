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
        #region MEMBERS

        [Header("Import Settings")]
        [SerializeField, BrowseFolderField()]
        protected string importFrom;

        [SerializeField, BrowseFolderField("Assets/")]
        protected string importTo;

        [SerializeField]
        protected IOOption importOption = IOOption.selectedFiles;

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

        #endregion

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
                    int total = scene.sceneObjects.Length;
                    int count = 0;
                    foreach (var sceneObject in scene.sceneObjects)
                    {
                        count++;

                        if (sceneObject.instanceReference.colliderGeometryPtr.IsNotNullPointer)
                        {
                            var meshName = sceneObject.nameCopy;
                            ImportUtility.ProgressBar<SceneInstanceReference>(count, total, $"st{sceneSobj.Value.ID:00} {meshName}");

                            // Create mesh
                            var mesh = CreateObjectColliderMesh(sceneObject);

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
                            prefab.AddComponent<ColliderObjectTag>();
                            PrefabUtility.SavePrefabAsset(prefab);
                        }
                    }
                }

                // Create static scene colliders
                {
                    //var meshes = CreateStaticColliderMeshes512(sceneSobj);
                    var meshes = CreateStaticColliderMeshes(sceneSobj);
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
                        ImportUtility.ProgressBar<SceneInstanceReference>(count, total, $"st{sceneSobj.Value.ID:00} {meshName}");

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

            ImportUtility.ProgressBar<SceneInstanceReference>(1, 1, $"Saving assets...");
            ImportUtility.FinalizeAssetImport();
        }

        public static Mesh[] CreateStaticColliderMeshes512(ColiSceneSobj sceneSobj)
        {
            // Get scene, assert validity
            var scene = sceneSobj.Value;
            Assert.IsTrue(scene.IsValidFile);

            // Get number of mesh. AX and GX differ in count.
            var meshCount = StaticColliderMeshes.GetSurfacesCount(scene);
            var meshes = new Mesh[meshCount];

            // Simplify access to tris/quads
            var scm = scene.staticColliderMeshes;
            var colliderTriangles = scm.colliderTriangles;
            var colliderQuads = scm.colliderQuads;

            //
            const int nLists = StaticColliderMeshTable16x16.kListCount;
            const int triBaseIndex = 0;
            const int quadBaseIndex = triBaseIndex + nLists;

            // Iterate over each table index
            for (int meshSurfaceType = 0; meshSurfaceType < meshCount; meshSurfaceType++)
            {
                // Create base data for mesh for EACH mesh type (boost, heal, etc)
                var mesh = new Mesh()
                {
                    name = $"st{scene.ID:00}_{meshSurfaceType:00}_{(StaticMeshColliderProperty)meshSurfaceType}",
                };

                // Each tri/quad set has fixed size 256 each, so 512 total
                var submeshes = new SubMeshDescriptor[nLists * 2];

                // Get triangle information for the current mesh type
                var triIndex16x16 = scm.triMeshIndexTables[meshSurfaceType];
                var triIndexLists = triIndex16x16.indexLists;
                Assert.IsTrue(triIndexLists.Length == 0 || triIndexLists.Length == StaticColliderMeshTable16x16.kListCount);
                // meshIndex = 0-255, change to double for loop for x for y?
                for (int index = 0; index < triIndexLists.Length; index++)
                {
                    // Get triangle indexes, get traingles from array using indexes, create submesh, then assign it.
                    var triIndexList = triIndexLists[index];
                    var colliderTriangleSubset = GetIndexes(colliderTriangles, triIndexList.Indexes);
                    var trianglesSubmesh = CreateTriSubmeshForMesh(mesh, colliderTriangleSubset);
                    //
                    int submeshIndex = triBaseIndex + index;
                    submeshes[submeshIndex] = trianglesSubmesh;
                }


                var quadMeshIndexes = scm.quadMeshIndexTables[meshSurfaceType];
                var quadIndexLists = quadMeshIndexes.indexLists;
                Assert.IsTrue(quadIndexLists.Length == 0 || quadIndexLists.Length == StaticColliderMeshTable16x16.kListCount);
                // meshIndex = 0-255
                for (int index = 0; index < quadIndexLists.Length; index++)
                {
                    // Get quad indexes, get traingles from array using indexes, create submesh, then assign it.
                    var quadIndexList = quadIndexLists[index];
                    var colliderQuadSubset = GetIndexes(colliderQuads, quadIndexList.Indexes);
                    var quadSubmesh = CreateQuadSubmeshForMesh(mesh, colliderQuadSubset);
                    //
                    int submeshIndex = quadBaseIndex + index;
                    submeshes[submeshIndex] = quadSubmesh;
                }


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

        public static Mesh[] CreateStaticColliderMeshes(ColiSceneSobj sceneSobj)
        {
            // Get scene, assert validity
            var scene = sceneSobj.Value;
            Assert.IsTrue(scene.IsValidFile);

            // Get number of mesh. AX and GX differ in count.
            var meshCount = StaticColliderMeshes.GetSurfacesCount(scene);
            var meshes = new Mesh[meshCount];

            // Simplify access to tris/quads
            var scm = scene.staticColliderMeshes;
            var colliderTriangles = scm.colliderTriangles;
            var colliderQuads = scm.colliderQuads;

            //
            const int triSubmeshIndex = 0;
            const int quadSubmeshIndex = 1;
            const bool createBackfaces = true;

            // Iterate over each table index
            for (int meshSurfaceType = 0; meshSurfaceType < meshCount; meshSurfaceType++)
            {
                // Create base data for mesh for EACH mesh type (boost, heal, etc)
                var mesh = new Mesh()
                {
                    name = $"st{scene.ID:00}_{meshSurfaceType:00}_{(StaticMeshColliderProperty)meshSurfaceType}",
                };
                // one submesh for tris, one for quads
                var submeshes = new SubMeshDescriptor[2];

                // Create triangle mesh from unique triangles
                var triIndexLists = scm.triMeshIndexTables[meshSurfaceType].indexLists;
                var triUniqueIndexes = GetUniqueIndexes(triIndexLists);
                var colliderTrianglesSubset = GetIndexes(colliderTriangles, triUniqueIndexes);
                var trianglesSubmesh = CreateTriSubmeshForMesh(mesh, colliderTrianglesSubset, createBackfaces);
                submeshes[triSubmeshIndex] = trianglesSubmesh;

                // Create triangle mesh from unique quads (quads are interpreted as triangles)
                var quadIndexLists = scm.quadMeshIndexTables[meshSurfaceType].indexLists;
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

        public static SubMeshDescriptor CreateTriSubmeshForMesh(Mesh mesh, ColliderTriangle[] triangles, bool addBackfaces = false)
        {
            var allVertices = new List<Vector3>();
            var allNormals = new List<Vector3>();
            var allIndexes = new List<int>();

            // Iterate over each triangle
            for (int triIndex = 0; triIndex < triangles.Length; triIndex++)
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

        public static SubMeshDescriptor CreateQuadSubmeshForMesh(Mesh mesh, ColliderQuad[] colliderQuads, bool addBackfaces = false)
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

                // Add vertices for quad as 2 triangles
                allVertices.Add(quad.vertex0);
                allVertices.Add(quad.vertex1);
                allVertices.Add(quad.vertex2);
                allVertices.Add(quad.vertex2);
                allVertices.Add(quad.vertex3);
                allVertices.Add(quad.vertex0);

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

        public Mesh CreateObjectColliderMesh(SceneObject sceneObject)
        {
            var collision = sceneObject.instanceReference.colliderGeometry;

            // Create base data for mesh
            var mesh = new Mesh();
            mesh.name = sceneObject.nameCopy;
            var submeshes = new SubMeshDescriptor[2];

            // TRIS
            {
                var vertCount = collision.triCount * 3;
                var vertices = new Vector3[vertCount];
                var indices = new int[vertCount];
                //
                for (int i = 0; i < collision.triCount; i++)
                {
                    // Offset for each iteration through
                    var triStride = i * 3;
                    // Set vertices
                    vertices[triStride + 0] = collision.tris[i].vertex0;
                    vertices[triStride + 1] = collision.tris[i].vertex1;
                    vertices[triStride + 2] = collision.tris[i].vertex2;
                    // Set indexes
                    indices[triStride + 0] = triStride + 0;
                    indices[triStride + 1] = triStride + 1;
                    indices[triStride + 2] = triStride + 2;
                }

                // Build submesh
                var triSubmesh = new SubMeshDescriptor();
                triSubmesh.baseVertex = mesh.vertexCount; // 0
                triSubmesh.firstVertex = mesh.vertexCount; // 0
                triSubmesh.indexCount = indices.Length;
                triSubmesh.indexStart = mesh.triangles.Length; // 0
                triSubmesh.topology = MeshTopology.Triangles;
                triSubmesh.vertexCount = vertices.Length;

                // Append to mesh
                var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                var trianglesConcat = mesh.triangles.Concat(indices).ToArray();

                // Assign values to mesh
                mesh.vertices = verticesConcat;
                mesh.triangles = trianglesConcat;

                // Set mesh to use submesh 0 for triangles
                submeshes[0] = triSubmesh;
            }

            // QUADS
            {
                var quadCount = collision.quadCount;
                var vertCount = quadCount * 4;
                var vertices = new Vector3[vertCount];
                var indices = new int[quadCount * 6];

                for (int i = 0; i < quadCount; i++)
                {
                    var vertexStride = i * 4;
                    vertices[vertexStride + 0] = collision.quads[i].vertex0;
                    vertices[vertexStride + 1] = collision.quads[i].vertex1;
                    vertices[vertexStride + 2] = collision.quads[i].vertex2;
                    vertices[vertexStride + 3] = collision.quads[i].vertex3;

                    var indexStride = i * 6;
                    indices[indexStride + 0] = vertexStride + 0;
                    indices[indexStride + 1] = vertexStride + 1;
                    indices[indexStride + 2] = vertexStride + 2;
                    indices[indexStride + 3] = vertexStride + 0;
                    indices[indexStride + 4] = vertexStride + 2;
                    indices[indexStride + 5] = vertexStride + 3;
                }

                // Build submesh
                var quadSubmesh = new SubMeshDescriptor();
                quadSubmesh.baseVertex = mesh.vertexCount;
                quadSubmesh.firstVertex = mesh.vertexCount;
                quadSubmesh.indexCount = indices.Length;
                quadSubmesh.indexStart = mesh.triangles.Length;
                quadSubmesh.topology = MeshTopology.Triangles;
                quadSubmesh.vertexCount = vertices.Length;

                // Append to mesh
                var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                var trianglesConcat = mesh.triangles.Concat(indices).ToArray();

                // Assign values to mesh
                mesh.vertices = verticesConcat;
                mesh.triangles = trianglesConcat;

                // Set mesh to use submesh 1 for triangles
                submeshes[1] = quadSubmesh;
            }

            // Set each submesh in the mesh
            mesh.subMeshCount = submeshes.Length;
            for (int i = 0; i < submeshes.Length; i++)
            {
                mesh.SetSubMesh(i, submeshes[i], MeshUpdateFlags.Default);
            }

            // Compute other Mesh data
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

    }
}