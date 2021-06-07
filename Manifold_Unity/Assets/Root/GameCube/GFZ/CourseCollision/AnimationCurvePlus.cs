using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AnimationCurvePlus :
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
                animationCurvePtrs = animationCurve.GetArrayPointer();
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
            // TODO: if anim not null, then if anim has keys, assert is not null ptr
            throw new NotImplementedException();

            //if (animationCurve != null)
            //{
            //    Assert.IsTrue(anima);
            //}
        }
    }
}