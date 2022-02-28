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
        private int skinnedVertexBCount;
        private Offset skinnedVerticesARelPtr;
        private Offset skinnedVerticesBRelPtr;
        private Offset skinBoneBindingsRelPtr;
        private Offset unkBoneMatrixIndicesRelPtr;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int SkinnedVerticesACount
        {
            get => (UnkBoneMatrixIndicesRelPtr - SkinnedVerticesARelPtr) / 0x20;
        }
        public int SkinnedVerticesBCount { get => skinnedVertexBCount; set => skinnedVertexBCount = value; }
        public Offset SkinnedVerticesARelPtr { get => skinnedVerticesARelPtr; set => skinnedVerticesARelPtr = value; }
        public Offset SkinnedVerticesBRelPtr { get => skinnedVerticesBRelPtr; set => skinnedVerticesBRelPtr = value; }
        public Offset SkinBoneBindingsRelPtr { get => skinBoneBindingsRelPtr; set => skinBoneBindingsRelPtr = value; }
        public Offset UnkBoneMatrixIndicesRelPtr { get => unkBoneMatrixIndicesRelPtr; set => unkBoneMatrixIndicesRelPtr = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref skinnedVertexBCount);
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