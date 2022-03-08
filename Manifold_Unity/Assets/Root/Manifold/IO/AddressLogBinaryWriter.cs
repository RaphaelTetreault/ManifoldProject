using System;
using System.IO;
using System.Text;

namespace Manifold.IO
{
    /// <summary>
    /// A hack-ish writer which keeps track of addresses written to and (optionally)
    /// raises an error when the same address is written to twice. Useful for finding
    /// errors in serialization overwriting data when you don't want it to.
    /// </summary>
    public sealed class AddressLogBinaryWriter : BinaryWriter
    {
        private bool[] addrBuffer = new bool[1024];

        public bool MemoryLogActive { get; set; } = true;


        public AddressLogBinaryWriter() : base()
        { }

        public AddressLogBinaryWriter(Stream output) : base(output)
        {
            InitMemoryGuard();
        }

        public AddressLogBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
            InitMemoryGuard();
        }

        public AddressLogBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
        {
            InitMemoryGuard();
        }


        public void InitMemoryGuard()
        {
            addrBuffer = new bool[1024];
        }

        public override void Write(byte value)
        {
            // Forward
            Write(new byte[] { value });
        }

        public override void Write(byte[] buffer)
        {
            if (MemoryLogActive)
            {
                AddressRange addressRange = new AddressRange()
                {
                    StartAddress = (int)BaseStream.Position,
                    EndAddress = (int)BaseStream.Position + buffer.Length,
                };

                bool isLesserThan = addrBuffer.Length < addressRange.EndAddress;
                if (isLesserThan)
                {
                    // double the buffer size
                    var newBuffer = new bool[addrBuffer.Length * 2];
                    addrBuffer.CopyTo(newBuffer, 0);
                    addrBuffer = newBuffer;
                }

                // Loop from start address to end address -1
                for (int addr = (int)addressRange.StartAddress; addr < (int)addressRange.EndAddress; addr++)
                {
                    bool addressWrittenTo = addrBuffer[addr];
                    if (addressWrittenTo)
                    {
                        throw new AccessViolationException($"Address 0x{addr:x8} is being written to again!");
                    }
                    addrBuffer[addr] = true;
                }
            }

            base.Write(buffer);
        }

        public override void Write(byte[] buffer, int index, int count)
        {
            throw new NotImplementedException();
            //base.Write(buffer, index, count);
        }


    }
}
