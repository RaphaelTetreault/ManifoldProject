using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using GameCube.GFZX01.Camera;
using Manifold.IO.GFZX01.Camera;
using System.IO;

namespace Manifold.IO.Camera
{
    [CreateAssetMenu(menuName =MenuConst.GFZX01_Camera + "Camera Pan Permuter")]
    public class CameraPermutationGenerator : ExecutableScriptableObject
    {
        [SerializeField]
        private CameraPanSobj @base;

        public override string ExecuteText => "Generate Camera Pan Permutations";

        public override void Execute()
        {
            var currPath = AssetDatabase.GetAssetPath(@base);

            for (int i = 0; i < 32; i++)
            {
                var newPath = currPath;
                newPath = newPath.Substring(0, newPath.Length - ".asset".Length);
                newPath += $" - FROM 0x1C {i + 1}.asset";

                AssetDatabase.CopyAsset(currPath, newPath);
                var obj = AssetDatabase.LoadAssetAtPath<CameraPanSobj>(newPath);

                // Generate new single flag
                obj.value.from.modifier = (CameraPanModifier)(1 << i);
            }

            for (int i = 0; i < 32; i++)
            {
                var newPath = currPath;
                newPath = newPath.Substring(0, newPath.Length - ".asset".Length);
                newPath += $" - TO 0x1C {i + 1}.asset";

                AssetDatabase.CopyAsset(currPath, newPath);
                var obj = AssetDatabase.LoadAssetAtPath<CameraPanSobj>(newPath);

                // Generate new single flag
                obj.value.to.modifier = (CameraPanModifier)(1 << i);
            }

        }

    }
}