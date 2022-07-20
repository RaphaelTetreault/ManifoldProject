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
            for (int i = 0; i < newKeys.Length - 1; i++)
            {
                int curr = i;
                int next = curr + 1;

                var oldDeltaTime = GetDeltaTime(keys[curr], keys[next]);
                var newDeltaTime = GetDeltaTime(newKeys[curr], newKeys[next]);
                var deltaTime = newDeltaTime / oldDeltaTime;

                newKeys[curr].outTangent /= deltaTime;
                newKeys[next].inTangent /= deltaTime;
            }
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
