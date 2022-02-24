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
    public class Gma :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        IFile
    {
        // 
        internal static int FileOffset { get; private set; }

        // METADATA
        public AddressRange AddressRange { get; set; }
        public string FileName { get; set; }

        // FIELDS
        private int modelsCount;
        private Pointer modelBasePtr;
        private ModelEntry[] modelEntries;
        //
        private Model[] models;

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
