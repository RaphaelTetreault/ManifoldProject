using System.IO;
using Unity.Mathematics;

namespace Manifold.IO
{
    public static partial class BinaryReaderExtensions
    {
        public static float2 ReadFloat2(this BinaryReader reader)
        {
            return new float2(
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader));
        }

        public static float3 ReadFloat3(this BinaryReader reader)
        {
            return new float3(
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader));
        }

        public static float4 ReadFloat4(this BinaryReader reader)
        {
            return new float4(
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader));
        }

        public static quaternion ReadMathQuaternion(this BinaryReader reader)
        {
            return new quaternion(
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader));
        }


        // Function forwarding
        public static float2 ReadX(this BinaryReader reader, ref float2 value)
            => value = reader.ReadFloat2();

        public static float3 ReadX(this BinaryReader reader, ref float3 value)
            => value = reader.ReadFloat3();

        public static float4 ReadX(this BinaryReader reader, ref float4 value)
            => value = reader.ReadFloat4();

        public static quaternion ReadX(this BinaryReader reader, ref quaternion value)
            => value = reader.ReadMathQuaternion();

        public static float2[] ReadX(this BinaryReader reader, ref float2[] value, int length)
            => value = BinaryIoUtility.ReadArray(reader, length, ReadFloat2);

        public static float3[] ReadX(this BinaryReader reader, ref float3[] value, int length)
            => value = BinaryIoUtility.ReadArray(reader, length, ReadFloat3);

        public static float4[] ReadX(this BinaryReader reader, ref float4[] value, int length)
            => value = BinaryIoUtility.ReadArray(reader, length, ReadFloat4);

        public static quaternion[] ReadX(this BinaryReader reader, ref quaternion[] value, int length)
            => value = BinaryIoUtility.ReadArray(reader, length, ReadMathQuaternion);
    }
}
