using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;

namespace Manifold.IO
{
    /// <summary>
    /// Extensions to make matrix4x4 easier to handle.
    /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Extension is for 'float4x4'")]
    public static partial class float4x4Extensions
    {
        public static float3 Position(this float4x4 matrix)
        {
            return matrix.c3.xyz;
        }

        // Currently very lacking. Rotations do not come out right.
        public static quaternion Rotation(this float4x4 matrix)
        {
            return new quaternion(matrix);
        }

        public static float3 RotationEuler(this float4x4 matrix)
        {
            var mtx3x3 = matrix.AsFloat3x3();
            var eulers = GameCube.GFZ.CompressedRotation.DecomposeRotationDegrees(mtx3x3);
            return eulers;
        }

        public static float3 Scale(this float4x4 matrix)
        {
            return new float3(
                math.length(matrix.c0),
                math.length(matrix.c1),
                math.length(matrix.c2)
                );
        }

        public static float3x3 AsFloat3x3(this float4x4 m4)
        {
            return new float3x3(m4.c0.xyz, m4.c1.xyz, m4.c2.xyz);
        }
    }
}
