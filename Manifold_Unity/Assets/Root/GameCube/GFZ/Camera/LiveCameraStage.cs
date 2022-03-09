using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Camera
{
    [Serializable]
    public sealed class LiveCameraStage :
        IBinarySerializable,
        IFileType
    {
        // FIELDS
        private CameraPan[] pans;


        // PROPERTIES
        public string FileName { get; set; }
        public string FileExtension => ".bin";
        public CameraPan[] Pans
        {
            get => pans;
            set => pans = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            // Figure out how many camera pans are in this file
            var nPans = (int)(reader.BaseStream.Length / CameraPan.kStructureSize);
            
            // Read that many structures out of the file
            reader.ReadX(ref pans, nPans);

            // Sanity check. We should be at the end of the stream
            Assert.IsTrue(reader.BaseStream.IsAtEndOfStream());
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(pans);
        }

    }
}
