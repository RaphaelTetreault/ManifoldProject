using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A metadata-enhanced AnimationCurve.
    /// </summary>
    [Serializable]
    public sealed class AnimationClipCurve :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // FIELDS
        private uint unk_0x00;
        private uint unk_0x04;
        private uint unk_0x08;
        private uint unk_0x0C;
        private ArrayPointer animationCurvePtr;
        // REFERENCE FIELDS
        private AnimationCurve animationCurve;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public AnimationCurve AnimationCurve { get => animationCurve; set => animationCurve = value; }
        public ArrayPointer AnimationCurvePtr { get => animationCurvePtr; set => animationCurvePtr = value; }
        public uint Unk_0x00 { get => unk_0x00; set => unk_0x00 = value; }
        public uint Unk_0x04 { get => unk_0x04; set => unk_0x04 = value; }
        public uint Unk_0x08 { get => unk_0x08; set => unk_0x08 = value; }
        public uint Unk_0x0C { get => unk_0x0C; set => unk_0x0C = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref animationCurvePtr);
            }
            this.RecordEndAddress(reader);
            {
                if (animationCurvePtr.IsNotNull)
                {
                    reader.JumpToAddress(animationCurvePtr);
                    animationCurve = new AnimationCurve(animationCurvePtr.length);
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

                animationCurvePtr = ptr;
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(unk_0x08);
                writer.WriteX(unk_0x0C);
                writer.WriteX(animationCurvePtr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // 2022/01/19: Since this is a wrapper class, check if the curve pointer
            // for the collection (wehich points to the base item [0]) is the same
            // as what the array pointer points to.
            Assert.ReferencePointer(animationCurve, animationCurvePtr);
            // Ensure that we have the same amount of keyables as we say we do.
            if (animationCurve != null)
                Assert.IsTrue(animationCurve.KeyableAttributes.Length == animationCurvePtr.length);
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

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            // Write the main structure on one line
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, animationCurve);
        }

    }
}