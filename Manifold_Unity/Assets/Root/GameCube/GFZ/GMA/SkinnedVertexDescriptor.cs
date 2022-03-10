using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.GMA
{
    public class SkinnedVertexDescriptor :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private int skinnedVertexBCount;
        private Offset skinnedVerticesAPtrOffset;
        private Offset skinnedVerticesBPtrOffset;
        private Offset skinBoneBindingsPtrOffset;
        private Offset unkBoneIndicesPtrOffset;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int SkinnedVerticesACount
        {
            get => (UnkBoneIndicesPtrOffset - SkinnedVerticesAPtrOffset) / 0x20;
        }
        public int SkinnedVerticesBCount { get => skinnedVertexBCount; set => skinnedVertexBCount = value; }
        public Offset SkinnedVerticesAPtrOffset { get => skinnedVerticesAPtrOffset; set => skinnedVerticesAPtrOffset = value; }
        public Offset SkinnedVerticesBPtrOffset { get => skinnedVerticesBPtrOffset; set => skinnedVerticesBPtrOffset = value; }
        public Offset SkinBoneBindingsPtrOffset { get => skinBoneBindingsPtrOffset; set => skinBoneBindingsPtrOffset = value; }
        public Offset UnkBoneIndicesPtrOffset { get => unkBoneIndicesPtrOffset; set => unkBoneIndicesPtrOffset = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref skinnedVertexBCount);
                reader.ReadX(ref skinnedVerticesAPtrOffset);
                reader.ReadX(ref skinnedVerticesBPtrOffset);
                reader.ReadX(ref skinBoneBindingsPtrOffset);
                reader.ReadX(ref unkBoneIndicesPtrOffset);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(skinnedVertexBCount);
                writer.WriteX(skinnedVerticesAPtrOffset);
                writer.WriteX(skinnedVerticesBPtrOffset);
                writer.WriteX(skinBoneBindingsPtrOffset);
                writer.WriteX(unkBoneIndicesPtrOffset);
            }
            this.RecordEndAddress(writer);
        }

    }

}