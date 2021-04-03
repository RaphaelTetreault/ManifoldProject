using System;
using System.IO;
using UnityEngine;

namespace Manifold.IO
{
    public static partial class BinaryWriterExtensions
    {
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
                    Debug.Log("Cannot restore world space scale, only local space. Be aware that only root " +
                        "objects should be saved with <i>Space.World</i>.");
                    writer.WriteX(value.localScale);
                    break;
            }
        }

    }
}
