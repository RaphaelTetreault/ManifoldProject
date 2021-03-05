using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GX
{
    [Serializable]
    public struct DisplayCommand : IBinarySerializable
    {
        [SerializeField] Primitive primitive;
        [SerializeField] VertexFormat vertexFormat;
        [SerializeField] byte command;

        public byte Command
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

        public Primitive Primitive
        {
            get
            {
                // 5 highest bits
                return (Primitive)(command & 0b11111000);
            }
        }

        public VertexFormat VertexFormat
        {
            get
            {
                // 3 lowest bits
                return (VertexFormat)(command & 0b00000111);
            }
        } 

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