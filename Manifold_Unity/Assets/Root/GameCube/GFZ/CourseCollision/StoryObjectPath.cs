using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Defines a path for a Story Mode object.
    /// 
    /// Example: Chapter 2's falling rocks.
    /// </summary>
    public class StoryObjectPath :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public ArrayPointer animationCurvePtr;
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
                reader.ReadX(ref animationCurvePtr);
            }
            this.RecordEndAddress(reader);
            {
                if (animationCurvePtr.IsNotNullPointer)
                {
                    // Init anim curve, jump, read without creating new instance
                    animationCurve = new AnimationCurve(animationCurvePtr.Length);
                    reader.JumpToAddress(animationCurvePtr);
                    reader.ReadX(ref animationCurve, false);
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

    }
}
