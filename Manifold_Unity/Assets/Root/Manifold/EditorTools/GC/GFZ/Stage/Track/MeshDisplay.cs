using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshDisplay : MonoBehaviour
    {
        public const string DefaultMaterialPath = "Assets/Root/Manifold/EditorTools/GC/GFZ/Materials/mat_VertexColor.mat";

        [field: Header("Mesh Properties")]
        [field: SerializeField, ReadOnlyGUI] public MeshFilter MeshFilter { get; protected set; }
        [field: SerializeField, ReadOnlyGUI] public MeshRenderer MeshRenderer { get; protected set; }
        [field: SerializeField, ReadOnlyGUI] public Mesh Mesh { get; protected set; }
        [field: SerializeField] public Material DefaultMaterial { get; protected set; }


        public void UpdateMesh(Mesh mesh)
        {
            // Update to new mesh
            Mesh = mesh;
            MeshFilter.sharedMesh = mesh;

            int numTristrips = Mesh.subMeshCount;
            var materials = new Material[numTristrips];
            for (int i = 0; i < materials.Length; i++)
                materials[i] = DefaultMaterial;
            MeshRenderer.sharedMaterials = materials;
        }
        public void UpdateMesh(Mesh mesh, Material[] sharedMaterials)
        {
            Mesh = mesh;
            MeshFilter.sharedMesh = mesh;

            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i] == null)
                {
                    sharedMaterials[i] = DefaultMaterial;
                }
            }
            MeshRenderer.sharedMaterials = sharedMaterials;
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (MeshRenderer == null)
                MeshRenderer = GetComponent<MeshRenderer>();

            if (MeshFilter == null)
                MeshFilter = GetComponent<MeshFilter>();

            if (DefaultMaterial == null)
                DefaultMaterial = AssetDatabase.LoadAssetAtPath<Material>(DefaultMaterialPath);
        }

        [MenuItem(GfzMenuItems.Stage.DeleteOldMeshDisplay, priority = GfzMenuItems.Stage.Priority.DeleteOldMeshDisplay)]
        public static void DeleteOldMeshDisplays()
        {
            var objs = FindObjectsOfType<MeshDisplay>(true);
            foreach (var obj in objs)
            {
                if (obj.name == "Mesh Display")
                {
                    DestroyImmediate(obj.gameObject);
                }
            }

            var shapes = FindObjectsOfType<GfzShape>(true);
            foreach (var shape in shapes)
            {
                shape.ValidateMeshDisplay();
                shape.UpdateMesh();
                EditorUtility.SetDirty(shape);
            }
        }
    }
}
