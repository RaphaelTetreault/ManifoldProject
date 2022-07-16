using UnityEngine;
using Unity.Mathematics;

namespace Manifold.EditorTools
{
    /// <summary>
    /// Extensions to pull meaningful values from Matrix4x4
    /// </summary>
    public static partial class Matrix4x4Extensions
    {
        /// <summary>
        /// Get position from <paramref name="matrix"/>.
        /// </summary>
        /// <param name="matrix">The matrix to sample.</param>
        /// <returns>Matrix's position.</returns>
        public static Vector3 Position(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        /// <summary>
        /// Get rotation from <paramref name="matrix"/>.
        /// </summary>
        /// <param name="matrix">The matrix to sample.</param>
        /// <returns>Matrix's rotation as quaternion.</returns>
        public static Quaternion Rotation(this Matrix4x4 matrix)
        {
            return matrix.rotation;
        }

        /// <summary>
        /// Get position from <paramref name="matrix"/>.
        /// </summary>
        /// <param name="matrix">The matrix to sample.</param>
        /// <returns>Matrix's rotation as Euler angle..</returns>
        public static Vector3 RotationEuler(this Matrix4x4 matrix)
        {
            return matrix.rotation.eulerAngles;
        }

        /// <summary>
        /// Get scale from <paramref name="matrix"/>.
        /// </summary>
        /// <param name="matrix">The matrix to sample.</param>
        /// <returns>Matrix's scale.</returns>
        public static Vector3 Scale(this Matrix4x4 matrix)
        {
            return matrix.lossyScale;
        }

        // TODO: THIS SHOULD NOT BE HERE.
        // Hiding this here for now. Matrix from Unity.Mathematics is wrong, so convert
        // between matrices.
        public static Matrix4x4 ToUnityMatrix4x4(this float4x4 matrix)
        {
            return new Matrix4x4(matrix.c0, matrix.c1, matrix.c2, matrix.c3);
        }
    }
}
