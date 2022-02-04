using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzBezierSplineSegment : MonoBehaviour
    {
        [SerializeField]
        //private BezierPoint[] points;
        private List<BezierPoint> points;


        [SerializeField, HideInInspector]
        private bool isLoop = false;

        [SerializeField, HideInInspector]
        private AnimationCurve widthCurve = new();

        [SerializeField, HideInInspector]
        private AnimationCurve rollCurve = new();

        // TODO
        //      add widths => animation curve
        //      add rolls  => animation curve
        //      add option for loop? Extend array by 1, copy first into last, last into first

        public bool IsLoop
        {
            get => isLoop;
            set => isLoop = value;
        }

        public AnimationCurve WidthCurve
        {
            get => widthCurve;
            set => widthCurve = value;
        }

        public AnimationCurve RollCurve
        {
            get => rollCurve;
            set => rollCurve = value;
        }


        public int PointsCount
        {
            get => points.Count;
        }

        public int CurveCount
        {
            get => points.Count - 1;
        }

        public BezierPoint GetBezierPoint(int index)
        {
            return points[index];
        }

        public void SetBezierPoint(int index, BezierPoint point)
        {
            points[index] = point;
        }


        public AnimationCurve WidthsToCurve()
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

        public AnimationCurve RollsToCurve()
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

        public Vector3 GetPosition(float time01)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time01);
            var point = GetPosition(t, i);
            return point;
        }

        public Vector3 GetPosition(float time, int index)
        {
            var bezier0 = points[index + 0];
            var bezier1 = points[index + 1];
            var p0 = bezier0.position;
            var p1 = bezier0.outTangent;
            var p2 = bezier1.inTangent;
            var p3 = bezier1.position;

            var point = Bezier.GetPoint(p0, p1, p2, p3, time);
            var p = transform.TransformPoint(point);

            return p;
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


        public void AddPointAtEnd()
        {
            // Get the direction of the final spline curve point, point position
            var direction = GetDirection(1f);
            var lastIndex = points.Count - 1;
            var bezier = points[lastIndex];

            //
            var newBezier = new BezierPoint();
            newBezier.inTangent = bezier.position - direction * 25f;
            newBezier.outTangent = bezier.position + direction * 50f;
            newBezier.position = bezier.position - direction * 100f;

            points.Insert(lastIndex, newBezier);
        }

        public void AddPointAtStart()
        {
            // Get the direction of the final spline curve point, point position
            var direction = GetDirection(0f);
            var bezier = points[0];

            //
            var newBezier = new BezierPoint();
            newBezier.position = bezier.position - direction * 200f;
            newBezier.inTangent = newBezier.position + direction * 50f;
            newBezier.outTangent = newBezier.position - direction * 50f;

            points.Insert(0, newBezier);
        }


        public bool IsValidIndex(int index)
        {
            bool isValid = index > 0 && index < points.Count - 1;
            return isValid;
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
            bezier.position = GetPosition(0.5f, index);
            bezier.inTangent = bezier.position - direction;
            bezier.outTangent = bezier.position + direction;
            bezier.tangentMode = BezierControlPointMode.Mirrored;
            bezier.width = (bezier0.width + bezier1.width) / 2f;
            bezier.roll = (bezier0.roll + bezier1.roll) / 2f;

            points.Insert(index+1, bezier);
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
                    inTangent = new Vector3(0f, 0f, 100f),
                    outTangent = new Vector3(0f, 0f, -100f),
                    width = 64f,
                    roll = 0f,
                },

                // Firs point which can form a curve with the start
                new BezierPoint()
                {
                    tangentMode = BezierControlPointMode.Mirrored,
                    position = new Vector3(0f, 0f, -400f),
                    inTangent = new Vector3(0f, 0f, -300f),
                    outTangent = new Vector3(0f, 0f, -500f),
                    width = 64f,
                    roll = 0f,
                },
            };
        }

    }
}
