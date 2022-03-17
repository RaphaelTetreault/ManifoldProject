using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace Manifold.EditorTools
{
    public static partial class BinaryReaderExtensions
    {
        public static Vector2 ReadVector2(this EndianBinaryReader reader)
        {
            return new Vector2(
                reader.ReadFloat(),
                reader.ReadFloat());
        }

        public static Vector3 ReadVector3(this EndianBinaryReader reader)
        {
            return new Vector3(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat());
        }

        public static Vector4 ReadVector4(this EndianBinaryReader reader)
        {
            return new Vector4(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat());
        }

        public static Quaternion ReadQuaternion(this EndianBinaryReader reader)
        {
            return new Quaternion(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat());
        }

        public static Color32 ReadColor32(this EndianBinaryReader reader)
        {
            return new Color32(
                reader.ReadUInt8(),
                reader.ReadUInt8(),
                reader.ReadUInt8(),
                reader.ReadUInt8());
        }

        public static Color ReadColor(this EndianBinaryReader reader)
        {
            return new Color(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat());
        }

        public static Transform ReadTransform(this EndianBinaryReader reader, ref Transform value, Space space = Space.Self)
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
        public static Vector2 Read(this EndianBinaryReader reader, ref Vector2 value)
            => value = reader.ReadVector2();

        public static Vector3 Read(this EndianBinaryReader reader, ref Vector3 value)
            => value = reader.ReadVector3();

        public static Vector4 Read(this EndianBinaryReader reader, ref Vector4 value)
            => value = reader.ReadVector4();

        public static Quaternion Read(this EndianBinaryReader reader, ref Quaternion value)
            => value = reader.ReadQuaternion();

        public static Color32 Read(this EndianBinaryReader reader, ref Color32 value)
            => value = reader.ReadColor32();

        public static Color Read(this EndianBinaryReader reader, ref Color value)
            => value = reader.ReadColor();

        public static Transform Read(this EndianBinaryReader reader, ref Transform value, Space space = Space.Self)
            => value = reader.ReadTransform(ref value, space);

        public static Vector2[] Read(this EndianBinaryReader reader, ref Vector2[] value, int length)
            => value = reader.ReadArray(length, ReadVector2);
        
        public static Vector3[] Read(this EndianBinaryReader reader, ref Vector3[] value, int length)
            => value = reader.ReadArray(length, ReadVector3);
        
        public static Vector4[] Read(this EndianBinaryReader reader, ref Vector4[] value, int length)
            => value = reader.ReadArray(length, ReadVector4);
        
        public static Quaternion[] Read(this EndianBinaryReader reader, ref Quaternion[] value, int length)
            => value = reader.ReadArray(length, ReadQuaternion);
        
        public static Color32[] Read(this EndianBinaryReader reader, ref Color32[] value, int length)
            => value = reader.ReadArray(length, ReadColor32);
        
        public static Color[] Read(this EndianBinaryReader reader, ref Color[] value, int length)
            => value = reader.ReadArray(length, ReadColor);

    }
}
