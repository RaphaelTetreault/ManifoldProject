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
    public class SkinnedVertexDescriptor :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private int vertexCount;
        private Pointer unkType1RelPtr; // ARRAY PTR
        private Pointer unkType2RelPtr; // STRUCTS BEGIN 
        private Pointer unkType3RelPtr;
        private Pointer unkType4RelPtr;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int VertexCount { get => vertexCount; set => vertexCount = value; }
        public Pointer UnkType1RelPtr { get => unkType1RelPtr; set => unkType1RelPtr = value; }
        public Pointer UnkType2RelPtr { get => unkType2RelPtr; set => unkType2RelPtr = value; }
        public Pointer UnkType3RelPtr { get => unkType3RelPtr; set => unkType3RelPtr = value; }
        public Pointer UnkType4RelPtr { get => unkType4RelPtr; set => unkType4RelPtr = value; }

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