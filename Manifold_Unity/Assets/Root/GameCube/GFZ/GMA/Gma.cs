using Manifold.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// Represents a GMA file.
    /// </summary>
    public class Gma :
        IBinarySerializable,
        IHasReference,
        IFileType
    {
        // FIELDS
        private int modelsCount;
        private Offset modelBasePtrOffset;
        private ModelEntry[] modelEntries;
        // Pseudo-fields.
        private Model[] models;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public string FileName { get; set; }
        public string FileExtension => ".gma";
        public int ModelsCount { get => modelsCount; set => modelsCount = value; }
        public Offset ModelBasePtr { get => modelBasePtrOffset; set => modelBasePtrOffset = value; }
        public ModelEntry[] ModelEntries { get => modelEntries; set => modelEntries = value; }
        public Model[] Models { get => models; set => models = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref modelsCount);
            reader.ReadX(ref modelBasePtrOffset);
            reader.ReadX(ref modelEntries, modelsCount);

            Offset nameBasePtrOffset = reader.GetPositionAsPointer().address;
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

        public void Serialize(BinaryWriter writer)
        {
            // Collect all names and GCMF values from Models
            var modelNames = new List<ShiftJisCString>();
            var modelGCMFs = new List<Gcmf>();
            foreach (var model in models)
            {
                if (model is null)
                    continue;

                modelNames.Add(model.Name);
                modelGCMFs.Add(model.Gcmf);
            }

            // Write GCMFs to a memory stream, collect their pointer
            var gcmfOffsets = new Offset[modelGCMFs.Count];
            var gcmfWriter = new BinaryWriter(new MemoryStream());
            for (int i = 0; i < modelGCMFs.Count; i++)
            {
                var gcmf = modelGCMFs[i];
                gcmfWriter.WriteX(gcmf);
                gcmfOffsets[i] = gcmf.GetPointer().address;
            }
            gcmfWriter.SeekBegin();

            // Write names to memory stream, collet their pointers
            var nameOffsets = new Offset[modelGCMFs.Count];
            var nameWriter = new BinaryWriter(new MemoryStream());
            for (int i = 0; i < modelGCMFs.Count; i++)
            {
                var name = modelNames[i];
                nameWriter.WriteX(name);
                nameOffsets[i] = name.GetPointer().address;
            }
            nameWriter.SeekBegin();

            // Construct model entries
            modelEntries = new ModelEntry[modelGCMFs.Count];
            for (int i = 0; i < modelGCMFs.Count; i++)
            {
                modelEntries[i] = new ModelEntry()
                {
                    GcmfRelPtr = gcmfOffsets[i],
                    NameRelPtr = nameOffsets[i],
                };
            }

            // This will be grabage/blank
            writer.WriteX(modelsCount);
            writer.WriteX(modelBasePtrOffset);

            // Write entries (offsets)
            foreach (var modelEntry in modelEntries)
                writer.WriteX(modelEntry);

            // Copy written memory stream data over to file stream
            nameWriter.BaseStream.CopyTo(writer.BaseStream);
            writer.WriteAlignment(GX.GXUtility.GX_FIFO_ALIGN);

            // Update some metadata
            modelBasePtrOffset = writer.GetPositionAsPointer().address;
            modelsCount = modelEntries.Length;

            // Write model data
            gcmfWriter.BaseStream.CopyTo(writer.BaseStream);

            // Re-write values
            writer.JumpToAddress(0);
            writer.WriteX(modelsCount);
            writer.WriteX(modelBasePtrOffset);
        }

        public void ValidateReferences()
        {
            throw new NotImplementedException();
        }
    }
}
