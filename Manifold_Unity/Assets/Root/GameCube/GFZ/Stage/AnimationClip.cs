using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Expresses an animation with associated metadata. Animation can set position,
    /// rotation, scale, and/or affect the alpha channel of certain textures.
    /// 
    /// There appears to be some matrix math involved. Animation values are transformed by
    /// associated Transform on the same object. In that sense, they are local, or child of
    /// a transform and thus affected by the parent's transform.
    /// </summary>
    [Serializable]
    public sealed class AnimationClip :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference, // The AnimationClipCurve has references
        ITextPrintable
    {
        // CONSTANTS
        /// <summary>
        /// Number of animation curves. Order: scale.xyz, rotation.xyz, position.xyz, unknown, texture alpha
        /// </summary>
        public const int kAnimationCurvesCount = 11;
        private const int kZeroes0x08 = 0x10;


        // FIELDS
        private float unk_0x00;
        private float unk_0x04;
        private byte[] zeroes_0x08 = new byte[kZeroes0x08];
        private EnumFlags32 unk_layer_0x18;
        private AnimationClipCurve[] curves; // field, NOT reference field


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public AnimationClipCurve Alpha { get => curves[10]; set => curves[10] = value; }
        public AnimationClipCurve[] Curves { get => curves; set => curves = value; }
        public AnimationClipCurve PositionX { get => curves[6]; set => curves[6] = value; }
        public AnimationClipCurve PositionY { get => curves[7]; set => curves[7] = value; }
        public AnimationClipCurve PositionZ { get => curves[8]; set => curves[8] = value; }
        public AnimationClipCurve RotationX { get => curves[3]; set => curves[3] = value; }
        public AnimationClipCurve RotationY { get => curves[4]; set => curves[4] = value; }
        public AnimationClipCurve RotationZ { get => curves[5]; set => curves[5] = value; }
        public AnimationClipCurve ScaleX { get => curves[0]; set => curves[0] = value; }
        public AnimationClipCurve ScaleY { get => curves[1]; set => curves[1] = value; }
        public AnimationClipCurve ScaleZ { get => curves[2]; set => curves[2] = value; }
        public AnimationClipCurve Unused { get => curves[9]; set => curves[9] = value; }
        public float Unk_0x04 { get => unk_0x04; set => unk_0x04 = value; }
        public float Unk_0x00 { get => unk_0x00; set => unk_0x00 = value; }
        public EnumFlags32 Unk_layer_0x18 { get => unk_layer_0x18; set => unk_layer_0x18 = value; }



        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref zeroes_0x08, kZeroes0x08);
                reader.ReadX(ref unk_layer_0x18);
                reader.ReadX(ref curves, kAnimationCurvesCount);
            }
            this.RecordEndAddress(reader);
            {
                foreach (var zero in zeroes_0x08)
                    Assert.IsTrue(zero == 0);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                foreach (var zero in zeroes_0x08)
                    Assert.IsTrue(zero == 0);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);
                writer.WriteX(unk_0x04);
                writer.WriteX(new byte[kZeroes0x08]);
                writer.WriteX(unk_layer_0x18);
                writer.WriteX(curves);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // TRICKY! The inlined classes make it look not like a referer

            foreach (var zero in zeroes_0x08)
                Assert.IsTrue(zero == 0);

            foreach (var curve in curves)
            {
                Assert.IsTrue(curve != null);
                Assert.ReferencePointer(curve.AnimationCurve, curve.AnimationCurvePtr);
            }
        }

        public override string ToString()
        {
            return PrintSingleLine();
        }

        public string PrintSingleLine()
        {
            return
                $"{nameof(AnimationClip)}(" +
                $"{nameof(Unk_0x00)}: {Unk_0x00}, " +
                $"{nameof(Unk_0x04)}: {Unk_0x04}, " +
                $"{nameof(Unk_layer_0x18)}: {Unk_layer_0x18}" +
                $")";
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            string[] labels = new string[] {
                "Scale.X",
                "Scale.Y",
                "Scale.Z",
                "Rotation.X",
                "Rotation.Y",
                "Rotation.Z",
                "Position.X",
                "Position.Y",
                "Position.Z",
                "(Unused - how'd this print?)",
                "Alpha",
            };

            // Write the main structure on one line
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            indentLevel++;

            for (int i = 0; i < curves.Length; i++)
            {
                var animClipCurve = curves[i];
                if (animClipCurve.AnimationCurve == null)
                    continue;

                var prefix = $"{labels[i]} [{i:00}/{curves.Length}]";
                builder.AppendLineIndented(indent, indentLevel, prefix);
                builder.AppendLineIndented(indent, indentLevel + 1, animClipCurve);
            }
        }

    }
}