using Manifold.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// Three AnimationCurve3s combined to represent position, rotation, and scale; a transform.
    /// </summary>
    [System.Serializable]
    public class AnimationCurveTRS :
         IDeepCopyable<AnimationCurveTRS>
    {
        [field: SerializeField] public AnimationCurve3 Position { get; private set; } = new();
        [field: SerializeField] public AnimationCurve3 Rotation { get; private set; } = new();
        [field: SerializeField] public AnimationCurve3 Scale { get; private set; } = new();


        public AnimationCurveTRS() { }
        public AnimationCurveTRS(AnimationCurve3 position, AnimationCurve3 rotation, AnimationCurve3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }


        public AnimationCurve[] AnimationCurves
        {
            get => new AnimationCurve[]
            {
                Position.x, Position.y, Position.z,
                Rotation.x, Rotation.y, Rotation.z,
                Scale.x, Scale.y, Scale.z,
            };
        }


        public AnimationCurve this[int i]
        {
            get => AnimationCurves[i];
            set => AnimationCurves[i] = value;
        }


        public float GetMaxTime()
        {
            var curves = new List<AnimationCurve>();
            var maxTimes = new List<float>();

            curves.AddRange(Position.GetCurves());
            curves.AddRange(Rotation.GetCurves());
            curves.AddRange(Scale.GetCurves());

            foreach (var curve in Position.GetCurves())
            {
                if (curve.length != 0)
                {
                    var maxTime = curve.GetMaxTime();
                    maxTimes.Add(maxTime);
                }
            }

            // start at index 1, first comparision would always be true
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

        public GameCube.GFZ.Stage.AnimationCurveTRS ToTrackSegment()
        {
            var trackCurves = new GameCube.GFZ.Stage.AnimationCurveTRS();
            var rightHanded = CreateGfzCoordinateSpace();

            trackCurves.PositionX = AnimationCurveConverter.ToGfz(rightHanded.Position.x);
            trackCurves.PositionY = AnimationCurveConverter.ToGfz(rightHanded.Position.y);
            trackCurves.PositionZ = AnimationCurveConverter.ToGfz(rightHanded.Position.z);

            trackCurves.RotationX = AnimationCurveConverter.ToGfz(rightHanded.Rotation.x);
            trackCurves.RotationY = AnimationCurveConverter.ToGfz(rightHanded.Rotation.y);
            trackCurves.RotationZ = AnimationCurveConverter.ToGfz(rightHanded.Rotation.z);

            trackCurves.ScaleX = AnimationCurveConverter.ToGfz(rightHanded.Scale.x);
            trackCurves.ScaleY = AnimationCurveConverter.ToGfz(rightHanded.Scale.y);
            trackCurves.ScaleZ = AnimationCurveConverter.ToGfz(rightHanded.Scale.z);

            return trackCurves;
        }

        public AnimationCurveTRS CreateGfzCoordinateSpace()
        {
            // Something people seem to get wrong is the coordinate space of GFZ.
            // Many believe you need to invert the X axis in a scene. However, this
            // then requires you to add 180 degrees (rotate around). This is not
            // how coordinate spaces work!
            //
            // In reality, it is the Z axis which is inverted. Taking ST01 as an example,
            // the first turn goes +X/right. In the pre-mirrored coordinate space (at least, 
            // in Unity), this is correct. What isn't is that, since Z is still mirrored,
            // if you place yourself on the track (rather than looking at it from global
            // space), then it appears to go left. However, once you invert Z, the turn
            // goes right from taht perspective. Once this is done, rotations about the X and Y
            // axis also need to be inverted (due to inverted handidness). Once done,
            // you're good to go! Z+ is forward, X+ is right, and Y+ stays upwards.

            var p = Position.CreateDeepCopy();
            var r = Rotation.CreateDeepCopy();
            var s = Scale.CreateDeepCopy();

            p.z = p.z.CreateInverted();
            r.x = r.x.CreateInverted();
            r.y = r.y.CreateInverted();

            return new AnimationCurveTRS()
            {
                Position = p,
                Rotation = r,
                Scale = s,
            };
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
                var currPosition = Position.Evaluate(currDistance);
                var nextPosition = Position.Evaluate(nextDistance);
                // Get distance between 2 points, store delta
                var delta = Vector3.Distance(currPosition, nextPosition);
                distance += delta;
            }
            return distance;
        }

        public Matrix4x4 EvaluateMatrix(float time)
        {
            var matrix = new Matrix4x4();
            var p = Position.Evaluate(time);
            var r = Rotation.Evaluate(time);
            var s = Scale.EvaluateDefault(time, Vector3.one);
            matrix.SetTRS(p, Quaternion.Euler(r), s);
            return matrix;
        }

        public Matrix4x4 EvaluateMatrix(double time)
        {
            return EvaluateMatrix((float)time);
        }

        public void AddKeys(float time, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position.AddKeys(time, position);
            Rotation.AddKeys(time, rotation);
            Scale.AddKeys(time, scale);
        }

        public void CleanDuplicateKeys()
        {
            // Clean keys
            foreach (var animationCurve in AnimationCurves)
                CleanDuplicateKeys(animationCurve);

            // ISSUE: empty curves are used, does not fall back on static transform!

            //foreach (var curve in Position.CurvesXYZ)
            //    DeleteKeysIfDefault(curve, 0f);
            //foreach (var curve in Rotation.CurvesXYZ)
            //    DeleteKeysIfDefault(curve, 0f);
            //foreach (var curve in Scale.CurvesXYZ)
            //    DeleteKeysIfDefault(curve, 1f);
        }

        private void CleanDuplicateKeys(AnimationCurve animationCurve)
        {
            const float valueDelta = 1e-4f;
            const float tangentDelta = 5e-3f;
            var keysToRemove = new List<int>();
            int sameKeyValuesCount = 0;
            int maxRemovableKeys = animationCurve.keys.Length - 2;
            for (int i = 0; i < maxRemovableKeys; i++)
            {
                var key0 = animationCurve.keys[i + 0];
                var key1 = animationCurve.keys[i + 1];
                var key2 = animationCurve.keys[i + 2];

                bool isSameValueAsKey0 = IsBetween(key1.value, key0.value - valueDelta, key0.value + valueDelta);
                bool isSameValueAsKey2 = IsBetween(key1.value, key2.value - valueDelta, key2.value + valueDelta);
                if (isSameValueAsKey0 && isSameValueAsKey2)
                    sameKeyValuesCount++;
                else
                    continue;

                bool isSameTangentIn = IsBetween(key1.inTangent, key0.outTangent - tangentDelta, key0.outTangent + tangentDelta);
                bool isSameTangentOut = IsBetween(key1.outTangent, key2.inTangent - tangentDelta, key2.inTangent + tangentDelta);
                if (isSameTangentIn && isSameTangentOut)
                    keysToRemove.Add(i + 1);
            }

            // Any key that has value and tangent similar to prev and next.
            // In removing all in-between keys, we shift the index as array shrinks.
            for (int i = 0; i < keysToRemove.Count; i++)
            {
                int index = keysToRemove[i] - i;
                animationCurve.RemoveKey(index);
            }

            // If all removable keys want to be removed, we have a flat line.
            bool isFlatAnimationCurve = keysToRemove.Count == maxRemovableKeys;
            if (isFlatAnimationCurve)
            {
                // Smooth both tangents
                for (int i = 0; i < animationCurve.keys.Length; i++)
                {
                    animationCurve.SmoothTangents(i, 1);
                }
            }
        }

        private void DeleteKeysIfDefault(AnimationCurve animationCurve, float defaultValue)
        {
            // If keys are zero, remove keys
            bool isValueDefault =
                animationCurve.keys[0].value == defaultValue &&
                animationCurve.keys[1].value == defaultValue;
            if (isValueDefault)
            {
                animationCurve.RemoveKey(0);
                animationCurve.RemoveKey(0);
            }
        }

        private bool IsBetween(float value, float min, float max)
        {
            bool isGreaterThanMin = value > min;
            bool isLessThanMax = value < max;
            bool isBetween = isGreaterThanMin && isLessThanMax;
            return isBetween;
        }

        public AnimationCurveTRS CreateDeepCopy()
        {
            var deepcopy = new AnimationCurveTRS()
            {
                Position = Position.CreateDeepCopy(),
                Rotation = Rotation.CreateDeepCopy(),
                Scale = Scale.CreateDeepCopy(),
            };
            return deepcopy;
        }

        //public static AnimationCurveTRS CreateDefault(float maxTime)
        //{
        //    var trs = new AnimationCurveTRS();
        //    trs.Position.AddKeys(0, Vector3.zero); 
        //    trs.Position.AddKeys(maxTime, Vector3.zero);
        //    trs.Rotation.AddKeys(0, Vector3.zero);
        //    trs.Rotation.AddKeys(maxTime, Vector3.zero);
        //    trs.Scale.AddKeys(0, Vector3.one);
        //    trs.Scale.AddKeys(maxTime, Vector3.one);
        //    return trs;
        //}
    }
}
