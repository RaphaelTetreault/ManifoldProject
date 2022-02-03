using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzBezierSplineSegment : MonoBehaviour
    {
        public Line.Point[] points;


        public Vector3 GetPoint(float t)
        {
            var p0 = points[0].position;
            var p1 = points[1].position;
            var p2 = points[2].position;
            var p3 = points[3].position;
            var p = transform.TransformPoint(BezierCurve.GetPoint(p0, p1, p2, p3, t));
            return p;
        }

        public Vector3 GetVelocity(float t)
        {
            var p0 = points[0].position;
            var p1 = points[1].position;
            var p2 = points[2].position;
            var p3 = points[3].position;
            var firstDerivitive = BezierCurve.GetFirstDerivative(p0, p1, p2, p3, t);

            var origin = transform.position;
            var velocity = transform.TransformPoint(firstDerivitive) - origin;
            return velocity;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public int GetCurveCount
        {
            get => (points.Length - 1) / 3;
        }

        public void AddCurve()
        {
            int length = points.Length;
            var point = points[length - 1];
            Array.Resize(ref points, length + 3);
            point.position.x += 10f;
            points[length - 3] = point;
            point.position.x += 10f;
            points[length - 2] = point;
            point.position.x += 10f;
            points[length - 1] = point;
        }






        public void Reset()
        {
            points = new Line.Point[]
            {
                new Line.Point(new Vector3(10f, 0f, 0f)),
                new Line.Point(new Vector3(20f, 0f, 0f)),
                new Line.Point(new Vector3(30f, 0f, 0f)),
                new Line.Point(new Vector3(40f, 0f, 0f)),
            };
        }
    }
}
