﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzBezierSplineSegment : SegmentGenerator,
        IPositionEvaluable
    {
        [SerializeField]
        private List<BezierPoint> points;

        [SerializeField, HideInInspector]
        private bool isLoop = false;

        [SerializeField, HideInInspector]
        private AnimationCurve widthsCurve = new();

        [SerializeField, HideInInspector]
        private AnimationCurve heightsCurve = new();

        [SerializeField, HideInInspector]
        private AnimationCurve rollsCurve = new();

        //
        [SerializeField]
        private bool viewDirection = true;

        [Min(1)]
        [SerializeField]
        private int viewDirectionArrowsPerCurve = 1;

        [Min(10f)]
        [SerializeField]
        private float viewDirectionScale = 50f;

        [Range(0.02f, 0.1f)]
        [SerializeField]
        private float bezierHandleSize = 0.06f;

        [Range(1f, 5f)]
        [SerializeField]
        private float splineThickness = 3f;

        [Range(1f, 3f)]
        [SerializeField]
        private float outterLineThickness = 2f;


        // TODO
        //      add widths => animation curve
        //      add rolls  => animation curve
        //      add option for loop? Extend array by 1, copy first into last, last into first

        public bool IsLoop
        {
            get => isLoop;
        }

        public bool ViewDirection
        {
            get => viewDirection;
            set => viewDirection = value;
        }

        public AnimationCurve WidthsCurve
        {
            get => widthsCurve;
            set => widthsCurve = value;
        }

        public AnimationCurve HeightsCurve
        {
            get => heightsCurve;
            set => heightsCurve = value;
        }

        public AnimationCurve RollsCurve
        {
            get => rollsCurve;
            set => rollsCurve = value;
        }


        public int PointsCount
        {
            get => points.Count;
        }

        public int CurveCount
        {
            get => points.Count - 1;
        }

        public int LoopCurveCount
        {
            get => isLoop
                ? CurveCount - 1
                : CurveCount;
        }

        public int ViewDirectionArrowsPerCurve { get => viewDirectionArrowsPerCurve; set => viewDirectionArrowsPerCurve = value; }
        public float ViewDirectionScale { get => viewDirectionScale; set => viewDirectionScale = value; }
        public float BezierHandleSize { get => bezierHandleSize; set => bezierHandleSize = value; }
        public float SplineThickness { get => splineThickness; set => splineThickness = value; }
        public float OutterLineThickness { get => outterLineThickness; set => outterLineThickness = value; }

        public BezierPoint GetBezierPoint(int index)
        {
            return points[index];
        }

        public void SetBezierPoint(int index, BezierPoint point)
        {
            points[index] = point;
        }


        public AnimationCurve CreateWidthsCurve()
        {
            var curve = new AnimationCurve();
            for (int i = 0; i < points.Count; i++)
            {
                var key = new Keyframe()
                {
                    time = i,
                    value = points[i].width,
                };
                curve.AddKey(key);
            }
            return curve;
        }

        public AnimationCurve CreateHeightsCurve()
        {
            var curve = new AnimationCurve();
            for (int i = 0; i < points.Count; i++)
            {
                var key = new Keyframe()
                {
                    time = i,
                    value = points[i].height,
                };
                curve.AddKey(key);
            }
            return curve;
        }

        public AnimationCurve CreateRollsCurve()
        {
            var curve = new AnimationCurve();
            for (int i = 0; i < points.Count; i++)
            {
                var key = new Keyframe()
                {
                    time = i,
                    value = points[i].roll,
                };
                curve.AddKey(key);
            }
            return curve;
        }



        public (float time, int index) NormalizedTimeToTimeAndIndex(float t)
        {
            int index;

            // if time is above normalized limit
            if (t >= 1f)
            {
                // clamp time
                t = 1f;
                // get final index
                index = points.Count - 2;
            }
            else
            {
                // Clamp time, convert normalized time into curve segment index
                t = Mathf.Clamp01(t) * CurveCount;
                // Cast time to int, turns time into curve index
                index = (int)t;
                // Place T back into 0-1 range
                t -= index;
            }

            return (t, index);
        }

        public Vector3 GetPositionRelative(float time01)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time01);
            var positionRelative = GetPositionRelative(t, i);
            return positionRelative;
        }

        public Vector3 GetPositionRelative(float time, int index)
        {
            var position = GetPosition(time, index);
            var positionRelative = transform.TransformPoint(position);
            return positionRelative;
        }

        public Vector3 GetPosition(float time01)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time01);
            var position = GetPosition(t, i);
            return position;
        }

        public Vector3 GetPosition(float time, int index)
        {
            var bezier0 = points[index + 0];
            var bezier1 = points[index + 1];
            var p0 = bezier0.position;
            var p1 = bezier0.outTangent;
            var p2 = bezier1.inTangent;
            var p3 = bezier1.position;

            var position = Bezier.GetPoint(p0, p1, p2, p3, time);
            return position;
        }


        public Vector3 GetVelocity(float time01)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time01);
            var velocity = GetVelocity(t, i);
            return velocity;
        }

        public Vector3 GetVelocity(float time, int index)
        {
            var bezier0 = points[index + 0];
            var bezier1 = points[index + 1];
            var p0 = bezier0.position;
            var p1 = bezier0.outTangent;
            var p2 = bezier1.inTangent;
            var p3 = bezier1.position;

            var firstDerivitive = Bezier.GetFirstDerivative(p0, p1, p2, p3, time);

            var origin = transform.position;
            var velocity = transform.TransformPoint(firstDerivitive) - origin;
            return velocity;
        }

        public Vector3 GetDirection(float time01)
        {
            return GetVelocity(time01).normalized;
        }

        public Vector3 GetDirection(float time, int index)
        {
            return GetVelocity(time, index).normalized;
        }


        public Quaternion GetOrientation(float time, int index)
        {
            var xy = Quaternion.LookRotation(GetDirection(time, index));
            var z = Quaternion.Euler(0, 0, rollsCurve.Evaluate(index + time));
            var orientation = xy * z;
            // Re-orient as per transform orientation
            orientation = orientation * transform.rotation;
            return orientation;
        }

        public Quaternion GetOrientation(float time01)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time01);
            var orientation = GetOrientation(t, i);
            return orientation;
        }

        public Vector3 GetScale(float time, int index)
        {
            var width = widthsCurve.Evaluate(index + time);
            var height = heightsCurve.Evaluate(index + time);
            var scale = new Vector3(width, height, 1f);
            return scale;
        }

        public Vector3 GetScale(float time01)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time01);
            var scale = GetScale(t, i);
            return scale;
        }

        public void AddPointAtEnd()
        {
            var lastIndex = points.Count - 1;
            var lastBezier = points[lastIndex];
            var length = (lastBezier.outTangent - lastBezier.position).magnitude;
            // Get the direction of the final spline curve point
            var direction = GetDirection(1f);

            //
            var newBezier = new BezierPoint();
            newBezier.position = lastBezier.position + direction * length * 2f;
            newBezier.tangentMode = BezierControlPointMode.Mirrored;
            newBezier.inTangent = newBezier.position - direction * length / 4f;
            newBezier.outTangent = newBezier.position + direction * length / 4f;
            newBezier.width = lastBezier.width;
            newBezier.roll = lastBezier.roll;

            points.Insert(lastIndex + 1, newBezier);
        }

        public void AddPointAtStart()
        {
            var firstBezier = points[0];
            var length = (firstBezier.outTangent - firstBezier.position).magnitude;
            // Get the direction of the final spline curve point, point position
            var direction = GetDirection(0f);

            //
            var newBezier = new BezierPoint();
            newBezier.position = firstBezier.position - direction * length * 2f;
            newBezier.tangentMode = BezierControlPointMode.Mirrored;
            newBezier.inTangent = newBezier.position - direction * length / 4f;
            newBezier.outTangent = newBezier.position + direction * length / 4f;
            newBezier.width = firstBezier.width;
            newBezier.roll = firstBezier.roll;

            points.Insert(0, newBezier);
        }

        public void RemovePoint(int index)
        {
            points.RemoveAt(index);
        }

        public void InsertPoint(int index)
        {
            var bezier0 = points[index + 0];
            var bezier1 = points[index + 1];
            var bezier = new BezierPoint();

            var direction = GetDirection(0.5f, index) * 50f;
            bezier.position = GetPositionRelative(0.5f, index);
            bezier.inTangent = bezier.position - direction;
            bezier.outTangent = bezier.position + direction;
            bezier.tangentMode = BezierControlPointMode.Mirrored;
            bezier.width = (bezier0.width + bezier1.width) / 2f;
            bezier.roll = (bezier0.roll + bezier1.roll) / 2f;

            points.Insert(index + 1, bezier);
        }

        public void InsertBefore(int index)
        {
            if (index == 0)
            {
                AddPointAtStart();
            }
            else
            {
                InsertPoint(index - 1);
            }

            if (IsLoop)
            {
                // Conform to loop, make last point same as first
                points[points.Count - 1] = points[0];
            }
        }

        public void InsertAfter(int index)
        {
            bool isLastIndex = (index >= points.Count - 1);
            if (isLastIndex && !IsLoop)
            {
                AddPointAtEnd();
            }
            else
            {
                InsertPoint(index - 1);
            }

            if (IsLoop)
            {
                // Conform to loop, make first point same as last
                points[0] = points[points.Count - 1];
            }
        }


        public void SetLoop(bool isLoop)
        {
            if (this.isLoop == isLoop)
                return;

            if (isLoop)
            {
                var firstBezierPoint = points[0];
                points.Add(firstBezierPoint);
            }
            else
            {
                int lastIndex = points.Count - 1;
                points.RemoveAt(lastIndex);
            }

            this.isLoop = isLoop;
        }

        public void Reset()
        {
            points = new List<BezierPoint>()
            {
                // Mandatory first node
                new BezierPoint()
                {
                    tangentMode = BezierControlPointMode.Mirrored,
                    position = new Vector3(0f, 0f, 0f),
                    inTangent = new Vector3(0f, 0f, -100f),
                    outTangent = new Vector3(0f, 0f, 100f),
                    width = 64f,
                    height = 1f,
                    roll = 0f,
                },

                // First point which can form a curve with the start
                new BezierPoint()
                {
                    tangentMode = BezierControlPointMode.Mirrored,
                    position = new Vector3(0f, 0f, 400f),
                    inTangent = new Vector3(0f, 0f, 300f),
                    outTangent = new Vector3(0f, 0f, 500f),
                    width = 64f,
                    height = 1f,
                    roll = 0f,
                },
            };
        }


        public override AnimationCurveTRS GetAnimationCurveTRS()
        {
            var trs = new AnimationCurveTRS();

            int numSegments = points.Count - 1;
            double[] distances = new double[numSegments];
            double totalDistance = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                double timeStart = (double)(i + 0) / numSegments;
                double timeEnd = (double)(i + 1) / numSegments;
                double distance = CurveLengthUtility.GetDistanceBetweenRepeated(this, timeStart, timeEnd); // consider raising powerExp to 4?
                distances[i] = distance;
                totalDistance += distance;
                Debug.Log($"Distance {i}: {distance}");
            }

            Debug.Log("Total distance: " + totalDistance);

            var lastRotation = GetOrientation(0, 0).eulerAngles;
            double currDistance = 0;
            for (int i = 0; i < numSegments; i++)
            {
                float currLength = (float)distances[i];

                // TODO: add and use samples per curve in bezier point
                for (int s = 0; s < 32; s++)
                {
                    var t = (float)(s + 0) / 32;
                    var position = GetPosition(t, i);
                    var scale = GetScale(t, i);
                    var rotation = GetOrientation(t, i).eulerAngles;
                    rotation = CleanRotation(lastRotation, rotation);
                    lastRotation = rotation;

                    var timeNormalized = (currDistance + (t * currLength)) / totalDistance;
                    trs.AddKeys((float)timeNormalized, position, rotation, scale);
                }

                currDistance += distances[i];
            }

            // Add last key at time = 1f
            {
                float t = 1f;
                int i = numSegments - 1;
                var position = GetPosition(t, i);
                var scale = GetScale(t, i);
                var rotation = GetOrientation(t, i).eulerAngles;
                rotation = CleanRotation(lastRotation, rotation);
                trs.AddKeys(t, position, rotation, scale);
            }

            return trs;
        }

        public float3 EvaluatePosition(double time)
        {
            return GetPositionRelative((float)time);
        }

        public Vector3 CleanRotation(Vector3 lastEulers, Vector3 currEulers)
        {
            var x = CleanRotation(lastEulers.x, currEulers.x);
            var y = CleanRotation(lastEulers.y, currEulers.y);
            var z = CleanRotation(lastEulers.z, currEulers.z);
            return new Vector3(x, y, z);
        }

        public float CleanRotation(float lastAngle, float currAngle)
        {
            float delta = currAngle - lastAngle;

            if (delta > 45)
            {
                currAngle -= 360f;
            }
            else if (delta < -45f)
            {
                currAngle += 360;
            }

            return currAngle;
        }


    }
}
