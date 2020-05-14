using GameCube.GFZX01.GMA;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manifold.IO.GFZX01.GMA
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_GMA + "GMA Model Importer")]
    public class GmaModelImporter : ExecutableScriptableObject,
        IImportable
    {
        [Header("Import Settings")]
        [SerializeField, BrowseFolderField("Assets/")]
        protected string importSource;
        [SerializeField, BrowseFolderField("Assets/")]
        protected string importDestination;
        [SerializeField]
        protected IOOption importOption = IOOption.selectedFiles;

        [Header("TEMP: Apply basic material")]
        [SerializeField] protected UnityEngine.Material defaultMat;

        [Header("Import Files")]
        [SerializeField] protected GmaSobj[] gmaSobjs;

        public override string ExecuteText => "Import GMA Models";

        public override void Execute() => Import();

        public void Import()
        {
            gmaSobjs = IOUtility.GetSobjByOption(gmaSobjs, importOption, importSource);

            int submeshes = 0;
            int totalModels = CountModels(gmaSobjs, out submeshes);

            int count = 1;
            foreach (GmaSobj sobj in gmaSobjs)
            {
                Gma gma = sobj.Value;

                var folderName = Path.GetFileNameWithoutExtension(sobj.FileName);
                folderName = folderName.Replace(',', '_');

                var unityPath = AssetDatabase.GetAssetPath(sobj);
                unityPath = Path.GetDirectoryName(unityPath);
                unityPath = UnityPathUtility.EnforceUnitySeparators(unityPath);

                // Create folder if it doesn't already exist
                var folder = UnityPathUtility.CombineUnityPath(unityPath, folderName);
                if (!AssetDatabase.IsValidFolder(folder))
                    AssetDatabase.CreateFolder(unityPath, folderName);

                unityPath += $"/{folderName}/";

                foreach (var gcmf in gma.GCMF)
                {
                    if (string.IsNullOrEmpty(gcmf.ModelName))
                        continue;

                    var importTitle = $"Importing Model ({count}/{totalModels}) Submesh Total ({submeshes})";
                    var mesh = CreateSingleMeshFromGcmf(gcmf, unityPath, importTitle);
                    var prefab = CreatePrefabFromModel(mesh, unityPath);
                    DestroyImmediate(prefab);
                    count++;
                }
            }
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public int[] GetTriangleFromTriangleStrip(int numVerts)
        {
            // Construct triangles from GameCube GX TRIANLE_STRIP
            // Apparently the issue is even/odd numbered tri strips
            // You can flip/flop between those if you know the count,
            // which you definitely do somewhere.

            /*/
            // This code mostly works, but still has some errors in it.
            
            const int vertStride = 3;

            var stripIsOdd = (numVerts % 2) > 0;
            var nTriangles = numVerts - 2;
            int[] triangles = new int[nTriangles * vertStride];
            for (int i = 0; i < nTriangles; i++)
            {
                var triIdx = i * vertStride;
                var indexIsOdd = (i % 2) > 0;
                var isOdd = !(stripIsOdd ^ indexIsOdd);
                //var isOdd = indexIsOdd;
                //var isOdd = stripIsOdd;

                if (isOdd)
                {
                    triangles[triIdx + 0] = i + 0;
                    triangles[triIdx + 1] = i + 1;
                    triangles[triIdx + 2] = i + 2;
                }
                else
                {
                    //triangles[triIdx + 0] = i + 1;
                    //triangles[triIdx + 1] = i + 0;
                    //triangles[triIdx + 2] = i + 2;

                    triangles[triIdx + 0] = i + 0;
                    triangles[triIdx + 1] = i + 2;
                    triangles[triIdx + 2] = i + 1;
                }
            }
             /*/

            var nTriangles = numVerts - 2;
            int[] triangles = new int[nTriangles * 3 * 2];
            for (int i = 0; i < nTriangles; i++)
            {
                var triIdx = i * 6;
                triangles[triIdx + 0] = i + 0;
                triangles[triIdx + 1] = i + 1;
                triangles[triIdx + 2] = i + 2;

                triIdx += 3;
                triangles[triIdx + 0] = i + 1;
                triangles[triIdx + 1] = i + 0;
                triangles[triIdx + 2] = i + 2;
            }
            return triangles;
        }

        public Mesh CreateSingleMeshFromGcmf(Gcmf gcmf, string path, string title)
        {
            // Count how many submeshes we will need to iterate through
            var numSubmeshes = 0;
            foreach (var gcmfMesh in gcmf.Submeshes)
            {
                if (gcmfMesh.DisplayList0.GxDisplayLists != null)
                    numSubmeshes += gcmfMesh.DisplayList0.GxDisplayLists.Length;
                if (gcmfMesh.DisplayList1.GxDisplayLists != null)
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
                var info = $"{path}{gcmf.ModelName}";
                EditorUtility.DisplayProgressBar(title, info, progress);

                // Go over each list0
                if (gcmfMesh.DisplayList1.GxDisplayLists != null)
                    foreach (var list in gcmfMesh.DisplayList0.GxDisplayLists)
                    {
                        var submesh = CreateSubMesh(list, ref mesh);
                        submeshes[subIndex] = submesh;
                        subIndex++;
                    }
                // Go over each list1
                if (gcmfMesh.DisplayList1.GxDisplayLists != null)
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

            // TEMP: Suppress COM# named model errors on Windows
            string name = $"{gcmf.ModelName}.asset";
            try
            {
                AssetDatabase.CreateAsset(mesh, $"{path}/{name}");
            }
            catch
            {
                AssetDatabase.CreateAsset(mesh, $"{path}/_{name}");
            }
            return mesh;
        }

        public SubMeshDescriptor CreateSubMesh(GameCube.GX.GxDisplayList list, ref Mesh mesh)
        {
            var submesh = new SubMeshDescriptor();

            // This made colors disappear/white in a lot of places...?
            //// HACK
            //// This code should be making separate meshes (as Unity import type)
            //// where the meshes are under 1 structure in the editor. This current
            //// code is done in a loop where all meshes are submeshes, which require
            //// the same number of colors as verts, hence this hack.
            //var listClr0 = list.clr0;
            //if (listClr0.Length == 0)
            //{
            //    listClr0 = new Color32[list.pos.Length];
            //    for (int i = 0; i < listClr0.Length; i++)
            //    {
            //        listClr0[i] = new Color32(255, 255, 255, 255);
            //    }
            //}

            // TODO:
            // Generate submeshes correctly (ie: stop concatonating them all) so that
            // there are no issues with varied UV, UV2, UV3, and COLOR counts.

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

        public int CountModels(GmaSobj[] gmaSobjs, out int submeshes)
        {
            submeshes = 0;
            int count = 0;

            foreach (GmaSobj sobj in gmaSobjs)
            {
                var gma = sobj.Value;
                foreach (Gcmf gcmf in gma.GCMF)
                {
                    // Some GCMFs can be null, check via model name
                    if (string.IsNullOrEmpty(gcmf.ModelName))
                    {
                        continue;
                    }

                    count++;

                    foreach (GcmfSubmesh gcmfMesh in gcmf.Submeshes)
                    {
                        // Go over each list0
                        if (gcmfMesh.DisplayList0.GxDisplayLists != null)
                            foreach (var list in gcmfMesh.DisplayList0.GxDisplayLists)
                                submeshes++;

                        // Go over each list1
                        if (gcmfMesh.DisplayList1.GxDisplayLists != null)
                            foreach (var list in gcmfMesh.DisplayList1.GxDisplayLists)
                                submeshes++;
                    }
                }
            }
            return count;
        }
    }
}