using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public sealed class GfzSceneObjectDynamic : MonoBehaviour,
        IGfzConvertable<SceneObjectDynamic>
    {
        // NOTE: order below is to make flipping through items in
        // inspector easier. Last entry is variable height.

        [System.Serializable]
        private struct GfzLOD
        {
            public string modelName;
            public MeshFilter model;
            public float lodDistance;
        }


        // Object Reference data
        [Header("Scene Object LOD")]
        [SerializeField] private MeshFilter model;
        [SerializeField] private GfzLOD[] levelOfDetails;
        // Instance data
        [Header("Scene Object")]
        [SerializeField] private LodRenderFlags lodRenderFlags;
        [SerializeField] private MeshFilter colliderMesh;
        [SerializeField] private bool exportColliderMesh;
        [SerializeField] private ColliderGeometry srcColliderMesh;
        // Scene Object data
        [Header("Dynamic Data")]
        [SerializeField] [Hex] private int unk_0x00;
        [SerializeField] [Hex] private int unk_0x04;
        [SerializeField] private Vector2[] textureScrollFields;
        // Hold onto data in the meantime since I don't know how to convert from/to
        [SerializeField] private bool exportSkeletalAnimator;
        [SerializeField] private SkeletalAnimator srcSkeletalAnimator;
        [SerializeField] private bool exportAnimationClip;
        [SerializeField] private GameCube.GFZ.CourseCollision.AnimationClip srcAnimationClip;

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


        public void ImportGfz(SceneObject value)
        {
            throw new System.NotImplementedException();
        }

        public SceneObject ExportGfz()
        {
            //// Get GFZ transform
            //var transform = new GameCube.GFZ.CourseCollision.Transform();
            //transform.CopyUnityTransform(this.transform);

            //// Get transform matrix if not an animated object
            //TransformMatrix3x4 transformMatrix = null;
            //var isNotAnimatedObject = true;
            //if (isNotAnimatedObject)
            //{
            //    transformMatrix.CopyUnityTransform(this.transform);
            //}

            //var SceneObject
            return null;
        }

        //public void SetBaseValues(SceneObjectDynamic value)
        //{
        //    // TRANSFORM
        //    // Copy most reliable transform if available
        //    if (value.transformMatrix3x4 != null)
        //    {
        //        transform.CopyGfzTransformMatrix3x4(value.transformMatrix3x4);
        //    }
        //    else
        //    {
        //        transform.CopyGfzTransformPRXS(value.transformPRXS);
        //    }

        //    // DYNAMIC DATA
        //    {
        //        unk_0x00 = value.unk0x00;
        //        unk_0x04 = value.unk0x04;

        //        // Copy out texture scroll values
        //        if (value.textureScroll != null)
        //        {
        //            textureScrollFields = new Vector2[value.textureScroll.fields.Length];
        //            for (int i = 0; i < textureScrollFields.Length; i++)
        //            {
        //                var item = value.textureScroll.fields[i];
        //                if (item == null)
        //                    continue;

        //                textureScrollFields[i] = new Vector2(item.x, item.y);
        //            }
        //        }

        //        // Copy values in
        //        srcSkeletalAnimator = value.skeletalAnimator;
        //        exportSkeletalAnimator = srcSkeletalAnimator != null;

        //        // Unity will want to create instance, so keep track if we have data
        //        // to export here. Advantage in that we can omit export if we want.
        //        srcAnimationClip = value.animationClip;
        //        exportAnimationClip = srcAnimationClip != null;
        //    }

        //    // SceneObject Data
        //    {
        //        lodRenderFlags = value.sceneObject.lodRenderFlags;
        //        //
        //        var lodCount = value.sceneObject.lods.Length;
        //        levelOfDetails = new GfzLOD[lodCount];
        //        for (int i = 0; i < lodCount; i++)
        //        {
        //            var modelName = value.sceneObject.lods[i].name;
        //            MeshFilter model = null; // AssetDatabaseUtility.GetSobjByOption();
        //            levelOfDetails[i] = new GfzLOD
        //            {
        //                modelName = modelName,
        //                model = model,
        //                lodDistance = value.sceneObject.lods[i].lodDistance,
        //            };
        //        }

        //        srcColliderMesh = value.sceneObject.colliderGeometry;
        //        exportColliderMesh = srcColliderMesh != null;
        //    }

        //    // 
        //    model = GetComponent<MeshFilter>();
        //}

        SceneObjectDynamic IGfzConvertable<SceneObjectDynamic>.ExportGfz()
        {
            var value = new SceneObjectDynamic();
            value.unk0x00 = unk_0x00;
            value.unk0x04 = unk_0x04;

        }

        public void ImportGfz(SceneObjectDynamic value)
        {
            // TRANSFORM
            // Copy most reliable transform if available
            if (value.transformMatrix3x4 != null)
            {
                transform.CopyGfzTransformMatrix3x4(value.transformMatrix3x4);
            }
            else
            {
                transform.CopyGfzTransformPRXS(value.transformPRXS);
            }

            // DYNAMIC DATA
            {
                unk_0x00 = value.unk0x00;
                unk_0x04 = value.unk0x04;

                // Copy out texture scroll values
                if (value.textureScroll != null)
                {
                    textureScrollFields = new Vector2[value.textureScroll.fields.Length];
                    for (int i = 0; i < textureScrollFields.Length; i++)
                    {
                        var item = value.textureScroll.fields[i];
                        if (item == null)
                            continue;

                        textureScrollFields[i] = new Vector2(item.x, item.y);
                    }
                }

                // Copy values in
                srcSkeletalAnimator = value.skeletalAnimator;
                exportSkeletalAnimator = srcSkeletalAnimator != null;

                // Unity will want to create instance, so keep track if we have data
                // to export here. Advantage in that we can omit export if we want.
                srcAnimationClip = value.animationClip;
                exportAnimationClip = srcAnimationClip != null;
            }

            // SceneObject Data
            {
                lodRenderFlags = value.sceneObject.lodRenderFlags;
                //
                var lodCount = value.sceneObject.lods.Length;
                levelOfDetails = new GfzLOD[lodCount];
                for (int i = 0; i < lodCount; i++)
                {
                    var modelName = value.sceneObject.lods[i].name;
                    MeshFilter model = null; // AssetDatabaseUtility.GetSobjByOption();
                    levelOfDetails[i] = new GfzLOD
                    {
                        modelName = modelName,
                        model = model,
                        lodDistance = value.sceneObject.lods[i].lodDistance,
                    };
                }

                srcColliderMesh = value.sceneObject.colliderGeometry;
                exportColliderMesh = srcColliderMesh != null;
            }

            // 
            model = GetComponent<MeshFilter>();
        }



        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = new Color32(255, 0, 0, 127);
        //    Gizmos.DrawSphere(transform.position, unkLod);
        //    Gizmos.DrawWireSphere(transform.position, unkLod * 10f);
        //}

    }
}
