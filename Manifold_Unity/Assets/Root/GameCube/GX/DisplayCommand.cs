using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GX
{
    [Serializable]
    public struct DisplayCommand :
        IBinarySerializable
    {
        // FIELDS
        private ushort command;
        private Primitive primitive;
        private VertexFormat vertexFormat;


        // PROPERTIES
        public ushort Command
        {
            get
            {
                return command;
            }

            set
            {
                command = value;
                UpdateBitFields();
            }
        }
        public Primitive Primitive => primitive;
        public VertexFormat VertexFormat => vertexFormat;
        public byte VertexFormatIndex => (byte)VertexFormat;


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref command);
            UpdateBitFields();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(command);
        }

        private void UpdateBitFields()
        {
            primitive = (Primitive)(command & 0b_00000000_11111000); // 5 highest bits
            vertexFormat = (VertexFormat)(command & 0b_00000000_00000111); // 3 lowest bits
        }

    }
}