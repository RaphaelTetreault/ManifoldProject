using Manifold.IO;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// Appears to be a GX vertex meant for skinning. It is not part of a formal
    /// display list. The GXAttributes properly describe this as having position,
    /// normal, and texture0. May have something to do with GXAttributes.GX_VA_PNMTXIDX.
    /// </summary>
    /// <remarks>
    /// Notes: regarding textureUV0: it is indeed stored as TEX0 in the GXAttributes.
    /// However, the data does not need to be for texturing. TEX0.u stores some 
    /// "magic bits" while TEX0.v stores a (normalized?) float, perhaps for weighting.
    /// </remarks>
    public class SkinnedVertexA :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private float3 position;
        private float3 normal;
        private float2 textureUV0;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public float3 Position { get => position; set => position = value; }
        public float3 Normal { get => normal; set => normal = value; }
        public float2 TextureUV0 { get => textureUV0; set => textureUV0 = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref position);
                reader.ReadX(ref normal);
                reader.ReadX(ref textureUV0);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(position);
                writer.WriteX(normal);
                writer.WriteX(textureUV0);
            }
            this.RecordEndAddress(writer);
        }

    }
}