using Manifold.EditorTools.GC.GFZ;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
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
            trackCurves.animationCurves[0] = AnimationCurveConverter.ToGfz(scale.x);
            trackCurves.animationCurves[1] = AnimationCurveConverter.ToGfz(scale.y);
            trackCurves.animationCurves[2] = AnimationCurveConverter.ToGfz(scale.z);
            // ROTATION
            trackCurves.animationCurves[3] = AnimationCurveConverter.ToGfz(rotation.x);
            trackCurves.animationCurves[4] = AnimationCurveConverter.ToGfz(rotation.y);
            trackCurves.animationCurves[5] = AnimationCurveConverter.ToGfz(rotation.z);
            // POSITION
            trackCurves.animationCurves[6] = AnimationCurveConverter.ToGfz(position.x);
            trackCurves.animationCurves[7] = AnimationCurveConverter.ToGfz(position.y);
            trackCurves.animationCurves[8] = AnimationCurveConverter.ToGfz(position.z);

            //// rotation Y is inverted
            //foreach (var key in trackCurves.animationCurves[4].keyableAttributes)
            //{
            //    key.value = -key.value;
            //    key.zTangentIn = -key.zTangentIn;
            //    key.zTangentOut = -key.zTangentOut;
            //}

            //// invert Z axis
            //foreach (var key in trackCurves.animationCurves[8].keyableAttributes)
            //{
            //    key.value = -key.value;
            //    key.zTangentIn = -key.zTangentIn;
            //    key.zTangentOut = -key.zTangentOut;
            //}

            return trackCurves;
        }

        public AnimationCurve3 ComputerRotationXY()
        {
            //rotation.x = new AnimationCurve();
            //rotation.y = new AnimationCurve();
            var temp = new AnimationCurve3();

            int interations = 100;
            for (int i = 0; i < interations; i++)
            {
                var time = (float)((double)i / interations);
                var p0 = Position.EvaluateNormalized(time);
                var p1 = Position.EvaluateNormalized(time + 0.00001f);
                var forward = (p1 - p0).normalized;

                var zUp = rotation.z.EvaluateNormalized(time);
                var up = Quaternion.Euler(0, 0, zUp) * Vector3.up;
                var orientation = Quaternion.LookRotation(forward, up);
                var eulers = orientation.eulerAngles;

                temp.x.AddKey(time, eulers.x);
                temp.y.AddKey(time, eulers.y);
                temp.z.AddKey(time, eulers.z);
            }

            return temp;
        }





        public float GetDistanceBetweenRepeated(double timeStart, double timeEnd, int baseIterations)
        {
            int loopCount = 0;
            const int maxIterations = 1_000_000;
            const float minDiff = 0.01f; // 1cm

            float delta;
            float distance;
            float prevDistance = 0f;
            do
            {
                loopCount++;
                int power = loopCount;
                int iterations = (int)Mathf.Pow(baseIterations, power);
                Debug.Log($"Base: {baseIterations}, Power: {power}, Iterations: {iterations}");

                distance = GetDistanceBetween(timeStart, timeEnd, iterations);
                delta = Mathf.Abs(distance - prevDistance);
                prevDistance = distance;

                if (iterations >= maxIterations)
                {
                    Debug.LogError($"Max iterations hit. Delta: {delta}");
                    break;
                }
            }
            while (delta >= minDiff);

            return distance;
        }


        // (curveTimeOffset + checkpointTimeDelta) = curveTimeOffset
        public float GetDistanceBetween(double timeStart, double timeEnd, int nIterations)
        {
            // If we start at, say, 0.3 and end at 0.45, get the difference or total "step"
            var timeDelta = timeEnd - timeStart;

            var distance = 0f;
            for (int i = 0; i <= nIterations; i++)
            {
                // Get the curve times for current + increment towards next checkpoint
                // Each sampling begins at 'timeStart' (for example, 0.3 / 1.0)
                // Each sampling ends at 'timeEnd' (for example, 0.45 / 1.0)
                // We sample from that start to the next iteration. For instance, if we
                // iterate 100 times, and the start is from 0.3000 to 0.3015 since we
                // will travel 0.15s through time from start to end, and each iteration
                // is 1/100.
                var currDistance = timeStart + (timeDelta / nIterations * (i + 0));
                var nextDistance = timeStart + (timeDelta / nIterations * (i + 1));
                // Compute the distance between these 2 points
                var currPosition = position.EvaluateNormalized(currDistance);
                var nextPosition = position.EvaluateNormalized(nextDistance);
                // Get distance between 2 points, store delta
                var delta = Vector3.Distance(currPosition, nextPosition);
                distance += delta;
            }
            return distance;
        }


    }
}
