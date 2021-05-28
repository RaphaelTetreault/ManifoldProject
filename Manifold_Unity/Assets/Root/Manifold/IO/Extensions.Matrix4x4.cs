using UnityEngine;
using Unity.Mathematics;

namespace Manifold.IO
{
    public static partial class Matrix4x4Extensions
    {
        public static Vector3 Position(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        public static Quaternion Rotation(this Matrix4x4 matrix)
        {
            return matrix.rotation;
        }

        public static Vector3 RotationEuler(this Matrix4x4 matrix)
        {
            return matrix.rotation.eulerAngles;
        }

        public static Vector3 Scale(this Matrix4x4 matrix)
        {
            return matrix.lossyScale;
        }

        // Hiding this here for now. Matrix from Unity.Mathematics is wrong, so convert
        // between matrices.
        public static Matrix4x4 ToUnityMatrix4x4(this float4x4 matrix)
        {
            return new Matrix4x4(matrix.c0, matrix.c1, matrix.c2, matrix.c3);
        }
    }
}
