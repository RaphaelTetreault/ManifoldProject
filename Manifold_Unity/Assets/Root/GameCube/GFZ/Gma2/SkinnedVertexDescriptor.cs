using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    public class SkinnedVertexDescriptor :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private int skinnedVertexCount;
        private Pointer skinnedVerticesARelPtr;
        private Pointer skinnedVerticesBRelPtr;
        private Pointer skinBoneBindingsRelPtr;
        private Pointer unkBoneMatrixIndicesRelPtr;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int SkinnedVertexCount { get => skinnedVertexCount; set => skinnedVertexCount = value; }
        public Pointer SkinnedVerticesARelPtr { get => skinnedVerticesARelPtr; set => skinnedVerticesARelPtr = value; }
        public Pointer SkinnedVerticesBRelPtr { get => skinnedVerticesBRelPtr; set => skinnedVerticesBRelPtr = value; }
        public Pointer SkinBoneBindingsRelPtr { get => skinBoneBindingsRelPtr; set => skinBoneBindingsRelPtr = value; }
        public Pointer UnkBoneMatrixIndicesRelPtr { get => unkBoneMatrixIndicesRelPtr; set => unkBoneMatrixIndicesRelPtr = value; }

        public int SkinnedVerticesACount
        {
            get => (UnkBoneMatrixIndicesRelPtr - SkinnedVerticesARelPtr) / 0x20;
        }



        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref skinnedVertexCount);
                reader.ReadX(ref skinnedVerticesARelPtr);
                reader.ReadX(ref skinnedVerticesBRelPtr);
                reader.ReadX(ref skinBoneBindingsRelPtr);
                reader.ReadX(ref unkBoneMatrixIndicesRelPtr);
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