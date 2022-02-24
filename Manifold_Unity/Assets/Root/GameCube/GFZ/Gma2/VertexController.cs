using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GFZ.Gma2
{
    internal class VertexController :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private int vertexCount;
        private Pointer unkType1RelPtr;
        private Pointer unkType2RelPtr;
        private Pointer unkType3RelPtr;
        private Pointer unkType4RelPtr;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int Unk1Count
        {
            get
            {
                return (unkType4RelPtr - unkType1RelPtr) / 0x20;
            }
        }

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref vertexCount);
                reader.ReadX(ref unkType1RelPtr);
                reader.ReadX(ref unkType2RelPtr);
                reader.ReadX(ref unkType3RelPtr);
                reader.ReadX(ref unkType4RelPtr);
            }
            this.RecordEndAddress(reader);
            reader.AlignTo(GX.GXUtility.GX_FIFO_ALIGN);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                //writer.WriteX();
            }
            this.RecordEndAddress(writer);

            throw new NotImplementedException();
        }

    }

}