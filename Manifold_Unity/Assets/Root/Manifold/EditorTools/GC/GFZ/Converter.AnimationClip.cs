using GameCube.GFZ.Stage;
using System;
using UnityEngine;

namespace Manifold.Conversion.GFZ.CourseCollision
{
    public static class AnimationClipConverter
    {
        public static UnityEngine.AnimationClip GfzToUnity(GameCube.GFZ.Stage.AnimationClip animationClip)
        {
            // 2020/01/29 Raph: refer to notes in notebook.
            // http://gyanendushekhar.com/2018/03/18/create-play-animation-runtime-unity-tutorial/

            Assert.IsTrue(animationClip.Unused is null);

            var unityAnimClip = new UnityEngine.AnimationClip();
            var transformType = typeof(Transform);

            SetCurve(unityAnimClip, animationClip.RotationX, transformType, "localRotation.x");
            SetCurve(unityAnimClip, animationClip.RotationY, transformType, "localRotation.y");
            SetCurve(unityAnimClip, animationClip.RotationZ, transformType, "localRotation.z");
            SetCurve(unityAnimClip, animationClip.PositionX, transformType, "localPosition.x");
            SetCurve(unityAnimClip, animationClip.PositionY, transformType, "localPosition.y");
            SetCurve(unityAnimClip, animationClip.PositionZ, transformType, "localPosition.z");
            SetCurve(unityAnimClip, animationClip.ScaleX, transformType, "localScale.x");
            SetCurve(unityAnimClip, animationClip.ScaleY, transformType, "localScale.y");
            SetCurve(unityAnimClip, animationClip.ScaleZ, transformType, "localScale.z");
            SetCurve(unityAnimClip, animationClip.Alpha, typeof(Material), "_Color.a");

            return unityAnimClip;
        }

        public static void SetCurve(UnityEngine.AnimationClip unityClip, AnimationClipCurve gfzCurve, Type type, string propertyName)
        {
            // ignore empty anims
            if (gfzCurve.AnimationCurve.Length == 0)
                return;

            var unityCurve = new UnityEngine.AnimationCurve();
            foreach (var keyableAttribute in gfzCurve.AnimationCurve.KeyableAttributes)
            {
                var time = keyableAttribute.Time;
                var value = keyableAttribute.Value;
                unityCurve.AddKey(time, value);
            }
            unityClip.SetCurve("", type, propertyName, unityCurve);
        }
    }
}