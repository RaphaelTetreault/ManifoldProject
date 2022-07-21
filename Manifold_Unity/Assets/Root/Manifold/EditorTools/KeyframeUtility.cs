using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools
{
    public static class KeyframeUtility
    {
        /// <summary>
        /// Given two sets of keyframes, will correct the tangents of <paramref name="newKeys"/> based
        /// on the tangents of <paramref name="oldKeys"/>. It does so by solving the difference in time
        /// from <paramref name="oldKeys"/> to <paramref name="newKeys"/> and scaling accordingly.
        /// </summary>
        /// <param name="oldKeys"></param>
        /// <param name="newKeys"></param>
        /// <returns></returns>
        public static Keyframe[] ScaleKeyTangents(Keyframe[] oldKeys, Keyframe[] newKeys)
        {
            for (int i = 0; i < newKeys.Length - 1; i++)
            {
                int curr = i;
                int next = curr + 1;

                var oldDeltaTime = GetDeltaTime(oldKeys[curr], oldKeys[next]);
                var newDeltaTime = GetDeltaTime(newKeys[curr], newKeys[next]);
                var deltaTime = newDeltaTime / oldDeltaTime;

                newKeys[curr].outTangent /= deltaTime;
                newKeys[next].inTangent /= deltaTime;
            }
            return newKeys;
        }

        public static Keyframe[] RecomputeKeyTangents(Keyframe[] oldKeys, Keyframe[] newKeys)
        {
            // one such error is that key tangents appear to be different, means borken tangents.
            // Also, downward-slope tangents seem to be wrecked due to inversion?

            for (int i = 0; i < newKeys.Length - 1; i++)
            {
                int curr = i;
                int next = i + 1;

                // Get TAN for old keys, get ratio between angle and TAN value
                var oldDeltaX = Mathf.Abs(oldKeys[next].time - oldKeys[curr].time);
                var oldDeltaY = Mathf.Abs(oldKeys[next].value - oldKeys[curr].value);
                var oldTangent = Mathf.Tan(oldDeltaY / oldDeltaX);
                var oldTangent2 = Mathf.Tan(oldDeltaX / oldDeltaY);
                var oldArcTangent = Mathf.Atan(oldDeltaY / oldDeltaX);
                var oldArcTangent2 = Mathf.Atan(oldDeltaX / oldDeltaY);

                var oldTanDeg = oldTangent * Mathf.Rad2Deg;
                var oldTan2Deg = oldTangent2 * Mathf.Rad2Deg;
                var oldAtanDeg = oldArcTangent * Mathf.Rad2Deg;
                var oldAtan2Deg = oldArcTangent2 * Mathf.Rad2Deg;
                var oldTanOutDeg = oldKeys[curr].outTangent * Mathf.Rad2Deg;
                var oldTanInDeg = oldKeys[next].inTangent * Mathf.Rad2Deg;


                var newDeltaX = (newKeys[next].time - newKeys[curr].time);
                var newDeltaY = (newKeys[next].value - newKeys[curr].value);

                {
                    float atanOld = Mathf.Atan(oldDeltaY / oldDeltaX);
                    float atanOldOut = Mathf.Atan(oldKeys[curr].outTangent);
                    float atanOldIn = Mathf.Atan(oldKeys[next].inTangent);

                    float deltaOut = atanOldOut - atanOld;
                    float deltaIn = atanOldIn - atanOld;

                    float atanNew = Mathf.Atan(newDeltaY / newDeltaX);
                    float atanNewOut = atanNew + deltaOut;
                    float atanNewIn = atanNew + deltaIn;

                    newKeys[curr].outTangent = Mathf.Tan(atanNewOut);
                    newKeys[next].inTangent = Mathf.Tan(atanNewIn);
                    newKeys[curr].outTangent = atanNewOut;
                    newKeys[next].inTangent = atanNewIn;
                }



            }
            return newKeys;
        }

        public static Keyframe[] GetRenormalizedKeyRange(Keyframe[] keys, float newMinTime, float newMaxTime)
        {
            // If no keys, return empty array
            if (keys.Length == 0)
                return new Keyframe[0];

            var currMinTime = GetMinTime(keys);
            var currMaxTime = GetMaxTime(keys);
            var currTimeRange = currMaxTime - currMinTime;
            var newTimeRange = newMaxTime - newMinTime;

            var newKeys = new List<Keyframe>();
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var keyTimeNormalized = (key.time - currMinTime) / currTimeRange;
                var newKeyTime = keyTimeNormalized * newTimeRange;

                // In case timeRange is 0, resulting in div0
                if (float.IsNaN(newKeyTime))
                    newKeyTime = 0;

                key.time = newKeyTime;
                newKeys.Add(key);
            }

            return newKeys.ToArray();
        }

        public static Keyframe[] GetRenormalizedKeyRangeAndTangents(Keyframe[] keys, float newMinTime, float newMaxTime)
        {
            var newKeys = GetRenormalizedKeyRange(keys, newMinTime, newMaxTime);
            newKeys = ScaleKeyTangents(keys, newKeys);
            // TODO: use this, but first, fix
            //newKeys = RecomputeKeyTangents(keys, newKeys);
            return newKeys;
        }



        private static float GetDeltaTime(Keyframe a, Keyframe b)
        {
            var aTime = a.time;
            var bTime = b.time;
            var deltaTime = Mathf.Abs(aTime - bTime);
            return deltaTime;
        }

        public static float GetMinTime(Keyframe[] keys)
        {
            // Can error if keys.length is [0]
            if (keys.IsNullOrEmpty())
                return 0f;

            var minTime = keys[0].time;
            return minTime;
        }

        public static float GetMaxTime(Keyframe[] keys)
        {
            // Can error if keys.length is [0]
            if (keys.IsNullOrEmpty())
                return 0f;

            // Get max time value, normalize input time
            var lastIndex = keys.Length - 1;
            var maxTime = keys[lastIndex].time;
            return maxTime;
        }

        public static Keyframe[] InvertedKeys(Keyframe[] keys)
        {
            var invertedKeys = new Keyframe[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                // copy all misc data
                invertedKeys[i] = keys[i];
                // Invert values
                invertedKeys[i].value = -keys[i].value;
                invertedKeys[i].inTangent = -keys[i].inTangent;
                invertedKeys[i].outTangent = -keys[i].outTangent;
            }

            return invertedKeys;
        }

        public static Keyframe[] OffsetKeyValues(Keyframe[] keys, float valueOffset)
        {
            var offsetKeys = new Keyframe[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                offsetKeys[i] = keys[i];
                offsetKeys[i].value += valueOffset;
            }

            return offsetKeys;
        }

        public static Keyframe[] SetKeyTangents(Keyframe[] keys, float inOutTangent)
        {
            var offsetKeys = new Keyframe[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                offsetKeys[i] = keys[i];
                offsetKeys[i].inTangent = inOutTangent;
                offsetKeys[i].outTangent = inOutTangent;
            }

            return offsetKeys;
        }

        //public static Keyframe[] SubdivideCurve(AnimationCurve animationCurve, int subdivisions)
        //{
        //    var keys = new List<Keyframe>();
        //    for (int i = 0; i < animationCurve.length - 1; i++)
        //    {
        //        var key0 = animationCurve.keys[i + 0];
        //        var key1 = animationCurve.keys[i + 1];

        //        bool isDifferent =
        //            key0.value != key1.value ||
        //            key0.inTangent != key1.inTangent ||
        //            key0.outTangent != key1.outTangent;

        //        if (!isDifferent)
        //            continue;

        //        float startTime = key0.time;
        //        float endTime = key1.time;
        //        for (int subd = 0; subd < subdivisions; subd++)
        //        {
        //            float time = (float)subd / subdivisions;
        //            float keyTime = Mathf.Lerp(startTime, endTime, time);
        //            var value = animationCurve.Evaluate(keyTime);
        //            var key = new Keyframe(keyTime, value);
        //            keys.Add(key);
        //        }
        //    }

        //    var tempCurve = new AnimationCurve(animationCurve.keys);
        //    foreach (var key in keys)
        //    {
        //        int index = tempCurve.AddKey(key);
        //        if (index > 0)
        //            tempCurve.SmoothTangents(index, 1f / 3f);
        //    }

        //    return tempCurve.keys;
        //}
    }
}
