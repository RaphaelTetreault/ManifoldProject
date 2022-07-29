using GameCube.GFZ.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class ExperimentalGfzSpiralPath : GfzPathSegment,
        IPositionEvaluable
    {
        [SerializeField] private SpiralAxes SpiralAxes = SpiralAxes.VerticalUp;
        [SerializeField] private UnityEngine.AnimationCurve radiusCurve = new(new(0f, 30), new(1, 30));
        [SerializeField] private UnityEngine.AnimationCurve offsetCurve = new(new(0, 0), new(1, 0));
        [SerializeField] private UnityEngine.AnimationCurve rollsCurve = new(new(0, 0), new(1, 0));
        [SerializeField] private UnityEngine.AnimationCurve scaleX = new(new(0, 60), new(1, 60));
        [SerializeField] private UnityEngine.AnimationCurve scaleY = new(new(0, 1), new(1, 1));
        [SerializeField, Min(1f)] private float degrees = 90f;
        [SerializeField, Min(8)] private int nKeysPer360Degrees = 36;
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

        public Func<float, float, float, Vector3> ComponentsToSpiral(SpiralAxes SpiralAxes)
        {
            switch (SpiralAxes)
            {
                case SpiralAxes.HorizontalRight: return ComponentsToXZRight;
                case SpiralAxes.HorizontalLeft: return ComponentsToXZLeft;
                case SpiralAxes.VerticalUp: return ComponentsToXYUp;
                case SpiralAxes.VerticalDown: return ComponentsToXYDown;
                default:
                    throw new ArgumentException($"Invalid {nameof(SpiralAxes)} value.");
            }
        }
        public Vector3 ComponentsToXZRight(float x, float z, float offset) => new Vector3(-x, offset, z);
        public Vector3 ComponentsToXZLeft(float x, float z, float offset) => new Vector3(+x, offset, z);
        public Vector3 ComponentsToXYUp(float y, float z, float offset) => new Vector3(offset, -y, z);
        public Vector3 ComponentsToXYDown(float y, float z, float offset) => new Vector3(offset, +y, z);


        public Func<float, float, float, Quaternion> ComponentsToOrientation(SpiralAxes SpiralAxes)
        {
            switch (SpiralAxes)
            {
                case SpiralAxes.HorizontalRight: return ComponentsToNormalXZRight;
                case SpiralAxes.HorizontalLeft: return ComponentsToNormalXZLeft;
                case SpiralAxes.VerticalUp: return ComponentsToNormalYZUp;
                case SpiralAxes.VerticalDown: return ComponentsToNormalYZDown;

                default:
                    throw new ArgumentException($"Invalid {nameof(SpiralAxes)} value.");
            }
        }
        public Quaternion ComponentsToNormalXZRight(float cos, float sin, float angle)
        {
            var rotationX = Quaternion.AngleAxis(angle, Vector3.right);
            var forwardXZ = new Vector3(sin, 0, cos).normalized;
            var rotation = Quaternion.LookRotation(forwardXZ, Vector3.up);
            rotation = rotationX * rotation;
            return rotation;
        }
        public Quaternion ComponentsToNormalXZLeft(float cos, float sin, float angle)
            => ComponentsToNormalXZRight(cos, -sin, angle);
        public Quaternion ComponentsToNormalYZUp(float cos, float sin, float angle)
        {
            var rotationYZ = Quaternion.LookRotation(new(0, sin, cos), Vector3.right);
            var rotationX = Quaternion.AngleAxis(angle, Vector3.up);
            var rotation = rotationYZ * rotationX;
            return rotation;
        }
        public Quaternion ComponentsToNormalYZDown(float cos, float sin, float angle)
            => ComponentsToNormalYZUp(cos, -sin, angle);

        public Func<float, float, Keyframe, Vector3> ComponentsToRotation(SpiralAxes SpiralAxes)
        {
            switch (SpiralAxes)
            {
                case SpiralAxes.HorizontalRight: return ComponentsToRotationXZRight;
                case SpiralAxes.HorizontalLeft:
                case SpiralAxes.VerticalUp:
                case SpiralAxes.VerticalDown:

                default:
                    throw new ArgumentException($"Invalid {nameof(SpiralAxes)} value.");
            }
        }
        public Vector3 ComponentsToRotationXZRight(float cos, float sin, Keyframe keyframe)
        {
            float outTangent = keyframe.outTangent;
            float atan = math.atan(outTangent) * Mathf.Rad2Deg;
            float tan = math.tan(outTangent) * Mathf.Rad2Deg;
            float angle = -(atan + tan) / 2f;
            var rotationX = Quaternion.AngleAxis(angle, Vector3.right);

            var forwardXZ = new Vector3(sin, 0, cos).normalized;
            var rotationXZ = Quaternion.LookRotation(forwardXZ, Vector3.up);
            
            rotationXZ = rotationX * rotationXZ;
            return rotationXZ.eulerAngles;
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
            var rotationXYZ = new AnimationCurve3();

            var componentToSpiralFunction = ComponentsToSpiral(SpiralAxes);
            var componentToSprialNormal = ComponentsToOrientation(SpiralAxes);

            var maxTimeOffset = offsetCurve.GetMaxTime();
            var maxTimeRadius = radiusCurve.GetMaxTime();
            var maxTimeRolls = rollsCurve.GetMaxTime();
            var initRadius = radiusCurve.Evaluate(0);

            int nKeys = Mathf.CeilToInt(degrees * nKeysPer360Degrees / 360f);
            //float stepDegrees = degrees / nKeys;
            for (int i = 0; i <= nKeys; i++)
            {
                double percentage = i / (double)nKeys;
                double stepDistanceDegrees = percentage * degrees;
                double stepDistanceRadians = (stepDistanceDegrees % 360) * Mathf.Deg2Rad; // mod to increase precision
                float time = (float)percentage;

                // 
                float radius = radiusCurve.Evaluate(time * maxTimeRadius);
                double cos = Math.Cos(stepDistanceRadians);
                double sin = Math.Sin(stepDistanceRadians);

                // Compute position
                double componentA0 = cos * radius - initRadius;
                double componentB0 = sin * radius;
                float offset0 = offsetCurve.Evaluate(time * maxTimeOffset);
                Vector3 spiralPosition0 = componentToSpiralFunction((float)componentA0, (float)componentB0, offset0);
                Vector3 position0 = startPosition + spiralPosition0;
                positionXYZ.AddKeys(time, position0);
            }
            positionXYZ.y.SmoothTangents(0, 1/3f);

            //
            var prevRotation = Vector3.zero;
            for (int i = 0; i < nKeys; i++)
            {
                float time0 = (i+0) / (float)nKeys;
                float time1 = time0 + 0.0001f;
                Vector3 p0 = positionXYZ.Evaluate(time0);
                Vector3 p1 = positionXYZ.Evaluate(time1);
                Vector3 direction = (p1 - p0).normalized;

                Quaternion roll = Quaternion.Euler(new(0, 0, rollsCurve.Evaluate(time0 * maxTimeRolls)));
                int rotationsCompleted = Mathf.FloorToInt(time0 * (degrees / 360f));

                Quaternion rotation = Quaternion.LookRotation(direction, roll * Vector3.up); // req: up vec
                Vector3 eulers = rotation.eulerAngles;
                eulers.x = CleanEulerRotation360(prevRotation.x, eulers.x, 0); // req: correct axis
                eulers.y = CleanEulerRotation360(prevRotation.y, eulers.y, rotationsCompleted);
                rotationXYZ.AddKeys(time0, eulers);
                prevRotation = eulers;
            }
            // last key
            {
                Vector3 p0 = positionXYZ.Evaluate(1f - 0.0001f);
                Vector3 p1 = positionXYZ.Evaluate(1f);
                Vector3 direction = (p1 - p0).normalized;

                Quaternion roll = Quaternion.Euler(new(0, 0, rollsCurve.Evaluate(maxTimeRolls)));
                int rotationsCompleted = Mathf.FloorToInt(degrees / 360f);

                Quaternion rotation = Quaternion.LookRotation(direction, roll * Vector3.up); // req: up vec
                Vector3 eulers = rotation.eulerAngles;
                eulers.x = CleanEulerRotation360(prevRotation.x, eulers.x, 0); // req: correct axis
                eulers.y = CleanEulerRotation360(prevRotation.y, eulers.y, rotationsCompleted);
                rotationXYZ.AddKeys(1, eulers);
                //prevRotation = eulers;
            }

            rotationXYZ.x.SmoothTangents();
            rotationXYZ.y.SmoothTangents();

            //rotationXYZ.x = rotationXYZ.x.SetKeyTangents(0);




            // "Lazy" key time adjustment
            animationCurveTRS = new AnimationCurveTRS(positionXYZ, rotationXYZ, new());
            animationCurveTRS.CleanDuplicateKeys();

            float curveLength = (float)CurveUtility.GetDistanceBetweenRepeated(this, 0, 1);
            var keysPX = positionXYZ.x.GetRenormalizedKeyRangeAndTangents(0, curveLength);
            var keysPY = positionXYZ.y.GetRenormalizedKeyRangeAndTangents(0, curveLength);
            var keysPZ = positionXYZ.z.GetRenormalizedKeyRangeAndTangents(0, curveLength);
            var keysRX = rotationXYZ.x.GetRenormalizedKeyRangeAndTangents(0, curveLength);
            var keysRY = rotationXYZ.y.GetRenormalizedKeyRangeAndTangents(0, curveLength);
            var keysRZ = rollsCurve.GetRenormalizedKeyRangeAndTangents(0, curveLength);

            var trs = new AnimationCurveTRS(
                    new AnimationCurve3(keysPX, keysPY, keysPZ),
                    new AnimationCurve3(keysRX, keysRY, keysRZ),
                    CreateScaleCurvesXYZ(curveLength));

            animationCurveTRS = trs;
        }


        public static float CleanEulerRotation360(float prev, float curr, float nFullRotationsCompleted)
        {
            float delta = curr - prev;
            float offset = 360f * nFullRotationsCompleted;

            // If the difference is lower than max range
            if (delta > 180)
                curr -= offset;
            // if the difference is above the min range
            else if (delta < -180)
                curr += offset;

            return curr;
        }
        public static float3 CleanEulerRotation3(float3 prev, float3 curr, float nFullRotationsCompleted)
        {
            var x = CleanEulerRotation360(prev.x, curr.x, nFullRotationsCompleted);
            var y = CleanEulerRotation360(prev.y, curr.y, nFullRotationsCompleted);
            var z = CleanEulerRotation360(prev.z, curr.z, nFullRotationsCompleted);
            return new float3(x, y, z);
        }

    }
}
