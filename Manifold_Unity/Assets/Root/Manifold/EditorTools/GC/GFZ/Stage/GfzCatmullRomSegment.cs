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

        [Header("Other Data")]
        public AnimationCurve width = new AnimationCurve();
        public AnimationCurve roll = new AnimationCurve();



        public struct Point
        {
            public Vector3 position;
            public Vector3 tangent;
            public float width;
            public float roll;
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

            OrientNodes();
            GetOtherAnims();
            DrawSpline();
        }

        public void DrawSpline()
        {
            // Bring in scope optimization
            var a = alpha;
            // catmull-rom spline
            var crs = new List<Point>();

            // for each space between 2 nodes
            for (int i = +1; i < points.Length - 2; i++)
            {
                float3 p0 = points[i - 1].transform.position;
                float3 p1 = points[i - 0].transform.position;
                float3 p2 = points[i + 1].transform.position;
                float3 p3 = points[i + 2].transform.position;

                //
                int index = i - 1;
                int iters = points[i].Samples;
                for (int n = 0; n <= iters; n++)
                {
                    float t = (float)(n) / iters;
                    (double3 position, double3 tangent) = CatmullRomSpline.HackGetPositionDirection(p0, p1, p2, p3, a, t);
                    var point = new Point()
                    {
                        position = (float3)position,
                        tangent = (float3)tangent,
                        width = width.Evaluate(index + t),
                        roll = roll.Evaluate(index + t),
                    };
                    crs.Add(point);
                }
            }

            for (int i = 0; i < points.Length - 1; i++)
            {
                Debug.DrawLine(points[i].transform.position, points[i + 1].transform.position, Color.white);
            }

            foreach (var point in crs)
            {
                var tangent = point.tangent.normalized * 10f;
                var tanRot = Quaternion.LookRotation(tangent);
                var zRot = Quaternion.Euler(0, 0, point.roll);
                var up = tanRot * zRot * Vector3.up;
                var right = tanRot * zRot * Vector3.right * point.width / 2f;
                var fwd = tanRot * zRot * Vector3.forward;

                Debug.DrawLine(point.position, point.position + up, Color.green);
                Debug.DrawLine(point.position - right, point.position + right, Color.red);
                Debug.DrawLine(point.position, point.position + fwd, Color.blue);
            }
        }

        public void GetOtherAnims()
        {
            var widthKeys = new List<Keyframe>();
            var rollKeys = new List<Keyframe>();

            for (int i = +1; i < points.Length - 1; i++)
            {
                int keyframeIndex = i - 1;
                var point = points[i];
                if (!point.IgnoreWidth)
                {
                    var widthKey = new Keyframe();
                    widthKey.time = keyframeIndex;
                    widthKey.value = point.Width;
                    widthKeys.Add(widthKey);
                }

                if (!point.IgnoreRoll)
                {
                    var rollKey = new Keyframe();
                    rollKey.time = keyframeIndex;
                    rollKey.value = point.Roll;
                    rollKeys.Add(rollKey);
                }
            }

            width = new AnimationCurve(widthKeys.ToArray());
            roll = new AnimationCurve(rollKeys.ToArray());
        }

        public void OrientNodes()
        {
        //    var a = alpha;

        //    // for each space between 2 nodes
        //    for (int i = +1; i < points.Length - 2; i++)
        //    {
        //        float3 p0 = points[i - 1].transform.position;
        //        float3 p1 = points[i - 0].transform.position;
        //        float3 p2 = points[i + 1].transform.position;
        //        float3 p3 = points[i + 2].transform.position;

        //        (double3 _, double3 direction) = CatmullRomSpline.HackGetPositionDirection(p0, p1, p2, p3, a, 0);
        //        var tanRot = Quaternion.LookRotation((float3)direction);
        //        var zRot = Quaternion.Euler(0, 0, points[i].transform.rotation.eulerAngles.z);
        //        var rotation = tanRot * zRot;
        //        points[i].transform.rotation = rotation;
        //    }
        }

    }
}
