using System;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzPathSpiral : GfzPathSegment,
        IPositionEvaluable
    {
        [SerializeField] private SpiralAxes axes = SpiralAxes.HorizontalRight;
        [SerializeField] private AnimationCurve rotationZ = new(new(0, 0), new(1, 0));
        [SerializeField] private AnimationCurve scaleX = new(new(0, 60), new(1, 60));
        [SerializeField] private AnimationCurve scaleY = new(new(0, 1), new(1, 1));
        [SerializeField, Min(0)] private float radius0 = 200;
        [SerializeField, Min(0)] private float radius1 = 200;
        [SerializeField] private float axisOffset = 0;
        [SerializeField, Range(1f, 1800f)] private float rotateDegrees = 90f;
        [SerializeField, Min(8)] private int keysPer360Degrees = 36;
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();

        // Rather than linear interpolation, do smooth cubic interpolation
        private static readonly AnimationCurve RadiiCubicSmooth = new AnimationCurve(new(0, 0), new (1, 1));

        protected override AnimationCurveTRS TrackSegmentAnimationCurveTRS => animationCurveTRS;

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            var trs = isGfzCoordinateSpace
                ? animationCurveTRS.CreateGfzCoordinateSpace()
                : animationCurveTRS.CreateDeepCopy();

            return trs;
        }

        public override float GetMaxTime()
        {
            return animationCurveTRS.GetMaxTime();
        }

        public override float GetSegmentLength()
        {
            return animationCurveTRS.GetMaxTime();
        }

        public Func<float, float, float, Vector3> ComponentsToSpiral(SpiralAxes axes)
        {
            switch (axes)
            {
                case SpiralAxes.HorizontalRight: return ComponentsToXZRight;
                case SpiralAxes.HorizontalLeft: return ComponentsToXZLeft;
                case SpiralAxes.VerticalUp: return ComponentsToXYUp;
                case SpiralAxes.VerticalDown: return ComponentsToXYDown;
                default:
                    throw new ArgumentException($"Invalid {nameof(SpiralAxes)} value.");
            }
        }
        public Vector3 ComponentsToXZRight(float cos, float sin, float offset) => new Vector3(-cos, offset, sin);
        public Vector3 ComponentsToXZLeft(float cos, float sin, float offset) => new Vector3(+cos, offset, sin);
        public Vector3 ComponentsToXYUp(float cos, float sin, float offset) => new Vector3(offset, -cos, sin);
        public Vector3 ComponentsToXYDown(float cos, float sin, float offset) => new Vector3(offset, +cos, sin);

        public Func<float, float, (Keyframe[], Keyframe[])> ComponentsToRotationKeys(SpiralAxes axes)
        {
            switch (axes)
            {
                case SpiralAxes.HorizontalRight: return ComponentsToRotationXZRight;
                case SpiralAxes.HorizontalLeft: return ComponentsToRotationXZLeft;
                case SpiralAxes.VerticalUp: return ComponentsToRotationYZUp;
                case SpiralAxes.VerticalDown: return ComponentsToRotationYZDown;
                default:
                    throw new ArgumentException($"Invalid {nameof(SpiralAxes)} value.");
            }
        }
        public Keyframe[] GetSlopes(float rotation, float angleSlope, float maxTime)
        {
            float relativeDistance = 0.10f * maxTime; // 10%
            const float fixedDistance = 100f; // 100m
            // Select whichever "distance" along path is shortest.
            // Means smoothing for short curves is at 10% from ends, at 100m for long curves at ends.
            float smoothTime = relativeDistance < fixedDistance ? relativeDistance : fixedDistance;

            var keys = new Keyframe[]
            {
                new(0, rotation),
                new(maxTime - smoothTime, rotation + angleSlope),
                new(smoothTime, rotation + angleSlope),
                new(maxTime, rotation),
            };
            return keys;
        }
        public Keyframe[] GetDegrees(float rotation, float rotateDegrees, float maxTime)
        {
            var keys = new Keyframe[]
            {
                new(0, rotation),
                new(maxTime, rotation + rotateDegrees),
            };
            return keys;
        }
        public (Keyframe[] rx, Keyframe[] ry) ComponentsToRotationXZRight(float angleSlope, float maxTime)
        {
            Vector3 rotation = GetRotation().eulerAngles;
            var rx = GetSlopes(rotation.x, -angleSlope, maxTime);
            var ry = GetDegrees(rotation.y, +rotateDegrees, maxTime);
            return (rx, ry);
        }
        public (Keyframe[] rx, Keyframe[] ry) ComponentsToRotationXZLeft(float angleSlope, float maxTime)
        {
            Vector3 rotation = GetRotation().eulerAngles;
            var rx = GetSlopes(rotation.x, +angleSlope, maxTime);
            var ry = GetDegrees(rotation.y, -rotateDegrees, maxTime);
            return (rx, ry);
        }

        public (Keyframe[] rx, Keyframe[] ry) ComponentsToRotationYZUp(float angleSlope, float maxTime)
        {
            Vector3 rotation = GetRotation().eulerAngles;
            var rx = GetDegrees(rotation.x, -rotateDegrees, maxTime);
            var ry = GetSlopes(rotation.y, -angleSlope, maxTime);
            return (rx, ry);
        }
        public (Keyframe[] rx, Keyframe[] ry) ComponentsToRotationYZDown(float angleSlope, float maxTime)
        {
            Vector3 rotation = GetRotation().eulerAngles;
            var rx = GetDegrees(rotation.x, +rotateDegrees, maxTime);
            var ry = GetSlopes(rotation.y, +angleSlope, maxTime);
            return (rx, ry);
        }

        public AnimationCurve3 CreateScaleCurvesXYZ(float maxTime)
        {
            // TODO: multiply keys function
            var x = scaleX.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var y = scaleY.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            var z = new Keyframe[] { new(0, 1f) };
            var curves = new AnimationCurve3(x, y, z);
            return curves;
        }

        public float3 EvaluatePosition(double time)
        {
            return animationCurveTRS.Position.Evaluate(time);
        }

        public float CubicLerp(float a, float b, float time01)
        {
            float cubicTime = RadiiCubicSmooth.Evaluate(time01);
            float value = Mathf.Lerp(a, b, cubicTime);
            return value;
        }

        public override void UpdateTRS()
        {
            var startPosition = GetPosition();
            var startRotation = GetRotation().eulerAngles;
            var positionXYZ = new AnimationCurve3();

            var componentToSpiralPosition = ComponentsToSpiral(axes);
            var componentsToRotationKeys = ComponentsToRotationKeys(axes);
            var maxTimeRolls = rotationZ.GetMaxTime();

            Quaternion orientation = GetRotation();
            Vector3 prevEulers = Vector3.zero;
            //Vector3 prevEulers = orientation.eulerAngles;

            // Length of curve
            // Todo: archemides spiral calc?
            float radiusAverage = ((radius0 + radius1) / 2f);
            float length = 2 * Mathf.PI * radiusAverage * (rotateDegrees / 360f);
            float angleSlope = Mathf.Atan(axisOffset / length) * Mathf.Rad2Deg;

            int nKeys = Mathf.CeilToInt(rotateDegrees * keysPer360Degrees / 360f);
            for (int i = 0; i <= nKeys; i++)
            {
                // Time / step
                float time = i / (float)nKeys;
                float stepDistanceDegrees = time * rotateDegrees;
                float stepDistanceRadians = (stepDistanceDegrees % 360) * Mathf.Deg2Rad; // mod to increase precision

                // Compute radius point
                float radius = CubicLerp(radius0, radius1, time);
                float cos = Mathf.Cos(stepDistanceRadians);
                float sin = Mathf.Sin(stepDistanceRadians);
                // Compute position
                float pCos = cos * radius - radius0;
                float pSin = sin * radius;
                float offset = Mathf.Lerp(0, axisOffset, time);
                Vector3 spiralPosition = componentToSpiralPosition(pCos, pSin, offset);
                Vector3 position = startPosition + orientation * spiralPosition;
                positionXYZ.AddKeys(time, position);
            }


            bool isHorizontal = axes == SpiralAxes.HorizontalLeft || axes == SpiralAxes.HorizontalRight;
            bool isVertical = axes == SpiralAxes.VerticalUp || axes == SpiralAxes.VerticalDown;

            // POSITION
            var kPosition = GetPosition();
            var keysPX = positionXYZ.x.GetRenormalizedKeyRangeAndTangents(0, length);
            var keysPY = isHorizontal
                ? new Keyframe[] { new(0, kPosition.y), new(length, kPosition.y + axisOffset), }
                : positionXYZ.y.GetRenormalizedKeyRangeAndTangents(0, length);
            var keysPZ = positionXYZ.z.GetRenormalizedKeyRangeAndTangents(0, length);

            // ROTATION
            (var keysRX, var keysRY) = componentsToRotationKeys(angleSlope, length);
            var keysRZ = rotationZ.GetRenormalizedKeyRangeAndTangents(0, length);

            // Create TRS
            var trs = new AnimationCurveTRS(
                    new AnimationCurve3(keysPX, keysPY, keysPZ),
                    new AnimationCurve3(keysRX, keysRY, keysRZ),
                    CreateScaleCurvesXYZ(length));

            const float weight = 1 / 3f;

            if (isHorizontal)
            {
                // make Y curves linear
                trs.Position.y.SmoothTangents();
                trs.Rotation.y.SmoothTangents();
                // Smooth first and last tangents, makes angle change much less noticible
                trs.Rotation.x.SmoothTangents(0, weight);
                trs.Rotation.x.SmoothTangents(trs.Rotation.y.length - 1, weight);
                // Fix first key tangents
                trs.Position.x.SmoothTangents(0, weight);
            }
            else if (isVertical)
            {
                // make X curves linear
                trs.Position.x.SmoothTangents();
                trs.Rotation.x.SmoothTangents();
                // Smooth first and last tangents, makes angle change much less noticible
                trs.Rotation.y.SmoothTangents(0, weight);
                trs.Rotation.y.SmoothTangents(trs.Rotation.y.length-1, weight);
                
                // Fix first key tangents
                trs.Position.y.SmoothTangents(0, weight);
                // Temp hack: make first key flat if not X rotation.
                if (GetRotation().eulerAngles.x == 0f)
                    trs.Position.y.SetKeyTangents(0, 0f);
            }

            trs.Position.z.SmoothTangents(0, weight);


            animationCurveTRS = trs;
        }

    }
}
