using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Collider volume only available in Sand Ocean Lateral Shift. It is known to
    /// be a collider since has a reference stored in ColliderMap.
    /// GX: 6 instances, AX: 9 instances
    /// </summary>
    [Serializable]
    public class UnknownCollider :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public Pointer templateSceneObjectPtr;
        public TransformPRXS transform;
        //
        public SceneObject templateSceneObject;

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
                reader.ReadX(ref templateSceneObjectPtr);
                reader.ReadX(ref transform, true);
            }
            this.RecordEndAddress(reader);
            {
                if (templateSceneObjectPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(templateSceneObjectPtr);
                    reader.ReadX(ref templateSceneObject, true);
                }
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
                writer.WriteX(transform);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(UnknownCollider)}(" +
                $"{transform}" +
                $")";
        }

        public void ValidateReferences()
        {
            Assert.ReferencePointer(templateSceneObject, templateSceneObjectPtr);
        }
    }
}
