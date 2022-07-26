using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzUnknownCollider : MonoBehaviour,
        IGfzConvertable<UnknownCollider>
    {
        /// <summary>
        /// Unknown SOLS trigger scale (when compared to default Unity cube).
        /// THIS IS 100% A GUESS.
        /// </summary>
        public const float scale = 27.5f;

        [field: SerializeField] public GfzSceneObject SceneObject { get; private set; }


        public UnknownCollider ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformTRXS(this.transform, Space.World);

            var value = new UnknownCollider
            {
                SceneObject = SceneObject.ExportGfz(),
                Transform = transform
            };

            return value;
        }

        public void ImportGfz(UnknownCollider value)
        {
            transform.CopyGfzTransform(value.Transform, Space.Self);
            transform.localScale *= scale;

            var gobj = GameObject.Find(value.SceneObject.Name);
            SceneObject = gobj == null ? null : gobj.GetComponent<GfzSceneObject>();
            
            if (SceneObject is null)
            DebugConsole.Log("Hack fix did not work.");
        }
    }
}
