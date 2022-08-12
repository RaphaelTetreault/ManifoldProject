using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzPathBezier : GfzPathSegment,
        IPositionEvaluable
    {
        // TODO: maneage consts better
        private const int kNewSegmentLength = 300;
        private const int kNewTangentLength = 100;

        [SerializeField]
        private List<BezierPoint> points;

        [SerializeField, HideInInspector]
        private bool isLoop = false;

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

        [Range(1, 32)]
        [SerializeField]
        private int keysBetweenBezierPoints = 8;

        [SerializeField]
        private AnimationCurveTRS animationCurveTRS = new();


        public bool IsLoop
        {
            get => isLoop;
        }

        public bool ViewDirection
        {
            get => viewDirection;
            set => viewDirection = value;
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
            return xy;

            //var z = Quaternion.Euler(0, 0, 0);
            //var orientation = xy * z;
            // Re-orient as per transform orientation
            //orientation = orientation * transform.rotation;
            //return orientation;
        }
        public Quaternion GetOrientation(float time, int index, Vector3 up)
        {
            var direction = GetDirection(time, index);
            var xy = Quaternion.LookRotation(direction, up);
            return xy;

            //var z = Quaternion.Euler(0, 0, 0);
            //var orientation = xy * z;
            // Re-orient as per transform orientation
            //orientation = orientation * transform.rotation;
            //return orientation;
        }

        public Quaternion GetOrientation(float time01)
        {
            (float t, int i) = NormalizedTimeToTimeAndIndex(time01);
            var orientation = GetOrientation(t, i);
            return orientation;
        }

        public Vector3 GetScale(float time, int index)
        {
            var width = 0f;
            //var width = widthsCurve.Evaluate(index + time);
            //var height = heightsCurve.Evaluate(index + time);
            var scale = new Vector3(width, 1f, 1f);
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
            newBezier.widthTangentMode = lastBezier.widthTangentMode;
            newBezier.height = lastBezier.height;
            newBezier.heightTangentMode = lastBezier.heightTangentMode;
            newBezier.roll = lastBezier.roll;
            newBezier.rollTangentMode = lastBezier.rollTangentMode;

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
            newBezier.widthTangentMode = firstBezier.widthTangentMode;
            newBezier.height = firstBezier.height;
            newBezier.heightTangentMode = firstBezier.heightTangentMode;
            newBezier.roll = firstBezier.roll;
            newBezier.rollTangentMode = firstBezier.rollTangentMode;

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
            bezier.widthTangentMode = AnimationUtility.TangentMode.Free;
            bezier.height = (bezier0.height + bezier1.height) / 2f;
            bezier.heightTangentMode = AnimationUtility.TangentMode.Free;
            bezier.roll = (bezier0.roll + bezier1.roll) / 2f;
            bezier.rollTangentMode = AnimationUtility.TangentMode.Free;

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

        protected override void Reset()
        {
            //base.Reset();

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
                    widthTangentMode = AnimationUtility.TangentMode.Free,
                    height = 1f,
                    heightTangentMode = AnimationUtility.TangentMode.Free,
                    roll = 0f,
                    rollTangentMode = AnimationUtility.TangentMode.Free,
                },

                // First point which can form a curve with the start
                new BezierPoint()
                {
                    tangentMode = BezierControlPointMode.Mirrored,
                    position = new Vector3(0f, 0f, 400f),
                    inTangent = new Vector3(0f, 0f, 300f),
                    outTangent = new Vector3(0f, 0f, 500f),
                    width = 64f,
                    widthTangentMode = AnimationUtility.TangentMode.Free,
                    height = 1f,
                    heightTangentMode = AnimationUtility.TangentMode.Free,
                    roll = 0f,
                    rollTangentMode = AnimationUtility.TangentMode.Free,
                },
            };

            CallOnEdited();
        }

        public float3 EvaluatePosition(double time)
        {
            return GetPositionRelative((float)time);
        }


        public AnimationCurveTRS CreateTRS(int samplesBetweenControlsPoints = 16)
        {
            var trs = new AnimationCurveTRS();

            // Entire curve approximate length to within 100m
            const int nStartIterDistance = 50;
            double entireCurveApproximateLength = CurveUtility.GetDistanceBetweenRepeated(this, 0, 1, nStartIterDistance, 2, 1);
            int nApproximationIterations = Mathf.CeilToInt((float)(entireCurveApproximateLength / 200));

            // Compute curve lengths between each bezier control point
            int numCurves = points.Count - 1;
            double[] distances = new double[numCurves];
            double totalDistance = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                double timeStart = (double)(i + 0) / numCurves;
                double timeEnd = (double)(i + 1) / numCurves;
                double distance = BezierApproximateDistance(this, timeStart, timeEnd, nApproximationIterations);
                distances[i] = distance;
                totalDistance += distance;
            }

            //
            Vector3 basePosition = GetPosition();

            Vector3 previousRotation = GetOrientation(0, 0).eulerAngles;
            double currDistance = 0;
            for (int i = 0; i < numCurves; i++)
            {
                float currLength = (float)distances[i];

                for (int s = 0; s < samplesBetweenControlsPoints; s++)
                {
                    var t = (float)(s + 0) / samplesBetweenControlsPoints;
                    var position = GetPosition(t, i) + basePosition;
                    var rotation = GetOrientation(t, i).eulerAngles;
                    rotation = CurveUtility.CleanRotation(previousRotation, rotation);
                    previousRotation = rotation;
                    var time = (float)(currDistance + (t * currLength));
                    trs.Position.AddKeys(time, position);
                    trs.Rotation.AddKeys(time, rotation);
                }

                currDistance += distances[i];
            }

            // Add last key
            {
                float t = (float)totalDistance;
                int i = numCurves - 1;
                var position = GetPosition(t, i) + basePosition;
                var rotation = GetOrientation(t, i).eulerAngles;
                rotation = CurveUtility.CleanRotation(previousRotation, rotation);
                trs.Position.AddKeys(t, position);
                trs.Rotation.AddKeys(t, rotation);
            }

            // Assign animation curves for things we don't need to generate/sample/guess
            {
                var points = this.points.ToArray();
                // ROTATION.Z
                (float[] rolls, var rollModes) = GetRolls(points);
                trs.Rotation.z = CreateCurve(rolls, rollModes, distances, samplesBetweenControlsPoints);
                // SCALE.X
                (float[] widths, var widthModes) = GetWidths(points);
                trs.Scale.x = CreateCurve(widths, widthModes, distances, samplesBetweenControlsPoints);
                // SCALE.Y
                (float[] heights, var heightModes) = GetHeights(points);
                trs.Scale.y = CreateCurve(heights, heightModes, distances, samplesBetweenControlsPoints);
                // SCALE.Z, const scale of 1f
                var key0 = new Keyframe(0, 1f);
                var key1 = new Keyframe((float)totalDistance, 1f);
                trs.Scale.y = new AnimationCurve(key0, key1);
                trs.Scale.z = new AnimationCurve(key0, key1);
            }


            trs.CleanDuplicateKeys();
            return trs;
        }

        public void UpdateAnimationCurveTRS()
        {
            animationCurveTRS = CreateTRS(keysBetweenBezierPoints);
            //SegmentLength = animationCurveTRS.GetMaxTime();
        }

        public double BezierApproximateDistance(IPositionEvaluable evaluable, double timeStart, double timeEnd, int nApproximationIterations)
        {
            // TimeDelta: difference between start and end times
            double timeDelta = timeEnd - timeStart;
            // Total distance that has been approximated
            double approximateDistance = 0f;
            // How many times we should sample curve.
            double inverseNIterations = 1.0 / nApproximationIterations;

            //// Approximate (segment of) curve length to greater degree.
            //// Since this is a bezier, it means points may not be evenly spaced out.
            //double approximateDistanceTotal = 0;
            //double[] approximateDistances = new double[nApproximationIterations];
            //for (int i = 0; i < nApproximationIterations; i++)
            //{
            //    var distance1 = CurveLengthUtility.GetDistanceBetweenRepeated(evaluable, timeStart, timeEnd, 10f, 2, 1);
            //    approximateDistances[i] = distance1;
            //    approximateDistanceTotal += distance1;
            //}
            //// Figure out how long each segment should be if they were equally spaced
            //double distancePerSegment = approximateDistanceTotal / nApproximationIterations;

            // Sample distance using uneven lengths to compensate and compute a more accurate distance
            for (int i = 0; i < nApproximationIterations; i++)
            {
                // throttle sample rate to distance per segment
                //double sampleNormalizer = distancePerSegment / approximateDistances[i];

                // TODO: pretty this is wrong since smaple normalizer is not corrected between iterations?
                double currDistance = timeStart + (timeDelta * inverseNIterations * (i + 0));
                double nextDistance = timeStart + (timeDelta * inverseNIterations * (i + 1));

                // Compute the distance between these 2 points
                float3 currPosition = evaluable.EvaluatePosition(currDistance);
                float3 nextPosition = evaluable.EvaluatePosition(nextDistance);

                // Get distance between 2 points, store delta
                double delta = math.distance(currPosition, nextPosition);
                approximateDistance += delta;
            }

            return approximateDistance;
        }

        public AnimationCurve SubdivideCurve(AnimationCurve animationCurve, int subdivisions)
        {
            var keys = new List<Keyframe>();
            for (int i = 0; i < animationCurve.length - 1; i++)
            {
                var key0 = animationCurve.keys[i + 0];
                var key1 = animationCurve.keys[i + 1];

                bool isDifferent =
                    key0.value != key1.value ||
                    key0.inTangent != key1.inTangent ||
                    key0.outTangent != key1.outTangent;

                if (!isDifferent)
                    continue;

                float startTime = key0.time;
                float endTime = key1.time;
                for (int subd = 0; subd < subdivisions; subd++)
                {
                    float time = (float)subd / subdivisions;
                    //float keyTime = startTime + endTime * time;
                    float keyTime = math.lerp(startTime, endTime, time);
                    var value = animationCurve.Evaluate(keyTime);
                    var key = new Keyframe(keyTime, value);
                    keys.Add(key);
                }
            }

            //var curve = new AnimationCurve(animationCurve.keys);
            //keys.AddRange(animationCurve.keys);

            // Don't add key directly or it will set tangents of 0
            //foreach (var key in keys)
            //    animationCurve.AddKey(key.time, key.value);

            //var curve = new AnimationCurve(keys.ToArray());

            foreach (var key in keys)
            {
                int index = animationCurve.AddKey(key);

                if (index > 0)
                    animationCurve.SmoothTangents(index, 1f / 3f);
            }

            return animationCurve;
        }

        public AnimationCurve CreateCurve(float[] values, AnimationUtility.TangentMode[] modes, double[] distances, int subdivisions)
        {
            float time = 0f;
            var curve = new AnimationCurve();
            for (int i = 0; i <= distances.Length; i++)
            {
                // Create animation curve with time value only
                var key = new Keyframe(time, values[i], 0, 0);
                curve.AddKey(key);

                //
                if (i >= distances.Length)
                    break;

                //
                time += (float)distances[i];
            }

            // Set tange mode for all keys.
            // GFZ has tangent modes between keys, not on left/right of keys.
            // The below assigns it as so.
            curve.SetGfzTangentModes(modes);
            //for (int i = 0; i < curve.length - 1; i++)
            //{
            //    var mode = modes[i];
            //    AnimationUtility.SetKeyRightTangentMode(curve, i + 0, mode);
            //    AnimationUtility.SetKeyLeftTangentMode(curve, i + 1, mode);
            //}

            curve = SubdivideCurve(curve, subdivisions);

            return curve;
        }
        public (float[] widths, AnimationUtility.TangentMode[] widthModes) GetWidths(BezierPoint[] bezierPoints)
        {
            var widths = new float[bezierPoints.Length];
            var widthModes = new AnimationUtility.TangentMode[bezierPoints.Length];
            for (int i = 0; i < bezierPoints.Length; i++)
            {
                widths[i] = bezierPoints[i].width;
                widthModes[i] = bezierPoints[i].widthTangentMode;
            }
            return (widths, widthModes);
        }
        public (float[] heights, AnimationUtility.TangentMode[] heightModes) GetHeights(BezierPoint[] bezierPoints)
        {
            var heights = new float[bezierPoints.Length];
            var heightModes = new AnimationUtility.TangentMode[bezierPoints.Length];
            for (int i = 0; i < bezierPoints.Length; i++)
            {
                heights[i] = bezierPoints[i].height;
                heightModes[i] = bezierPoints[i].heightTangentMode;
            }
            return (heights, heightModes);
        }
        public (float[] rolls, AnimationUtility.TangentMode[] rollModes) GetRolls(BezierPoint[] bezierPoints)
        {
            var rolls = new float[bezierPoints.Length];
            var rollModes = new AnimationUtility.TangentMode[bezierPoints.Length];
            for (int i = 0; i < bezierPoints.Length; i++)
            {
                rolls[i] = bezierPoints[i].roll;
                rollModes[i] = bezierPoints[i].rollTangentMode;
            }
            return (rolls, rollModes);
        }

        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            var trs = isGfzCoordinateSpace
                ? animationCurveTRS.CreateGfzCoordinateSpace()
                : animationCurveTRS.CreateDeepCopy();

            return trs;
        }

        //protected override AnimationCurveTRS TrackSegmentAnimationCurveTRS => animationCurveTRS;

        public override float GetMaxTime()
        {
            return animationCurveTRS.GetMaxTime();
        }

        // DEPRECATE?
        public void CallOnEdited()
        {
            if (autoGenerateTRS)
            {
                InvokeUpdates();
            }
        }

        public void UpdateShapeNodeMeshes(GfzSegmentShape[] shapes)
        {
            foreach (var shape in shapes)
            {
                shape.UpdateMesh();
            }
        }

        public override float GetSegmentLength()
        {
            var root = GetRoot();

            if (this != root)
                throw new Exception("Bezier makes assumption that it is always root node!");

            //var segmentLength = SegmentLength;
            //if (segmentLength <= 0f)
            //{
            //    var msg = "Distance is 0 which is invalid. TRS animation curves must define path.";
            //    throw new System.ArgumentException(msg);
            //}
            var segmentLength = animationCurveTRS.GetMaxTime();

            return segmentLength;
        }

        public override void UpdateTRS()
        {
            animationCurveTRS = CreateTRS(keysBetweenBezierPoints);
        }

    }
}
