using Manifold.EditorTools.GC.GFZ;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// Three AnimationCurve3s combined to represent position, rotation, and scale; a transform.
    /// </summary>
    [System.Serializable]
    public class AnimationCurveTransform
    {
        [SerializeField] protected AnimationCurve3 position = new();
        [SerializeField] protected AnimationCurve3 rotation = new();
        [SerializeField] protected AnimationCurve3 scale = new();

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

            var corrected = GetGfzCoordSpaceAnimTransform();

            trackCurves.PositionX = AnimationCurveConverter.ToGfz(corrected.position.x);
            trackCurves.PositionY = AnimationCurveConverter.ToGfz(corrected.position.y);
            trackCurves.PositionZ = AnimationCurveConverter.ToGfz(corrected.position.z);

            trackCurves.RotationX = AnimationCurveConverter.ToGfz(corrected.rotation.x);
            trackCurves.RotationY = AnimationCurveConverter.ToGfz(corrected.rotation.y);
            trackCurves.RotationZ = AnimationCurveConverter.ToGfz(corrected.rotation.z);

            trackCurves.ScaleX = AnimationCurveConverter.ToGfz(corrected.scale.x);
            trackCurves.ScaleY = AnimationCurveConverter.ToGfz(corrected.scale.y);
            trackCurves.ScaleZ = AnimationCurveConverter.ToGfz(corrected.scale.z);

            return trackCurves;
        }

        public AnimationCurveTransform GetGfzCoordSpaceAnimTransform()
        {
            var p = position.CreateDeepCopy();
            var r = rotation.CreateDeepCopy();
            var s = scale.CreateDeepCopy();

            var convertCoordinateSpace = GfzProjectWindow.GetSettings().ConvertCoordSpace;
            if (convertCoordinateSpace)
            {
                // Position X is inverted compared to Unity
                // 2022/01/31: This does not work with inverting Z axis!
                p.x = p.x.GetInverted();

                // As a result of X's inversion:
                // Rotation Y is inverted compared to Unity
                r.y = r.y.GetInverted();
            }

            return new AnimationCurveTransform()
            {
                position = p,
                rotation = r,
                scale = s,
            };
        }


        public AnimationCurve3 ComputerRotationXY()
        {
            //rotation.x = new AnimationCurve();
            //rotation.y = new AnimationCurve();
            var temp = new AnimationCurve3();

            int interations = 100;
            for (int i = 0; i <= interations; i++)
            {
                var time = (float)((double)i / interations);
                var p0 = Position.Evaluate(time);
                var p1 = Position.Evaluate(time + 0.00001f);
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

        /// <summary>
        /// Continually increases sampling precision until distance gained is less than <paramref name="minDelta"/>
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="minDelta"></param>
        /// <param name="powerBase"></param>
        /// <param name="powerExp"></param>
        /// <returns></returns>
        public float GetDistanceBetweenRepeated(double timeStart, double timeEnd, float minDelta = 0.01f, int powerBase = 2, int powerExp = 1)
        {
            // Limit on how many cycles we can do
            const int maxIterations = 1 << 20; // 2^20 = 1,048,576

            // Values to store distance state between sampling points
            float delta = 0f;
            float currDistance = 0f;
            float prevDistance = 0f;

            do
            {
                // Compute how many samplings to do in this loop iteration
                int iterations = (int)Mathf.Pow(powerBase, powerExp);
                powerExp++;

                // Break if the next iteration goes above max.
                // Since we calculate iteratively, this means we already did this many iterations minus 1.
                if (iterations >= maxIterations)
                {
                    Debug.LogWarning($"Max iterations hit. Delta: {delta}");
                    break;
                }

                // Compute distance between the 2 sampled points on the curve 'iterations' times
                currDistance = GetDistanceBetween(timeStart, timeEnd, iterations);
                delta = Mathf.Abs(currDistance - prevDistance);
                prevDistance = currDistance;
            }
            // Continue this process so long as the distance gained from more precise sampling is more than 'minDelta'
            while (delta >= minDelta);

            // If we stop, 'currDistance' holds the most precise distance value
            return currDistance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="nIterations"></param>
        /// <returns></returns>
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
                var currPosition = position.Evaluate(currDistance);
                var nextPosition = position.Evaluate(nextDistance);
                // Get distance between 2 points, store delta
                var delta = Vector3.Distance(currPosition, nextPosition);
                distance += delta;
            }
            return distance;
        }


    }
}
