using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [System.Serializable]
    public struct FixedBezierPoint
    {
        public Vector3 position;
        [SerializeField] private Vector3 eulerOrientation;
        [SerializeField] private Quaternion orientation;
        public float inLength;
        public float outLength;
        public bool3 keyPosition;
        public bool3 keyOrientation;

        public Quaternion Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
                eulerOrientation = orientation.eulerAngles;
            }
        }
        public Vector3 EulerOrientation
        {
            get
            {
                return eulerOrientation;
            }
            set
            {
                eulerOrientation = value;
                orientation = Quaternion.Euler(eulerOrientation);
            }
        }

        public Vector3 InTangent => Orientation * Vector3.back * inLength;
        public Vector3 OutTangent => Orientation * Vector3.forward * outLength;
        public Vector3 InTangentPosition => InTangent + position;
        public Vector3 OutTangentPosition => OutTangent + position;
    }

    public class GfzFixedBezierPath : GfzPathSegment
    {
        [SerializeField] private List<FixedBezierPoint> controlPoints = DefaultControlPoints();
        [SerializeField] private int selectedIndex = 1;
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();


        protected override AnimationCurveTRS TrackSegmentAnimationCurveTRS => animationCurveTRS;
        //internal FixedBezierPoint[] ControlPoints => controlPoints.ToArray();
        public int ControlPointsLength => controlPoints.Count;

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            // TODO: make the TRS here
            return animationCurveTRS;
        }

        public override float GetMaxTime()
        {
            return animationCurveTRS.GetMaxTime();
        }

        public override float GetSegmentLength()
        {
            return animationCurveTRS.GetMaxTime();
        }

        public override void UpdateTRS()
        {
            animationCurveTRS = CreateAnimationCurveTRS(false);
        }



        // THE CORE
        public void InsertControlPoint(int index, FixedBezierPoint controlPoint)
        {
            controlPoints.Insert(index, controlPoint);
        }
        public void InsertBefore(int index, FixedBezierPoint controlPoint)
            => InsertControlPoint(index, controlPoint);
        public void InsertAfter(int index, FixedBezierPoint controlPoint)
            => InsertControlPoint(index+1, controlPoint);

        public static List<FixedBezierPoint> DefaultControlPoints()
        {
            var controlPoint0 = new FixedBezierPoint()
            {
                position = new Vector3(0, 0, 0),
                Orientation = Quaternion.identity,
                keyPosition = new bool3(true),
                keyOrientation = new bool3(true),
            };
            var controlPoint1 = new FixedBezierPoint()
            {
                position = new Vector3(0, 0, 500),
                Orientation = Quaternion.identity,
                keyPosition = new bool3(true),
                keyOrientation = new bool3(true),
            };
            var list = new List<FixedBezierPoint>();
            list.Add(controlPoint0);
            list.Add(controlPoint1);

            return list;
        }


        protected override void Reset()
        {
            base.Reset();
        }


        public void SetControlPoint(int index, FixedBezierPoint controlPoint)
        {
            controlPoints[index] = controlPoint;
        }
        public FixedBezierPoint GetControlPoint(int index)
        {
            return controlPoints[index];
        }


    }
}
