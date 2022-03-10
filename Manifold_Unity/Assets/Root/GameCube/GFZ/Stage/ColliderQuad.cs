using Manifold;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// An individual quad as part of a collider mesh.
    /// </summary>
    [Serializable]
    public sealed class ColliderQuad :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FIELDS
        private float dotProduct;
        private float3 normal;
        private float3 vertex0;
        private float3 vertex1;
        private float3 vertex2;
        private float3 vertex3;
        private float3 precomputed0;
        private float3 precomputed1;
        private float3 precomputed2;
        private float3 precomputed3;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        /// <remarks>
        /// The dot product of dot(normal, vertex0/1/2/4). All result in the same scalar.
        /// </remarks>
        public float DotProduct { get => dotProduct; set => dotProduct = value; }
        public float3 Normal { get => normal; set => normal = value; }
        public float3 Precomputed0 { get => precomputed0; set => precomputed0 = value; }
        public float3 Precomputed1 { get => precomputed1; set => precomputed1 = value; }
        public float3 Precomputed2 { get => precomputed2; set => precomputed2 = value; }
        public float3 Precomputed3 { get => precomputed3; set => precomputed3 = value; }
        public float3 Vertex0 { get => vertex0; set => vertex0 = value; }
        public float3 Vertex1 { get => vertex1; set => vertex1 = value; }
        public float3 Vertex2 { get => vertex2; set => vertex2 = value; }
        public float3 Vertex3 { get => vertex3; set => vertex3 = value; }

        // METHODS

        /// <summary>
        /// Computes and stores the dotProduct of this quadrangle.
        /// </summary>
        public void ComputeDotProduct()
        {
            // NOTE you can dot any of the vertices you want with
            //      the normal and will always get the same scalar.
            float dotProduct =
                Normal.x * Vertex0.x +
                Normal.y * Vertex0.y +
                Normal.z * Vertex0.z;

            this.DotProduct = dotProduct;
        }

        public float3[] GetVerts()
        {
            return new float3[] { Vertex0, Vertex1, Vertex2, Vertex3 };
        }
        public float3[] GetPrecomputes()
        {
            return new float3[] { Precomputed0, Precomputed1, Precomputed2, Precomputed3 };
        }

        public float3 VertCenter()
        {
            return (Vertex0 + Vertex1 + Vertex2 + Vertex3) / 4f;
        }

        public float3 PrecomputeCenter()
        {
            // Division inline since the values are BIG and would
            // lose more precision if summed first.
            return
                Precomputed0 / 4f +
                Precomputed1 / 4f +
                Precomputed2 / 4f + 
                Precomputed3 / 4f;
        }

        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref dotProduct);
                reader.ReadX(ref normal);
                reader.ReadX(ref vertex0);
                reader.ReadX(ref vertex1);
                reader.ReadX(ref vertex2);
                reader.ReadX(ref vertex3);
                reader.ReadX(ref precomputed0);
                reader.ReadX(ref precomputed1);
                reader.ReadX(ref precomputed2);
                reader.ReadX(ref precomputed3);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(dotProduct);
                writer.WriteX(normal);
                writer.WriteX(vertex0);
                writer.WriteX(vertex1);
                writer.WriteX(vertex2);
                writer.WriteX(vertex3);
                writer.WriteX(precomputed0);
                writer.WriteX(precomputed1);
                writer.WriteX(precomputed2);
                writer.WriteX(precomputed3);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString() => PrintSingleLine();

        public string PrintSingleLine()
        {
            return nameof(ColliderQuad);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(ColliderQuad));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(dotProduct)}: {dotProduct}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(vertex0)}: {vertex0}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(vertex1)}: {vertex1}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(vertex2)}: {vertex2}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(vertex3)}: {vertex3}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(precomputed0)}: {precomputed0}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(precomputed1)}: {precomputed1}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(precomputed2)}: {precomputed2}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(precomputed3)}: {precomputed3}");
        }
    }
}