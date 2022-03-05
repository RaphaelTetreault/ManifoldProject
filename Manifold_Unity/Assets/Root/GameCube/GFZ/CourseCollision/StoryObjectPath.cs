using Manifold;
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
        // FIELDS
        public ArrayPointer animationCurvePtr;
        // FIELDS (deserialized from pointer)
        public AnimationCurve animationCurve;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref animationCurvePtr);
            }
            this.RecordEndAddress(reader);
            {
                if (animationCurvePtr.IsNotNull)
                {
                    // Init anim curve, jump, read without creating new instance
                    reader.JumpToAddress(animationCurvePtr);
                    animationCurve = new AnimationCurve(animationCurvePtr.Length);
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

    }
}
