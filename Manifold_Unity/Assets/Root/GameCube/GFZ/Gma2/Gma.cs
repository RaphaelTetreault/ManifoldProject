using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    /// <summary>
    /// Represents a GMA file.
    /// </summary>
    public class Gma :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        IFile
    {
        // FIELDS
        private int modelsCount;
        private Offset modelBasePtrOffset;
        private ModelEntry[] modelEntries;
        //
        private Model[] models;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public string FileName { get; set; }
        public int ModelsCount { get => modelsCount; set => modelsCount = value; }
        public Offset ModelBasePtr { get => modelBasePtrOffset; set => modelBasePtrOffset = value; }
        public ModelEntry[] ModelEntries { get => modelEntries; set => modelEntries = value; }
        public Model[] Models { get => models; set => models = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref modelsCount);
                reader.ReadX(ref modelBasePtrOffset);
                reader.ReadX(ref modelEntries, modelsCount, true);
            }
            this.RecordEndAddress(reader);
            {
                Offset nameBasePtrOffset = AddressRange.endAddress;
                var modelList = new List<Model>();

                // Add offsets necessary for pointers to be correct
                for (int i = 0; i < modelsCount; i++)
                {
                    var modelEntry = modelEntries[i];
                    modelEntry.GcmfBasePtrOffset = modelBasePtrOffset;
                    modelEntry.NameBasePtrOffset = nameBasePtrOffset;

                    if (modelEntry.IsNull)
                        continue;

                    var model = new Model()
                    {
                        GcmfPtr = modelEntry.GcmfPtr,
                        NamePtr = modelEntry.NamePtr,
                    };
                    model.Deserialize(reader);
                    modelList.Add(model);
                }
                models = modelList.ToArray();
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(modelsCount);
                writer.WriteX(modelBasePtrOffset);
            }
            this.RecordEndAddress(writer);
            {
                throw new NotImplementedException();
            }
        }

        public void ValidateReferences()
        {
            throw new NotImplementedException();
        }
    }
}
