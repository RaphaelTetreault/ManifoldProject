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
            var gfxAnimClipScaleX = animationClip.animCurves[0];
            var gfxAnimClipScaleY = animationClip.animCurves[1];
            var gfxAnimClipScaleZ = animationClip.animCurves[2];
            // Rotation
            var gfxAnimClipRotationX = animationClip.animCurves[3];
            var gfxAnimClipRotationY = animationClip.animCurves[4];
            var gfxAnimClipRotationZ = animationClip.animCurves[5];
            // Position
            var gfxAnimClipPositionX = animationClip.animCurves[6];
            var gfxAnimClipPositionY = animationClip.animCurves[7];
            var gfxAnimClipPositionZ = animationClip.animCurves[8];
            // Unknown
            var gfxAnimClipUnknown = animationClip.animCurves[9];
            // Light
            var gfxAnimClipLight = animationClip.animCurves[10];


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

        public static void SetCurve(UnityEngine.AnimationClip unityClip, AnimationCurvePlus gfzCurve, Type type, string propertyName)
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