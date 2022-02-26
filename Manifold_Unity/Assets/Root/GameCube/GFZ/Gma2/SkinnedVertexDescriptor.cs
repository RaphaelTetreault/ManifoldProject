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
        private int skinnedVertexCount;
        private Pointer unkType1RelPtr;
        private Pointer skinnedVerticesRelPtr;
        private Pointer skinBoneBindingsRelPtr;
        private Pointer unkType4RelPtr;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int SkinnedVertexCount { get => skinnedVertexCount; set => skinnedVertexCount = value; }
        public Pointer UnkType1RelPtr { get => unkType1RelPtr; set => unkType1RelPtr = value; }
        public Pointer SkinnedVerticesRelPtr { get => skinnedVerticesRelPtr; set => skinnedVerticesRelPtr = value; }
        public Pointer SkinBoneBindingsRelPtr { get => skinBoneBindingsRelPtr; set => skinBoneBindingsRelPtr = value; }
        public Pointer UnkType4RelPtr { get => unkType4RelPtr; set => unkType4RelPtr = value; }

        public int UnkType1Count
        {
            get => (UnkType4RelPtr - UnkType1RelPtr) / 0x20;
        }



        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref skinnedVertexCount);
                reader.ReadX(ref unkType1RelPtr);
                reader.ReadX(ref skinnedVerticesRelPtr);
                reader.ReadX(ref skinBoneBindingsRelPtr);
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