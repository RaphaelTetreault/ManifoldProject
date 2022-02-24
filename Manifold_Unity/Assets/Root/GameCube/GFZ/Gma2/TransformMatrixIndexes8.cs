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
    internal struct TransformMatrixIndexes8 :
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONSTANTS
        private const int kIndexCount = 8;

        //
        public TransformMatrixIndexes8()
        {
            AddressRange = new AddressRange();
            indexes = new byte[kIndexCount];
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