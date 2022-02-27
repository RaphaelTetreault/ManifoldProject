using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    internal class SkinBoneBinding :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private int count;
        private Pointer[] verticePtrs;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int Count { get => count; set => count = value; }
        public Pointer[] VerticePtrs { get => verticePtrs; set => verticePtrs = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref count);
                reader.ReadX(ref verticePtrs, count, true);
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