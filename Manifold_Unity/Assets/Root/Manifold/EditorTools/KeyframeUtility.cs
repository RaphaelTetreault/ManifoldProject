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
        public static Keyframe[] CorrectKeyTangents(Keyframe[] oldKeys, Keyframe[] newKeys)
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

        public static Keyframe[] CorrectKeyTangents2(Keyframe[] oldKeys, Keyframe[] newKeys)
        {
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
                var oldAtanDeg = oldArcTangent* Mathf.Rad2Deg;
                var oldAtan2Deg = oldArcTangent2 * Mathf.Rad2Deg;
                var oldTanOutDeg = oldKeys[curr].outTangent * Mathf.Rad2Deg;
                var oldTanInDeg = oldKeys[next].inTangent * Mathf.Rad2Deg;


                var newDeltaX = (newKeys[next].time - newKeys[curr].time);
                var newDeltaY = (newKeys[next].value - newKeys[curr].value);
                var newTangent = newDeltaY / newDeltaX;
                var newTanOut = newTangent;// * oldOutRatio;
                var newTanIn = newTangent;// * oldInRatio;
                //newKeys[curr].outTangent = Mathf.Atan(newTanOut);
                //newKeys[next].inTangent = Mathf.Atan(newTanIn);

                {
                    float atanOld = Mathf.Atan(oldDeltaY / oldDeltaX);
                    float atanOldOut = Mathf.Atan(oldKeys[curr].outTangent);
                    float atanOldIn = Mathf.Atan(oldKeys[next].inTangent);

                    float deltaOut = atanOldOut - atanOld;
                    float deltaIn = atanOldIn - atanOld;

                    float atanNew = Mathf.Atan(newDeltaY / oldDeltaX);
                    float atanNewOut = atanNew + deltaOut;
                    float atanNewIn = atanNew + deltaIn;

                    newKeys[curr].outTangent = Mathf.Tan(atanNewOut);
                    newKeys[next].inTangent = Mathf.Tan(atanNewIn);
                }



            }
            return newKeys;
        }

        public static Keyframe[] CorrectKeyTangents3XXX(Keyframe[] oldKeys, Keyframe[] newKeys)
        {
            for (int i = 0; i < newKeys.Length - 1; i++)
            {
                int curr = i;
                int next = i + 1;

                // Get TAN for old keys, get ratio between angle and TAN value
                var oldDeltaX = Mathf.Abs(oldKeys[next].time - oldKeys[curr].time);
                var oldDeltaY = Mathf.Abs(oldKeys[next].value - oldKeys[curr].value);
                var oldTan = Mathf.Tan(oldDeltaY / oldDeltaX);
                // Find how many degrees (in radians) the tangent was offset from default TAN(theta)
                // For instance, maybe old tan was 60 degrees and tangent was set to 65. Delta = 5 degrees.
                var oldDeltaOut = Mathf.Atan(oldKeys[curr].outTangent) - oldTan;
                var oldDeltaIn = Mathf.Atan(oldKeys[next].inTangent) - oldTan;
                //var oldRatioOut = oldKeys[curr].outTangent / oldTan;
                //var oldRatioIn = oldKeys[next].inTangent / oldTan;
                //
                var newDeltaX = Mathf.Abs(newKeys[next].time - newKeys[curr].time);
                var newDeltaY = Mathf.Abs(newKeys[next].value - newKeys[curr].value);
                var tanOut = Mathf.Tan(newDeltaY / newDeltaX);
                var tanIn = Mathf.Tan(newDeltaX / newDeltaY);
                // Add degrees offset (in radians) to new TAN(theta)
                // Conitnuing the example: if the old delta was 5 degrees, and our new tangent has a value of
                // of 63.5 degres, we add the 5 degrees back to it.
                var newTanOut = tanOut + oldDeltaOut;
                var newTanIn = tanIn + oldDeltaIn;
                //var newTanOut = newTan * oldRatioOut;
                //var newTanIn = newTan * oldRatioIn;

                // assign to keys
                newKeys[curr].outTangent = Mathf.Atan(newTanOut);
                newKeys[next].inTangent = Mathf.Atan(newTanIn);
                //newKeys[curr].outTangent = newTanOut;
                //newKeys[next].inTangent = newTanIn;
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
            newKeys = CorrectKeyTangents2(keys, newKeys);
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
    }
}
