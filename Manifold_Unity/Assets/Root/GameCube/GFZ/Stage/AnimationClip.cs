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
    public class AnimationClip :
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
        const int kZeroes0x08 = 0x10;


        // FIELDS
        public float unk_0x00;
        public float unk_0x04;
        public byte[] zeroes_0x08 = new byte[kZeroes0x08];
        public EnumFlags32 unk_layer_0x18;
        public AnimationClipCurve[] curves; // field, NOT reference field


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public AnimationClipCurve Alpha => curves[10];
        public AnimationClipCurve PositionX => curves[6];
        public AnimationClipCurve PositionY => curves[7];
        public AnimationClipCurve PositionZ => curves[8];
        public AnimationClipCurve RotationX => curves[3];
        public AnimationClipCurve RotationY => curves[4];
        public AnimationClipCurve RotationZ => curves[5];
        public AnimationClipCurve ScaleX => curves[0];
        public AnimationClipCurve ScaleY => curves[1];
        public AnimationClipCurve ScaleZ => curves[2];
        public AnimationClipCurve Unused => curves[9];



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
                Assert.ReferencePointer(curve.animationCurve, curve.animationCurvePtrs);
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
                $"{nameof(unk_0x00)}: {unk_0x00}, " +
                $"{nameof(unk_0x04)}: {unk_0x04}, " +
                $"{nameof(unk_layer_0x18)}: {unk_layer_0x18}" +
                $")";
        }

        public string PrintMultiLine(int indentLevel = 0, string indent = "\t")
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

            var builder = new System.Text.StringBuilder();
            // Write the main structure on one line
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            indentLevel++;

            for (int i = 0; i < curves.Length; i++)
            {
                var animClipCurves = curves[i];
                if (animClipCurves.animationCurve == null)
                    continue;

                var prefix = $"{labels[i]} [{i:00}/{curves.Length}]";
                builder.AppendLineIndented(indent, indentLevel, prefix);

                var multilineText = animClipCurves.PrintMultiLine(indentLevel+1, indent);
                builder.Append(multilineText);
            }

            return builder.ToString();
        }

    }
}