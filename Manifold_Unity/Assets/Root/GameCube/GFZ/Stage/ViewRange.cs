using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Represents a near/far range. Used primarily for Fog range.
    /// </summary>
    [Serializable]
    public struct ViewRange :
        IBinarySerializable,
        ITextPrintable
    {
        public float near;
        public float far;

        public ViewRange(float near, float far)
        {
            this.near = near;
            this.far = far;
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref near);
            reader.ReadX(ref far);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(near);
            writer.WriteX(far);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
        }

        public string PrintSingleLine()
        {
            return $"{nameof(ViewRange)}({nameof(near)}: {near:0}, {nameof(far)}: {far:0})";
        }

        public override string ToString() => PrintSingleLine();

    }
}
