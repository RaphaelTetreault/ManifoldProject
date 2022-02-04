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
        private BezierPoint[] points;

        // TODO
        //      add widths => animation curve
        //      add rolls  => animation curve
        //      add option for loop? Extend array by 1, copy first into last, last into first


        public int ControlPointCount
        {
            get => points.Length;
        }

        public BezierPoint GetBezierPoint(int index)
        {
            return points[index];
        }

        public void SetBezierPoint(int index, BezierPoint point)
        {
            points[index] = point;
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
                index = points.Length - 1;
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

        public Vector3 GetPoint(float time)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time);

            var bezier0 = points[i+0]; 
            var bezier1 = points[i+1];
            var p0 = bezier0.position;
            var p1 = bezier0.inTangent;
            var p2 = bezier1.inTangent;
            var p3 = bezier1.position;

            var point = Bezier.GetPoint(p0, p1, p2, p3, t);
            var p = transform.TransformPoint(point);

            return p;
        }

        public Vector3 GetVelocity(float time)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time);

            var bezier0 = points[i - 1];
            var bezier1 = points[i + 0];
            var p0 = bezier0.position;
            var p1 = bezier0.inTangent;
            var p2 = bezier1.inTangent;
            var p3 = bezier1.position;

            var firstDerivitive = Bezier.GetFirstDerivative(p0, p1, p2, p3, t);

            var origin = transform.position;
            var velocity = transform.TransformPoint(firstDerivitive) - origin;
            return velocity;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public int CurveCount
        {
            get => points.Length - 1;
        }

        public void AddCurve()
        {
            // Get the direction of the final spline curve point
            var direction = GetDirection(1f);

            // Get last point
            var prevBezier = points[points.Length - 1];
            // Add 1 to list
            Array.Resize(ref points, points.Length + 1);

            //
            var newCurve = new BezierPoint();
            newCurve.inTangent  = prevBezier.position + direction * 25f;// prevBezier.point - direction * 50f;
            newCurve.outTangent = prevBezier.position - direction * 50f;
            newCurve.position      = prevBezier.position + direction * 100f;
            //
            points[points.Length - 1] = newCurve;
        }


        public void Reset()
        {
            points = new BezierPoint[]
            {
                // Mandatory first node
                new BezierPoint()
                {
                    tangentMode = BezierControlPointMode.Mirrored,
                    position = new Vector3(0f, 0f, 0f),
                    inTangent = new Vector3(0f, 0f, -100f),
                    width = 64f,
                    roll = 0f,
                },

                // Firs point which can form a curve with the start
                new BezierPoint()
                {
                    tangentMode = BezierControlPointMode.Mirrored,
                    position = new Vector3(0f, 0f, -300f),
                    inTangent = new Vector3(0f, 0f, -200f),
                    width = 64f,
                    roll = 0f,
                },
            };
        }

    }
}
