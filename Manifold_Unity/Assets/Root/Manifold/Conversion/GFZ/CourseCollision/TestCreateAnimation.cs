using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;

using Manifold.IO;
using Manifold.Conversion.GFZ.CourseCollision;
using Manifold.IO.GFZ.CourseCollision;

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
            var md5 = MD5.Create();

            foreach (var gobj in scene.Value.dynamicSceneObjects)
            {
                if (gobj.animation.curve.Length == 0)
                {
                    continue;
                }

                var clip = AnimationConverter.GfzToUnity(gobj.animation);
                var hash = HashUtility.HashBinary(md5, gobj.animation);
                var name = $"anim_{gobj.nameCopy}_{hash}.anim";
                var path = $"Assets/Untracked/Anim/{name}";
                AssetDatabase.CreateAsset(clip, path);
                Debug.Log(path);

                //break;
            }
        }
    }
}
