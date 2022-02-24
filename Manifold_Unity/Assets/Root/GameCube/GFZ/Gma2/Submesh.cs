using GameCube.GX;
using Manifold;
using Manifold.IO;
using System.IO;


namespace GameCube.GFZ.Gma2
{
    internal class Submesh :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private Material material;
        private DisplayList displayListCW;
        private DisplayList displayListCCW;
        private SkinnedMeshDescriptor skinnedMeshDescriptor;
        private DisplayList skinnedDisplayListCW;
        private DisplayList skinnedDisplayListCCW;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref material, true);

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
        }

    }

}