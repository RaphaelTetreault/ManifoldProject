using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// An object which does not use a transform for placement within the scene. The model's
    /// origin will align with the scene's origin.
    /// 
    /// This kind of object was used for all test objects in old AX scenes.
    /// </summary>
    [Serializable]
    public class SceneObjectStatic :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        [UnityEngine.SerializeField] private string nameCopy;

        // FIELDS
        public Pointer sceneInstanceReferencePtr;
        // REFERENCE FIELDS
        public SceneObjectDynamicReference instanceReference;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public string NameCopy => nameCopy;

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref sceneInstanceReferencePtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(sceneInstanceReferencePtr.IsNotNullPointer);
                reader.JumpToAddress(sceneInstanceReferencePtr);
                reader.ReadX(ref instanceReference, true);

                nameCopy = instanceReference.nameCopy;
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                sceneInstanceReferencePtr = instanceReference.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(sceneInstanceReferencePtr);
            }
            this.RecordEndAddress(writer);

        }

        public void ValidateReferences()
        {
            Assert.IsTrue(sceneInstanceReferencePtr.IsNotNullPointer);
        }


        public override string ToString()
        {
            return 
                $"{nameof(SceneObjectStatic)}(" +
                $"Name: {nameCopy}" +
                $")";
        }
    }
}