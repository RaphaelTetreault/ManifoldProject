using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold
{
    [System.Serializable]
    public class AnimationCurveTransform
    {
        [SerializeField] protected AnimationCurve3 position = new AnimationCurve3();
        [SerializeField] protected AnimationCurve3 rotation = new AnimationCurve3();
        [SerializeField] protected AnimationCurve3 scale = new AnimationCurve3();

        public AnimationCurve3 Position => position;
        public AnimationCurve3 Rotation => rotation;
        public AnimationCurve3 Scale => scale;

        public float GetMaxTime()
        {
            var curves = new List<AnimationCurve>();
            var maxTimes = new List<float>();

            curves.AddRange(position.GetCurves());
            curves.AddRange(rotation.GetCurves());
            curves.AddRange(scale.GetCurves());

            foreach (var curve in position.GetCurves())
            {
                if (curve.length != 0)
                {
                    var maxTime = curve.GetMaxTime();
                    maxTimes.Add(maxTime);
                }
            }

            // start at index 1, fiorst comparision would always be true
            // since it would compare to itself
            for (int i = 1; i < maxTimes.Count; i++)
            {
                bool isSame = maxTimes[i] == maxTimes[0];
                if (!isSame)
                {
                    throw new System.Exception();
                }
            }

            var allCurvesMaxTime = maxTimes.Count == 0 ? 0f : maxTimes[0];
            return allCurvesMaxTime;
        }


        public GameCube.GFZ.CourseCollision.TrackCurves ToTrackCurves()
        {
            var trackCurves = new GameCube.GFZ.CourseCollision.TrackCurves();
            trackCurves.animationCurves = new GameCube.GFZ.CourseCollision.AnimationCurve[9];
            // SCALE
            trackCurves.animationCurves[0] = Manifold.IO.GFZ.AnimationCurveConverter.ToGfz(scale.x);
            trackCurves.animationCurves[1] = Manifold.IO.GFZ.AnimationCurveConverter.ToGfz(scale.y);
            trackCurves.animationCurves[2] = Manifold.IO.GFZ.AnimationCurveConverter.ToGfz(scale.z);
            // ROTATION
            trackCurves.animationCurves[3] = Manifold.IO.GFZ.AnimationCurveConverter.ToGfz(rotation.x);
            trackCurves.animationCurves[4] = Manifold.IO.GFZ.AnimationCurveConverter.ToGfz(rotation.y);
            trackCurves.animationCurves[5] = Manifold.IO.GFZ.AnimationCurveConverter.ToGfz(rotation.z);
            // POSITION
            trackCurves.animationCurves[6] = Manifold.IO.GFZ.AnimationCurveConverter.ToGfz(position.x);
            trackCurves.animationCurves[7] = Manifold.IO.GFZ.AnimationCurveConverter.ToGfz(position.y);
            trackCurves.animationCurves[8] = Manifold.IO.GFZ.AnimationCurveConverter.ToGfz(position.z);

            return trackCurves;
        }

    }
}
