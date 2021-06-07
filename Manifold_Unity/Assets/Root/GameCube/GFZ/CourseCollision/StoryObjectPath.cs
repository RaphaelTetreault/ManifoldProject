using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    public class StoryObjectPath :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public ArrayPointer animationCurvePtrs;
        // FIELDS (deserialized from pointer)
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
                reader.ReadX(ref animationCurvePtrs);
            }
            this.RecordEndAddress(reader);
            {
                if (animationCurvePtrs.IsNotNullPointer)
                {
                    // Init anim curve, jump, read without creating new instance
                    animationCurve = new AnimationCurve(animationCurvePtrs.Length);
                    reader.JumpToAddress(animationCurvePtrs);
                    reader.ReadX(ref animationCurve, false);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            //
            {
                animationCurvePtrs = animationCurve.GetArrayPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(animationCurvePtrs);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            if (animationCurve != null)
                Assert.IsTrue(animationCurvePtrs.IsNotNullPointer);
        }

    }
}
