﻿using GameCube.GFZ.Stage;
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
            Vector3 startOffset = new Vector3(0, rotationY.EvaluateMin(), rotationZ.EvaluateMin());
            Vector3 rotation = transform.localRotation.eulerAngles;
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

        public override AnimationCurveTRS CreateAnimationCurveTRS(Scope scope, bool isGfzCoordinateSpace)
        {
            var trs = AnimationCurveTRS.CreateDeepCopy();

            if (scope == Scope.Global)
            {
                var mtx = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                //var invMtx = mtx.inverse;
                // The problem is that each point would need to be rotated :/, along with rotation, etc.
                trs.Position.x = trs.Position.x.CreateValueOffset(pos.x);
                trs.Position.y = trs.Position.y.CreateValueOffset(pos.y);
                trs.Position.z = trs.Position.z.CreateValueOffset(pos.z);
                trs.Rotation.x = trs.Rotation.x.CreateValueOffset(transform.rotation.eulerAngles.x);
                trs.Rotation.y = trs.Rotation.y.CreateValueOffset(transform.rotation.eulerAngles.y);
                trs.Rotation.z = trs.Rotation.z.CreateValueOffset(transform.rotation.eulerAngles.z);
            }

            if (isGfzCoordinateSpace)
                trs = trs.CreateGfzCoordinateSpace();

            return trs;
        }


        public override TrackSegment CreateTrackSegment()
        {
            var trs = CreateAnimationCurveTRS(Scope.Global, false);

            var trackSegment = new TrackSegment();
            trackSegment.SegmentType = TrackSegmentType.IsMatrix;
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
            var x = new Keyframe[] { new(0, 0), new(maxTime, EndPositionX) };
            var y = new Keyframe[] { new(0, 0), new(maxTime, EndPositionY) };
            var z = new Keyframe[] { new(0, 0), new(maxTime, EndPositionZ) };
            var curves = new AnimationCurve3(x, y, z);
            SetAnimationKeysAsLinear(curves);
            return curves;
        }

        public AnimationCurve3 CreateRotationCurvesXYZ(float maxTime)
        {
            var x = new Keyframe[0];
            var y = rotationY.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var z = rotationZ.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var curves = new AnimationCurve3(x, y, z);
            return curves;
        }

        public AnimationCurve3 CreateScaleCurvesXYZ(float maxTime)
        {
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
