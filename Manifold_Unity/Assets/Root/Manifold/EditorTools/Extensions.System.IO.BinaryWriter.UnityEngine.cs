using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace Manifold.EditorTools
{
    public static partial class BinaryWriterExtensions
    {
        public static void Write(this EndianBinaryWriter writer, Vector2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static void Write(this EndianBinaryWriter writer, Vector3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static void Write(this EndianBinaryWriter writer, Vector4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static void Write(this EndianBinaryWriter writer, Quaternion value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static void Write(this EndianBinaryWriter writer, Color value)
        {
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }

        public static void Write(this EndianBinaryWriter writer, Color32 value)
        {
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }

        public static void Write(this EndianBinaryWriter writer, Transform value, Space space = Space.Self)
        {
            switch (space)
            {
                default:
                    throw new NotImplementedException();

                case Space.Self:
                    writer.Write(value.localPosition);
                    writer.Write(value.localRotation);
                    writer.Write(value.localScale);
                    break;

                case Space.World:
                    writer.Write(value.position);
                    writer.Write(value.rotation);
                    Debug.Log("Cannot restore world space scale, only local space. Be aware that only root " +
                        "objects should be saved with <i>Space.World</i>.");
                    writer.Write(value.localScale);
                    break;
            }
        }

    }
}
