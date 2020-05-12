using System;
using System.CodeDom;
using System.IO;
using System.Text;

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

        if (type == typeof(uint) || type == typeof(int))
            WriteFlags(writer, (TEnum)(object)uint.MaxValue);
        else if (type == typeof(ushort) || type == typeof(short))
            WriteFlags(writer, (TEnum)(object)ushort.MaxValue);
        else if (type == typeof(byte) || type == typeof(sbyte))
            WriteFlags(writer, (TEnum)(object)byte.MaxValue);
    }

    public static void WriteFlags<TEnum>(this StreamWriter writer, TEnum values)
        where TEnum : struct, Enum
    {
        foreach (var flag in Manifold.IO.AnalyzerUtility.GetFlags(values))
        {
            if (flag != null)
                writer.PushCol(flag);
            else
                writer.PushCol(string.Empty);
        }
    }
}