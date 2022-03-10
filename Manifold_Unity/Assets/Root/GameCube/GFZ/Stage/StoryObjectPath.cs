using Manifold;
using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Defines a path for a Story Mode object.
    /// </summary>
    /// <remarks>
    /// Example: Chapter 2's falling rocks.
    /// </remarks>
    public sealed class StoryObjectPath :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // FIELDS
        private ArrayPointer animationCurvePtr;
        // FIELDS (deserialized from pointer)
        private AnimationCurve animationCurve;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public AnimationCurve AnimationCurve { get => animationCurve; set => animationCurve = value; }
        public ArrayPointer AnimationCurvePtr { get => animationCurvePtr; set => animationCurvePtr = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref animationCurvePtr);
            }
            this.RecordEndAddress(reader);
            {
                if (AnimationCurvePtr.IsNotNull)
                {
                    // Init anim curve, jump, read without creating new instance
                    reader.JumpToAddress(animationCurvePtr);
                    animationCurve = new AnimationCurve(animationCurvePtr.length);
                    animationCurve.Deserialize(reader);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            //
            {
                animationCurvePtr = animationCurve.GetArrayPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(animationCurvePtr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            Assert.ReferencePointer(animationCurve, animationCurvePtr);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(StoryObjectPath));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, animationCurve);
        }

        public string PrintSingleLine()
        {
            return nameof(StoryObjectPath);
        }

        public override string ToString() => PrintSingleLine();

    }
}
