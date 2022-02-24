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
    public class Gcmf :
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONSTANTS
        /// <summary>
        /// Equivilent to ASCII/Shift-JIS "GCMF"
        /// </summary>
        public const uint kMagic = 0x47434D46;
        public const int kZeroes0x24 = 4;

        // FIELDS
        private uint magic;
        private GcmfAttributes attributes;
        private BoundingSphere boundingSphere;
        private ushort textureCount;
        private ushort materialCount;
        private ushort translucidMaterialCount;
        private byte transformMatrixCount;
        private byte zero0x1F;
        private uint gcmfTexturesSize;
        private byte[] zeroes0x24;
        private TransformMatrixIndexes8 transformMatrixIndexes8;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref magic);
                reader.ReadX(ref attributes);
                reader.ReadX(ref boundingSphere, true);
                reader.ReadX(ref textureCount);
                reader.ReadX(ref materialCount);
                reader.ReadX(ref translucidMaterialCount);
                reader.ReadX(ref transformMatrixCount);
                reader.ReadX(ref zero0x1F);
                reader.ReadX(ref gcmfTexturesSize);
                reader.ReadX(ref zeroes0x24, kZeroes0x24);
                reader.ReadX(ref transformMatrixIndexes8, true);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(magic == kMagic);
                Assert.IsTrue(zero0x1F == 0);
                foreach (var zero in zeroes0x24)
                    Assert.IsTrue(zero == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                throw new NotFiniteNumberException();
                //writer.WriteX();
            }
            this.RecordEndAddress(writer);
        }

    }

}