using StarkTools.IO;
using System;
using System.IO;
using System.Linq;

namespace Manifold.IO
{
    public static class StreamWriterExtensions
    {
        const char CharTabulator = '\t';
        const char CharLineFeed = '\n';
        const char CharCarriageReturn = '\r';

        public static void GoToEnd(this StreamWriter writer)
        {
            var offset = writer.BaseStream.Length == 0
                ? 0 : writer.BaseStream.Length - 1;

            writer.BaseStream.Seek(offset, SeekOrigin.Begin);
        }

        public static void PushCol(this StreamWriter writer)
        {
            writer.Write(CharTabulator);
        }

        public static void PushRow(this StreamWriter writer)
        {
            writer.Write(CharCarriageReturn);
            writer.Write(CharLineFeed);
        }

        public static void PushCol(this StreamWriter writer, object value)
        {
            writer.Write(value);
            PushCol(writer);
        }

        public static void PushRow(this StreamWriter writer, object value)
        {
            writer.Write(value);
            PushRow(writer);
        }

        public static void PushCol(this StreamWriter writer, string value)
        {
            writer.Write(value);
            PushCol(writer);
        }

        public static void PushRow(this StreamWriter writer, string value)
        {
            writer.Write(value);
            PushRow(writer);
        }


        public static void WriteFlagNames<TEnum>(this StreamWriter writer)
            where TEnum : struct, Enum
        {
            var type = Enum.GetUnderlyingType(typeof(TEnum));

            if (type == typeof(uint))
                WriteFlags(writer, (TEnum)(object)uint.MaxValue);
            else if (type == typeof(int))
                WriteFlags(writer, (TEnum)(object)int.MaxValue);
            else if (type == typeof(ushort))
                WriteFlags(writer, (TEnum)(object)ushort.MaxValue);
            else if (type == typeof(short))
                WriteFlags(writer, (TEnum)(object)short.MaxValue);
            else if (type == typeof(byte))
                WriteFlags(writer, (TEnum)(object)byte.MaxValue);
            else if (type == typeof(sbyte))
                WriteFlags(writer, (TEnum)(object)sbyte.MaxValue);
        }

        public static void WriteFlags<TEnum>(this StreamWriter writer, TEnum values)
            where TEnum : struct, Enum
        {
            var flags = AnalyzerUtility.GetFlags(values).Reverse();

            foreach (var flag in flags)
            {
                if (flag != null)
                    writer.PushCol(flag);
                else
                    writer.PushCol(string.Empty);
            }
        }

        public static void WriteColNicify(this StreamWriter writer, string value)
        {
            var name = value.Replace("_", " ");
            var prettyName = UnityEditor.ObjectNames.NicifyVariableName(name);
            writer.PushCol(prettyName);
        }

        public static void WriteStartAddress(this StreamWriter writer, IBinaryAddressable binaryAddressable)
        {
            writer.PushCol("0x" + binaryAddressable.StartAddress.ToString("X8"));
        }
    }
}
