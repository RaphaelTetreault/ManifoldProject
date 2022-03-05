using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// A metadata-enhanced AnimationCurve.
    /// </summary>
    [Serializable]
    public class AnimationClipCurve :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // FIELDS
        public uint unk_0x00;
        public uint unk_0x04;
        public uint unk_0x08;
        public uint unk_0x0C;
        public ArrayPointer animationCurvePtrs;
        // REFERENCE FIELDS
        public AnimationCurve animationCurve;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref animationCurvePtrs);
            }
            this.RecordEndAddress(reader);
            {
                if (animationCurvePtrs.IsNotNull)
                {
                    reader.JumpToAddress(animationCurvePtrs);
                    animationCurve = new AnimationCurve(animationCurvePtrs.Length);
                    animationCurve.Deserialize(reader);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                // Make sure to serialize this in ColiScene!
                var ptr = animationCurve != null
                    ? animationCurve.GetArrayPointer()
                    : new ArrayPointer();

                animationCurvePtrs = ptr;
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(unk_0x08);
                writer.WriteX(unk_0x0C);
                writer.WriteX(animationCurvePtrs);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // 2022/01/19: Since this is a wrapper class, check if the curve pointer
            // for the collection (wehich points to the base item [0]) is the same
            // as what the array pointer points to.
            Assert.ReferencePointer(animationCurve, animationCurvePtrs);
            // Ensure that we have the same amount of keyables as we say we do.
            if (animationCurve != null)
                Assert.IsTrue(animationCurve.keyableAttributes.Length == animationCurvePtrs.Length);
        }

        public override string ToString()
        {
            return PrintSingleLine();
        }

        public string PrintSingleLine()
        {
            return
                $"{nameof(AnimationClipCurve)}" +
                $"({nameof(unk_0x00)}: {unk_0x00}," +
                $" {nameof(unk_0x04)}: {unk_0x04}," +
                $" {nameof(unk_0x08)}: {unk_0x08}," +
                $" {nameof(unk_0x0C)}: {unk_0x0C}," +
                $" Has {nameof(animationCurve)}: {animationCurve != null})";
        }

        public string PrintMultiLine(int indentLevel = 0, string indent = "\t")
        {
            var builder = new System.Text.StringBuilder();
            // Write the main structure on one line
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            indentLevel++;
            builder.Append(animationCurve.PrintMultiLine(indentLevel, indent));

            return builder.ToString();
        }

    }
}