using Manifold;
using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.Stage
{
    [Serializable]
    public struct Plane : 
        IBinarySerializable
    {
        /// <summary>
        /// The dot product of this plane. 'dot(direction, position)'
        /// </summary>
        public float dotProduct;
        /// <summary>
        /// The facing direction of this plane.
        /// </summary>
        public float3 normal;
        /// <summary>
        /// The origin position of this plane.
        /// </summary>
        public float3 origin;


        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref dotProduct);
            reader.ReadX(ref normal);
            reader.ReadX(ref origin);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(dotProduct);
            writer.WriteX(normal);
            writer.WriteX(origin);
        }

        /// <summary>
        /// Computes and stores the dotProduct of this Plane.
        /// </summary>
        public void ComputeDotProduct()
        {
            float dotProduct =
                normal.x * origin.x +
                normal.y * origin.y +
                normal.z * origin.z;

            // dot product is inverted
            this.dotProduct = -dotProduct;
        }

        public Plane GetMirror()
        {
            return GetPlaneMirrored(this);
        }

        public static Plane GetPlaneMirrored(Plane plane)
        {
            var mirroredPlane = new Plane();
            mirroredPlane.dotProduct = -plane.dotProduct;
            mirroredPlane.normal = -plane.normal;
            mirroredPlane.origin = plane.origin;
            return mirroredPlane;
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(Plane));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(origin)}: {origin}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(normal)}: {normal}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(dotProduct)}: {dotProduct}");
        }

        public string PrintSingleLine()
        {
            return nameof(Plane);
        }

        public override string ToString() => PrintSingleLine();

    }
}
