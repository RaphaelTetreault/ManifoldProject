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
    public class ModelEntry :
        IBinaryAddressable,
        IBinarySerializable
    {
        //
        private Pointer gcmfRelPtr;
        private Pointer nameRelPtr;

        //
        public AddressRange AddressRange { get; set; }
        public Pointer GcmfPtr { get => new Pointer(GcmfBasePtr + GcmfRelPtr); }
        public Pointer NamePtr { get => new Pointer(NameBasePtr + NameRelPtr); }
        public Pointer GcmfRelPtr { get => gcmfRelPtr; set => gcmfRelPtr = value; }
        public Pointer NameRelPtr { get => nameRelPtr; set => nameRelPtr = value; }
        public Pointer GcmfBasePtr { get; set; }
        public Pointer NameBasePtr { get; set; }
        public bool IsNull { get => gcmfRelPtr == -1 && NameRelPtr == 0; }
        public bool IsNotNull { get => !IsNull; }

        //
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref gcmfRelPtr);
                reader.ReadX(ref nameRelPtr);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(GcmfRelPtr);
                writer.WriteX(NameRelPtr);
            }
            this.RecordEndAddress(writer);
        }

    }
}
