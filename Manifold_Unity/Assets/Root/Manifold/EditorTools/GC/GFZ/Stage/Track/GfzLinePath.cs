using GameCube.GFZ.Stage;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [ExecuteInEditMode]
    public class GfzLinePath : GfzTrackSegmentRootNode
    {
        [SerializeField] private float endPositionX;
        [SerializeField] private float endPositionY;
        [SerializeField, Min(1f)] private float endPositionZ = 100f;
        [SerializeField] private UnityEngine.AnimationCurve rotationY = CreateDefaultCurve();
        [SerializeField] private UnityEngine.AnimationCurve rotationZ = CreateDefaultCurve();
        [SerializeField] private UnityEngine.AnimationCurve scaleX = CreateDefaultCurve(64);
        [SerializeField] private UnityEngine.AnimationCurve scaleY = CreateDefaultCurve(1);
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();
        //
        [SerializeField, Min(1f)] private float step = 10f;
        [SerializeField] private bool autoGenerateTRS = true;
        [SerializeField] private bool showGizmos = true;

        public float EndPositionX { get => endPositionX; set => endPositionX = value; }
        public float EndPositionY { get => endPositionY; set => endPositionY = value; }
        public float EndPositionZ { get => endPositionZ; set => endPositionZ = value; }
        public float Step { get => step; set => step = value; }
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
            var z = new Keyframe[0];
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

        private void Update()
        {
            if (autoGenerateTRS && transform.hasChanged)
            {
                UpdateTRS();
                UpdateShapeMeshes();
            }
        }


        public void UpdateTrsPosition()
        {
            var maxTime = GetSegmentLength();
            var positionsXYZ = CreatePositionCurvesXYZ(maxTime);
            var rotationsXYZ = new AnimationCurve3(
                animationCurveTRS.Rotation.x.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Rotation.y.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Rotation.z.GetRenormalizedKeyRangeAndTangents(0, maxTime));
            var scaleXYZ = new AnimationCurve3(
                animationCurveTRS.Scale.x.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Scale.y.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Scale.z.GetRenormalizedKeyRangeAndTangents(0, maxTime));

            animationCurveTRS = new AnimationCurveTRS(positionsXYZ, rotationsXYZ, scaleXYZ);
            UpdateShapeMeshes();
        }

        public void UpdateTrsRotation()
        {
            var maxTime = GetMaxTime();
            var positionsXYZ = new AnimationCurve3(
                animationCurveTRS.Position.x.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Position.y.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Position.z.GetRenormalizedKeyRangeAndTangents(0, maxTime));
            var rotationsXYZ = CreateRotationCurvesXYZ(maxTime);
            var scaleXYZ = new AnimationCurve3(
                animationCurveTRS.Scale.x.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Scale.y.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Scale.z.GetRenormalizedKeyRangeAndTangents(0, maxTime));

            animationCurveTRS = new AnimationCurveTRS(positionsXYZ, rotationsXYZ, scaleXYZ);
            UpdateShapeMeshes();
        }

        public void UpdateTrsScale()
        {
            var maxTime = GetMaxTime();
            var positionsXYZ = new AnimationCurve3(
                animationCurveTRS.Position.x.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Position.y.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Position.z.GetRenormalizedKeyRangeAndTangents(0, maxTime));
            var rotationsXYZ = new AnimationCurve3(
                animationCurveTRS.Rotation.x.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Rotation.y.GetRenormalizedKeyRangeAndTangents(0, maxTime),
                animationCurveTRS.Rotation.z.GetRenormalizedKeyRangeAndTangents(0, maxTime));
            var scaleXYZ = CreateScaleCurvesXYZ(maxTime);

            animationCurveTRS = new AnimationCurveTRS(positionsXYZ, rotationsXYZ, scaleXYZ);
            UpdateShapeMeshes();
        }
    }
}
