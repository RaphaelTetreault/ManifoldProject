﻿using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzAnimationClip : MonoBehaviour,
        IGfzConvertable<GameCube.GFZ.Stage.AnimationClip>
    {
        [SerializeField] private float unk_0x00; // guess: anim start time?
        [SerializeField] private float unk_0x04; // max anim time in frames (60 per second)
        [SerializeField] private EnumFlags32 unk_layer_0x18;
        [SerializeField]
        private GfzAnimationClipCurve
            scaleX = new GfzAnimationClipCurve(),
            scaleY = new GfzAnimationClipCurve(),
            scaleZ = new GfzAnimationClipCurve(),
            rotationX = new GfzAnimationClipCurve(),
            rotationY = new GfzAnimationClipCurve(),
            rotationZ = new GfzAnimationClipCurve(),
            positionX = new GfzAnimationClipCurve(),
            positionY = new GfzAnimationClipCurve(),
            positionZ = new GfzAnimationClipCurve(),
            unknown = new GfzAnimationClipCurve(),
            alpha = new GfzAnimationClipCurve();

        private GfzAnimationClipCurve[] GetCurves => new GfzAnimationClipCurve[]
        {
            scaleX, scaleY, scaleZ,
            rotationX, rotationY, rotationZ,
            positionX, positionY, positionZ,
            unknown, alpha
        };


        [System.Serializable]
        private class GfzAnimationClipCurve
        {
            public uint unk0x00;
            public uint unk0x04;
            public uint unk0x08;
            public uint unk0x0C;
            public UnityEngine.AnimationCurve curve;
        }


        public GameCube.GFZ.Stage.AnimationClip ExportGfz()
        {
            var animationClip = new GameCube.GFZ.Stage.AnimationClip();
            animationClip.Unk_0x00 = unk_0x00;
            animationClip.Unk_0x04 = unk_0x04;
            animationClip.Unk_layer_0x18 = unk_layer_0x18;

            var curves = GetCurves;
            animationClip.Curves = new AnimationClipCurve[curves.Length];
            for (int i = 0; i < curves.Length; i++)
            {
                animationClip.Curves[i] = new AnimationClipCurve()
                {
                    Unk_0x00 = curves[i].unk0x00,
                    Unk_0x04 = curves[i].unk0x04,
                    Unk_0x08 = curves[i].unk0x08,
                    Unk_0x0C = curves[i].unk0x0C,
                    AnimationCurve = AnimationCurveConverter.ToGfz(curves[i].curve),
                };
            }

            //// Flip z positions
            //var curvePositionZ = animationClip.curves[6].animationCurve;
            //if (curvePositionZ != null)
            //{
            //    for (int i = 0; i < curvePositionZ.Length; i++)
            //    {
            //        var key = curvePositionZ.keyableAttributes[i];
            //        key.value = -key.value;
            //    }
            //}

            return animationClip;
        }

        public void ImportGfz(GameCube.GFZ.Stage.AnimationClip animationClip)
        {
            unk_0x00 = animationClip.Unk_0x00;
            unk_0x04 = animationClip.Unk_0x04;
            unk_layer_0x18 = animationClip.Unk_layer_0x18;

            var curves = GetCurves;
            for (int i = 0; i < GameCube.GFZ.Stage.AnimationClip.kAnimationCurvesCount; i++)
            {
                curves[i].unk0x00 = animationClip.Curves[i].Unk_0x00;
                curves[i].unk0x04 = animationClip.Curves[i].Unk_0x04;
                curves[i].unk0x08 = animationClip.Curves[i].Unk_0x08;
                curves[i].unk0x0C = animationClip.Curves[i].Unk_0x0C;
                curves[i].curve = AnimationCurveConverter.ToUnity(animationClip.Curves[i].AnimationCurve);
            }

            //var curvePositionZ = curves[6].curve;
            //if (curvePositionZ != null)
            //{
            //    for (int i = 0; i < curvePositionZ.length; i++)
            //    {
            //        var key = curvePositionZ.keys[i];
            //        curvePositionZ.keys[i] = new Keyframe(key.time, -key.value);
            //    }
            //}
        }

    }
}
