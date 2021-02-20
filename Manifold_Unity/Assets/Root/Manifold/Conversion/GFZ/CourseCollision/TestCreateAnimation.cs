using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Manifold.IO;
using Manifold.Conversion.GFZ.CourseCollision;
using Manifold.IO.GFZX01.CourseCollision;

namespace Manifold.Conversion
{
    [CreateAssetMenu]
    public class TestCreateAnimation : ExecutableScriptableObject
    {
        [SerializeField]
        private ColiSceneSobj scene;

        public override string ExecuteText => "Test";

        public override void Execute()
        {
            foreach (var gobj in scene.Value.gameObjects)
            {
                if (gobj.animation.animCurves.Length == 0)
                {
                    continue;
                }

                var clip = AnimationConverter.GfzToUnity(gobj.animation);
                var hash = HashSerializables.Hash(gobj.animation);
                var name = $"anim_{gobj.name}_{hash}.anim";
                var path = $"Assets/Untracked/Anim/{name}";
                AssetDatabase.CreateAsset(clip, path);
                Debug.Log(path);

                //break;
            }
        }
    }
}
