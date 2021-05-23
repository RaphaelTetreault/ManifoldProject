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

        [Header("Import Files")]
        [SerializeField] protected ColiSceneSobj[] sceneSobjs;

        #endregion

        public override string ExecuteText => "Import COLI Object Collision";

        public override void Execute() => Import();

        //// Missing: Mine?, Lava?
        //new Color(1.0f, 1.0f, 1.0f), // 1  WHITE - Drive-able
        //new Color(1.0f, 0.0f, 1.0f), // 2  MGNTA - Recover
        //new Color(0.0f, 1.0f, 0.0f), // 3  GREEN - "Bonk" collision?
        //new Color(1.0f, 1.0f, 0.0f), // 4  YLLOW - Boost Pads
        //new Color(1.0f, 0.7f, 0.0f), // 5  ORNGE - Jump Pads
        //new Color(0.0f, 1.0f, 1.0f), // 6  CYAN  - Ice/Slip
        //new Color(0.8f, 0.5f, 0.2f), // 7  BROWN - Dirt
        //new Color(0.7f, 0.5f, 1.0f), // 8  BLUE  - Lava/Laser/Damage
        //new Color(0.5f, 0.5f, 0.5f), // 9  GREY  - OOB, Out of Bounds Plane
        //new Color(1.0f, 0.5f, 0.5f), // 10 GREY-RED - Insta-Kill Collider. In AX, it would kill you but you'd skip/recochet off of it
        //new Color(1.0f, 0.4f, 0.0f), // 11 RED-ORANGE - Instant Kill
        //new Color(1.0f, 0.2f, 0.0f), // 12 RED-ORANGE - Kill Plane A
        //new Color(0.0f, 0.0f, 0.0f), // 13 BLACK - Kill Plane B (duplicate)
        //new Color(1.0f, 0.0f, 0.0f), // 14 RED   - Kill Plane B

        public void Import()
        {
            sceneSobjs = AssetDatabaseUtility.GetSobjByOption(sceneSobjs, importOption, importFrom);

            foreach (var sceneSobj in sceneSobjs)
            {
                var scene = sceneSobj.Value;

                //// Create object-based collider meshes
                //int total = scene.sceneObjects.Length;
                //int count = 0;
                //foreach (var sceneObject in scene.sceneObjects)
                //{
                //    count++;

                //    if (sceneObject.colliderBinding.colliderGeometryPtr.IsNotNullPointer)
                //    {
                //        var meshName = sceneObject.name;
                //        ImportUtility.ProgressBar<ColliderObject>(count, total, $"st{sceneSobj.Value.ID:00} {meshName}");

                //        // Create mesh
                //        var mesh = CreateObjectColliderMesh(sceneObject);

                //        // Save mesh to Asset Database
                //        var assetPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/coli_{meshName}.asset";
                //        AssetDatabase.CreateAsset(mesh, assetPath);

                //        // Refresh instance reference
                //        mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

                //        // Create mesh prefab
                //        var materials = new Material[] { triMaterial, quadMaterial };
                //        var prefabPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/pf_{meshName}.prefab";
                //        var prefab = ImportUtility.CreatePrefabFromModel(mesh, materials, prefabPath);

                //        // Edit then save again
                //        prefab.AddComponent<ColliderObjectTag>();
                //        PrefabUtility.SavePrefabAsset(prefab);
                //    }
                //}


                // Create static scene colliders
                var meshes = CreateStaticColliderMesh2(sceneSobj);
                int total = meshes.Length;
                int count = 0;

                foreach (var mesh in meshes)
                {
                    var materials = new Material[mesh.subMeshCount];
                    var half = mesh.subMeshCount / 2;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i] = i < half ? triMaterial : quadMaterial;
                    }

                    count++;
                    var meshName = mesh.name;
                    ImportUtility.ProgressBar<ColliderObject>(count, total, $"st{sceneSobj.Value.ID:00} {meshName}");

                    // Save mesh to Asset Database
                    var assetPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/coli_{meshName}.asset";
                    AssetDatabase.CreateAsset(mesh, assetPath);

                    // Create mesh prefab
                    var prefabPath = $"Assets/{importTo}/st{sceneSobj.Value.ID:00}/pf_{meshName}.prefab";
                    var prefab = ImportUtility.CreatePrefabFromModel(mesh, materials, prefabPath);

                    // Edit then save again
                    //prefab.AddComponent<ColliderObjectTag>();
                    //PrefabUtility.SavePrefabAsset(prefab);
                }
            }

            ImportUtility.FinalizeAssetImport();
        }

        //public Mesh[] CreateStaticColliderMesh(ColiSceneSobj sceneSobj)
        //{
        //    var scene = sceneSobj.Value;

        //    // Create static scene colliders
        //    // Ensure either or (XOR), but not both
        //    // TODO: do this in header directly
        //    Assert.IsTrue(scene.header.IsFileAX ^ scene.header.IsFileGX);

        //    // Turn into function within StaticMeshTable (take param header)
        //    var surfaceTypeCount = scene.header.IsFileAX
        //        ? StaticMeshTable.kCountAxSurfaceTypes
        //        : StaticMeshTable.kCountGxSurfaceTypes;

        //    var meshes = new Mesh[surfaceTypeCount];

        //    // Simplify access to tris/quads
        //    var triVerts = scene.surfaceAttributeMeshTable.colliderTriangles;
        //    var quadVerts = scene.surfaceAttributeMeshTable.colliderQuads;


        //    for (int surfaceTypeIndex = 0; surfaceTypeIndex < surfaceTypeCount; surfaceTypeIndex++)
        //    {
        //        var colliderProperty = (CollisionProperty)surfaceTypeIndex;

        //        // Create base data for mesh for EACH mesh type (boost, heal, etc)
        //        var mesh = new Mesh();
        //        mesh.name = $"{sceneSobj.name}_{surfaceTypeIndex:00}_{colliderProperty}";
        //        // Each tri/quad set has fixed size 256 each, so 512 total
        //        var submeshes = new SubMeshDescriptor[512];

        //        var triMeshIndexes = scene.surfaceAttributeMeshTable.triMeshIndexes[surfaceTypeIndex];
        //        var triIndexes = triMeshIndexes.indexes.GetArrays();
        //        Assert.IsTrue(triIndexes.Length == 0 || triIndexes.Length == MeshIndexes.kIndexArrayPtrsSize);


        //        // Add all verts to mesh
        //        var allVertices = triVerts.Concat(quadVerts).ToArray();
        //        mesh.SetVertices(allVertices);

        //        // meshIndex = 0-255
        //        // triIndex = the indexes for the mesh 
        //        for (int meshIndex = 0; meshIndex < triIndexes.Length; meshIndex++)
        //        {
        //            // Extract sequence length
        //            int numTriangleIndexes = triIndexes[meshIndex].Length;

        //            //if (numTriangleIndexes > 0)
        //            //    Debug.Log($"{sceneSobj.name} type:{colliderProperty} index:{meshIndex} indexes:{numTriangleIndexes}");

        //            // Convert ushort[][] indexes into int[]
        //            var indexes = new int[numTriangleIndexes];
        //            for (int triIndex = 0; triIndex < indexes.Length; triIndex++)
        //            {
        //                indexes[triIndex] = triIndexes[meshIndex][triIndex];
        //            }
        //            //
        //            indexes = MeshUtility.GetTrianglesFromTriangleStrip(indexes);

        //            var vertices = triVerts;

        //            //
        //            var hasZeroIndexes = indexes.Length == 0;
        //            //var firstIndex = indexes[0];
        //            //var lastIndex = indexes[indexes.Length - 1];
        //            var vertexCount = hasZeroIndexes ? 0 : indexes[indexes.Length - 1] - indexes[0];
        //            var firstVertex = hasZeroIndexes ? 0 : indexes[0];

        //            // Build submesh
        //            var triSubmesh = new SubMeshDescriptor();
        //            triSubmesh.baseVertex = 0; // ???
        //            triSubmesh.firstVertex = firstVertex; // first vert is first of array
        //            triSubmesh.vertexCount = vertexCount; //
        //            triSubmesh.indexStart = mesh.triangles.Length; // first index
        //            triSubmesh.indexCount = indexes.Length; // total number of indexes
        //            triSubmesh.topology = MeshTopology.Triangles;

        //            // Append to mesh
        //            var trianglesConcat = mesh.triangles.Concat(indexes).ToArray();
        //            // Assign values to mesh
        //            mesh.triangles = trianglesConcat;
        //            // Set mesh to use submesh 0-255
        //            submeshes[meshIndex] = triSubmesh;
        //        }

        //        var quadIndexes = scene.surfaceAttributeMeshTable.quadMeshIndexes[surfaceTypeIndex].indexes;
        //        Assert.IsTrue(quadIndexes.Length == 0 || quadIndexes.Length == MeshIndexes.kIndexArrayPtrsSize);

        //        // Set each submesh in the mesh
        //        mesh.subMeshCount = submeshes.Length;
        //        for (int submeshIndex = 0; submeshIndex < submeshes.Length; submeshIndex++)
        //        {
        //            mesh.SetSubMesh(submeshIndex, submeshes[submeshIndex], MeshUpdateFlags.Default);
        //        }

        //        // Compute other Mesh data
        //        mesh.RecalculateBounds();
        //        mesh.RecalculateNormals();

        //        meshes[surfaceTypeIndex] = mesh;
        //    }

        //    return meshes;
        //}

        public Mesh[] CreateStaticColliderMesh2(ColiSceneSobj sceneSobj)
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
                mesh.name = $"{sceneSobj.name}_{surfaceTypeIndex:00}_{(CollisionProperty)surfaceTypeIndex}";
                // Each tri/quad set has fixed size 256 each, so 512 total
                var submeshes = new SubMeshDescriptor[512];

                //
                var triMeshIndexes = scene.surfaceAttributeMeshTable.triMeshIndexes[surfaceTypeIndex];
                var triIndexes = triMeshIndexes.indexes.GetArrays();
                Assert.IsTrue(triIndexes.Length == 0 || triIndexes.Length == MeshIndexes.kIndexArrayPtrsSize);

                // meshIndex = 0-255
                // triIndex = the indexes for the mesh 
                for (int meshIndex = 0; meshIndex < triIndexes.Length; meshIndex++)
                {
                    // Get ushort[] for all triangles used
                    var indexes = triIndexes[meshIndex];

                    // iterate over each triangle
                    var allVertices = new List<Vector3>();
                    var allIndexes = new List<int>();
                    for (int i = 0; i < indexes.Length; i++)
                    {
                        var index = indexes[i];
                        var triangle = colliderTriangles[index];

                        // Get vertices
                        //allVertices.AddRange(triangle.GetVerts());
                        allVertices.Add(triangle.vertex0);
                        allVertices.Add(triangle.vertex1);
                        allVertices.Add(triangle.vertex2);

                        // Get indices for those vertices
                        allIndexes.Add(i * 3 + 0);
                        allIndexes.Add(i * 3 + 1);
                        allIndexes.Add(i * 3 + 2);
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
                    var trianglesConcat = mesh.triangles.Concat(allIndexes).ToArray();
                    // Assign values to mesh
                    mesh.vertices = verticesConcat;
                    mesh.triangles = trianglesConcat;
                    // Set mesh to use submesh 0-255
                    submeshes[meshIndex] = triSubmesh;
                }

                var quadMeshIndexes = scene.surfaceAttributeMeshTable.quadMeshIndexes[surfaceTypeIndex];
                var quadIndexes = quadMeshIndexes.indexes.GetArrays();
                Assert.IsTrue(quadIndexes.Length == 0 || quadIndexes.Length == MeshIndexes.kIndexArrayPtrsSize);

                // meshIndex = 0-255
                // triIndex = the indexes for the mesh 
                for (int meshIndex = 0; meshIndex < quadIndexes.Length; meshIndex++)
                {
                    // Get ushort[] for all triangles used
                    var indexes = quadIndexes[meshIndex];

                    // iterate over each triangle
                    var allVertices = new List<Vector3>();
                    var allIndexes = new List<int>();
                    for (int i = 0; i < indexes.Length; i++)
                    {
                        var index = indexes[i];
                        var quad = colliderQuads[index];

                        // Get vertices
                        allVertices.Add(quad.vertex0);
                        allVertices.Add(quad.vertex1);
                        allVertices.Add(quad.vertex2);
                        //
                        allVertices.Add(quad.vertex2);
                        allVertices.Add(quad.vertex3);
                        allVertices.Add(quad.vertex0);

                        // Get indices for those vertices
                        allIndexes.Add(i * 6 + 0);
                        allIndexes.Add(i * 6 + 1);
                        allIndexes.Add(i * 6 + 2);
                        allIndexes.Add(i * 6 + 3);
                        allIndexes.Add(i * 6 + 4);
                        allIndexes.Add(i * 6 + 5);
                    }

                    // Build submesh
                    var quadSubmesh = new SubMeshDescriptor();
                    quadSubmesh.baseVertex = mesh.vertexCount;
                    quadSubmesh.firstVertex = mesh.vertexCount;
                    quadSubmesh.indexCount = allIndexes.Count;
                    quadSubmesh.indexStart = mesh.triangles.Length;
                    quadSubmesh.topology = MeshTopology.Triangles;
                    quadSubmesh.vertexCount = allVertices.Count;

                    // Append to mesh
                    var verticesConcat = mesh.vertices.Concat(allVertices).ToArray();
                    var trianglesConcat = mesh.triangles.Concat(allIndexes).ToArray();
                    // Assign values to mesh
                    mesh.vertices = verticesConcat;
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
                mesh.RecalculateNormals();

                meshes[surfaceTypeIndex] = mesh;
            }

            return meshes;
        }


        public Mesh CreateObjectColliderMesh(SceneObject sceneObject)
        {
            var collision = sceneObject.colliderBinding.colliderGeometry;

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