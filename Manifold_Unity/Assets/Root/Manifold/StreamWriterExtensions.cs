using System;
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
}