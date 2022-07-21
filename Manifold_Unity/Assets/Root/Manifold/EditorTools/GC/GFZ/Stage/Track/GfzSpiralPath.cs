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
    public class GfzSpiralPath : GfzTrackSegmentRootNode,
        IPositionEvaluable
    {
        public enum Axes
        {
            YZUp,
            YZDown,
            XZLeft,
            XZRight,
        }

        [SerializeField] private Axes axes = Axes.YZUp;
        [SerializeField] private UnityEngine.AnimationCurve radiusCurve = new(new(0f, 30), new(1, 30));
        [SerializeField] private UnityEngine.AnimationCurve offsetCurve = new(new(0, 0), new(1, 0));
        [SerializeField] private UnityEngine.AnimationCurve rollsCurve = new(new(0, 0), new(1, 0));
        [SerializeField] private UnityEngine.AnimationCurve scaleX = new(new(0, 60), new(1, 60));
        [SerializeField] private UnityEngine.AnimationCurve scaleY = new(new(0, 1), new(1, 1));
        [SerializeField] private float degrees = 90f;
        [SerializeField] private int nKeysPer360Degrees = 36;
        [SerializeField] private bool autoGenerateTRS = true;
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private float gizmosStep = 10f;
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();


        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            var trs = isGfzCoordinateSpace
                ? animationCurveTRS.CreateGfzCoordinateSpace()
                : animationCurveTRS.CreateDeepCopy();

            return trs;
        }

        public override TrackSegment CreateTrackSegment()
        {
            throw new NotImplementedException();
        }

        public override float GetMaxTime()
        {
            return animationCurveTRS.GetMaxTime();
        }

        public override float GetSegmentLength()
        {
            return animationCurveTRS.GetMaxTime();
        }

        public Func<float, float, float, Vector3> ComponentsToSpiral(Axes axes)
        {
            switch (axes)
            {
                case Axes.XZRight: return ComponentsToXZRight;
                case Axes.XZLeft: return ComponentsToXZLeft;
                case Axes.YZUp: return ComponentsToXYUp;
                case Axes.YZDown: return ComponentsToXYDown;
                default:
                    throw new ArgumentException($"Invalid {nameof(Axes)} value.");
            }
        }
        public Vector3 ComponentsToXZRight(float x, float z, float offset) => new Vector3(-x, offset, z);
        public Vector3 ComponentsToXZLeft(float x, float z, float offset) => new Vector3(+x, offset, z);
        public Vector3 ComponentsToXYUp(float y, float z, float offset) => new Vector3(offset, -y, z);
        public Vector3 ComponentsToXYDown(float y, float z, float offset) => new Vector3(offset, +y, z);


        public Func<float, float, float, Quaternion> ComponentsToOrientation(Axes axes)
        {
            switch (axes)
            {
                case Axes.XZRight:
                case Axes.XZLeft:
                    return ComponentsToNormalXZ;

                case Axes.YZUp:
                case Axes.YZDown:
                    return ComponentsToNormalYZ;

                default:
                    throw new ArgumentException($"Invalid {nameof(Axes)} value.");
            }
        }

        public Quaternion ComponentsToNormalXZ(float cos, float sin, float offset)
        {
            var rotationXZ = Quaternion.LookRotation(new(sin, 0, cos), Vector3.up);
            var normalY = new Vector3(0, offset, 1).normalized;
            var rotationY = Quaternion.LookRotation(normalY, Vector3.up);
            var rotation = rotationXZ * rotationY;
            return rotation;
        }

        public Quaternion ComponentsToNormalYZ(float cos, float sin, float offset)
        {
            var rotationYZ = Quaternion.LookRotation(new(0, sin, cos), Vector3.right);
            var normalX = new Vector3(offset, 1, 0).normalized;
            var rotationY = Quaternion.LookRotation(normalX, Vector3.up);
            var rotation = rotationYZ * rotationY;
            return rotation;
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
            var rotationXYZ = new AnimationCurve3();

            var componentToSpiralFunction = ComponentsToSpiral(axes);
            var componentToSprialNormal = ComponentsToOrientation(axes);

            var maxTimeOffset = offsetCurve.GetMaxTime();
            var maxTimeRadius = radiusCurve.GetMaxTime();
            var initRadius = radiusCurve.Evaluate(0);
            var prevRotation = Vector3.zero;

            int nKeys = Mathf.CeilToInt(degrees * nKeysPer360Degrees / 360f);
            float stepDegrees = degrees / nKeys;
            for (int i = 0; i <= nKeys; i++)
            {
                float percentage = i / (float)nKeys;
                float stepDistance = percentage * degrees * Mathf.Deg2Rad;

                // Compute position
                float radius = radiusCurve.Evaluate(percentage * maxTimeRadius);
                float cos = Mathf.Cos(stepDistance);
                float sin = Mathf.Sin(stepDistance);
                float componentA0 = cos * radius - initRadius;
                float componentB0 = sin * radius;
                float offset0 = offsetCurve.Evaluate(percentage * maxTimeOffset);
                Vector3 spiralPosition0 = componentToSpiralFunction(componentA0, componentB0, offset0);
                Vector3 position0 = startPosition + spiralPosition0;

                // compute rotation
                float timeEpsilon = percentage + 0.001f;
                float offset1 = offsetCurve.Evaluate(timeEpsilon * maxTimeOffset);
                float offsetDelta = offset1 - offset0;
                // Convert normal to direction
                Quaternion qRotation0 = componentToSprialNormal(cos, sin, offsetDelta);
                Vector3 eRotation = CurveUtility.CleanRotation(prevRotation, qRotation0.eulerAngles);
                Vector3 rotation0 = startRotation + eRotation;
                prevRotation = eRotation;
                // TODO: clean rotation

                // Add keys normalized
                positionXYZ.AddKeys(percentage, position0);
                rotationXYZ.AddKeys(percentage, rotation0);
            }

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

    }
}
