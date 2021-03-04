using Manifold.IO;
using System.IO;
using System.Collections.Generic;

namespace GameCube.GFZ.CourseCollision
{
    public static class ColiCourseUtil
    { 
        /// <summary>
        /// Terminated by 0xFFFF
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ushort[] ReadShortArray(BinaryReader reader)
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
