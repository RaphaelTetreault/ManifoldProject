using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumpHex
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
            LOOP_BEGIN:
                Console.WriteLine("Enter file name:");
                string input = Console.ReadLine();
                Console.WriteLine();

                bool fileExists = File.Exists(input);
                if (!fileExists)
                {
                    Console.WriteLine($"File {input} does not exists!\n");
                    goto LOOP_BEGIN;
                }


                using (var file = File.OpenRead(input))
                {
                    using (BinaryReader reader = new BinaryReader(file))
                    {
                        var directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        var filename = Path.GetFileName(input);
                        var output = $"{Path.Combine(directory, filename)}.tsv";

                        using (var outputFile = File.OpenWrite(output))
                        {
                            using (var writer = new StreamWriter(outputFile))
                            {
                                // HEADER
                                var endianness = BitConverter.IsLittleEndian ? "LE" : "BE";
                                writer.WriteNextCol($"Hex 32 ({endianness})");
                                writer.WriteNextCol("Integer 32");
                                writer.WriteNextCol("Integer 16 (R)");
                                writer.WriteNextCol("Integer 16 (L)");
                                writer.WriteNextCol("Float");
                                writer.NextColumn();
                                //
                                endianness = !BitConverter.IsLittleEndian ? "LE" : "BE";
                                writer.WriteNextCol($"Hex 32 ({endianness})");
                                writer.WriteNextCol("Integer 32");
                                writer.WriteNextCol("Integer 16 (R)");
                                writer.WriteNextCol("Integer 16 (L)");
                                writer.WriteNextRow("Float");

                                int stride = 4;
                                while (reader.BaseStream.Position < reader.BaseStream.Length - stride)
                                {
                                    var bytes32 = reader.ReadBytes(stride);
                                    var bytes16_1 = new byte[] { bytes32[0], bytes32[1] };
                                    var bytes16_2 = new byte[] { bytes32[2], bytes32[3] };

                                    // take into consideration:
                                    // if we reverse bits for 4byte int, we lose proper
                                    // order for 2 byte ints.

                                    var uint32 = BitConverter.ToUInt32(bytes32, 0);
                                    var uint16_1 = BitConverter.ToUInt16(bytes16_1, 0);
                                    var uint16_2 = BitConverter.ToUInt16(bytes16_2, 0);
                                    var @float = BitConverter.ToSingle(bytes32, 0);

                                    writer.WriteNextCol(uint32.ToString("X8"));
                                    writer.WriteNextCol(uint32.ToString());
                                    writer.WriteNextCol(uint16_1.ToString());
                                    writer.WriteNextCol(uint16_2.ToString());
                                    writer.WriteNextCol(@float.ToString());
                                    writer.NextColumn();

                                    Array.Reverse(bytes32);
                                    Array.Reverse(bytes16_1);
                                    Array.Reverse(bytes16_2);

                                    uint32 = BitConverter.ToUInt32(bytes32, 0);
                                    uint16_1 = BitConverter.ToUInt16(bytes16_1, 0);
                                    uint16_2 = BitConverter.ToUInt16(bytes16_2, 0);
                                    @float = BitConverter.ToSingle(bytes32, 0);
                                    writer.WriteNextCol(uint32.ToString("X8"));
                                    writer.WriteNextCol(uint32.ToString());
                                    writer.WriteNextCol(uint16_1.ToString());
                                    writer.WriteNextCol(uint16_2.ToString());
                                    writer.WriteNextRow(@float.ToString());
                                }
                            }
                        }
                        Console.WriteLine($"Created '{output}'\n");
                    }
                }
            }

        }
    }

    public static class Exts
    {
        const char CharTabulator = '\t';
        const char CharLineFeed = '\n';
        const char CharCarriageReturn = '\r';

        public static void NextColumn(this StreamWriter writer)
        {
            writer.Write(CharTabulator);
        }

        public static void NextRow(this StreamWriter writer)
        {
            writer.Write(CharCarriageReturn);
            writer.Write(CharLineFeed);
        }

        public static void WriteNextCol(this StreamWriter writer, string value)
        {
            writer.Write(value);
            NextColumn(writer);
        }
        public static void WriteNextRow(this StreamWriter writer, string value)
        {
            writer.Write(value);
            NextRow(writer);
        }
    }
}