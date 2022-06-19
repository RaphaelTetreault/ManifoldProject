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
        [SerializeField] private string sceneObject;

        // PROPERTIES
        public string SceneObject
        {
            get => sceneObject;
            set => sceneObject = value;
        }

        // METHODS
        public UnknownCollider ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformTRXS(this.transform);

            throw new System.NotImplementedException();
            var value = new UnknownCollider
            {
                //sceneObject = sceneObject,
                Transform = transform
            };

            return value;
        }

        public void ImportGfz(UnknownCollider value)
        {
            transform.CopyGfzTransformTRXS(value.Transform);
            transform.localScale *= scale;
            sceneObject = value.SceneObject.Name;
        }
    }
}