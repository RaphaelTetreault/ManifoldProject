using Manifold.IO;
using System.Collections.Generic;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{

    

    /// <summary>
    /// 
    /// </summary>
    public static class ColiCourseUtility
    {
        private const int kAxConst0x20 = 0xE4;
        private const int kAxConst0x24 = 0xF8;
        private const int kGxConst0x20 = 0xE8;
        private const int kGxConst0x24 = 0xFC;

        //public struct SerializePatchable<T>
        //    where T : IBinarySerializable
        //{
        //    public string hash;
        //    public T value;
        //    public List<Pointer> references;
        //}


        //public static List<SerializePatchable<CString>> objectNames = new List<SerializePatchable<CString>>();
        //public static List<SerializePatchable<SceneObjectReference>> objectReferences = new List<SerializePatchable<SceneObjectReference>>();
        //public static List<SerializePatchable<SceneInstanceReference>> instanceReferences = new List<SerializePatchable<SceneInstanceReference>>();
        //public static List<SerializePatchable<SceneObject>> sceneObjects = new List<SerializePatchable<SceneObject>>();

        public static int Index { get; set; } = 0;
        public static Pointer Pointer { get; set; } = -1;

        

        public static void ResetDebugIndex()
        {
            Index = 0;
        }

        // STATIC FIELDS
        /// <summary>
        /// When true, serializes text inside COLI_COURSE structures
        /// </summary>
        public static bool SerializeVerbose { get; set; } = true;

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

    }
}
