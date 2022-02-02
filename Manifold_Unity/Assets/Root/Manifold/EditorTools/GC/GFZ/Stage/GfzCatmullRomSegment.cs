using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Mathematics;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [ExecuteInEditMode]
    public class GfzCatmullRomSegment : MonoBehaviour
    {
        public bool includeInactive = false;
        [Range(-180f, 180f)]
        public float testRotation = 0f;
        [Range(0f, 1f)]
        public float alpha = 0.5f;
        public CatmullRomPoint[] points;

        public struct Point
        {
            public Vector3 position;
            public Vector3 tangent;
        }


        public void CollectChildren()
        {
            points = GetComponentsInChildren<CatmullRomPoint>(includeInactive);
        }

        public void AssignAnimCurve()
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            if (points == null)
                return;

            var a = alpha;

            // catmull-rom spline
            var crs = new List<Point>();

            // for each space between 2 nodes
            for (int i = +1; i < points.Length -2; i++)
            {
                float3 p0 = points[i - 1].transform.position;
                float3 p1 = points[i - 0].transform.position;
                float3 p2 = points[i + 1].transform.position;
                float3 p3 = points[i + 2].transform.position;

                //
                int iters = points[i].Samples;
                for (int n = 0; n <= iters; n++)
                {
                    float t = (float)(n) / iters;
                    (double3 position, double3 tangent) = CatmullRomSpline.HackGetPpsitionDirection(p0, p1, p2, p3, a, t);
                    var point = new Point()
                    {
                        position = (float3)position,
                        tangent = (float3)tangent,
                    };
                    crs.Add(point);
                }
            }

            for (int i = 0; i < points.Length -1; i++)
            {
                Debug.DrawLine(points[i].transform.position, points[i+1].transform.position, Color.white);
            }

            foreach (var point in crs)
            {
                var tangent = point.tangent.normalized * 10f;
                var tanRot = Quaternion.LookRotation(tangent);
                var zRot = Quaternion.Euler(0, 0, testRotation);
                var up = tanRot * zRot * Vector3.up;
                var right = tanRot * zRot * Vector3.right;
                var fwd = tanRot * zRot * Vector3.forward;

                Debug.DrawLine(point.position, point.position + up, Color.green);
                Debug.DrawLine(point.position, point.position + right, Color.red);
                Debug.DrawLine(point.position, point.position + fwd, Color.blue);
            }

        }


    }
}
