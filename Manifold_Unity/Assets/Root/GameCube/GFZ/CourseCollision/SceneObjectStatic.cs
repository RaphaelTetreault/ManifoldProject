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
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public Pointer templateSceneObjectPtr;
        // REFERENCE FIELDS
        public SceneObjectTemplate templateSceneObject;

        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public string Name => templateSceneObject.Name;

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref templateSceneObjectPtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(templateSceneObjectPtr.IsNotNullPointer);
                reader.JumpToAddress(templateSceneObjectPtr);
                reader.ReadX(ref templateSceneObject, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                templateSceneObjectPtr = templateSceneObject.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(templateSceneObjectPtr);
            }
            this.RecordEndAddress(writer);

        }

        public void ValidateReferences()
        {
            Assert.IsTrue(templateSceneObjectPtr.IsNotNullPointer);
        }


        public override string ToString()
        {
            return 
                $"{nameof(SceneObjectStatic)}(" +
                $"Name: {Name}" +
                $")";
        }
    }
}