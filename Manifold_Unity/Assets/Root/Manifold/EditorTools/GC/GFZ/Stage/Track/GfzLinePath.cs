using GameCube.GFZ.Stage;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzLinePath : GfzTrackSegmentRootNode
    {
        [SerializeField] private float endPositionX;
        [SerializeField] private float endPositionY;
        [SerializeField, Min(1f)] private float endPositionZ = 1f;
        [SerializeField] private float endRotationY;
        [SerializeField] private float endRotationZ;
        [SerializeField] private AnimationCurve3 scaleCurvesXYZ = new();
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();

        public float EndPositionX { get => endPositionX; set => endPositionX = value; }
        public float EndPositionY { get => endPositionY; set => endPositionY = value; }
        public float EndPositionZ { get => endPositionZ; set => endPositionZ = value; }
        public float EndRotationY { get => endRotationY; set => endRotationY = value; }
        public float EndRotationZ { get => endRotationZ; set => endRotationZ = value; }
        public AnimationCurve3 ScaleCurvesXYZ { get => scaleCurvesXYZ; private set => scaleCurvesXYZ = value; }
        public AnimationCurveTRS AnimationCurveTRS { get => animationCurveTRS; private set => animationCurveTRS = value; }

        [field: SerializeField]
        public float Step { get; set; } = 10f;


        public Vector3 GetPositionStart()
        {
            return transform.localPosition;
        }
        public Vector3 GetPositionOffset()
        {
            Vector3 offset = new Vector3(endPositionX, endPositionY, endPositionZ);
            return offset;
        }
        public Vector3 GetPositionEnd()
        {
            Vector3 start = GetPositionStart();
            Vector3 offset = GetPositionOffset();
            offset = transform.localRotation * offset;
            Vector3 end = start + offset;
            return end;
        }

        public Vector3 GetRotationStart()
        {
            Vector3 rotation = transform.localRotation.eulerAngles;
            return rotation;
        }
        public Vector3 GetRotationOffset()
        {
            Vector3 offset = new Vector3(0, endRotationY, endRotationZ);
            return offset;
        }
        public Vector3 GetRotationEnd()
        {
            Vector3 start = GetRotationStart();
            Vector3 offset = GetRotationEnd();
            Vector3 end = start + offset;
            return end;
        }

        public float GetLineLength()
        {
            Vector3 start = GetPositionStart();
            Vector3 end = GetPositionEnd();
            float length = Vector3.Distance(start, end);
            return length;
        }

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            var trs = isGfzCoordinateSpace
                ? animationCurveTRS.CreateGfzCoordinateSpace()
                : animationCurveTRS.CreateDeepCopy();

            if (isGfzCoordinateSpace)
                trs = trs.CreateGfzCoordinateSpace();

            return trs;
        }

        public override TrackSegment CreateTrackSegment()
        {
            var trs = AnimationCurveTRS.CreateDeepCopy();

            var trackSegment = new TrackSegment();
            trackSegment.SegmentType = TrackSegmentType.IsMatrix;
            trackSegment.LocalPosition = transform.localPosition;
            trackSegment.LocalRotation = transform.localRotation.eulerAngles;
            trackSegment.LocalScale = transform.localScale;
            trackSegment.AnimationCurveTRS = trs.ToTrackSegment();
            trackSegment.BranchIndex = GetBranchIndex();
            trackSegment.Children = CreateChildTrackSegments();

            return trackSegment;
        }

        public override float GetSegmentLength()
        {
            var length = GetLineLength();
            return length;
        }

        public override float GetMaxTime()
        {
            var length = animationCurveTRS.GetMaxTime();
            return length;
        }

        public AnimationCurve3 CreatePositionCurvesXYZ(float maxTime)
        {
            var positionOffset = GetPositionOffset();

            var curves = new AnimationCurve3();
            curves.x = new UnityEngine.AnimationCurve(new(0, 0), new(maxTime, positionOffset.x));
            curves.y = new UnityEngine.AnimationCurve(new(0, 0), new(maxTime, positionOffset.y));
            curves.z = new UnityEngine.AnimationCurve(new(0, 0), new(maxTime, positionOffset.z));
            SetAnimationKeysAsLinear(curves);

            return curves;
        }

        public AnimationCurve3 CreateRotationCurvesXYZ(float maxTime)
        {
            // For curve, start is always zero, static matrix has Transform component
            var rotationOffset = GetRotationOffset();
            var curves = new AnimationCurve3();
            curves.x = new UnityEngine.AnimationCurve(new(0, 0), new(maxTime, rotationOffset.x));
            curves.y = new UnityEngine.AnimationCurve(new(0, 0), new(maxTime, rotationOffset.y));
            curves.z = new UnityEngine.AnimationCurve(new(0, 0), new(maxTime, rotationOffset.z));
            SetAnimationKeysAsLinear(curves);

            return curves;
        }

        public AnimationCurve3 CreateScaleCurvesXYZ(float maxTime)
        {
            var copy = scaleCurvesXYZ.CreateDeepCopy();
            var x = new UnityEngine.AnimationCurve(GetKeysRenormalized(copy.x, maxTime));
            var y = new UnityEngine.AnimationCurve(GetKeysRenormalized(copy.y, maxTime));
            var z = new UnityEngine.AnimationCurve(GetKeysRenormalized(copy.z, maxTime));

            var curves = new AnimationCurve3()
            {
                x = x,
                y = y,
                z = z,
            };

            return curves;
        }

        public void SetAnimationKeysAsLinear(AnimationCurve3 curves)
        {
            foreach (var curve in curves.GetCurves())
            {
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                    AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                    curve.SmoothTangents(i, 1 / 3f);
                }
            }
        }

        public Keyframe[] GetRenormalizedKeyRange(UnityEngine.AnimationCurve curve, float newMaxTime)
        {
            if (curve.length == 0)
                return new Keyframe[0];

            var currMinTime = curve.GetMinTime();
            var currMaxTime = curve.GetMaxTime();
            var timeRange = currMaxTime - currMinTime;

            var newKeys = new List<Keyframe>();
            for (int i = 0; i < curve.keys.Length; i++)
            {
                var key = curve.keys[i];
                var keyTimeNormalized = (key.time - currMinTime) / timeRange;
                var newKeyTime = keyTimeNormalized * newMaxTime;

                // In case timeRange is 0, resulting in div0
                if (float.IsNaN(newKeyTime))
                    newKeyTime = 0;

                key.time = newKeyTime;
                newKeys.Add(key);
            }

            return newKeys.ToArray();
        }

        public Keyframe[] GetKeysRenormalized(UnityEngine.AnimationCurve curve, float newMaxTime)
        {
            var oldKeys = curve.keys;
            var newKeys = GetRenormalizedKeyRange(curve, newMaxTime);

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

        public float GetDeltaTime(Keyframe a, Keyframe b)
        {
            var aTime = a.time;
            var bTime = b.time;
            var deltaTime = MathF.Abs(aTime - bTime);
            return deltaTime;
        }


        internal void UpdateTRS()
        {
            var maxTime = GetLineLength();
            var position = CreatePositionCurvesXYZ(maxTime);
            var rotation = CreateRotationCurvesXYZ(maxTime);
            var scale = CreateScaleCurvesXYZ(maxTime);
            var trs = new AnimationCurveTRS(position, rotation, scale);
            animationCurveTRS = trs;
        }

        internal void UpdateMaxKeyTimes()
        {
            var maxTime = GetMaxTime();
            foreach (var curve in animationCurveTRS.AnimationCurves)
            {
                curve.keys = GetKeysRenormalized(curve, maxTime);
            }
        }

        //private Vector2 KeyToVector2(Keyframe keyframe)
        //{
        //    var vector2 = new Vector2(keyframe.time, keyframe.value);
        //    return vector2;
        //}

        //private float GetAtanBetween(Keyframe from, Keyframe to)
        //{
        //    float x = to.time - from.time;
        //    float y = to.value - from.value;
        //    float angle = y / x;
        //    float tangentRadians = Mathf.Atan(angle * Mathf.Deg2Rad);
        //    float tangentDegrees = tangentRadians * Mathf.Rad2Deg;
        //    return tangentDegrees;
        //}

        // NOTE: isintangent could be figure out based on from index < or > to index
        private Keyframe[] SmoothCircleTangent(UnityEngine.AnimationCurve curve, int indexFrom, int indexTo, bool isInTangent)
        {
            var keys = curve.keys;
            float x = keys[indexTo].time - keys[indexFrom].time;
            float y = keys[indexTo].value - keys[indexFrom].value;
            float angle = y / x;
            float tangentRadians = Mathf.Atan(angle * Mathf.Deg2Rad);
            float tangentDegrees = tangentRadians * Mathf.Rad2Deg;
            float tangentSmooth = tangentDegrees * (5 / 3f);

            if (isInTangent)
                keys[indexTo].inTangent = tangentSmooth;
            else
                keys[indexFrom].outTangent = tangentSmooth;

            return keys;
        }

        internal Keyframe[] SmoothCircleTangentToNext(UnityEngine.AnimationCurve curve, int index)
        {
            bool indexTooLow = index < 0;
            bool indexTooHigh = index >= curve.length - 1;
            if (indexTooLow || indexTooHigh)
                return curve.keys;

            return SmoothCircleTangent(curve, index, index + 1, false);
        }

        internal Keyframe[] SmoothCircleTangentFromPrevious(UnityEngine.AnimationCurve curve, int index)
        {
            bool indexTooLow = index <= 0;
            bool indexTooHigh = index >= curve.length;
            if (indexTooLow || indexTooHigh)
                return curve.keys;

            return SmoothCircleTangent(curve, index, index - 1, true);
        }
    }
}
