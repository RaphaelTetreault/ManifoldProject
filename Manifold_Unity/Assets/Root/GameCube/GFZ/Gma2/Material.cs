using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace GameCube.GFZ.Gma2
{
    public class Material :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private ushort zeroes0x00; //2
        private MatFlags0x02 unk0x02; // <- COLOR blending stuff?
        private MatFlags0x03 unk0x03; // <- cont
        private uint color0;
        private uint color1;
        private uint color2;
        private MatFlags0x10 unk0x10;
        private MatFlags0x11 unk0x11;
        private byte textureCount;
        private DisplayListRenderFlags displayListRenderFlags;
        private byte unk0x14;
        private MatFlags0x15 unk0x15;
        private short textureIndex0 = -1;
        private short textureIndex1 = -1;
        private short textureIndex2 = -1;
        private GXAttributes gxAttributes;
        private TransformMatrixIndexes8 transformMatrixIndexes;
        // this onwards feels like it's own things
        private int materialDisplayListSize;
        private int translucidMaterialDisplayListSize;
        private float3 origin;
        private uint unk0x3C;
        private MatFlags0x40 unk0x40;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }



        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zeroes0x00);
                reader.ReadX(ref unk0x02);
                reader.ReadX(ref unk0x03);
                reader.ReadX(ref color0);
                reader.ReadX(ref color1);
                reader.ReadX(ref color2);
                reader.ReadX(ref unk0x10);
                reader.ReadX(ref unk0x11);
                reader.ReadX(ref textureCount);
                reader.ReadX(ref displayListRenderFlags);
                reader.ReadX(ref unk0x14);
                reader.ReadX(ref unk0x15);
                reader.ReadX(ref textureIndex0);
                reader.ReadX(ref textureIndex1);
                reader.ReadX(ref textureIndex2);
                reader.ReadX(ref gxAttributes);
                reader.ReadX(ref transformMatrixIndexes, true);
                reader.ReadX(ref materialDisplayListSize);
                reader.ReadX(ref translucidMaterialDisplayListSize);
                reader.ReadX(ref origin);
                reader.ReadX(ref unk0x3C);
                reader.ReadX(ref unk0x40);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zeroes0x00 == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zeroes0x00 == 0);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(zeroes0x00);
                writer.WriteX(unk0x02);
                writer.WriteX(unk0x03);
                writer.WriteX(color0);
                writer.WriteX(color1);
                writer.WriteX(color2);
                writer.WriteX(unk0x10);
                writer.WriteX(unk0x11);
                writer.WriteX(textureCount);
                writer.WriteX(displayListRenderFlags);
                writer.WriteX(unk0x14);
                writer.WriteX(unk0x15);
                writer.WriteX(textureIndex0);
                writer.WriteX(textureIndex1);
                writer.WriteX(textureIndex2);
                writer.WriteX(gxAttributes);
                writer.WriteX(transformMatrixIndexes);
                writer.WriteX(materialDisplayListSize);
                writer.WriteX(translucidMaterialDisplayListSize);
                writer.WriteX(origin);
                writer.WriteX(unk0x3C);
                writer.WriteX(unk0x40);
            }
            this.RecordEndAddress(writer);
        }

    }

}