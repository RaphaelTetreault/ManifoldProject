using System;
using System.IO;
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
        public static void Comment(this BinaryWriter writer, string message, bool doWrite, char padding = ' ', int alignment = 16)
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
        public static void CommentType<T>(this BinaryWriter writer, bool doWrite, char padding = ' ', int alignment = 16)
            => Comment(writer, typeof(T).Name, doWrite, padding, alignment);
        public static void CommentType<T>(this BinaryWriter writer, T _, bool doWrite, char padding = ' ', int alignment = 16)
            => CommentType<T>(writer, doWrite, padding, alignment);

        public static void CommentNewLine(this BinaryWriter writer, bool doWrite, char padding = ' ', int alignment = 16)
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            byte padByte = (byte)padding;

            writer.AlignTo(alignment, padByte);
            for (int i = 0; i < alignment; i++)
                writer.WriteX(padByte);
        }
        public static void CommentAlign(this BinaryWriter writer, bool doWrite, char padding = ' ', int alignment = 16)
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            writer.AlignTo(alignment, (byte)padding);
        }

        public static void CommentLineWide(this BinaryWriter writer, string lMsg, string rMsg, bool doWrite, char padding = ' ', int alignment = 16)
        {
            int lengthLeft = lMsg.Length;
            int lenghtRight = alignment - lengthLeft;
            var message = $"{lMsg}{rMsg.PadLeft(lenghtRight)}";
            Comment(writer, message, doWrite, padding, alignment);
        }

        public static void CommentIdx(this BinaryWriter writer, int count, bool doWrite, char padding = ' ', int alignment = 16, string format = "d")
            => CommentLineWide(writer, "Index:", count.ToString(format), doWrite, padding, alignment);

        public static void CommentCnt(this BinaryWriter writer, int count, bool doWrite, char padding = ' ', int alignment = 16, string format = "d")
            => CommentLineWide(writer, "Count:", count.ToString(format), doWrite, padding, alignment);

        public static void CommentPtr(this BinaryWriter writer, int address, bool doWrite, char padding = ' ', int alignment = 16, string format = "x8")
            => CommentLineWide(writer, "PtrAdr:", address.ToString(format), doWrite, padding, alignment);

        public static void CommentPtr(this BinaryWriter writer, IPointer pointer, bool doWrite, char padding = ' ', int alignment = 16)
            => CommentPtr(writer, pointer.Address, doWrite, padding, alignment);

        public static void CommentPtr(this BinaryWriter writer, AddressRange addresRange, bool doWrite, char padding = ' ', int alignment = 16)
            => CommentPtr(writer, (int)addresRange.startAddress, doWrite, padding, alignment);

        public static void CommentPtr(this BinaryWriter writer, ArrayPointer pointer, bool doWrite, char padding = ' ', int alignment = 16)
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            CommentPtr(writer, pointer.Pointer, doWrite, padding, alignment);
            CommentCnt(writer, pointer.Length, doWrite, padding, alignment, "x8");
            CommentCnt(writer, pointer.Length, doWrite, padding, alignment);
        }


        public static void CommentTypeDesc<T>(this BinaryWriter writer, T type, Pointer pointer, bool doWrite, char padding = ' ', int alignment = 16)
        {
            if (!doWrite)
                return;

            writer.AlignTo(alignment, (byte)padding);
            CommentNewLine(writer, true, '-', alignment);
            writer.CommentType(type, true, ' ', alignment);
            if (typeof(T).IsArray)
            {
                var length = (type as Array).Length;
                writer.CommentPtr(new ArrayPointer(length, pointer.address), true, padding, alignment);
            }
            else
            {
                writer.CommentPtr(pointer, true, padding, alignment);
            }
            CommentNewLine(writer, true, '-', alignment);
        }


        public static void CommentDateAndCredits(this BinaryWriter writer, bool doWrite)
        {
            if (!doWrite)
                return;

            writer.CommentNewLine(true);
            writer.CommentNewLine(true, '-');
            writer.Comment("Auto Generated", true);
            writer.Comment("by Manifold", true);
            writer.Comment($"Date: {DateTime.Now:yyyy-MM-dd}", true);
            writer.Comment($"Time: {DateTime.Now:HH:mm:ss}", true);
            writer.CommentNewLine(true);
            writer.Comment("Manifold", true);
            writer.Comment("created by", true);
            writer.WriteX(new byte[] { 0x52, 0x61, 0x70, 0x68, 0x61, 0xeb, 0x6c, 0x54, 0xe9, 0x74, 0x72, 0x65, 0x61, 0x75, 0x6c, 0x74 }, false); // RaphaëlTétreault
            writer.Comment("aka StarkNebula", true);
            writer.CommentNewLine(true, '-');
            writer.CommentNewLine(true);
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
