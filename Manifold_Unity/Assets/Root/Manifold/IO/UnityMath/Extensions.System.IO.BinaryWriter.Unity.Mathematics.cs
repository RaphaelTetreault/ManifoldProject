using System.IO;
using Unity.Mathematics;

namespace Manifold.IO
{
    public static partial class BinaryWriterExtensions
    {
        public static void WriteX(this BinaryWriter writer, float2 value)
        {
            writer.WriteX(value.x);
            writer.WriteX(value.y);
        }

        public static void WriteX(this BinaryWriter writer, float3 value)
        {
            writer.WriteX(value.x);
            writer.WriteX(value.y);
            writer.WriteX(value.z);
        }

        public static void WriteX(this BinaryWriter writer, float4 value)
        {
            writer.WriteX(value.x);
            writer.WriteX(value.y);
            writer.WriteX(value.z);
            writer.WriteX(value.w);
        }

        public static void WriteX(this BinaryWriter writer, quaternion value)
        {
            // value.value pulls the float4 from within quaternion
            writer.WriteX(value.value.x);
            writer.WriteX(value.value.y);
            writer.WriteX(value.value.z);
            writer.WriteX(value.value.w);
        }

    }
}
