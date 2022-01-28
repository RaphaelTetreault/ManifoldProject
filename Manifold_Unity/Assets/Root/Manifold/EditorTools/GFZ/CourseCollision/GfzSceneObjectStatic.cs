using GameCube.GFZ.CourseCollision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.CourseCollision
{
    public class GfzSceneObjectStatic : MonoBehaviour,
        IGfzConvertable<SceneObjectStatic>
    {
        [SerializeField] private GfzSceneObject sceneObject;

        public GfzSceneObject GfzSceneObject => sceneObject;

        internal void SetSceneObject(GfzSceneObject sceneObject)
        {
            this.sceneObject = sceneObject;
        }

        public SceneObjectStatic ExportGfz()
        {
            return new SceneObjectStatic()
            {
                sceneObject = sceneObject.ExportGfz(),
            };
        }

        public void ImportGfz(SceneObjectStatic value)
        {
            // nothing!
        }

        private void OnValidate()
        {
            // Enforce how a static object behaves in-editor

            if (transform.position != Vector3.zero)
                transform.position = Vector3.zero;

            if (transform.rotation != Quaternion.identity)
                transform.rotation = Quaternion.identity;

            if (transform.localScale != Vector3.one)
                transform.localScale = Vector3.one;
        }

    }
}
