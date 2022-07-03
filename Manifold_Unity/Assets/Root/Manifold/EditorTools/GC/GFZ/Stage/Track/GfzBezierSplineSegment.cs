using System;
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
    public class GfzBezierSplineSegment : SegmentPathGenerator,
        IPositionEvaluable
    {
        // TODO: maneage consts better
        private const int kNewSegmentLength = 300;
        private const int kNewTangentLength = 100;

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
            CallOnEdited();
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


        public Matrix4x4 GetMatrix(float time, int index)
        {
            var position = GetPosition(time, index);
            var rotation = GetOrientation(time, index);
            var scale = GetScale(time, index);
            var matrix = new Matrix4x4();
            matrix.SetTRS(position, rotation, scale);
            return matrix;
        }

        public Matrix4x4 GetMatrix(float time01)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time01);
            var matrix = GetMatrix(t, i);
            return matrix;
        }

        public void AddPointAtEnd()
        {
            var lastIndex = points.Count - 1;
            var lastBezier = points[lastIndex];
            //var length = (lastBezier.outTangent - lastBezier.position).magnitude;
            // Get the direction of the final spline curve point
            var direction = GetDirection(1f);

            //
            var newBezier = new BezierPoint();
            newBezier.position = lastBezier.position + direction * kNewSegmentLength;
            newBezier.tangentMode = BezierControlPointMode.Mirrored;
            newBezier.inTangent = newBezier.position - direction * kNewTangentLength;
            newBezier.outTangent = newBezier.position + direction * kNewTangentLength;
            newBezier.width = lastBezier.width;
            newBezier.roll = lastBezier.roll;

            points.Insert(lastIndex + 1, newBezier);
            CallOnEdited();
        }

        public void AddPointAtStart()
        {
            var firstBezier = points[0];
            //var length = (firstBezier.outTangent - firstBezier.position).magnitude;
            // Get the direction of the final spline curve point, point position
            var direction = GetDirection(0f);

            //
            var newBezier = new BezierPoint();
            newBezier.position = firstBezier.position - direction * kNewSegmentLength;
            newBezier.tangentMode = BezierControlPointMode.Mirrored;
            newBezier.inTangent = newBezier.position - direction * kNewTangentLength;
            newBezier.outTangent = newBezier.position + direction * kNewTangentLength;
            newBezier.width = firstBezier.width;
            newBezier.roll = firstBezier.roll;

            points.Insert(0, newBezier);
            CallOnEdited();
        }

        public void RemovePoint(int index)
        {
            points.RemoveAt(index);
            CallOnEdited();
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
            CallOnEdited();
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
            CallOnEdited();
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
                    height = 1f,
                    roll = 0f,
                },

                // First point which can form a curve with the start
                new BezierPoint()
                {
                    tangentMode = BezierControlPointMode.Mirrored,
                    position = new Vector3(0f, 0f, -400f),
                    inTangent = new Vector3(0f, 0f, -300f),
                    outTangent = new Vector3(0f, 0f, -500f),
                    width = 64f,
                    height = 1f,
                    roll = 0f,
                },
            };

            CallOnEdited();
        }


        public override AnimationCurveTRS GenerateAnimationCurveTRS()
        {
            //return ComputeMultiSampleV2();
            return ComputeMultiSample();
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
            const float minDelta = 180f;
            float delta = currAngle - lastAngle;

            if (delta > minDelta)
            {
                currAngle -= 360f;
            }
            else if (delta < -minDelta)
            {
                currAngle += 360;
            }

            return currAngle;
        }

        /// <summary>
        /// Iteration 1 curve sampling
        /// </summary>
        /// <returns></returns>
        public AnimationCurveTRS ComputeMultiSample(int samplesBetweenControlsPoints = 32)
        {
            var trs = new AnimationCurveTRS();

            // Compute curve lengths between each bezier control point
            int numCurves = points.Count - 1;
            double[] distances = new double[numCurves];
            double totalDistance = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                double timeStart = (double)(i + 0) / numCurves;
                double timeEnd = (double)(i + 1) / numCurves;
                double distance = CurveLengthUtility.GetDistanceBetweenRepeated(this, timeStart, timeEnd);
                distances[i] = distance;
                totalDistance += distance;
                Debug.Log($"Distance {i}: {distance}");
            }
            Debug.Log("Total distance: " + totalDistance);

            // GOAL: the inital X and Y rotation for any segment should/has to be x:0, y:0.
            // This is because X and Y rotation is controlled by/is the forward vector of the segment's
            // transform component. The animation data here should only represent the delta/difference
            // between the initial transform component and the sampled matrix at time 't'.
            // However, the Z/roll is independant of this XY rotation and should be preserved. However,
            // it too is relative to the inital transform. Thus, we use the transform's Z/roll as the offset,
            // leaving the Z/roll parameter of the bezier intact. If the transform is rotated +30 degrees about
            // the Z axis, and the Z/roll parameter of the first anim key is +45, the animation data would
            // generate an angle of +75. Inversing only the transform's +30 to -30 corrects this.
            var initRotation = GetOrientation(0, 0).eulerAngles;
            var inverseInitialRotation = Quaternion.Inverse(Quaternion.Euler(initRotation.x, initRotation.y, transform.eulerAngles.z));
            var previousRotation = inverseInitialRotation.eulerAngles;

            double currDistance = 0;
            for (int i = 0; i < numCurves; i++)
            {
                float currLength = (float)distances[i];

                for (int s = 0; s < samplesBetweenControlsPoints; s++)
                {
                    var t = (float)(s + 0) / samplesBetweenControlsPoints;
                    var position = GetPosition(t, i);
                    var scale = GetScale(t, i);
                    var qRotation = inverseInitialRotation * GetOrientation(t, i);
                    var rotation = qRotation.eulerAngles;
                    rotation = CleanRotation(previousRotation, rotation);
                    previousRotation = rotation;

                    var time = currDistance + (t * currLength);
                    trs.AddKeys((float)time, position, rotation, scale);
                    //var timeNormalized = (currDistance + (t * currLength)) / totalDistance;
                    //trs.AddKeys((float)timeNormalized, position, rotation, scale);
                }

                currDistance += distances[i];
            }

            // Add last key
            {
                float t = (float)totalDistance;
                int i = numCurves - 1;
                var position = GetPosition(t, i);
                var scale = GetScale(t, i);
                var qRotation = inverseInitialRotation * GetOrientation(t, i);
                var rotation = qRotation.eulerAngles;
                rotation = CleanRotation(previousRotation, rotation);
                trs.AddKeys(t, position, rotation, scale);
            }

            trs.CleanDuplicateKeys();
            return trs;
        }

    }
}
