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
    public class GfzCatmullRomSegment : SegmentPathGenerator
    {
        public bool includeInactive = false;
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
            public float time;
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

     

        public override AnimationCurveTRS GetAnimationCurveTRS()
        {
            var animationCurveTRS = new AnimationCurveTRS();

            // TODO:
            /* For each p1-p2:
             * Get all points for that part
             * Get approximate length (todo: check current estimator... in animTRS )
             * Use length to insert keyframes at the right time
             * 
             */

            // Bring in scope optimization
            var a = alpha;
            var animTRS = new AnimationCurveTRS();
            var distances = new float[points.Length-3];
            var totalDistance = 0f;

            // for each space between 2 nodes
            for (int pi = +1; pi < points.Length - 2; pi++)
            {
                //
                float3 p0 = points[pi - 1].transform.position;
                float3 p1 = points[pi - 0].transform.position;
                float3 p2 = points[pi + 1].transform.position;
                float3 p3 = points[pi + 2].transform.position;

                //
                int index = pi - 1;
                int iters = points[pi].Samples;
                var trsPoint = new Point[iters+1];
                var temp = new AnimationCurveTRS();

                //
                for (int si = 0; si <= iters; si++)
                {
                    float t = (float)si / iters;
                    (double3 position, double3 tangent) = CatmullRomSpline.HackGetPositionDirection(p0, p1, p2, p3, a, t);
                    var point = new Point()
                    {
                        position = (float3)position,
                        tangent = (float3)tangent,
                        width = width.Evaluate(index + t),
                        roll = roll.Evaluate(index + t),
                        time = t,
                    };
                    trsPoint[si] = point;
                    temp.Position.AddKeys(t, point.position);
                }
                var distance = temp.GetDistanceBetweenRepeated(0, 1);

                // Assign properly distanced keys
                // Position.XYZ
                // Rotation.XY, Z with Roll
                // Scale.X (YZ are const, outside of loop)
                var lastRX = 0f;
                var lastRY = 0f;
                foreach (var point in trsPoint)
                {
                    //
                    var evalTime = pi -1 + point.time;
                    var keyTime = point.time * distance + totalDistance;
                    //
                    animationCurveTRS.Position.AddKeys(keyTime, point.position);
                    //
                    var rotation = Quaternion.LookRotation(point.tangent.normalized).eulerAngles;
                    //rotation = Quaternion.Euler(0, 180, 0) * rotation;
                    var currRX = GetRotation(lastRX, rotation.x);
                    var currRY = GetRotation(lastRY, rotation.y);
                    animationCurveTRS.Rotation.x.AddKey(keyTime, currRX);
                    animationCurveTRS.Rotation.y.AddKey(keyTime, currRY);
                    lastRX = currRX;
                    lastRY = currRY;

                    // TODO: you don't need to sample these so many times?
                    //
                    var rotationZ = roll.Evaluate(evalTime);
                    animationCurveTRS.Rotation.z.AddKey(keyTime, rotationZ);
                    //
                    var scaleX = width.Evaluate(evalTime);
                    animationCurveTRS.Scale.x.AddKey(keyTime, scaleX);
                }
                totalDistance += distance;
                //Debug.Log("Distance: " + distance);
            }

            //
            var curves = new AnimationCurve[]
            {
                animationCurveTRS.Position.x,
                animationCurveTRS.Position.y,
                animationCurveTRS.Position.z,
                animationCurveTRS.Rotation.x,
                animationCurveTRS.Rotation.y,
                animationCurveTRS.Rotation.z,
                animationCurveTRS.Scale.x,
            };
            foreach (var curve in curves)
            {
                var keys = new List<Keyframe>();
                for (int i = 0; i < curve.length; i++)
                {
                    var key = curve.keys[i];
                    key.time = key.time / totalDistance;
                    keys.Add(key);
                }
                curve.keys = keys.ToArray();
            }

            // assign SCALE.YZ to single key value '1'
            // final tangent
            var defaultScale = new Keyframe[] {
                new Keyframe(){ time = 0, value = 1f },
                new Keyframe(){ time = 1, value = 1f },
            };
            animationCurveTRS.Scale.y = new AnimationCurve(defaultScale);
            animationCurveTRS.Scale.z = new AnimationCurve(defaultScale);

            foreach (var curve in animationCurveTRS.AnimationCurves)
            {
                EnforceLastKeyTime(curve);

                for (int i = 0; i < curve.length; i++)
                    curve.SmoothTangents(i, 1/3f);
            }


            return animationCurveTRS;
        }

        // TODO: build better heuristic
        public float GetRotation(float prev, float curr)
        {
            var delta = prev - curr;
            if (math.abs(delta) > 180f)
            {
                if (curr < prev)
                {
                    return curr + 360f;
                }
                else
                {
                    return curr - 360f;
                }
            }
            return curr;
        }

        public void EnforceLastKeyTime(AnimationCurve animationCurve)
        {
            var keys = animationCurve.keys;
            // ensure last time is exactly 1f
            var lastIndex = keys.Length - 1;
            var lastKey = keys[lastIndex];
            lastKey.time = 1f;
            keys[lastIndex] = lastKey;
            animationCurve.keys = keys;
        }

    }
}
