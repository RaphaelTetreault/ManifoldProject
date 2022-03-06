using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Collider volume only available in Sand Ocean Lateral Shift. It is known to
    /// be a collider since has a reference stored in StaticColliderMeshes.
    /// GX: 6 instances, AX: 9 instances
    /// </summary>
    [Serializable]
    public class UnknownCollider :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // FIELDS
        public Pointer templateSceneObjectPtr;
        public TransformTRXS transform;
        //
        public SceneObject sceneObject;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref templateSceneObjectPtr);
                reader.ReadX(ref transform);
            }
            this.RecordEndAddress(reader);
            {
                if (templateSceneObjectPtr.IsNotNull)
                {
                    reader.JumpToAddress(templateSceneObjectPtr);
                    reader.ReadX(ref sceneObject);
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                templateSceneObjectPtr = sceneObject.GetPointer();
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
            Assert.ReferencePointer(sceneObject, templateSceneObjectPtr);
        }
    }
}
