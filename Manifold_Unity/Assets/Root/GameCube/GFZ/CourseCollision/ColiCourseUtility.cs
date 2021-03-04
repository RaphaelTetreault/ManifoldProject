using Manifold.IO;
using System.IO;
using System.Collections.Generic;

namespace GameCube.GFZ.CourseCollision
{
    public static class ColiCourseUtility
    {
        private const int kAxConst0x20 = 0xE4;
        private const int kAxConst0x24 = 0xF8;
        private const int kGxConst0x20 = 0xE8;
        private const int kGxConst0x24 = 0xFC;


        public static bool IsFileAX(BinaryReader reader)
        {
            var address = reader.BaseStream.Position;

            reader.BaseStream.Seek(0x20, SeekOrigin.Begin);
            var value0x20 = reader.ReadX_Int32();
            reader.BaseStream.Seek(0x24, SeekOrigin.Begin);
            var value0x24 = reader.ReadX_Int32();

            reader.BaseStream.Position = address;

            var isFileAX =
                value0x20 == kAxConst0x20 &&
                value0x24 == kAxConst0x24;
            return isFileAX;
        }

        public static bool IsFileGX(BinaryReader reader)
        {
            var address = reader.BaseStream.Position;

            reader.BaseStream.Seek(0x20, SeekOrigin.Begin);
            var value0x20 = reader.ReadX_Int32();
            reader.BaseStream.Seek(0x24, SeekOrigin.Begin);
            var value0x24 = reader.ReadX_Int32();

            reader.BaseStream.Position = address;

            var isFileGX =
                value0x20 == kGxConst0x20 &&
                value0x24 == kGxConst0x24;
            return isFileGX;
        }

        /// <summary>
        /// Terminated by 0xFFFF
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ushort[] ReadUShortArray(BinaryReader reader)
        {
            var list = new List<ushort>();
            while (true)
            {
                var value = reader.ReadUInt16();
                list.Add(value);

                if (value == 0xFFFF)
                {
                    break;
                }
            }
            return list.ToArray();
        }
    }
}
