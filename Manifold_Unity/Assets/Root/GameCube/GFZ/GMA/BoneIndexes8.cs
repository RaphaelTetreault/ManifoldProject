using Manifold;
using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// A set of 8 matrix indexes. Null indexes read as 0xFF.
    /// </summary>
    /// <remarks>
    /// Consider making the backing a uint64, contruct array on demand.
    /// </remarks>
    public class BoneIndexes8 :
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONSTANTS
        private const int kIndexCount = 8;


        // CONSTRUCTORS
        public BoneIndexes8()
        {
            indexes = new sbyte[kIndexCount];
            for (int i = 0; i < indexes.Length; i++)
                indexes[i] = -1;
        }


        // FIELDS
        private sbyte[] indexes; // look into 'fixed' arrays


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
            {
                Assert.IsTrue(indexes.Length == kIndexCount);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(indexes);
            }
            this.RecordEndAddress(writer);
        }

    }

}