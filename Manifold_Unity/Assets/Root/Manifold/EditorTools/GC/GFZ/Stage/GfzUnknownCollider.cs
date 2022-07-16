using GameCube.GFZ.Stage;
using System.Collections;
using System.Collections.Generic;
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

        // INSPECTOR FIELDS
        [SerializeField] private GfzSceneObject sceneObject;


        // METHODS
        public UnknownCollider ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformTRXS(this.transform);

            var value = new UnknownCollider
            {
                SceneObject = sceneObject.ExportGfz(),
                Transform = transform
            };

            return value;
        }

        public void ImportGfz(UnknownCollider value)
        {
            transform.CopyGfzTransformTRXS(value.Transform);
            transform.localScale *= scale;

            var gobj = GameObject.Find(value.SceneObject.Name);
            sceneObject = gobj == null ? null : gobj.GetComponent<GfzSceneObject>();
            
            if (sceneObject is null)
            DebugConsole.Log("Hack fix did not work.");
        }
    }
}
