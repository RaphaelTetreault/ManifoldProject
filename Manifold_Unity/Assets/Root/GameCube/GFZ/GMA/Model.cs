using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.GMA
{
    /// <summary>
    /// A model comprised of a mesh (many display lists) and materials.
    /// </summary>
    public class Model
    {
        // FIELDS
        private ShiftJisCString name;
        private Gcmf gcmf;
        
        // PROPERTIES
        public Pointer GcmfPtr { get; set; }
        public Pointer NamePtr { get; set; }
        public ShiftJisCString Name { get => name; set => name = value; }
        public Gcmf Gcmf { get => gcmf; set => gcmf = value; }

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            // Pointers are assigned from outside the class.
            // If not set, Deserialize() is not expected to be called.
            Assert.IsTrue(GcmfPtr.IsNotNull);
            Assert.IsTrue(NamePtr.IsNotNull);

            reader.JumpToAddress(NamePtr);
            reader.ReadX(ref name);

            reader.JumpToAddress(GcmfPtr);
            reader.ReadX(ref gcmf);
        }

    }
}
