using System.IO;
using System.Linq;
using System.Text;

namespace Manifold.IO
{
    public static partial class BinaryWriterExtensions
    {
        /// <summary>
        /// Positions underlying stream at end of stream.
        /// </summary>
        /// <param name="writer"></param>
        public static void SeekEnd(this BinaryWriter writer)
        {
            long endOfStream = writer.BaseStream.Length;
            writer.BaseStream.Seek(endOfStream, SeekOrigin.Begin);
        }

        /// <summary>
        /// Positions underlying stream at beginning of stream.
        /// </summary>
        /// <param name="writer"></param>
        public static void SeekStart(this BinaryWriter writer)
        {
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
        }


        // Added the below extensions to help debug file outputs
        public static void Comment(this BinaryWriter writer, string message, bool doWrite = true, int alignment = 16, char padding = ' ')
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            var bytes = Encoding.ASCII.GetBytes(message);
            byte padByte = (byte)padding;

            writer.AlignTo(alignment, padByte);
            writer.WriteX(bytes, false);
            writer.AlignTo(alignment, padByte);
        }
        public static void CommentType<T>(this BinaryWriter writer, bool doWrite = true, int alignment = 16, char padding = ' ')
            => Comment(writer, typeof(T).Name, doWrite, alignment, padding);

        public static void CommentType<T>(this BinaryWriter writer, T _, bool doWrite = true, int alignment = 16, char padding = ' ')
            => CommentType<T>(writer, doWrite, alignment, padding);

        public static void CommentNewLine(this BinaryWriter writer, bool doWrite = true, int alignment = 16, char padding = ' ')
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            byte padByte = (byte)padding;

            writer.AlignTo(alignment, padByte);
            for (int i = 0; i < alignment; i++)
                writer.WriteX(padByte);
        }

        public static void PointerComment(this BinaryWriter writer, int address, bool doWrite, int alignment = 16, char padding = ' ')
            => Comment(writer, $"Ptr:    {address:x8}", doWrite, alignment, padding);
        //{
        //    // Allow option to write or not. Prevents a lot of if statements.
        //    if (!doWrite)
        //        return;

        //    var message = $"Ptr:    {address:x8}";
        //    Comment(writer, message, true, alignment, padding);
        //}

        public static void CommentPtr(this BinaryWriter writer, IPointer pointer, bool doWrite, int alignment = 16, char padding = ' ')
            => PointerComment(writer, pointer.Address, doWrite, alignment, padding);

        public static void PointerComment(this BinaryWriter writer, AddressRange addresRange, bool doWrite, int alignment = 16, char padding = ' ')
            => PointerComment(writer, (int)addresRange.startAddress, doWrite, alignment, padding);

        public static void CommentCnt(this BinaryWriter writer, int count, bool doWrite, int alignment = 16, char padding = ' ', string format = "d")
            => Comment(writer, $"Count:  {count.ToString(format),8}", doWrite, alignment, padding);
        //{
        //    // Allow option to write or not. Prevents a lot of if statements.
        //    if (!doWrite)
        //        return;

        //    var message = $"Count:  {count.ToString(format),8}";
        //    Comment(writer, message, true, alignment, padding);

        //    //var bytes = Encoding.ASCII.GetBytes(message);

        //    //writer.WriteX(bytes, false);
        //    //writer.AlignTo(alignment, padding);
        //}

        public static void CommentPtr(this BinaryWriter writer, ArrayPointer pointer, bool doWrite, int alignment = 16, char padding = ' ')
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            CommentPtr(writer, pointer.Pointer, doWrite, alignment, padding);
            CommentCnt(writer, pointer.Address, doWrite, alignment, padding, "x8");
            CommentCnt(writer, pointer.Address, doWrite, alignment, padding);
        }


        public static void ThisIsGettingOutOfHand<T>(this BinaryWriter writer, T _, ArrayPointer arrayPointer, char padding, bool doWrite, int alignment = 16)
        {
            if (!doWrite)
                return;

            CommentNewLine(writer, true, alignment, '-');
            CommentType(writer, _, true, alignment, padding);
            CommentPtr(writer, arrayPointer, true, alignment, default);
            CommentNewLine(writer, doWrite, alignment, '-');
        }

        public static void CommentTypeDesc<T>(this BinaryWriter writer, T type, Pointer pointer, bool doWrite, int alignment = 16, char padding = ' ')
        {
            if (!doWrite)
                return;

            writer.AlignTo(alignment, (byte)padding);
            CommentNewLine(writer, true, alignment, '-');
            writer.CommentType(type, true, alignment, ' ');
            if (typeof(T).IsArray)
                writer.CommentPtr(new ArrayPointer((type as System.Array).Length, pointer.address), true, alignment, default);
            else
                writer.CommentPtr(pointer, true, alignment, default);
            CommentNewLine(writer, true, alignment, '-');
        }

        public static void JumpToAddress(this BinaryWriter writer, Pointer pointer, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            writer.BaseStream.Seek(pointer.address, seekOrigin);
        }
        public static void JumpToAddress(this BinaryWriter writer, ArrayPointer arrayPointer, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            writer.BaseStream.Seek(arrayPointer.Address, seekOrigin);
        }

        public static Pointer GetPositionAsPointer(this BinaryWriter writer)
        {
            return new Pointer((int)writer.BaseStream.Position);
        }
    }
}
