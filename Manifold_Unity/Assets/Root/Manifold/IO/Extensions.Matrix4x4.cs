using UnityEngine;

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
    }
}
