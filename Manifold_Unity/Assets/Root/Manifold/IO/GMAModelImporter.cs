using System.IO;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using GameCube.FZeroGX.GMA;
using UnityEngine.Rendering;
using System.Linq;


namespace Manifold.IO.GFZX01
{
    [CreateAssetMenu(menuName = "Manifold/Import/" + "NEW GMA Model Importer")]
    public class GMAModelImporter : ExecutableScriptableObject,
        IImportable
    {
        [Header("Import Settings")]
        [SerializeField, BrowseFolderField("Assets/")]
        protected string importDestination;

        [Header("TEMP: Apply basic material")]
        [SerializeField] protected UnityEngine.Material defaultMat;

        [Header("Import Files")]
        [SerializeField] protected GMASobj[] gmaSobjs;

        public override string ExecuteText => "Import GMA Models";

        public override void Execute() => Import();

        public void Import()
        {
            // Temp: should make parameter
            //gmaSobjs = ImportUtility.GetAllOfTypeFromAssetDatabase<GMASobj>();

            foreach (GMASobj sobj in gmaSobjs)
            {
                var path = AssetDatabase.GetAssetPath(sobj);
                path = Path.GetDirectoryName(path);
                path = UnityPathUtility.EnforceUnitySeparators(path);
                path += $"/{Path.GetFileNameWithoutExtension(sobj.FileName)}/";

                foreach (var gcmf in sobj.value.GCMF)
                {
                    if (string.IsNullOrEmpty(gcmf.ModelName))
                        continue;

                    var mesh = CreateSingleMeshFromGcmf(gcmf, path);
                    var prefab = CreatePrefabFromModel(mesh, path);
                    DestroyImmediate(prefab);
                }
            }
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public int[] GetTriangleFromTriangleStrip(int numVerts)
        {
            // Construct triangles from GameCube GX TRIANLE_STRIP

            var nTriangles = numVerts - 2;
            int[] triangles = new int[nTriangles * 3 * 2];
            for (int i = 0; i < nTriangles; i++)
            {
                var triIdx = i * 6;
                triangles[triIdx + 0] = i + 0;
                triangles[triIdx + 1] = i + 1;
                triangles[triIdx + 2] = i + 2;

                triIdx += 3;
                triangles[triIdx + 0] = i + 0;
                triangles[triIdx + 1] = i + 2;
                triangles[triIdx + 2] = i + 1;
            }
            return triangles;
        }

        public Mesh CreateSingleMeshFromGcmf(GCMF gcmf, string path)
        {
            // Count how many submeshes we will need to iterate through
            var numSubmeshes = 0;
            foreach (var gcmfMesh in gcmf.Submeshes)
            {
                numSubmeshes += gcmfMesh.DisplayList0.GxDisplayLists.Length;
                numSubmeshes += gcmfMesh.DisplayList1.GxDisplayLists.Length;
            }

            var mesh = new Mesh();
            int subIndex = 0;
            var submeshes = new SubMeshDescriptor[numSubmeshes];

            // Go over each mesh in submeshes
            foreach (var gcmfMesh in gcmf.Submeshes)
            {
                // Temp progress bar
                var progress = (float)subIndex / numSubmeshes;
                EditorUtility.DisplayProgressBar("Importing Mesh Data", gcmf.ModelName, progress);

                // Go over each list0
                foreach (var list in gcmfMesh.DisplayList0.GxDisplayLists)
                {
                    var submesh = CreateSubMesh(list, ref mesh);
                    submeshes[subIndex] = submesh;
                    subIndex++;
                }
                // Go over each list0
                foreach (var list in gcmfMesh.DisplayList1.GxDisplayLists)
                {
                    var submesh = CreateSubMesh(list, ref mesh);
                    submeshes[subIndex] = submesh;
                    subIndex++;
                }
            }
            mesh.subMeshCount = submeshes.Length;
            for (int i = 0; i < submeshes.Length; i++)
            {
                mesh.SetSubMesh(i, submeshes[i], MeshUpdateFlags.Default);
            }
            mesh.RecalculateBounds();

            string name = $"{gcmf.ModelName}.asset";
            AssetDatabase.CreateAsset(mesh, $"{path}/{name}");
            return mesh;
        }

        public SubMeshDescriptor CreateSubMesh(GameCube.GX.GxDisplayList list, ref Mesh mesh)
        {
            var submesh = new SubMeshDescriptor();

            // New from this list/submesh
            var vertices = list.pos;
            var normals = list.nrm;
            var uv1 = list.tex0;
            var uv2 = list.tex1;
            var uv3 = list.tex2;
            var colors = list.clr0;
            var triangles = GetTriangleFromTriangleStrip(vertices.Length);

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

        public GameObject CreatePrefabFromModel(Mesh mesh, string path)
        {
            var pfPath = $"{path}/pf_{mesh.name}.prefab";
            var prefab = new GameObject();
            var meshFilter = prefab.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            var meshRenderer = prefab.AddComponent<MeshRenderer>();
            var mats = new UnityEngine.Material[mesh.subMeshCount];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = defaultMat;
            meshRenderer.sharedMaterials = mats;
            PrefabUtility.SaveAsPrefabAsset(prefab, pfPath);
            return prefab;
        }


    }
}