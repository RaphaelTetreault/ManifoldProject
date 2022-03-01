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

        private const int kAlignment = 16;
        private const char kWhiteSpace = ' ';
        private const char kPadding = '-';



        // Added the below extensions to help debug file outputs
        public static void Comment(this BinaryWriter writer, string message, bool doWrite, char padding = ' ', int alignment = 16)
        {
            // Allow option to write or not. Prevents a lot of if statements.
            if (!doWrite)
                return;

            if (string.IsNullOrEmpty(message))
                return;

            var bytes = Encoding.ASCII.GetBytes(message);
            byte padByte = (byte)padding;

            writer.AlignTo(alignment, padByte);
            writer.WriteX(bytes);
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
        public static void CommentLineWide(this BinaryWriter writer, string lMsg, object rMsg, bool doWrite, char padding = ' ', int alignment = 16)
            => CommentLineWide(writer, lMsg, rMsg.ToString(), doWrite, padding, alignment);

        public static void CommentIdx(this BinaryWriter writer, int index, bool doWrite, char padding = ' ', int alignment = 16, string format = "d")
            => CommentLineWide(writer, "Index:", index.ToString(format), doWrite, padding, alignment);

        public static void CommentCnt(this BinaryWriter writer, int count, bool doWrite, char padding = ' ', int alignment = 16, string format = "d")
            => CommentLineWide(writer, "Count:", count.ToString(format), doWrite, padding, alignment);

        public static void CommentPtr(this BinaryWriter writer, int address, bool doWrite, char padding = ' ', int alignment = 16, string format = "x8")
            => CommentLineWide(writer, "PtrAdr:", address.ToString(format), doWrite, padding, alignment);

        //public static void CommentPtr(this BinaryWriter writer, IPointer pointer, bool doWrite, char padding = ' ', int alignment = 16)
        //    => CommentPtr(writer, pointer.Address, doWrite, padding, alignment);

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


        public static void InlineDesc<T>(this BinaryWriter writer, bool doWrite, Pointer pointer, T type, char padding = ' ', int alignment = 16)
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

        public static void InlineDesc<T>(this BinaryWriter writer, bool doWrite, T type, char padding = ' ', int alignment = 16)
        {
            if (!doWrite)
                return;

            // Align with desired padding, not '-' from next call
            writer.AlignTo(alignment, (byte)padding);

            CommentNewLine(writer, true, '-', alignment);
            writer.CommentType(type, true, ' ', alignment);
            CommentNewLine(writer, true, '-', alignment);
        }

        public static void InlineComment(this BinaryWriter writer, bool doWrite, params string[] comments)
        {
            if (!doWrite)
                return;

            // Align with desired padding, not '-' from next call
            writer.AlignTo(kAlignment, (byte)kWhiteSpace);

            CommentNewLine(writer, true, '-', kAlignment);
            foreach (var comment in comments)
            {
                writer.Comment(comment, true, ' ', kAlignment);
            }
            CommentNewLine(writer, true, '-', kAlignment);
        }

        public static void CommentDateAndCredits(this BinaryWriter writer, bool doWrite)
        {
            if (!doWrite)
                return;

            //writer.CommentNewLine(true);
            writer.CommentNewLine(true, '-');
            writer.Comment("Auto Generated", true);
            writer.Comment("by Manifold", true);
            writer.Comment($"Date: {DateTime.Now:yyyy-MM-dd}", true);
            writer.Comment($"Time: {DateTime.Now:HH:mm:ss}", true);
            writer.CommentNewLine(true);
            writer.Comment("Manifold", true);
            writer.Comment("created by:", true);
            writer.WriteX(new byte[] { 0x52, 0x61, 0x70, 0x68, 0x61, 0xeb, 0x6c, 0x54, 0xe9, 0x74, 0x72, 0x65, 0x61, 0x75, 0x6c, 0x74 } ); // RaphaëlTétreault
            writer.Comment("aka StarkNebula", true);
            writer.CommentNewLine(true, '-');
            //writer.CommentNewLine(true);
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
            return new Pointer(writer.BaseStream.Position);
        }
    }
}
