using UnityEngine;

namespace Manifold.EditorTools
{
    public static partial class Vector3Extensions
    {
        public static Vector3 Multiply(this Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }
    }
}
