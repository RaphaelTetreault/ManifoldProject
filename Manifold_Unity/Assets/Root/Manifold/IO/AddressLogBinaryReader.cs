using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Manifold.IO
{
    public class AddressLogBinaryReader : BinaryReader
    {
        private bool[] addrLog = new bool[0];

        public bool ThrowErrorOnDoubleRead = false;

        public AddressLogBinaryReader(Stream input) : base(input)
        {
            InitAddressLog();
        }

        public AddressLogBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {
            InitAddressLog();
        }

        public AddressLogBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
            InitAddressLog();
        }

        public void InitAddressLog()
        {
            int length = (int)BaseStream.Length;
            addrLog = new bool[length];
        }

        public void LogAddress(int length)
        {
            LogAddress(length, (int)BaseStream.Position);
        }
        public void LogAddress(int length, long baseStreamPosition)
        {
            int address = (int)BaseStream.Position;
            for (int i = address; i < address + length; i++)
            {
                if (ThrowErrorOnDoubleRead)
                {
                    bool hasBeenRead = addrLog[i];
                    if (hasBeenRead)
                    {
                        throw new AccessViolationException();
                    }
                }
                addrLog[i] = true;
            }
        }

        public override int Read()
        {
            LogAddress(4);
            return base.Read();
        }

        public override int Read(byte[] buffer, int index, int count)
        {
            throw new NotImplementedException();
            //return base.Read(buffer, index, count);
        }

        public override int Read(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
            //return base.Read(buffer, index, count);
        }


        public override bool ReadBoolean()
        {
            LogAddress(1);
            return base.ReadBoolean();
        }

        public override byte ReadByte()
        {
            LogAddress(1);
            return base.ReadByte();
        }

        public override byte[] ReadBytes(int count)
        {
            LogAddress(count);
            return base.ReadBytes(count);
        }

        public override char ReadChar()
        {
            throw new NotImplementedException();
            //return base.ReadChar();
        }

        public override char[] ReadChars(int count)
        {
            throw new NotImplementedException();
            //LogAddress(count);
            //return base.ReadChars(count);
        }

        public override decimal ReadDecimal()
        {
            throw new NotImplementedException();
            //return base.ReadDecimal();
        }

        public override double ReadDouble()
        {
            LogAddress(8);
            return base.ReadDouble();
        }

        public override short ReadInt16()
        {
            LogAddress(2);
            return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            LogAddress(4);
            return base.ReadInt32();
        }

        public override long ReadInt64()
        {
            LogAddress(8);
            return base.ReadInt64();
        }

        public override sbyte ReadSByte()
        {
            LogAddress(1);
            return base.ReadSByte();
        }

        public override float ReadSingle()
        {
            LogAddress(4);
            return base.ReadSingle();
        }

        public override string ReadString()
        {
            throw new NotImplementedException();
            //return base.ReadString();
        }

        public override ushort ReadUInt16()
        {
            LogAddress(2);
            return base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            LogAddress(4);
            return base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            LogAddress(8);
            return base.ReadUInt64();
        }


        public void LogRangesRead(string filePath)
        {
            // Open up a file
            TextLogger log = new TextLogger(filePath);

            List<AddressRange> rangesRead = new List<AddressRange>();
            List<AddressRange> rangesUnread = new List<AddressRange>();
            AddressRange range = new AddressRange();
            bool activeType = true;

            for (int i = 0; i < addrLog.Length; i++)
            {
                var addressRead = addrLog[i];
                if (activeType == addressRead)
                {
                    //
                }
                else
                {
                    // capture where this ends
                    range.EndAddress = i;
                    //
                    if (activeType)
                    {
                        rangesRead.Add(range);
                    }
                    else
                    {
                        rangesUnread.Add(range);
                    }
                    // flips active type
                    activeType = addressRead;
                    // Start next range from current address
                    range.StartAddress = i;
                }
            }

            int unreadSize = 0;
            foreach (var rangeUnread in rangesUnread)
                unreadSize += rangeUnread.Size;

            log.WriteLine($"Unread ranges: {rangesUnread.Count}");
            log.WriteLine($"Unread size total: {unreadSize}, 0x{unreadSize:x}");
            log.WriteLine();

            log.WriteLine("RANGES UNREAD");
            foreach (var rangeUnread in rangesUnread)
            {
                log.Write(rangeUnread);
                log.Write("\t");
                BaseStream.Seek(rangeUnread.StartAddress, SeekOrigin.Begin);
                for (int i = (int)rangeUnread.StartAddress; i < (int)rangeUnread.EndAddress; i++)
                {
                    var data = ReadByte().ToString("x2");
                    log.Write(data);
                }
                log.WriteLine();
            }
            log.WriteLine();

            log.WriteLine("RANGES READ:");
            foreach (var rangeRead in rangesRead)
            {
                log.WriteLine(rangeRead);
            }
            log.WriteLine();


        }

        public void AssertUnreadIsZero()
        {
            List<AddressRange> rangesUnread;
            GetRanges(out _, out rangesUnread);

            foreach (var rangeUnread in rangesUnread)
            {
                BaseStream.Seek(rangeUnread.StartAddress, SeekOrigin.Begin);
                for (int i = (int)rangeUnread.StartAddress; i < (int)rangeUnread.EndAddress; i++)
                {
                    bool isEmpty = ReadByte() == 0;
                    if (!isEmpty)
                    {
                        var msg = $"Missed byte in range {rangeUnread} starting at {BaseStream.Position:x8}";
                        throw new Exception(msg);
                    }
                }
            }
        }

        public void GetRanges(out List<AddressRange> rangesRead, out List<AddressRange> rangesUnread)
        {
            rangesRead = new List<AddressRange>();
            rangesUnread = new List<AddressRange>();
            AddressRange range = new AddressRange();
            bool activeType = true;

            for (int i = 0; i < addrLog.Length; i++)
            {
                var addressRead = addrLog[i];
                if (activeType == addressRead)
                {
                    //
                }
                else
                {
                    // capture where this ends
                    range.EndAddress = i;
                    //
                    if (activeType)
                    {
                        rangesRead.Add(range);
                    }
                    else
                    {
                        rangesUnread.Add(range);
                    }
                    // flips active type
                    activeType = addressRead;
                    // Start next range from current address
                    range.StartAddress = i;
                }
            }
        }

    }
}
