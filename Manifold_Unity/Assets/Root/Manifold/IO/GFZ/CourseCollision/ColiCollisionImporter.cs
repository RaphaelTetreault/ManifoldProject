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
                            var meshName = sceneObject.name;
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

                        //EditorUtility.SetDirty(mesh);
                        //EditorUtility.SetDirty(prefab);

                        // if things don't work, see other function
                        //mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
                        //PrefabUtility.SavePrefabAsset(prefab);
                    }
                }
            }

            ImportUtility.ProgressBar<SceneInstanceReference>(1, 1, $"Saving assets...");
            ImportUtility.FinalizeAssetImport();
        }

        public Mesh[] CreateStaticColliderMeshes(ColiSceneSobj sceneSobj)
        {
            var scene = sceneSobj.Value;

            // Create static scene colliders
            // Ensure either or (XOR), but not both
            // TODO: do this in header directly
            Assert.IsTrue(scene.header.IsFileAX ^ scene.header.IsFileGX);

            // Turn into function within StaticMeshTable (take param header)
            var surfaceTypeCount = scene.header.IsFileAX
                ? StaticMeshTable.kCountAxSurfaceTypes
                : StaticMeshTable.kCountGxSurfaceTypes;

            var meshes = new Mesh[surfaceTypeCount];

            // Simplify access to tris/quads
            var colliderTriangles = scene.surfaceAttributeMeshTable.colliderTriangles;
            var colliderQuads = scene.surfaceAttributeMeshTable.colliderQuads;


            for (int surfaceTypeIndex = 0; surfaceTypeIndex < surfaceTypeCount; surfaceTypeIndex++)
            {
                // Create base data for mesh for EACH mesh type (boost, heal, etc)
                var mesh = new Mesh();
                mesh.name = $"st{scene.ID:00}_{surfaceTypeIndex:00}_{(StaticMeshColliderProperty)surfaceTypeIndex}";
                // Each tri/quad set has fixed size 256 each, so 512 total
                var submeshes = new SubMeshDescriptor[512];

                //
                var triMeshIndexTable = scene.surfaceAttributeMeshTable.triMeshIndexTable[surfaceTypeIndex];
                var triIndexLists = triMeshIndexTable.indexLists;
                Assert.IsTrue(triIndexLists.Length == 0 || triIndexLists.Length == StaticMeshTableIndexes.kListCount);

                // meshIndex = 0-255
                // triIndex = the indexes for the mesh 
                for (int meshIndex = 0; meshIndex < triIndexLists.Length; meshIndex++)
                {
                    // Get ushort[] for all triangles used
                    var indexList = triIndexLists[meshIndex];

                    // iterate over each triangle
                    var allVertices = new List<Vector3>();
                    var allNormals = new List<Vector3>();
                    var allIndexes = new List<int>();
                    for (int i = 0; i < indexList.Length; i++)
                    {
                        var index = indexList.Indexes[i];
                        var triangle = colliderTriangles[index];

                        // Get vertices
                        var verts = triangle.GetVerts();
                        var nVerts = verts.Length;
                        // Get indices for those vertices
                        for (int v = 0; v < nVerts; v++)
                        {
                            allVertices.Add(verts[v]);
                            allIndexes.Add(i * nVerts + v);
                            allNormals.Add(triangle.normal);
                        }
                    }
                    var backfaceIndexes = allIndexes.ToArray();
                    Array.Reverse(backfaceIndexes);
                    allIndexes.AddRange(backfaceIndexes);

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
                    // Set mesh to use submesh 0-255
                    submeshes[meshIndex] = triSubmesh;
                }


                var quadMeshIndexes = scene.surfaceAttributeMeshTable.quadMeshIndexTable[surfaceTypeIndex];
                var quadIndexLists = quadMeshIndexes.indexLists;
                Assert.IsTrue(quadIndexLists.Length == 0 || quadIndexLists.Length == StaticMeshTableIndexes.kListCount);

                // meshIndex = 0-255
                // triIndex = the indexes for the mesh 
                for (int meshIndex = 0; meshIndex < quadIndexLists.Length; meshIndex++)
                {
                    const int nVerts = 6;

                    // Get ushort[] for all triangles used
                    var indexList = quadIndexLists[meshIndex];

                    // iterate over each triangle
                    var allVertices = new List<Vector3>();
                    var allNormals = new List<Vector3>();
                    var allIndexes = new List<int>();
                    for (int i = 0; i < indexList.Length; i++)
                    {
                        var index = indexList.Indexes[i];
                        var quad = colliderQuads[index];

                        // Get vertices
                        allVertices.Add(quad.vertex0);
                        allVertices.Add(quad.vertex1);
                        allVertices.Add(quad.vertex2); // tri 0
                        allVertices.Add(quad.vertex2);
                        allVertices.Add(quad.vertex3);
                        allVertices.Add(quad.vertex0); // tri 1
                        // Get indices for those vertices
                        // Add normal for each
                        for (int nv = 0; nv < nVerts; nv++)
                        {
                            allIndexes.Add(i * nVerts + nv);
                            allNormals.Add(quad.normal);
                        }
                    }
                    var backfaceIndexes = allIndexes.ToArray();
                    Array.Reverse(backfaceIndexes);
                    allIndexes.AddRange(backfaceIndexes);

                    // Build submesh
                    var quadSubmesh = new SubMeshDescriptor();
                    quadSubmesh.baseVertex = mesh.vertexCount;
                    quadSubmesh.firstVertex = mesh.vertexCount;
                    quadSubmesh.indexCount = allIndexes.Count;
                    quadSubmesh.indexStart = mesh.triangles.Length;
                    quadSubmesh.topology = MeshTopology.Triangles;
                    quadSubmesh.vertexCount = allVertices.Count;

                    Assert.IsTrue(allVertices.Count == allNormals.Count, $"v:{allVertices.Count} != n:{allNormals.Count}");

                    // Append to mesh
                    var verticesConcat = mesh.vertices.Concat(allVertices).ToArray();
                    var normalsConcat = mesh.normals.Concat(allNormals).ToArray();
                    var trianglesConcat = mesh.triangles.Concat(allIndexes).ToArray();
                    // Assign values to mesh
                    mesh.vertices = verticesConcat;
                    mesh.normals = normalsConcat;
                    mesh.triangles = trianglesConcat;
                    // Set mesh to use submesh 0-255
                    submeshes[meshIndex + 256] = quadSubmesh;
                }

                // Set each submesh in the mesh
                mesh.subMeshCount = submeshes.Length;
                for (int submeshIndex = 0; submeshIndex < submeshes.Length; submeshIndex++)
                {
                    mesh.SetSubMesh(submeshIndex, submeshes[submeshIndex], MeshUpdateFlags.Default);
                }

                // Compute other Mesh data
                mesh.RecalculateBounds();
                //mesh.RecalculateNormals();

                meshes[surfaceTypeIndex] = mesh;
            }

            return meshes;
        }


        public Mesh CreateObjectColliderMesh(SceneObject sceneObject)
        {
            var collision = sceneObject.instanceReference.colliderGeometry;

            // Create base data for mesh
            var mesh = new Mesh();
            mesh.name = sceneObject.name;
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