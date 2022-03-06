using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A single data field for TextureMetadata.
    /// </summary>
    [Serializable]
    public class TextureScrollField :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        public float x; // range -3 to 6, indexes: 0-3
        public float y; // range -10 to 30, indexes: 0-3


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref x);
                reader.ReadX(ref y);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(x);
                writer.WriteX(y);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                //$"{nameof(TextureMetadataField)}(" +
                $"({nameof(x)}: {x}, " +
                $"{nameof(y)}: {y}" +
                $")";
        }

    }
}