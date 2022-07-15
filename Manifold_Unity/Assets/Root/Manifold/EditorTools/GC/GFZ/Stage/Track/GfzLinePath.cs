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
        [SerializeField] private float startRotationY;
        [SerializeField] private float startRotationZ;
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
            rotation += new Vector3(0, startRotationY, startRotationZ);
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
            Vector3 offset = GetRotationOffset();
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
            var curves = new AnimationCurve3();
            curves.x = new UnityEngine.AnimationCurve(new(0, 0), new(maxTime, EndPositionX));
            curves.y = new UnityEngine.AnimationCurve(new(0, 0), new(maxTime, EndPositionY));
            curves.z = new UnityEngine.AnimationCurve(new(0, 0), new(maxTime, EndPositionZ));
            SetAnimationKeysAsLinear(curves);

            return curves;
        }

        public AnimationCurve3 CreateRotationCurvesXYZ(float maxTime)
        {
            var curves = new AnimationCurve3();
            curves.x = new UnityEngine.AnimationCurve(new(0, 0             ), new(maxTime, 0           ));
            curves.y = new UnityEngine.AnimationCurve(new(0, startRotationY), new(maxTime, EndRotationY));
            curves.z = new UnityEngine.AnimationCurve(new(0, startRotationZ), new(maxTime, EndRotationZ));
            SetAnimationKeysAsLinear(curves);

            return curves;
        }

        public AnimationCurve3 CreateScaleCurvesXYZ(float maxTime)
        {
            var x = scaleCurvesXYZ.x.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var y = scaleCurvesXYZ.y.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var z = scaleCurvesXYZ.z.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var curves = new AnimationCurve3(x, y, z);
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
                curve.keys = curve.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            }
        }

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
