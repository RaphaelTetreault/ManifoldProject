using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCube.GFZ.Stage;

namespace Manifold.Conversion.GFZ.CourseCollision
{
    public static class AnimationClipConverter
    {
        public static UnityEngine.AnimationClip GfzToUnity(GameCube.GFZ.Stage.AnimationClip animationClip)
        {
            var unityAnimClip = new UnityEngine.AnimationClip();

            // 2020/01/29 Raph: refer to notes in notebook.
            // http://gyanendushekhar.com/2018/03/18/create-play-animation-runtime-unity-tutorial/

            // Scale
            var gfxAnimClipScaleX = animationClip.curves[0];
            var gfxAnimClipScaleY = animationClip.curves[1];
            var gfxAnimClipScaleZ = animationClip.curves[2];
            // Rotation
            var gfxAnimClipRotationX = animationClip.curves[3];
            var gfxAnimClipRotationY = animationClip.curves[4];
            var gfxAnimClipRotationZ = animationClip.curves[5];
            // Position
            var gfxAnimClipPositionX = animationClip.curves[6];
            var gfxAnimClipPositionY = animationClip.curves[7];
            var gfxAnimClipPositionZ = animationClip.curves[8];
            // Unknown
            var gfxAnimClipUnused = animationClip.curves[9];
            // Light
            var gfxAnimClipAlpha = animationClip.curves[10];


            var transformType = typeof(Transform);

            // Scale
            SetCurve(unityAnimClip, gfxAnimClipScaleX, transformType, "localScale.x");
            SetCurve(unityAnimClip, gfxAnimClipScaleY, transformType, "localScale.y");
            SetCurve(unityAnimClip, gfxAnimClipScaleZ, transformType, "localScale.z");
            // Rotation
            // localEulerAngles
            SetCurve(unityAnimClip, gfxAnimClipRotationX, transformType, "localRotation.x");
            SetCurve(unityAnimClip, gfxAnimClipRotationY, transformType, "localRotation.y");
            SetCurve(unityAnimClip, gfxAnimClipRotationZ, transformType, "localRotation.z");
            // Position
            SetCurve(unityAnimClip, gfxAnimClipPositionX, transformType, "localPosition.x");
            SetCurve(unityAnimClip, gfxAnimClipPositionY, transformType, "localPosition.y");
            SetCurve(unityAnimClip, gfxAnimClipPositionZ, transformType, "localPosition.z");
            //
            SetCurve(unityAnimClip, gfxAnimClipUnused, transformType, "unused");
            //
            SetCurve(unityAnimClip, gfxAnimClipAlpha, transformType, "alpha");

            return unityAnimClip;
        }

        public static void SetCurve(UnityEngine.AnimationClip unityClip, AnimationClipCurve gfzCurve, Type type, string propertyName)
        {
            // ignore empty anims
            if (gfzCurve.animationCurve.Length == 0)
            {
                return;
            }

            var curve = new UnityEngine.AnimationCurve();
            foreach (var keyableAttribute in gfzCurve.animationCurve.keyableAttributes)
            {
                var time = keyableAttribute.time;
                var value = keyableAttribute.value;

                // TOTAL HACK
                if (propertyName == "localPosition.z")
                {
                    value = -value;
                }

                curve.AddKey(time, value);
                Debug.Log($"time: {keyableAttribute.time}/nValue: {keyableAttribute.value}");
            }
            unityClip.SetCurve("", type, propertyName, curve);
        }
    }
}