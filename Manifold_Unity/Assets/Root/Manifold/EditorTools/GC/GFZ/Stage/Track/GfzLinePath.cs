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
        [SerializeField, Min(1f)] private float endPositionZ = 100f;
        [SerializeField] private UnityEngine.AnimationCurve rotationY = CreateDefaultCurve();
        [SerializeField] private UnityEngine.AnimationCurve rotationZ = CreateDefaultCurve();
        [SerializeField] private UnityEngine.AnimationCurve scaleX = CreateDefaultCurve(64);
        [SerializeField] private UnityEngine.AnimationCurve scaleY = CreateDefaultCurve(1);
        [SerializeField] private UnityEngine.AnimationCurve scaleZ = CreateDefaultCurve(1);
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();
        //
        [SerializeField, Min(1f)] private float step = 10f;

        public float EndPositionX { get => endPositionX; set => endPositionX = value; }
        public float EndPositionY { get => endPositionY; set => endPositionY = value; }
        public float EndPositionZ { get => endPositionZ; set => endPositionZ = value; }
        public float Step { get => step; set => step = value; }
        //public AnimationCurve3 ScaleCurvesXYZ { get => scaleCurvesXYZ; private set => scaleCurvesXYZ = value; }
        public AnimationCurveTRS AnimationCurveTRS { get => animationCurveTRS; private set => animationCurveTRS = value; }



        private static UnityEngine.AnimationCurve CreateDefaultCurve(float defaultValue = 0)
        {
            return new UnityEngine.AnimationCurve(new(0, defaultValue), new(1, defaultValue));
        }

        public Vector3 GetPositionStart() => GetPosition();

        public Vector3 GetPositionOffset()
        {
            Vector3 offset = new Vector3(endPositionX, endPositionY, endPositionZ);
            return offset;
        }
        public Vector3 GetPositionEnd()
        {
            Vector3 start = GetPositionStart();
            Vector3 offset = GetPositionOffset();
            Quaternion rotation = GetRotation();
            offset = rotation * offset;
            Vector3 end = start + offset;
            return end;
        }

        public Vector3 GetRotationStart()
        {
            Vector3 startOffset = new Vector3(0, rotationY.EvaluateMin(), rotationZ.EvaluateMin());
            Vector3 rotation = GetRotation().eulerAngles;
            Vector3 finalRotation = rotation + startOffset;
            return finalRotation;
        }
        public Vector3 GetRotationOffset()
        {
            Vector3 offset = new Vector3(0, rotationY.EvaluateMax(), rotationZ.EvaluateMax());
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
            return trs;
        }

        public override TrackSegment CreateTrackSegment()
        {
            //var trs = AnimationCurveTRS.CreateDeepCopy();

            //var trackSegmentX = new TrackSegment();
            //var trackSegmentY = new TrackSegment();
            //var trackSegmentZ = new TrackSegment();
            //trackSegmentX.Children = new TrackSegment[] { trackSegmentY };
            //trackSegmentY.Children = new TrackSegment[] { trackSegmentZ };
            //trackSegmentZ.Children = CreateChildTrackSegments();

            //trackSegmentX.BranchIndex =
            //trackSegmentY.BranchIndex =
            //trackSegmentZ.BranchIndex = GetBranchIndex();

            //{
            //    var trsX = new AnimationCurveTRS();
            //    trsX.Position.x = new UnityEngine.AnimationCurve(trs.Position.x.keys);
            //    trsX.Rotation.x = new UnityEngine.AnimationCurve(trs.Rotation.x.keys);
            //    trsX.Scale.x = new UnityEngine.AnimationCurve(trs.Scale.x.keys);
            //    trackSegmentX.AnimationCurveTRS = trsX.ToTrackSegment();
            //}
            //{
            //    var trsY = new AnimationCurveTRS();
            //    trsY.Position.y = new UnityEngine.AnimationCurve(trs.Position.y.keys);
            //    trsY.Rotation.y = new UnityEngine.AnimationCurve(trs.Rotation.y.keys);
            //    trsY.Scale.y = new UnityEngine.AnimationCurve(trs.Scale.y.keys);
            //    trackSegmentY.AnimationCurveTRS = trsY.ToTrackSegment();
            //}
            //{
            //    var trsZ = new AnimationCurveTRS();
            //    trsZ.Position.z = new UnityEngine.AnimationCurve(trs.Position.z.keys);
            //    trsZ.Rotation.z = new UnityEngine.AnimationCurve(trs.Rotation.z.keys);
            //    trsZ.Scale.z = new UnityEngine.AnimationCurve(trs.Scale.z.keys);
            //    trackSegmentZ.AnimationCurveTRS = trsZ.ToTrackSegment();
            //}

            //return trackSegmentX;

            var trs = AnimationCurveTRS.CreateDeepCopy();

            var trackSegmentRoot = new TrackSegment();
            var trackSegmentRZ = new TrackSegment();
            trackSegmentRoot.Children = new TrackSegment[] { trackSegmentRZ };
            trackSegmentRZ.Children = CreateChildTrackSegments();

            trackSegmentRoot.BranchIndex = trackSegmentRZ.BranchIndex = GetBranchIndex();

            {
                var trsXY = trs.CreateDeepCopy();
                trsXY.Rotation.z = new UnityEngine.AnimationCurve();
                trackSegmentRoot.AnimationCurveTRS = trsXY.ToTrackSegment();
            }
            {
                var trsRZ = new AnimationCurveTRS();
                trsRZ.Rotation.z = trs.Rotation.z;
                trackSegmentRZ.AnimationCurveTRS = trsRZ.ToTrackSegment();
            }

            return trackSegmentRoot;
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
            var position0 = GetPositionStart();
            var position1 = GetPositionEnd();
            var x = new Keyframe[] { new(0, position0.x), new(maxTime, position1.x) };
            var y = new Keyframe[] { new(0, position0.y), new(maxTime, position1.y) };
            var z = new Keyframe[] { new(0, position0.z), new(maxTime, position1.z) };
            var curves = new AnimationCurve3(x, y, z);
            SetAnimationKeysAsLinear(curves);
            return curves;
        }

        public AnimationCurve3 CreateRotationCurvesXYZ(float maxTime)
        {
            var rotation = GetRotation().eulerAngles;
            var x = new Keyframe[] { new(0, rotation.x) };
            var y = rotationY.CreateValueOffset(rotation.y).GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var z = rotationZ.CreateValueOffset(rotation.z).GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var curves = new AnimationCurve3(x, y, z);
            return curves;
        }

        public AnimationCurve3 CreateScaleCurvesXYZ(float maxTime)
        {
            // TODO: multiply keys function
            var x = scaleX.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var y = scaleY.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var z = scaleZ.GetRenormalizedKeyRangeAndTangents(0, maxTime);
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

        private void Reset()
        {
            UpdateTRS();
        }
    }
}
