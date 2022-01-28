using Unity.Mathematics;

namespace Manifold.IO
{
    /// <summary>
    /// Extensions to make matrix4x4 easier to handle.
    /// </summary>
    public static partial class float4x4Extensions
    {
        public static float3 Position(this float4x4 matrix)
        {
            return matrix.c3.xyz;
        }

        // Currently very lacking. Rotations do not come out right.
        public static quaternion RotationBad(this float4x4 matrix)
        {
            return new quaternion(matrix);
        }

        public static float3 RotationEuler(this float4x4 matrix)
        {
            return matrix.RotationBad().ComputeAngles();
        }

        public static float3 Scale(this float4x4 matrix)
        {
            return new float3(
                math.length(matrix.c0),
                math.length(matrix.c1),
                math.length(matrix.c2)
                );
        }
    }
}
