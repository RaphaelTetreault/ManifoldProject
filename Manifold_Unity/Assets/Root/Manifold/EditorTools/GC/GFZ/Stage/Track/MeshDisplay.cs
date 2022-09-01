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
        [field: SerializeField] public MeshFilter MeshFilter { get; protected set; }
        [field: SerializeField] public MeshRenderer MeshRenderer { get; protected set; }
        [field: SerializeField] public Mesh Mesh { get; protected set; }
        [field: SerializeField] public Material DefaultMaterial { get; protected set; }
        [field: SerializeField] public bool HideGameObjectInEditor { get; protected set; }


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

            // Hide this gameobject in 
            if (HideGameObjectInEditor)
                gameObject.hideFlags |= HideFlags.HideInHierarchy;
        }

        public void SetHideGameObjectInEditor(bool hideGameObjectInEditor)
        {
            HideGameObjectInEditor = hideGameObjectInEditor;

            // Hide this gameobject in 
            if (HideGameObjectInEditor)
                gameObject.hideFlags |= HideFlags.HideInHierarchy;
        }

        [MenuItem("Manifold/Mesh Tools/Unhide all MeshDisplay")]
        public static void UnhideAll()
        {
            var objs = FindObjectsOfType<MeshDisplay>(true);
            foreach (var obj in objs)
            {
                obj.gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
                obj.HideGameObjectInEditor = false;
            }
        }

        [MenuItem("Manifold/Mesh Tools/Hide all MeshDisplay")]
        public static void HideAll()
        {
            var objs = FindObjectsOfType<MeshDisplay>(true);
            foreach (var obj in objs)
            {
                obj.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                obj.HideGameObjectInEditor = false;
            }
        }

        [MenuItem(itemName: "Manifold/Mesh Tools/Force update meshes _F7")]
        public static void UpdateMeshes()
        {
            var objs = FindObjectsOfType<GfzSegmentShape>();
            foreach (var obj in objs)
            {
                obj.UpdateMesh();
            }
        }

    }
}
