using Manifold;
using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    public class TransformMatrixIndexes8 :
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONSTANTS
        private const int kIndexCount = 8;

        //
        public TransformMatrixIndexes8()
        {
            indexes = new byte[kIndexCount];
            for (int i = 0; i < indexes.Length; i++)
                indexes[i] = 0xFF;
        }

        // FIELDS
        private byte[] indexes;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref indexes, kIndexCount);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            Assert.IsTrue(indexes.Length == kIndexCount);

            this.RecordStartAddress(writer);
            {
                writer.WriteX(indexes, false);
            }
            this.RecordEndAddress(writer);
        }

    }

}