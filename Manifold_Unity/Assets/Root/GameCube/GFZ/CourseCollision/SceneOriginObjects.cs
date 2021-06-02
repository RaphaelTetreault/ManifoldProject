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
        IBinarySeralizableReference
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;

        // FIELDS
        public Pointer sceneObjectReferencePtr;
        // FIELDS (deserialized from pointers)
        public SceneObjectReference collisionObjectReference;


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
                reader.JumpToAddress(sceneObjectReferencePtr);
                reader.ReadX(ref collisionObjectReference, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            // write
            {
                // COMMENT: perhaps leave for table? (will be in ColiScene if so)
                //var ptr = writer.GetPositionAsPointer();
                //writer.CommentTypeDesc(collisionObjectReference, ptr, ColiCourseUtility.SerializeVerbose);
                sceneObjectReferencePtr = collisionObjectReference.SerializeWithReference(writer).GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(sceneObjectReferencePtr);
            }
            this.RecordEndAddress(writer);

        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            Serialize(writer);
            return addressRange;
        }
    }
}