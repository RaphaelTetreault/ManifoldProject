using Manifold;
using Manifold.IO;
using System.IO;
using System;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Represents a Maya (4.X) KeyableAttribute (animation keyframe).
    /// </summary>
    [Serializable]
    public class KeyableAttribute :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // FEILDS
        public InterpolationMode easeMode;
        public float time;
        public float value;
        public float tangentIn;
        public float tangentOut;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref easeMode);
                reader.ReadX(ref time);
                reader.ReadX(ref value);
                reader.ReadX(ref tangentIn);
                reader.ReadX(ref tangentOut);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(easeMode);
                writer.WriteX(time);
                writer.WriteX(value);
                writer.WriteX(tangentIn);
                writer.WriteX(tangentOut);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return PrintSingleLine();
        }

        public string PrintSingleLine()
        {
            // Prints all values on single line, limits float precision
            return
                $"{nameof(KeyableAttribute)}" +
                $"({nameof(easeMode)}: {easeMode}," +
                $" {nameof(time)}: {time:0.###}," +
                $" {nameof(value)}: {value:0.###}," +
                $" {nameof(tangentIn)}: {tangentIn:0.##}," +
                $" {nameof(tangentOut)}: {tangentOut:0.##})";
        }

        public string PrintMultiLine(int indentLevel = 0, string indent = "\t")
        {
            var builder = new System.Text.StringBuilder();

            builder.AppendLineIndented(indent, indentLevel, nameof(KeyableAttribute));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(easeMode)}: {easeMode}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(time)}: {time}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(tangentIn)}: {tangentIn}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(tangentOut)}: {tangentOut}");

            return builder.ToString();
        }
    }
}