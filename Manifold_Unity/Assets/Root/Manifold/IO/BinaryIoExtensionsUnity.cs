// Created by Raphael "Stark" Tetreault /2017
// Copyright (c) 2017 Raphael Tetreault
// Last updated 

using System;
using System.IO;
using UnityEngine;

namespace Manifold.IO
{
    public static partial class StreamExtensions
    {
        public const string serializeTransformWarningFormat0 = "Cannot restore " +
            "world space scale, only local space. Be aware that only root objects " +
            "should be saved with <i>Space.World</i>.";

        #region WriteX
        public static void WriteX(this BinaryWriter writer, Vector2 value)
        {
            writer.WriteX(value.x);
            writer.WriteX(value.y);
        }

        public static void WriteX(this BinaryWriter writer, Vector3 value)
        {
            writer.WriteX(value.x);
            writer.WriteX(value.y);
            writer.WriteX(value.z);
        }

        public static void WriteX(this BinaryWriter writer, Vector4 value)
        {
            writer.WriteX(value.x);
            writer.WriteX(value.y);
            writer.WriteX(value.z);
            writer.WriteX(value.w);
        }

        public static void WriteX(this BinaryWriter writer, Quaternion value)
        {
            writer.WriteX(value.x);
            writer.WriteX(value.y);
            writer.WriteX(value.z);
            writer.WriteX(value.w);
        }

        public static void WriteX(this BinaryWriter writer, Color value)
        {
            writer.WriteX(value.r);
            writer.WriteX(value.g);
            writer.WriteX(value.b);
            writer.WriteX(value.a);
        }

        public static void WriteX(this BinaryWriter writer, Color32 value)
        {
            writer.WriteX(value.r);
            writer.WriteX(value.g);
            writer.WriteX(value.b);
            writer.WriteX(value.a);
        }

        public static void WriteX(this BinaryWriter writer, Transform value, Space space = Space.Self)
        {
            switch (space)
            {
                default:
                    throw new NotImplementedException();

                case Space.Self:
                    writer.WriteX(value.localPosition);
                    writer.WriteX(value.localRotation);
                    writer.WriteX(value.localScale);
                    break;

                case Space.World:
                    writer.WriteX(value.position);
                    writer.WriteX(value.rotation);
                    Debug.LogWarningFormat(serializeTransformWarningFormat0);
                    writer.WriteX(value.localScale);
                    break;
            }
        }
        #endregion

        #region Read Value
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
        #endregion

        #region ReadX

        public static Vector2 ReadX(this BinaryReader reader, ref Vector2 value)
        {
            return value = reader.ReadVector2();
        }

        public static Vector3 ReadX(this BinaryReader reader, ref Vector3 value)
        {
            return value = reader.ReadVector3();
        }

        public static Vector4 ReadX(this BinaryReader reader, ref Vector4 value)
        {
            return value = reader.ReadVector4();
        }

        public static Quaternion ReadX(this BinaryReader reader, ref Quaternion value)
        {
            return value = reader.ReadQuaternion();
        }

        public static Color32 ReadX(this BinaryReader reader, ref Color32 value)
        {
            return value = reader.ReadColor32();
        }

        public static Color ReadX(this BinaryReader reader, ref Color value)
        {
            return value = reader.ReadColor();
        }

        public static Transform ReadX(this BinaryReader reader, ref Transform value, Space space = Space.Self)
        {
            return reader.ReadTransform(ref value, space);
        }

        #endregion


    }
}