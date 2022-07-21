using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzSpiralPathSimple : GfzTrackSegmentRootNode,
        IPositionEvaluable
    {
        public enum Axes
        {
            XZLeft,
            XZRight,
            YZUp,
            YZDown,
        }



        [SerializeField] private Axes axes = Axes.XZRight;
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

        //public override GameCube.GFZ.Stage.TrackSegment CreateTrackSegment()
        //{
        //    var trs = animationCurveTRS.CreateDeepCopy();

        //    var trackSegmentRoot = new GameCube.GFZ.Stage.TrackSegment();
        //    var trackSegmentRZ = new GameCube.GFZ.Stage.TrackSegment();
        //    trackSegmentRoot.Children = new GameCube.GFZ.Stage.TrackSegment[] { trackSegmentRZ };
        //    trackSegmentRZ.Children = CreateChildTrackSegments();

        //    trackSegmentRoot.BranchIndex = trackSegmentRZ.BranchIndex = GetBranchIndex();

        //    {
        //        var trsXY = trs.CreateDeepCopy();
        //        trsXY.Rotation.z = new AnimationCurve();
        //        trackSegmentRoot.AnimationCurveTRS = trsXY.ToTrackSegment();
        //    }
        //    {
        //        var trsRZ = new AnimationCurveTRS();
        //        trsRZ.Rotation.z = trs.Rotation.z;
        //        trackSegmentRZ.AnimationCurveTRS = trsRZ.ToTrackSegment();
        //    }

        //    return trackSegmentRoot;
        //}

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

        public Func<float, float, float, Vector3> ComponentsToRotation(Axes axes)
        {
            switch (axes)
            {
                case Axes.XZRight: return ComponentsToRotationXZRight;
                case Axes.XZLeft: return ComponentsToRotationXZLeft;
                case Axes.YZUp:
                case Axes.YZDown:
                default:
                    throw new ArgumentException($"Invalid {nameof(Axes)} value.");
            }
        }
        public Vector3 ComponentsToRotationXZRight(float cos, float sin, float slope)
        {
            Vector3 normalXZ = new Vector3(sin, 0, cos);
            Quaternion rotationY = Quaternion.LookRotation(normalXZ, Vector3.up);
            Quaternion rotationX = Quaternion.AngleAxis(slope, Vector3.right);
            Quaternion rotation = rotationX * rotationY;
            Vector3 eulers = rotation.eulerAngles;
            return eulers;
        }
        public Vector3 ComponentsToRotationXZLeft(float cos, float sin, float slope)
            => ComponentsToRotationXZRight(cos, -sin, slope);



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
            var componentsToRotation = ComponentsToRotation(axes);
            var maxTimeRolls = rollsCurve.GetMaxTime();

            Vector3 prevEulers = Vector3.zero;

            // Length of curve
            // Todo: archemides spiral calc
            float length = 2 * Mathf.PI * radius0 * (degrees / 360f);
            float slope = Mathf.Atan(length / offset1);

            int nKeys = Mathf.CeilToInt(degrees * nKeysPer360Degrees / 360f);
            for (int i = 0; i <= nKeys; i++)
            {
                float time = i / (float)nKeys;
                float stepDistanceDegrees = time * degrees;
                float stepDistanceRadians = (stepDistanceDegrees % 360) * Mathf.Deg2Rad; // mod to increase precision
                int rotationsCompleted = Mathf.FloorToInt(stepDistanceDegrees / 360f);

                // 
                float radius = Mathf.Lerp(radius0, radius1, time);
                float cos = Mathf.Cos(stepDistanceRadians);
                float sin = Mathf.Sin(stepDistanceRadians);

                // Compute position
                float componentA0 = cos * radius - radius0;
                float componentB0 = sin * radius;
                float offset = Mathf.Lerp(0, offset1, time);
                Vector3 spiralPosition0 = componentToSpiralFunction(componentA0, componentB0, offset);
                Vector3 position = startPosition + spiralPosition0;
                positionXYZ.AddKeys(time, position);
            }
            positionXYZ.y.SmoothTangents();

            // "Lazy" key time adjustment
            animationCurveTRS = new AnimationCurveTRS(positionXYZ, rotationXYZ, new());
            animationCurveTRS.CleanDuplicateKeys();

            float curveLength = length;
            //float curveLength = (float)CurveUtility.GetDistanceBetweenRepeated(this, 0, 1);
            var keysPX = positionXYZ.x.GetRenormalizedKeyRangeAndTangents(0, curveLength);
            var keysPY = positionXYZ.y.GetRenormalizedKeyRangeAndTangents(0, curveLength);
            var keysPZ = positionXYZ.z.GetRenormalizedKeyRangeAndTangents(0, curveLength);

            var eRotation = GetRotation().eulerAngles;
            var keysRX = new Keyframe[]
            {
                new(0, eRotation.x + slope),
                new(curveLength, eRotation.x + slope),
            };
            var keysRY = new Keyframe[]
            {
                new(0, eRotation.y),
                new(curveLength, eRotation.y + degrees),
            };
            //var keysRX = rotationXYZ.x.GetRenormalizedKeyRangeAndTangents(0, curveLength);
            //var keysRY = rotationXYZ.y.GetRenormalizedKeyRangeAndTangents(0, curveLength);
            var keysRZ = rollsCurve.GetRenormalizedKeyRangeAndTangents(0, curveLength);

            var trs = new AnimationCurveTRS(
                    new AnimationCurve3(keysPX, keysPY, keysPZ),
                    new AnimationCurve3(keysRX, keysRY, keysRZ),
                    CreateScaleCurvesXYZ(curveLength));

            // make linear
            trs.Rotation.y.SmoothTangents();

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
