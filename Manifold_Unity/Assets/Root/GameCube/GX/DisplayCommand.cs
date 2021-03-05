using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GX
{
    [Serializable]
    public struct DisplayCommand : IBinarySerializable
    {
        [SerializeField] GXPrimitive primitive;
        [SerializeField] GXVtxFmt vertexFormat;
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

        public GXPrimitive Primitive
        {
            get
            {
                // 5 highest bits
                return (GXPrimitive)(command & 0b11111000);
            }
        }

        public GXVtxFmt VertexFormat
        {
            get
            {
                // 3 lowest bits
                return (GXVtxFmt)(command & 0b00000111);
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
            primitive = (GXPrimitive)(command & 0b_00000000_11111000); // 5 highest bits
            vertexFormat = (GXVtxFmt)(command & 0b_00000000_00000111); // 3 lowest bits
        }
    }
}