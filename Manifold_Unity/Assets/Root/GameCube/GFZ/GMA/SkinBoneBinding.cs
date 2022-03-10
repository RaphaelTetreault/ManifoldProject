using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// Conjecture: appears to 
    /// </summary>
    public class SkinBoneBinding :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // FIELDS
        private int count;
        private Offset[] verticePtrOffsets;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int Count { get => verticePtrOffsets.Length; }
        public Offset[] VerticePtrOffsets { get => verticePtrOffsets; set => verticePtrOffsets = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref count);
                reader.ReadX(ref verticePtrOffsets, count);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                // TODO: assign offsets here? (similar to getting pointers)
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(Count);
                writer.WriteX(verticePtrOffsets);
            }
            this.RecordEndAddress(writer);
            {
                //throw new NotImplementedException();
            }
        }

        public void ValidateReferences()
        {
            throw new NotImplementedException();
        }
    }

}