using System;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzSpiralPathSimple : GfzTrackSegmentRootNode,
        IPositionEvaluable
    {
        [SerializeField] private SpiralAxes axes = SpiralAxes.XZRight;
        [SerializeField] private AnimationCurve rotationZ = new(new(0, 0), new(1, 0));
        [SerializeField] private AnimationCurve scaleX = new(new(0, 60), new(1, 60));
        [SerializeField] private AnimationCurve scaleY = new(new(0, 1), new(1, 1));
        [SerializeField, Min(0)] private float radius0 = 200;
        [SerializeField, Min(0)] private float radius1 = 200;
        [SerializeField] private float axisOffset = 0;
        [SerializeField, Min(1)] private float rotateDegrees = 90f;
        [SerializeField, Min(8)] private int keysPer360Degrees = 36;
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();

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
                case SpiralAxes.XZRight: return ComponentsToXZRight;
                case SpiralAxes.XZLeft: return ComponentsToXZLeft;
                case SpiralAxes.YZUp: return ComponentsToXYUp;
                case SpiralAxes.YZDown: return ComponentsToXYDown;
                default:
                    throw new ArgumentException($"Invalid {nameof(SpiralAxes)} value.");
            }
        }
        public Vector3 ComponentsToXZRight(float cos, float sin, float offset) => new Vector3(-cos, offset, sin);
        public Vector3 ComponentsToXZLeft(float cos, float sin, float offset) => new Vector3(+cos, offset, sin);
        public Vector3 ComponentsToXYUp(float cos, float sin, float offset) => new Vector3(offset, -cos, sin);
        public Vector3 ComponentsToXYDown(float cos, float sin, float offset) => new Vector3(offset, +cos, sin);

        // ideally this value would scale with the length of the curve.
        private const float minDelta = 0.5f;
        private const float percentSmooth = 0.05f;
        public Func<float, float, (Keyframe[], Keyframe[])> ComponentsToRotationKeys(SpiralAxes axes)
        {
            switch (axes)
            {
                case SpiralAxes.XZRight: return ComponentsToRotationXZRight;
                case SpiralAxes.XZLeft: return ComponentsToRotationXZLeft;
                case SpiralAxes.YZUp: return ComponentsToRotationYZUp;
                case SpiralAxes.YZDown: return ComponentsToRotationYZDown;
                default:
                    throw new ArgumentException($"Invalid {nameof(SpiralAxes)} value.");
            }
        }
        public Keyframe[] GetSlopes(float rotation, float angleSlope, float maxTime)
        {
            float smoothTime = percentSmooth * maxTime;
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

        public override void UpdateTRS()
        {
            var startPosition = GetPosition();
            var startRotation = GetRotation().eulerAngles;
            var positionXYZ = new AnimationCurve3();

            var componentToSpiralFunction = ComponentsToSpiral(axes);
            var componentsToRotationKeys = ComponentsToRotationKeys(axes);
            var maxTimeRolls = rotationZ.GetMaxTime();

            Quaternion orientation = GetRotation();
            Vector3 prevEulers = Vector3.zero;

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
                float radius = Mathf.Lerp(radius0, radius1, time);
                float cos = Mathf.Cos(stepDistanceRadians);
                float sin = Mathf.Sin(stepDistanceRadians);
                // Compute position
                float componentA0 = cos * radius - radius0;
                float componentB0 = sin * radius;
                float offset = Mathf.Lerp(0, axisOffset, time);
                Vector3 spiralPosition0 = componentToSpiralFunction(componentA0, componentB0, offset);
                Vector3 position = startPosition + orientation * spiralPosition0;
                positionXYZ.AddKeys(time, position);
            }


            bool isXZ = axes == SpiralAxes.XZLeft || axes == SpiralAxes.XZRight;
            bool isYZ = axes == SpiralAxes.YZUp || axes == SpiralAxes.YZDown;

            // POSITION
            var kPosition = GetPosition();
            var keysPX = isYZ
                ? new Keyframe[] { new(0, kPosition.x), new(length, kPosition.x + axisOffset), }
                : positionXYZ.x.GetRenormalizedKeyRangeAndTangents(0, length);
            var keysPY = isXZ
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

            if (isXZ)
            {
                // make Y curves linear
                trs.Position.y.SmoothTangents();
                trs.Rotation.y.SmoothTangents();
                // Smooth first and last tangents, makes angle change much less noticible
                trs.Rotation.x.SmoothTangents(0, 1 / 3f);
                trs.Rotation.x.SmoothTangents(trs.Rotation.y.length - 1, 1 / 3f);
            }
            else if (isYZ)
            {
                // make X curves linear
                trs.Position.x.SmoothTangents();
                trs.Rotation.x.SmoothTangents();
                // Smooth first and last tangents, makes angle change much less noticible
                trs.Rotation.y.SmoothTangents(0, 1 / 3f);
                trs.Rotation.y.SmoothTangents(trs.Rotation.y.length-1, 1 / 3f);
            }

            animationCurveTRS = trs;
        }

    }
}
