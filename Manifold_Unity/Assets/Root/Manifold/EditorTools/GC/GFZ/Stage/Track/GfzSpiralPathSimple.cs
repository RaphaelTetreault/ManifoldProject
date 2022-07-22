using System;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzSpiralPathSimple : GfzTrackSegmentRootNode,
        IPositionEvaluable
    {
        [SerializeField] private SpiralAxes axes = SpiralAxes.XZRight;
        [SerializeField] private AnimationCurve rollsCurve = new(new(0, 0), new(1, 0));
        [SerializeField] private AnimationCurve scaleX = new(new(0, 60), new(1, 60));
        [SerializeField] private AnimationCurve scaleY = new(new(0, 1), new(1, 1));
        [SerializeField] private float radius0 = 200;
        [SerializeField] private float radius1 = 200;
        [SerializeField] private float offset1 = 0;
        [SerializeField] private float degrees = 90f;
        [SerializeField] private int nKeysPer360Degrees = 36;
        [SerializeField] private bool autoGenerateTRS = true;
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private float gizmosStep = 10f;
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
        public (Keyframe[] rx, Keyframe[] ry) ComponentsToRotationXZRight(float angleSlope, float maxTime)
        {
            // Subtract slope, add degrees
            Vector3 rotation = GetRotation().eulerAngles;
            var rx = new Keyframe[] { new(0, rotation.x), new(0.001f, rotation.x - angleSlope), new(maxTime, rotation.x - angleSlope), };
            var ry = new Keyframe[] { new(0, rotation.y), new(maxTime, rotation.y + degrees), };
            return (rx, ry);
        }
        public (Keyframe[] rx, Keyframe[] ry) ComponentsToRotationXZLeft(float angleSlope, float maxTime)
        {
            // Add slope, subtract degrees
            Vector3 rotation = GetRotation().eulerAngles;
            var rx = new Keyframe[] { new(0, rotation.x), new(0.001f, rotation.x + angleSlope), new(maxTime, rotation.x + angleSlope), };
            var ry = new Keyframe[] { new(0, rotation.y), new(maxTime, rotation.y - degrees), };
            return (rx, ry);
        }

        public (Keyframe[] rx, Keyframe[] ry) ComponentsToRotationYZUp(float angleSlope, float maxTime)
        {
            //
            Vector3 rotation = GetRotation().eulerAngles;
            var rx = new Keyframe[] { new(0, rotation.x), new(maxTime, rotation.x - degrees), };
            var ry = new Keyframe[] { new(0, rotation.y), new(0.001f, rotation.y - angleSlope), new(maxTime, rotation.y - angleSlope), };
            return (rx, ry);
        }
        public (Keyframe[] rx, Keyframe[] ry) ComponentsToRotationYZDown(float angleSlope, float maxTime)
        {
            //
            Vector3 rotation = GetRotation().eulerAngles;
            var rx = new Keyframe[] { new(0, rotation.x), new(maxTime, rotation.x + degrees), };
            var ry = new Keyframe[] { new(0, rotation.y), new(0.001f, rotation.y + angleSlope), new(maxTime, rotation.y + angleSlope), };
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

        public void UpdateTRS()
        {
            var startPosition = GetPosition();
            var startRotation = GetRotation().eulerAngles;
            var positionXYZ = new AnimationCurve3();

            var componentToSpiralFunction = ComponentsToSpiral(axes);
            var componentsToRotationKeys = ComponentsToRotationKeys(axes);
            var maxTimeRolls = rollsCurve.GetMaxTime();

            Quaternion orientation = GetRotation();
            Vector3 prevEulers = Vector3.zero;

            // Length of curve
            // Todo: archemides spiral calc?
            float radiusAverage = ((radius0 + radius1) / 2f);
            float length = 2 * Mathf.PI * radiusAverage * (degrees / 360f);
            float angleSlope = Mathf.Atan(offset1 / length) * Mathf.Rad2Deg;

            int nKeys = Mathf.CeilToInt(degrees * nKeysPer360Degrees / 360f);
            for (int i = 0; i <= nKeys; i++)
            {
                // Time / step
                float time = i / (float)nKeys;
                float stepDistanceDegrees = time * degrees;
                float stepDistanceRadians = (stepDistanceDegrees % 360) * Mathf.Deg2Rad; // mod to increase precision

                // Compute radius point
                float radius = Mathf.Lerp(radius0, radius1, time);
                float cos = Mathf.Cos(stepDistanceRadians);
                float sin = Mathf.Sin(stepDistanceRadians);
                // Compute position
                float componentA0 = cos * radius - radius0;
                float componentB0 = sin * radius;
                float offset = Mathf.Lerp(0, offset1, time);
                Vector3 spiralPosition0 = componentToSpiralFunction(componentA0, componentB0, offset);
                Vector3 position = startPosition + orientation * spiralPosition0;
                positionXYZ.AddKeys(time, position);
            }


            bool isXZ = axes == SpiralAxes.XZLeft || axes == SpiralAxes.XZRight;
            bool isYZ = axes == SpiralAxes.YZUp || axes == SpiralAxes.YZDown;

            // POSITION
            var kPosition = GetPosition();
            var keysPX = isYZ
                ? new Keyframe[] { new(0, kPosition.x), new(length, kPosition.x + offset1), }
                : positionXYZ.x.GetRenormalizedKeyRangeAndTangents(0, length);
            var keysPY = isXZ
                ? new Keyframe[] { new(0, kPosition.y), new(length, kPosition.y + offset1), }
                : positionXYZ.y.GetRenormalizedKeyRangeAndTangents(0, length);
            var keysPZ = positionXYZ.z.GetRenormalizedKeyRangeAndTangents(0, length);

            // ROTATION
            (var keysRX, var keysRY) = componentsToRotationKeys(angleSlope, length);
            var keysRZ = rollsCurve.GetRenormalizedKeyRangeAndTangents(0, length);

            // Create TRS
            var trs = new AnimationCurveTRS(
                    new AnimationCurve3(keysPX, keysPY, keysPZ),
                    new AnimationCurve3(keysRX, keysRY, keysRZ),
                    CreateScaleCurvesXYZ(length));

            // make linear curves linear
            if (isXZ)
            {
                trs.Position.y.SmoothTangents();
                trs.Rotation.y.SmoothTangents();
            }
            else if (isYZ)
            {
                trs.Position.x.SmoothTangents();
                trs.Rotation.x.SmoothTangents();
            }

            animationCurveTRS = trs;
        }

    }
}
