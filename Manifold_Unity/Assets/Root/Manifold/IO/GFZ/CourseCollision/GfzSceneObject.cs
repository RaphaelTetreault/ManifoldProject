using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public sealed class GfzSceneObject : MonoBehaviour
    //IGfzConvertable<SceneObject>
    {
        // NOTE: order below is to make flipping through items in
        // inspector easier. Last entry is variable height.

        // Object Reference data
        [Header("Object Reference Data")]
        [SerializeField] private MeshFilter model;
        [SerializeField] private float unkLod;
        // Instance data
        [Header("Instance Data")]
        [SerializeField] private UnkInstanceFlag unk_flag;
        [SerializeField] private UnkInstanceOption unk_option = UnkInstanceOption.unk1_default;
        [SerializeField] private MeshFilter colliderMesh;
        // Scene Object data
        [Header("SceneObject Data")]
        [SerializeField] private UnknownObjectBitfield lodFar;
        [SerializeField] private UnknownObjectBitfield lodNear;
        [SerializeField] private Vector2[] unkData;
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
                transform.CopyGfzTransform(value.transform);
            }

            // Scene object data
            //lodNear = value.lodNear;
            //lodFar = value.lodFar;

            // Copy out values
            if (value.textureMetadataPtr.IsNotNullPointer)
            {
                unkData = new Vector2[value.textureMetadata.fields.Length];
                for (int i = 0; i < unkData.Length; i++)
                {
                    var item = value.textureMetadata.fields[i];
                    unkData[i] = new Vector2(item.x, item.y);
                }
            }

            // Instance data
            unk_flag = value.templateSceneObject.unk_0x00;
            unk_option = value.templateSceneObject.unk_0x04;

            // Reference data
            unkLod = value.templateSceneObject.sceneObject.unk_0x0C;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color32(255, 0, 0, 127);
            Gizmos.DrawSphere(transform.position, unkLod);
            Gizmos.DrawWireSphere(transform.position, unkLod * 10f);
        }

    }
}
