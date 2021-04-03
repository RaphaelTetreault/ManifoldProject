using GameCube.GX;
using GameCube.GFZ.GMA;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Manifold.IO.GFZ.GMA
{
    [CreateAssetMenu(menuName = Const.Menu.GfzGMA + "GMA Model Importer")]
    public class GmaModelImporter : ExecutableScriptableObject,
        IImportable
    {
        #region MEMBERS

        [Header("Import Settings")]
        [FormerlySerializedAs("importSource")]
        [SerializeField, BrowseFolderField("Assets/")]
        protected string importFrom;
        
        [FormerlySerializedAs("importDestination")]
        [SerializeField, BrowseFolderField("Assets/")]
        protected string importTo;

        [SerializeField]
        protected IOOption importOption = IOOption.selectedFiles;

        [Header("Hacks")]
        [SerializeField]
        protected Color32 defaultVertexColor = Color.white;
        [SerializeField]
        protected UnityEngine.Material defaultMat;

        [Header("Import Files")]
        [SerializeField] protected GmaSobj[] gmaSobjs;

        #endregion

        public override string ExecuteText => "Import GMA Models";

        public override void Execute() => Import();

        public void Import()
        {
            gmaSobjs = AssetDatabaseUtility.GetSobjByOption(gmaSobjs, importOption, importFrom);

            int totalModels = CountModels(gmaSobjs, out int submeshes);

            int count = 1;
            foreach (GmaSobj sobj in gmaSobjs)
            {
                Gma gma = sobj.Value;

                var folderName = Path.GetFileNameWithoutExtension(sobj.FileName);
                folderName = folderName.Replace(',', '_');

                // TODO: is there not a helper function for this?
                var unityPath = AssetDatabase.GetAssetPath(sobj);
                unityPath = Path.GetDirectoryName(unityPath);
                unityPath = UnityPathUtility.EnforceUnitySeparators(unityPath);

                // TODO: make this a generic helper function
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

        public int[] GetTrianglesFromTriangleStrip(int numVerts, bool baseCCW)
        {
            // Construct triangles from GameCube GX TRIANGLE_STRIP
            // For one, note that we need to unwind the tristrip.
            // We can use the index to know if the indice is odd or even.
            // However, in GFZ, the winding for different display lists
            // inverts based on it's "index," so to speak.
            // To compensate, we need to XOR the odd/even value with whether
            // the base index of the strip is meant to be CCW or CW.
            
            const int vertStride = 3;

            var nTriangles = numVerts - 2;
            int[] triangles = new int[nTriangles * vertStride];
            for (int i = 0; i < nTriangles; i++)
            {
                var triIdx = i * vertStride;
                var indexIsCW = (i % 2) > 0;
                var isCCW = (baseCCW ^ indexIsCW);

                if (isCCW)
                {
                    triangles[triIdx + 0] = i + 0;
                    triangles[triIdx + 1] = i + 1;
                    triangles[triIdx + 2] = i + 2;
                }
                else
                {
                    triangles[triIdx + 0] = i + 0;
                    triangles[triIdx + 1] = i + 2;
                    triangles[triIdx + 2] = i + 1;
                }
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

                //
                var displayList0 = gcmfMesh.DisplayList0.GxDisplayLists;
                if (displayList0 != null)
                {
                    foreach (var list in displayList0)
                    {
                        var submesh = CreateSubMesh(list, ref mesh, true);
                        submeshes[subIndex] = submesh;
                        subIndex++;
                    }
                }

                //
                var displayList1 = gcmfMesh.DisplayList1.GxDisplayLists;
                if (displayList1 != null)
                {
                    foreach (var list in displayList1)
                    {
                        var submesh = CreateSubMesh(list, ref mesh, false);
                        submeshes[subIndex] = submesh;
                        subIndex++;
                    }
                }
            }

            mesh.subMeshCount = submeshes.Length;
            for (int i = 0; i < submeshes.Length; i++)
            {
                mesh.SetSubMesh(i, submeshes[i], MeshUpdateFlags.Default);
            }
            mesh.RecalculateBounds();

            // Also gets by COM# models
            string name = $"mdl_{gcmf.ModelName}.asset";
            AssetDatabase.CreateAsset(mesh, $"{path}/{name}");
            return mesh;
        }

        public Vector2[] HackUVs(Vector2[] uvs, int vertCount)
        {
            return uvs.Length == 0
                ? new Vector2[vertCount]
                : uvs;
        }

        public Vector3[] HackNormals(Vector3[] normals, int vertCount)
        {
            // Normals will get recalculated based on polygon
            return normals.Length == 0
                ? new Vector3[vertCount]
                : normals;
        }

        public Color32[] HackColors(Color32[] colors, int vertCount)
        {
            // make local
            var defaultColor = defaultVertexColor;

            if (colors.Length == 0)
            {
                colors = new Color32[vertCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = defaultColor;
                }
            }
            return colors;
        }

        public SubMeshDescriptor CreateSubMesh(DisplayList displayList, ref Mesh mesh, bool isCCW)
        {
            var submesh = new SubMeshDescriptor();

            // TODO:
            // Generate submeshes correctly (ie: stop concatonating them all) so that
            // there are no issues with varied UV, UV2, UV3, and COLOR counts.

            // HACKS
            // This code should be making separate meshes (as Unity import type)
            // where the meshes are under 1 structure in the editor. This current
            // code is done in a loop where all meshes are submeshes, which require
            // the same number of colors as verts, hence this hack.
            var nVerts = displayList.pos.Length;
            // This logic is applied to UVs and colors
            // 2020/06/02 Raph: for the following code, I disabled UV to speed import
            //                  times as UV information is not yet used
            
            // New from this list/submesh
            var vertices = displayList.pos;
            var normals = HackNormals(displayList.nrm, nVerts);
            //var uv1 = HackUVs(list.tex0, nVerts);
            //var uv2 = HackUVs(list.tex1, nVerts);
            //var uv3 = HackUVs(list.tex2, nVerts);
            var colors = HackColors(displayList.clr0, nVerts);
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
            //var uv1Concat = mesh.uv.Concat(uv1).ToArray();
            //var uv2Concat = mesh.uv2.Concat(uv2).ToArray();
            //var uv3Concat = mesh.uv3.Concat(uv3).ToArray();
            var colorsConcat = mesh.colors32.Concat(colors).ToArray();
            //if (list.nbt != null)
            //    mesh.tangents = list.nbt;
            var trianglesConcat = mesh.triangles.Concat(triangles).ToArray();

            // Assign values to mesh
            mesh.vertices = verticesConcat;
            mesh.normals = normalsConcat;
            //mesh.uv = uv1Concat;
            //mesh.uv2 = uv2Concat;
            //mesh.uv3 = uv3Concat;
            mesh.colors32 = colorsConcat;
            mesh.triangles = trianglesConcat;

            return submesh;
        }

        public GameObject CreatePrefabFromModel(Mesh mesh, string path)
        {
            // Remove "mdl_" prefix to build prefab name
            var modelName = mesh.name.Remove(0, 4);
            var prefabPath = $"{path}/pf_{modelName}.prefab";
            // Construct the prefab to store the model data
            var prefab = new GameObject();
            var meshFilter = prefab.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            var meshRenderer = prefab.AddComponent<MeshRenderer>();
            // HACK: Add a generic material to each model
            // In the future, generate materials for models
            var mats = new UnityEngine.Material[mesh.subMeshCount];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = defaultMat;
            meshRenderer.sharedMaterials = mats;
            // Save model to asset database, return
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
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
                    //var skipEmptyGCMF = string.IsNullOrEmpty(gcmf.ModelName);
                    if (gcmf == null || string.IsNullOrEmpty(gcmf.ModelName))
                    {
                        continue;
                    }

                    // Increment model count as model data exists
                    count++;

                    foreach (GcmfSubmesh gcmfMesh in gcmf.Submeshes)
                    {
                        var displayList0 = gcmfMesh.DisplayList0.GxDisplayLists;
                        var displayList1 = gcmfMesh.DisplayList1.GxDisplayLists;

                        if (displayList0 != null)
                            foreach (var list in displayList0)
                                submeshes++;

                        if (displayList1 != null)
                            foreach (var list in displayList1)
                                submeshes++;
                    }
                }
            }
            return count;
        }
    }
}