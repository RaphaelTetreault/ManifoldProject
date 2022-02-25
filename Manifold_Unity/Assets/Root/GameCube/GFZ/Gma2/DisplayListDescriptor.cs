using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    public class DisplayListDescriptor :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private TransformMatrixIndexes8 transformMatrixIndexes;
        private int materialDisplayListSize;
        private int translucidMaterialDisplayListSize;

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public TransformMatrixIndexes8 TransformMatrixIndexes { get => transformMatrixIndexes; set => transformMatrixIndexes = value; }
        public int MaterialSize { get => materialDisplayListSize; set => materialDisplayListSize = value; }
        public int TranslucidMaterialSize { get => translucidMaterialDisplayListSize; set => translucidMaterialDisplayListSize = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transformMatrixIndexes, true);
                reader.ReadX(ref materialDisplayListSize);
                reader.ReadX(ref translucidMaterialDisplayListSize);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(transformMatrixIndexes);
                writer.WriteX(materialDisplayListSize);
                writer.WriteX(translucidMaterialDisplayListSize);
            }
            this.RecordEndAddress(writer);
        }

    }

}