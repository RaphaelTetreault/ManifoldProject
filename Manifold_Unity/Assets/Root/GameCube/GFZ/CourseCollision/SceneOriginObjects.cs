using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// This structure points to an object which does not use a transform for placement within the scene.
    /// </summary>
    [Serializable]
    public class SceneOriginObjects :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public Pointer sceneObjectReferencePtr;
        // REFERENCE FIELDS
        public SceneInstanceReference instanceReference;


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
                reader.ReadX(ref sceneObjectReferencePtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(sceneObjectReferencePtr.IsNotNullPointer);
                reader.JumpToAddress(sceneObjectReferencePtr);
                reader.ReadX(ref instanceReference, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                sceneObjectReferencePtr = instanceReference.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(sceneObjectReferencePtr);
            }
            this.RecordEndAddress(writer);

        }

        public void ValidateReferences()
        {
            Assert.IsTrue(sceneObjectReferencePtr.IsNotNullPointer);
        }

    }
}