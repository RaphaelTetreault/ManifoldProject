using System;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [Serializable]
    public struct FixedBezierPoint
    {
        public const float Weight = 1f / 3f;

        public Vector3 position;
        [SerializeField] private Vector3 eulerOrientation;
        [SerializeField] private Quaternion orientation;
        public Vector2 scale;
        public float linearDistanceIn;
        public float linearDistanceOut;
        public bool2 keyScale;

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

        public float LengthIn => linearDistanceIn * Weight;
        public float LengthOut => linearDistanceOut * Weight;
        public Vector3 InTangent => Orientation * Vector3.back * LengthIn;
        public Vector3 OutTangent => Orientation * Vector3.forward * LengthOut;
        public Vector3 InTangentPosition => InTangent + position;
        public Vector3 OutTangentPosition => OutTangent + position;
    }
}
