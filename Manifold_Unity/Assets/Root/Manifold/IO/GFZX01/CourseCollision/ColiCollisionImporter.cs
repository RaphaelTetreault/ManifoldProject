using GameCube.GFZX01.CourseCollision;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manifold.IO.GFZX01.CourseCollision
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_CourseCollision + "COLI Collision Importer")]
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

        [Header("Import Files")]
        [SerializeField] protected ColiSceneSobj[] colis;

        #endregion

        public override string ExecuteText => "Import COLI Object Collision";

        public override void Execute() => Import();

        public void Import()
        {
            colis = IOUtility.GetSobjByOption(colis, importOption, importFrom);

            foreach (var coli in colis)
            {
                foreach (var gobj in coli.Value.gameObjects)
                {
                    if (gobj.collisionBinding.collisionRelPtr != 0)
                    {
                        var mesh = CreateMesh(gobj);
                        AssetDatabase.CreateAsset(mesh, $"Assets/{coli.name}_{mesh.name}.asset");
                        var prefab = CreatePrefabFromModel(mesh, "Assets/");
                        DestroyImmediate(prefab);
                    }
                }
            }

            ImportUtility.WrapUpAssetImport();
        }

        public Mesh CreateMesh(GameCube.GFZX01.CourseCollision.GameObject gameObject)
        {
            var collision = gameObject.collisionBinding.collision;

            var mesh = new Mesh();
            mesh.name = gameObject.name;
            var submeshes = new SubMeshDescriptor[2];

            // TRIS
            {
                var triCount = collision.triCount;
                var vertCount = triCount * 3;
                //
                var vertices = new Vector3[vertCount];
                //var normals = new Vector3[triCount];
                var indices = new int[vertCount];
                //
                for (int i = 0; i < triCount; i++)
                {
                    //normals[i] = collision.tris[i].normal;

                    var stride = i * 3;
                    //
                    vertices[stride + 0] = collision.tris[i].vertex0;
                    vertices[stride + 1] = collision.tris[i].vertex1;
                    vertices[stride + 2] = collision.tris[i].vertex2;
                    //
                    indices[stride + 0] = stride + 0;
                    indices[stride + 1] = stride + 1;
                    indices[stride + 2] = stride + 2;
                }
                var triSubmesh = new SubMeshDescriptor();
                // Build submesh
                triSubmesh.baseVertex = mesh.vertexCount; // 0
                triSubmesh.firstVertex = mesh.vertexCount; // 0
                triSubmesh.indexCount = indices.Length;
                triSubmesh.indexStart = mesh.triangles.Length; // 0
                triSubmesh.topology = MeshTopology.Triangles;
                triSubmesh.vertexCount = vertices.Length;

                // Append to mesh
                var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                //var normalsConcat = mesh.normals.Concat(normals).ToArray();
                var trianglesConcat = mesh.triangles.Concat(indices).ToArray();

                // Assign values to mesh
                mesh.vertices = verticesConcat;
                //mesh.normals = normalsConcat;
                mesh.triangles = trianglesConcat;

                //
                //mesh.SetSubMesh(0, triSubmesh);
                submeshes[0] = triSubmesh;
            }
            // QUADS
            {
                var quadCount = collision.quadCount;
                var vertCount = quadCount * 4;
                //
                var vertices = new Vector3[vertCount];
                //var normals = new Vector3[quadCount];
                var indices = new int[quadCount * 6];
                //
                for (int i = 0; i < quadCount; i++)
                {
                    //normals[i] = collision.quads[i].normal;

                    var vertStride = i * 4;
                    vertices[vertStride + 0] = collision.quads[i].vertex0;
                    vertices[vertStride + 1] = collision.quads[i].vertex1;
                    vertices[vertStride + 2] = collision.quads[i].vertex2;
                    vertices[vertStride + 3] = collision.quads[i].vertex3;

                    var quadStride = i * 6;
                    indices[quadStride + 0] = vertStride + 0;
                    indices[quadStride + 1] = vertStride + 1;
                    indices[quadStride + 2] = vertStride + 2;
                    indices[quadStride + 3] = vertStride + 0;
                    indices[quadStride + 4] = vertStride + 2;
                    indices[quadStride + 5] = vertStride + 3;
                }
                var quadSubmesh = new SubMeshDescriptor();
                // Build submesh
                quadSubmesh.baseVertex = mesh.vertexCount;
                quadSubmesh.firstVertex = mesh.vertexCount;
                quadSubmesh.indexCount = indices.Length;
                quadSubmesh.indexStart = mesh.triangles.Length;
                quadSubmesh.topology = MeshTopology.Triangles;
                quadSubmesh.vertexCount = vertices.Length;

                // Append to mesh
                var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                //var normalsConcat = mesh.normals.Concat(normals).ToArray();
                var trianglesConcat = mesh.triangles.Concat(indices).ToArray();

                // Assign values to mesh
                mesh.vertices = verticesConcat;
                //mesh.normals = normalsConcat;
                mesh.triangles = trianglesConcat;

                // Record submesh
                //mesh.SetSubMesh(1, quadSubmesh);
                submeshes[1] = quadSubmesh;
            }

            // Compute Mesh
            mesh.subMeshCount = submeshes.Length;
            for (int i = 0; i < submeshes.Length; i++)
            {
                mesh.SetSubMesh(i, submeshes[i], MeshUpdateFlags.Default);
            }
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        // Copied from GmaModelImporter...
        public UnityEngine.GameObject CreatePrefabFromModel(Mesh mesh, string path)
        {
            var pfPath = $"{path}/pf_{mesh.name}.prefab";
            var prefab = new UnityEngine.GameObject();
            var meshFilter = prefab.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            var meshRenderer = prefab.AddComponent<MeshRenderer>();
            var mats = new UnityEngine.Material[mesh.subMeshCount];
            //for (int i = 0; i < mats.Length; i++)
            //    mats[i] = defaultMat;
            //meshRenderer.sharedMaterials = mats;
            PrefabUtility.SaveAsPrefabAsset(prefab, pfPath);
            return prefab;
        }
    }
}