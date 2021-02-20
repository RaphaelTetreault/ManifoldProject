using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Manifold.IO
{
    public static partial class BinaryIoExtensions
    {
        #region READ

        public static void ReadX(this BinaryReader reader, ref DateTime value)
        {
            long dateTimeBinary = 0;
            reader.ReadX(ref dateTimeBinary);
            value = DateTime.FromBinary(dateTimeBinary);
        }

        #endregion

        #region WRITE

        public static void WriteX(this BinaryWriter writer, DateTime value)
        {
            long dateTimeBinary = value.ToBinary();
            writer.WriteX(dateTimeBinary);
        }

        #endregion
    }
}