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
        [SerializeField]
        private Vector3[] points;

        [SerializeField]
        private BezierControlPointMode[] modes;


        public int ControlPointCount
        {
            get => points.Length;
        }

        public Vector3 GetControlPoint(int index)
        {
            return points[index];
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - points[index];
                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length)
                {
                    points[index + 1] += delta;
                }
            }
            points[index] = point;
            EnforceMode(index);
        }

        public BezierControlPointMode GetControlPointMode(int index)
        {
            return modes[(index + 1) / 3];
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode)
        {
            modes[(index + 1) / 3] = mode;
            EnforceMode(index);
        }

        private void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;

            // Only enforce mode if we need to
            var mode = modes[modeIndex];
            if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Length - 1)
            {
                return;
            }

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                enforcedIndex = middleIndex + 1;
            }
            else
            {
                fixedIndex = middleIndex + 1;
                enforcedIndex = middleIndex - 1;
            }

            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
            {
                float magnitude = Vector3.Distance(middle, points[enforcedIndex]);
                enforcedTangent = enforcedTangent.normalized * magnitude;
            }

            points[enforcedIndex] = middle + enforcedTangent;
        }


        public (float time, int index) GetSplineTimeIndex(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }

            return (t, i);
        }

        public Vector3 GetPoint(float t)
        {
            var spline = GetSplineTimeIndex(t);

            var p0 = points[spline.index + 0];
            var p1 = points[spline.index + 1];
            var p2 = points[spline.index + 2];
            var p3 = points[spline.index + 3];

            var p = transform.TransformPoint(BezierSpline.GetPoint(p0, p1, p2, p3, spline.time));
            return p;
        }

        public Vector3 GetVelocity(float t)
        {
            var spline = GetSplineTimeIndex(t);

            var p0 = points[spline.index + 0];
            var p1 = points[spline.index + 1];
            var p2 = points[spline.index + 2];
            var p3 = points[spline.index + 3];

            var firstDerivitive = BezierSpline.GetFirstDerivative(p0, p1, p2, p3, spline.time);

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
            get => (points.Length - 1) / 3;
        }

        public void AddCurve()
        {
            // TODO: use direction to extend points.

            var point = points[points.Length - 1];
            Array.Resize(ref points, points.Length + 3);
            point.x += 10f;
            points[points.Length - 3] = point;
            point.x += 10f;
            points[points.Length - 2] = point;
            point.x += 10f;
            points[points.Length - 1] = point;

            // Resize modes between curves
            Array.Resize(ref modes, modes.Length + 1);
            modes[modes.Length - 1] = modes[modes.Length - 2];

            //
            EnforceMode(points.Length - 4);
        }


        public void Reset()
        {
            points = new Vector3[]
            {
                new Vector3(10f, 0f, 0f),
                new Vector3(20f, 0f, 0f),
                new Vector3(30f, 0f, 0f),
                new Vector3(40f, 0f, 0f),
            };

            modes = new BezierControlPointMode[]
            {
                BezierControlPointMode.Free,
                BezierControlPointMode.Free
            };
        }
    }
}
