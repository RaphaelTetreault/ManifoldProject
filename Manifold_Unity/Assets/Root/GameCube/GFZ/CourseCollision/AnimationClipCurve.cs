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
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public uint unk_0x00;
        public uint unk_0x04;
        public uint unk_0x08;
        public uint unk_0x0C;
        public ArrayPointer animationCurvePtrs;
        // REFERENCE FIELDS
        public AnimationCurve animationCurve;


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
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
                reader.ReadX(ref animationCurvePtrs);
            }
            this.RecordEndAddress(reader);
            {
                if (animationCurvePtrs.IsNotNullPointer)
                {
                    reader.JumpToAddress(animationCurvePtrs);
                    animationCurve = new AnimationCurve(animationCurvePtrs.Length);
                    reader.ReadX(ref animationCurve, false);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
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
            // Ensure both animation curve and ptr match up as null or not-null

            // 2021/09/18: problem is you can't guarantee this on desirealize-reserialize
            // since ref is made but ptr is null.

            //var refNotNull = animationCurve != null;
            //var ptrNotNull = animationCurvePtrs.IsNotNullPointer;
            //var isSameState = !(refNotNull ^ ptrNotNull);
            //Assert.IsTrue(isSameState);

            // seems like this might be the only valid check... see note above.
            if (animationCurvePtrs.IsNotNullPointer)
                Assert.IsTrue(animationCurve != null);

            //if (animationCurve != null)
            //    Assert.IsTrue(animationCurvePtrs.IsNotNullPointer);
        }

        public override string ToString()
        {
            return $"{unk_0x00}, {unk_0x04}, {unk_0x08}, {unk_0x0C}, {animationCurve}";
        }
    }
}