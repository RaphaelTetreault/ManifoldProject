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
                        mesh.name = gobj.name;
                        AssetDatabase.CreateAsset(mesh, $"Assets/{mesh.name}.asset");
                        var prefab = CreatePrefabFromModel(mesh, "Assets/");
                        DestroyImmediate(prefab);
                    }
                }
            }

            ImportUtility.WrapUpAssetImport();
        }

        public Mesh CreateMesh(GameCube.GFZX01.CourseCollision.GameObject gameObject)
        {
            var name = gameObject.name;
            var collision = gameObject.collisionBinding.collision;

            var mesh = new Mesh();
            //mesh.subMeshCount = 2;
            var submeshes = new SubMeshDescriptor[2];

            // TRIS
            {
                var triCount = collision.triCount;
                var vertCount = triCount * 3;
                //
                var vertices = new Vector3[vertCount];
                //var normals = new Vector3[triCount];
                var triangles = new int[vertCount];
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
                    triangles[stride + 0] = stride + 0;
                    triangles[stride + 1] = stride + 1;
                    triangles[stride + 2] = stride + 2;
                }
                var triSubmesh = new SubMeshDescriptor();
                // Build submesh
                triSubmesh.baseVertex = mesh.vertexCount; // 0
                triSubmesh.firstVertex = mesh.vertexCount; // 0
                triSubmesh.indexCount = triCount;
                triSubmesh.indexStart = mesh.triangles.Length; // 0
                triSubmesh.topology = MeshTopology.Triangles;
                triSubmesh.vertexCount = vertCount;

                // Append to mesh
                var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                //var normalsConcat = mesh.normals.Concat(normals).ToArray();
                var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

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
                const int vertsPerQuad = 6;
                var quadCount = collision.quadCount;
                var vertCount = quadCount * vertsPerQuad;
                //
                var vertices = new Vector3[vertCount];
                //var normals = new Vector3[quadCount];
                var quadTris = new int[vertCount];
                //
                for (int i = 0; i < quadCount; i++)
                {
                    //normals[i] = collision.quads[i].normal;

                    var stride = i * vertsPerQuad;
                    //
                    vertices[stride + 0] = collision.quads[i].vertex0;
                    vertices[stride + 1] = collision.quads[i].vertex1;
                    vertices[stride + 2] = collision.quads[i].vertex2;
                    ////////////////////
                    vertices[stride + 3] = collision.quads[i].vertex1;
                    vertices[stride + 4] = collision.quads[i].vertex3;
                    vertices[stride + 5] = collision.quads[i].vertex2;
                    //
                    quadTris[stride + 0] = stride + 0;
                    quadTris[stride + 1] = stride + 1;
                    quadTris[stride + 2] = stride + 2;
                    /////////////////
                    quadTris[stride + 1] = stride + 3;
                    quadTris[stride + 2] = stride + 4;
                    quadTris[stride + 3] = stride + 5;
                }
                var quadSubmesh = new SubMeshDescriptor();
                // Build submesh
                quadSubmesh.baseVertex = mesh.vertexCount;
                quadSubmesh.firstVertex = mesh.vertexCount;
                quadSubmesh.indexCount = quadCount;
                quadSubmesh.indexStart = mesh.triangles.Length;
                quadSubmesh.topology = MeshTopology.Quads;
                quadSubmesh.vertexCount = vertCount;

                // Append to mesh
                var verticesConcat = mesh.vertices.Concat(vertices).ToArray();
                //var normalsConcat = mesh.normals.Concat(normals).ToArray();
                var trianglesConcat = mesh.triangles.Concat(quadTris).ToArray();

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