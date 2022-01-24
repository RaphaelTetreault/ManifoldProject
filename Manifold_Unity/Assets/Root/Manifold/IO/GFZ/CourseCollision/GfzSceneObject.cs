using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public sealed class GfzSceneObject : MonoBehaviour
    //IGfzConvertable<SceneObject>
    {
        // NOTE: order below is to make flipping through items in
        // inspector easier. Last entry is variable height.

        [System.Serializable]
        private struct GfzLOD
        {
            public string modelName;
            public float lodDistance;
        }


        // Object Reference data
        [Header("Scene Object LOD")]
        [SerializeField] private MeshFilter model;
        [SerializeField] private GfzLOD[] LODs;
        // Instance data
        [Header("Scene Object")]
        [SerializeField] private LodRenderFlags lodRenderFlags;
        [SerializeField] private UnkInstanceOption unk_option = UnkInstanceOption.unk1_default;
        [SerializeField] private MeshFilter colliderMesh;
        // Scene Object data
        [Header("Dynamic Data")]
        [SerializeField] private UnknownObjectBitfield lodFar;
        [SerializeField] private UnknownObjectBitfield lodNear;
        [SerializeField] private Vector2[] field;


        // TODO:
        // + Animation
        // + Skeletal animator
        //

        public MeshFilter Model
        {
            get => model;
            set => model = value;
        }
        public MeshFilter ColliderMesh
        {
            get => colliderMesh;
            set => colliderMesh = value;
        }

        //public SceneObject ExportGfz()
        //{
        //    // Get GFZ transform
        //    var transform = new GameCube.GFZ.CourseCollision.Transform();
        //    transform.CopyUnityTransform(this.transform);

        //    // Get transform matrix if not an animated object
        //    TransformMatrix3x4 transformMatrix = null;
        //    var isNotAnimatedObject = true;
        //    if (isNotAnimatedObject)
        //    {
        //        transformMatrix.CopyUnityTransform(this.transform);
        //    }

        //    var SceneObject 

        //}

        public void SetBaseValues(SceneObjectDynamic value)
        {
            // Copy most reliable transform if available
            if (value.transformMatrix3x4Ptr.IsNotNullPointer)
            {
                transform.CopyGfzTransform(value.transformMatrix3x4);
            }
            else
            {
                transform.CopyGfzTransform(value.transformPRXS);
            }

            // Copy out values
            if (value.textureScroll != null)
            {
                field = new Vector2[value.textureScroll.fields.Length];
                for (int i = 0; i < field.Length; i++)
                {
                    var item = value.textureScroll.fields[i];
                    if (item == null)
                        continue;

                    field[i] = new Vector2(item.x, item.y);
                }
            }

            //
            lodRenderFlags = value.sceneObject.lodRenderFlags;

            //
            var lodCount = value.sceneObject.lods.Length;
            LODs = new GfzLOD[lodCount];
            for (int i = 0; i < lodCount; i++)
            {
                LODs[i] = new GfzLOD
                {
                    modelName = value.sceneObject.lods[i].name,
                    lodDistance = value.sceneObject.lods[i].lodDistance,
                };
            }
        }

        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = new Color32(255, 0, 0, 127);
        //    Gizmos.DrawSphere(transform.position, unkLod);
        //    Gizmos.DrawWireSphere(transform.position, unkLod * 10f);
        //}

    }
}
