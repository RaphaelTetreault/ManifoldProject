using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCube.GFZ.CourseCollision;

namespace Manifold.Conversion.GFZ.CourseCollision
{
    public static class AnimationConverter
    {
        public static UnityEngine.AnimationClip GfzToUnity(GameCube.GFZ.CourseCollision.AnimationClip animationClip)
        {
            var unityAnimClip = new UnityEngine.AnimationClip();

            // 2020/01/29 Raph: refer to notes in notebook.
            // http://gyanendushekhar.com/2018/03/18/create-play-animation-runtime-unity-tutorial/

            // Scale
            var gfxAnimClipScaleX = animationClip.curve[0];
            var gfxAnimClipScaleY = animationClip.curve[1];
            var gfxAnimClipScaleZ = animationClip.curve[2];
            // Rotation
            var gfxAnimClipRotationX = animationClip.curve[3];
            var gfxAnimClipRotationY = animationClip.curve[4];
            var gfxAnimClipRotationZ = animationClip.curve[5];
            // Position
            var gfxAnimClipPositionX = animationClip.curve[6];
            var gfxAnimClipPositionY = animationClip.curve[7];
            var gfxAnimClipPositionZ = animationClip.curve[8];
            // Unknown
            var gfxAnimClipUnknown = animationClip.curve[9];
            // Light
            var gfxAnimClipLight = animationClip.curve[10];


            var transformType = typeof(UnityEngine.Transform);

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
            //SetCurve(unityAnimClip, gfxAnimClipUnknown, transformType, "localPosition.z");
            //
            //SetCurve(unityAnimClip, gfxAnimClipLight, transformType, "localPosition.z");

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