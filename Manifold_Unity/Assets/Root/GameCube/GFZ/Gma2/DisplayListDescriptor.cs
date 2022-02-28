using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    /// <summary>
    /// Defines the size of opaque and translucide display lists and (presumably)
    /// the matrix indexes associate with it.
    /// </summary>
    /// <remarks>
    /// (TODO: find out exactly how. Theory: # mtx indexes = # display lists?
    /// </remarks>
    public class DisplayListDescriptor :
        IBinaryAddressable,
        IBinarySerializable
    {
        // FIELDS
        private TransformMatrixIndexes8 transformMatrixIndexes;
        private int opaqueMaterialDisplayListSize;
        private int translucidMaterialDisplayListSize;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public int OpaqueMaterialSize { get => opaqueMaterialDisplayListSize; set => opaqueMaterialDisplayListSize = value; }
        public TransformMatrixIndexes8 TransformMatrixIndexes { get => transformMatrixIndexes; set => transformMatrixIndexes = value; }
        public int TranslucidMaterialSize { get => translucidMaterialDisplayListSize; set => translucidMaterialDisplayListSize = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref transformMatrixIndexes, true);
                reader.ReadX(ref opaqueMaterialDisplayListSize);
                reader.ReadX(ref translucidMaterialDisplayListSize);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(transformMatrixIndexes);
                writer.WriteX(opaqueMaterialDisplayListSize);
                writer.WriteX(translucidMaterialDisplayListSize);
            }
            this.RecordEndAddress(writer);
        }

    }
}