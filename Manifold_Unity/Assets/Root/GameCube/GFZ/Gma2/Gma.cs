using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameCube.GFZ.Gma2
{
    public class Gma :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        IFile
    {
        // 
        internal static int FilePointersOffset { get; private set; }

        // FIELDS
        private int modelsCount;
        private Pointer modelBasePtr;
        private ModelEntry[] modelEntries;
        //
        private Model[] models;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public string FileName { get; set; }
        public int ModelsCount { get => modelsCount; set => modelsCount = value; }
        public Pointer ModelBasePtr { get => modelBasePtr; set => modelBasePtr = value; }
        public ModelEntry[] ModelEntries { get => modelEntries; set => modelEntries = value; }
        public Model[] Models { get => models; set => models = value; }



        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref modelsCount);
                reader.ReadX(ref modelBasePtr);
                reader.ReadX(ref modelEntries, modelsCount, true);
            }
            this.RecordEndAddress(reader);
            {
                Pointer nameBasePtr = AddressRange.endAddress;
                FilePointersOffset = modelBasePtr;
                var modelList = new List<Model>();

                // Add offsets necessary for pointers to be correct
                for (int i = 0; i < modelsCount; i++)
                {
                    var modelEntry = modelEntries[i];
                    modelEntry.GcmfBasePtr = modelBasePtr;
                    modelEntry.NameBasePtr = nameBasePtr;

                    if (modelEntry.IsNull)
                        continue;

                    var model = new Model()
                    {
                        GcmfPtr = modelEntry.GcmfPtr,
                        NamePtr = nameBasePtr,
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
                writer.WriteX(modelBasePtr);
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
