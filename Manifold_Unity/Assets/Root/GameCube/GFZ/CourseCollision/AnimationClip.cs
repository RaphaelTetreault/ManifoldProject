using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Expresses an animation with associated metadata. Animation can set positional data,
    /// rotational data, or affect the alpha channel of certain textures.
    /// 
    /// There appears to be some matrix math involved. Animation values are transformed by
    /// associated Transform on the same object. In that sense, they are local, or child of
    /// a transform and thus affected by the parent's transform.
    /// </summary>
    [Serializable]
    public class AnimationClip :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // CONSTANTS
        /// <summary>
        /// Number of animation curves. Order: scale.xyz, rotation.xyz, position.xyz, unknown, texture alpha
        /// </summary>
        public const int kSizeCurvesPtrs = 11;
        const int kSizeZero_0x08 = 0x10;

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public float unk_0x00;
        public float unk_0x04;
        public byte[] zeroes_0x08;
        public EnumFlags32 unk_layer_0x18;
        /// <summary>
        /// idx: 0,1,2: scale.xyz
        /// idx: 3,4,5: rotation.xyz
        /// idx: 6,7,8: position.xyz
        /// idx: 9: unused
        /// idx: 10: alpha channel
        /// </summary>
        public AnimationClipCurve[] curves; // Written inline, not pointer refs


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref zeroes_0x08, kSizeZero_0x08);
                reader.ReadX(ref unk_layer_0x18);
                reader.ReadX(ref curves, kSizeCurvesPtrs, true);
            }
            this.RecordEndAddress(reader);
            {
                foreach (var zero in zeroes_0x08)
                    Assert.IsTrue(zero == 0);

                //foreach (var curve in curves)
                //{
                //    Assert.IsTrue(curve != null);
                //    Assert.ReferencePointer(curve.animationCurve, curve.animationCurvePtrs);
                //}
            }
            // No jumping required
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
                writer.WriteX(zeroes_0x08, false);
                writer.WriteX(unk_layer_0x18);
                writer.WriteX(curves, false);
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
            return
                $"{nameof(AnimationClip)}(" +
                $"{nameof(unk_0x00)}: {unk_0x00}, " +
                $"{nameof(unk_0x04)}: {unk_0x04}, " +
                $"{nameof(unk_layer_0x18)}: {unk_layer_0x18}" +
                $")";
        }


    }
}