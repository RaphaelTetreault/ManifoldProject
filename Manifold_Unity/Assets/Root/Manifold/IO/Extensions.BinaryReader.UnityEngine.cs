using System;
using System.IO;
using UnityEngine;

namespace Manifold.IO
{
    public static partial class BinaryReaderExtensions
    {
        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            return new Vector2(
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader));
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader));
        }

        public static Vector4 ReadVector4(this BinaryReader reader)
        {
            return new Vector4(
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader));
        }

        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            return new Quaternion(
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader));
        }

        public static Color32 ReadColor32(this BinaryReader reader)
        {
            return new Color32(
                BinaryIoUtility.ReadUInt8(reader),
                BinaryIoUtility.ReadUInt8(reader),
                BinaryIoUtility.ReadUInt8(reader),
                BinaryIoUtility.ReadUInt8(reader));
        }

        public static Color ReadColor(this BinaryReader reader)
        {
            return new Color(
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader),
                BinaryIoUtility.ReadFloat(reader));
        }

        public static Transform ReadTransform(this BinaryReader reader, ref Transform value, Space space = Space.Self)
        {
            switch (space)
            {
                default:
                    throw new NotImplementedException();

                case Space.Self:
                    value.localPosition = reader.ReadVector3();
                    value.localRotation = reader.ReadQuaternion();
                    value.localScale = reader.ReadVector3();
                    break;

                case Space.World:
                    value.position = reader.ReadVector3();
                    value.rotation = reader.ReadQuaternion();
                    value.localScale = reader.ReadVector3();
                    break;
            }

            return value;
        }


        // Function forwarding
        public static Vector2 ReadX(this BinaryReader reader, ref Vector2 value)
            => value = reader.ReadVector2();

        public static Vector3 ReadX(this BinaryReader reader, ref Vector3 value)
            => value = reader.ReadVector3();

        public static Vector4 ReadX(this BinaryReader reader, ref Vector4 value)
            => value = reader.ReadVector4();

        public static Quaternion ReadX(this BinaryReader reader, ref Quaternion value)
            => value = reader.ReadQuaternion();

        public static Color32 ReadX(this BinaryReader reader, ref Color32 value)
            => value = reader.ReadColor32();

        public static Color ReadX(this BinaryReader reader, ref Color value)
            => value = reader.ReadColor();

        public static Transform ReadX(this BinaryReader reader, ref Transform value, Space space = Space.Self)
            => value = reader.ReadTransform(ref value, space);

        public static Vector2[] ReadX(this BinaryReader reader, ref Vector2[] value, int length)
            => value = BinaryIoUtility.ReadNewArray(reader, length, ReadVector2);
        
        public static Vector3[] ReadX(this BinaryReader reader, ref Vector3[] value, int length)
            => value = BinaryIoUtility.ReadNewArray(reader, length, ReadVector3);
        
        public static Vector4[] ReadX(this BinaryReader reader, ref Vector4[] value, int length)
            => value = BinaryIoUtility.ReadNewArray(reader, length, ReadVector4);
        
        public static Quaternion[] ReadX(this BinaryReader reader, ref Quaternion[] value, int length)
            => value = BinaryIoUtility.ReadNewArray(reader, length, ReadQuaternion);
        
        public static Color32[] ReadX(this BinaryReader reader, ref Color32[] value, int length)
            => value = BinaryIoUtility.ReadNewArray(reader, length, ReadColor32);
        
        public static Color[] ReadX(this BinaryReader reader, ref Color[] value, int length)
            => value = BinaryIoUtility.ReadNewArray(reader, length, ReadColor);

    }
}
