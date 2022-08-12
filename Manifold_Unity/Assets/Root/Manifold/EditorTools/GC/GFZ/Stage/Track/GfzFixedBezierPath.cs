using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Manifold.Spline;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzFixedBezierPath : GfzPathSegment
    {
        [SerializeField] private List<FixedBezierPoint> controlPoints;
        [SerializeField] private List<float> distancesBetweenControlPoints;
        [SerializeField] private int selectedIndex = 1;
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();

        protected override AnimationCurveTRS TrackSegmentAnimationCurveTRS => animationCurveTRS;
        public int ControlPointsLength => controlPoints.Count;

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            float distance = 0f;
            var trs = new AnimationCurveTRS();

            for (int i = 0; i < ControlPointsLength; i++)
            {
                var controlPoint = controlPoints[i];
                Vector3 position = transform.TransformPoint(controlPoint.position);
                Vector3 rotation = controlPoint.EulerOrientation + transform.rotation.eulerAngles;
                Vector3 scale = controlPoint.scale;

                // POSITION
                if (controlPoint.keyPosition.x)
                    trs.Position.x.AddKey(distance, position.x);
                if (controlPoint.keyPosition.y)
                    trs.Position.y.AddKey(distance, position.y);
                if (controlPoint.keyPosition.z)
                    trs.Position.z.AddKey(distance, position.z);

                // ROTATION
                if (controlPoint.keyOrientation.x)
                    trs.Rotation.x.AddKey(distance, rotation.x);
                if (controlPoint.keyOrientation.y)
                    trs.Rotation.y.AddKey(distance, rotation.y);
                if (controlPoint.keyOrientation.z)
                    trs.Rotation.z.AddKey(distance, rotation.z);

                // SCALE
                if (controlPoint.keyScale.x)
                    trs.Scale.x.AddKey(distance, scale.x);
                if (controlPoint.keyScale.y)
                    trs.Scale.y.AddKey(new Keyframe(distance, scale.y));
                
                // Move keysframes along distance
                if (i < distancesBetweenControlPoints.Count)
                    distance += distancesBetweenControlPoints[i];
            }

            return trs;
            // TODO: make the TRS here
            //return animationCurveTRS;
        }

        public override float GetMaxTime()
        {
            return animationCurveTRS.GetMaxTime();
        }

        public override float GetSegmentLength()
        {
            float segmentLength = 0f;
            foreach (var distance in distancesBetweenControlPoints)
                segmentLength += distance;
            return segmentLength;
        }

        public override void UpdateTRS()
        {
            animationCurveTRS = CreateAnimationCurveTRS(false);
        }



        // THE CORE
        public void InsertControlPoint(int index, FixedBezierPoint controlPoint)
        {
            bool isInvalidIndex = !IsValidIndex(index);
            if (isInvalidIndex)
                return;

            // Add control point
            controlPoints.Insert(index, controlPoint);
            // Add distance for new control point, compute distance
            distancesBetweenControlPoints.Insert(index, -1f);
            UpdateCurveDistanceTouchingControlPoint(index);
            UpdateLinearDistanceTouchingControlPoint(index);
        }
        public void InsertBefore(int index, FixedBezierPoint controlPoint)
            => InsertControlPoint(index, controlPoint);
        public void InsertAfter(int index, FixedBezierPoint controlPoint)
        {
            // insert point before last
            var currControlPoint = GetControlPoint(index);
            InsertControlPoint(index, currControlPoint);

            // replace next with what we want
            int nextIndex = index + 1;
            SetControlPoint(nextIndex, controlPoint);

            // Update distances
            UpdateCurveDistanceTouchingControlPoint(nextIndex);
            UpdateLinearDistanceTouchingControlPoint(nextIndex);
        }
            //=> InsertControlPoint(index + 1, controlPoint);
        public void InsertBetween(int index0, int index1)
        {
            var controlPoint0 = GetControlPoint(index0);
            var controlPoint1 = GetControlPoint(index1);

            // Compute position
            var p0 = controlPoint0.position;
            var p1 = controlPoint0.OutTangentPosition;
            var p2 = controlPoint1.InTangentPosition;
            var p3 = controlPoint1.position;
            var position = Bezier.GetPoint(p0, p1, p2, p3, 0.5f);

            // Compute orientation
            var eulerCurve = new AnimationCurve3();
            eulerCurve.AddKeys(0, controlPoint0.EulerOrientation);
            eulerCurve.AddKeys(1, controlPoint1.EulerOrientation);
            //var animCurveX = new AnimationCurve(new(0, controlPoint0.EulerOrientation.x), new(1, controlPoint1.EulerOrientation.x));
            //var animCurveY = new AnimationCurve(new(0, controlPoint0.EulerOrientation.y), new(1, controlPoint1.EulerOrientation.y));
            //var animCurveZ = new AnimationCurve(new(0, controlPoint0.EulerOrientation.z), new(1, controlPoint1.EulerOrientation.z));
            //var orientation = math.lerp(controlPoint0.EulerOrientation, controlPoint1.EulerOrientation, 0.5f);
            //var orientation = new Vector3(
            //    animCurveX.Evaluate(0.5f),
            //    animCurveY.Evaluate(0.5f),
            //    animCurveZ.Evaluate(0.5f));
            var eulerOrientation = eulerCurve.Evaluate(0.5f);

            var scale = Vector2.Lerp(controlPoint0.scale, controlPoint1.scale, 0.5f);

                // Make a new control point and insert it.
            var newControlPoint = new FixedBezierPoint()
            {
                position = position,
                EulerOrientation = eulerOrientation,
                scale = scale,
            };
            // Method will resolve distances between control points.
            InsertControlPoint(index1, newControlPoint);
        }

        public void RemoveControlPoint(int index)
        {
            bool cannotRemovePoints = controlPoints.Count <= 2;
            bool isInvalidIndex = !IsValidIndex(index);
            if (cannotRemovePoints || isInvalidIndex)
                return;

            // Remove control point
            controlPoints.RemoveAt(index);
            // Remove distance and recompute
            distancesBetweenControlPoints.RemoveAt(index);
            UpdateCurveDistanceTouchingControlPoint(index);
            UpdateLinearDistanceTouchingControlPoint(index);
        }
        public void RemoveAt(int index)
            => RemoveControlPoint(index);

        
        public static List<FixedBezierPoint> DefaultControlPoints()
        {
            float defaultScale = 90; // TODO make centralized
            var controlPoint0 = new FixedBezierPoint()
            {
                position = new Vector3(0, 0, 0),
                Orientation = Quaternion.identity,
                scale = Vector2.one * defaultScale,
                keyPosition = new bool3(true),
                keyOrientation = new bool3(true),
                keyScale = new bool2(true),
            };
            var controlPoint1 = new FixedBezierPoint()
            {
                position = new Vector3(0, 0, 500),
                Orientation = Quaternion.identity,
                scale = Vector2.one * defaultScale,
                keyPosition = new bool3(true),
                keyOrientation = new bool3(true),
                keyScale = new bool2(true),
            };
            var list = new List<FixedBezierPoint>();
            list.Add(controlPoint0);
            list.Add(controlPoint1);

            return list;
        }

        protected override void Reset()
        {
            base.Reset();

            // Create default control points
            controlPoints = DefaultControlPoints();
            // Make distances between control points
            distancesBetweenControlPoints = new List<float>(new float[controlPoints.Count-1]);
            UpdateCurveDistanceTouchingControlPoint(0);
            UpdateLinearDistanceTouchingControlPoint(0);
        }


        public void SetControlPoint(int index, FixedBezierPoint controlPoint)
        {
            controlPoints[index] = controlPoint;
        }
        public FixedBezierPoint GetControlPoint(int index)
        {
            return controlPoints[index];
        }


        private void UpdateCurveDistanceFromControlPointToNext(int index)
        {
            // Ignore call if invalid index
            bool isValidIndex = index >= 0 && index < distancesBetweenControlPoints.Count;
            if (!isValidIndex)
                return;

            float approximateDistance = ComputeApproximateDistance(index);
            distancesBetweenControlPoints[index] = approximateDistance;
        }
        public void UpdateCurveDistanceTouchingControlPoint(int index)
        {
            UpdateCurveDistanceFromControlPointToNext(index); // this to next 
            UpdateCurveDistanceFromControlPointToNext(index - 1); // previous to this
        }

        /// <summary>
        /// Compute distances
        /// </summary>
        /// <param name="index"></param>
        public void UpdateLinearDistanceFromControlPointToNext(int index)
        {
            int index0 = index + 0;
            int index1 = index + 1;

            bool isValidIndex0 = IsValidIndex(index0);
            bool isValidIndex1 = IsValidIndex(index1);
            if (!isValidIndex0 || !isValidIndex1)
                return;

            var controlPoint0 = GetControlPoint(index0);
            var controlPoint1 = GetControlPoint(index1);
            float distance = Vector3.Distance(controlPoint0.position, controlPoint1.position);
            controlPoint0.linearDistanceOut = distance;
            controlPoint1.linearDistanceIn = distance;

            // Update linear distances
            SetControlPoint(index0, controlPoint0);
            SetControlPoint(index1, controlPoint1);
        }
        public void UpdateLinearDistanceTouchingControlPoint(int index)
        {
            UpdateLinearDistanceFromControlPointToNext(index); // this to next 
            UpdateLinearDistanceFromControlPointToNext(index - 1); // previous to this
        }
        private float ComputeApproximateDistance(int index)
        {
            var controlPoint0 = GetControlPoint(index);
            var controlPoint1 = GetControlPoint(index + 1);

            // TEMP: todo - compute length with velocity?, etc.
            float distance = 0f;
            for (int i = 0; i < 1000; i++)
            {
                float t0 = (float)(i + 0) / (1000);
                float t1 = (float)(i + 1) / (1000);
                var p0 = controlPoint0.position;
                var p1 = controlPoint0.OutTangentPosition;
                var p2 = controlPoint1.InTangentPosition;
                var p3 = controlPoint1.position;
                var point0 = Bezier.GetPoint(p0, p1, p2, p3, t0);
                var point1 = Bezier.GetPoint(p0, p1, p2, p3, t1);
                float delta = Vector3.Distance(point0, point1);
                distance += delta;
            }
            return distance;
        }

        public bool IsValidIndex(int index)
        {
            bool isLargeEnough = index >= 0;
            bool isSmallEnough = index < ControlPointsLength;
            bool isValidIndex = isLargeEnough & isSmallEnough;
            return isValidIndex;
        }

        private void OnDrawGizmosSelected()
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            float length = GetSegmentLength();

            Gizmos.color = Color.green;
            for (int i = 0; i < 8; i++)
            {
                float time = i / 8f;
                var matrix = animationCurveTRS.EvaluateMatrix(time * length);

                var position = matrix.Position();
                var rotation = matrix.rotation;
                var scale = matrix.lossyScale + Vector3.one;
                Gizmos.DrawMesh(mesh, 0, transform.TransformPoint(position), transform.rotation * rotation, scale);
            }
        }
    }
}
