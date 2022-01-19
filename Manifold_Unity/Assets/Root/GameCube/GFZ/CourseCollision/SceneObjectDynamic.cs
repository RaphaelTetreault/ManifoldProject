using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SceneObjectDynamic :
        IBinaryAddressable,
        IBinarySerializable,
        ISerializedBinaryAddressableReferer
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;
        /// <summary>
        /// Object's name from table sub-structure
        /// </summary>
        public string nameCopy;

        // FIELDS
        /// <summary>
        /// Appears to be render flags. 0x00000001 seems to be basic value. Alpha clip.
        /// </summary>
        public UnknownObjectBitfield lodFar;
        /// <summary>
        /// 
        /// </summary>
        public UnknownObjectBitfield lodNear;
        public Pointer templateSceneObjectPtr;
        public Transform transform = new Transform();
        public int zero_0x2C; // null ptr?
        public Pointer animationClipPtr;
        public Pointer textureMetadataPtr;
        public Pointer skeletalAnimatorPtr;
        public Pointer transformMatrix3x4Ptr;
        // FIELDS (deserialized from pointers)
        public SceneObjectTemplate templateSceneObject;
        public AnimationClip animationClip;
        public TextureMetadata textureMetadata;
        public SkeletalAnimator skeletalAnimator;
        public TransformMatrix3x4 transformMatrix3x4;


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
                reader.ReadX(ref lodFar, false);
                reader.ReadX(ref lodNear, false);
                reader.ReadX(ref templateSceneObjectPtr);
                reader.ReadX(ref transform, true);
                reader.ReadX(ref zero_0x2C);
                reader.ReadX(ref animationClipPtr);
                reader.ReadX(ref textureMetadataPtr);
                reader.ReadX(ref skeletalAnimatorPtr);
                reader.ReadX(ref transformMatrix3x4Ptr);
            }
            this.RecordEndAddress(reader);
            {
                //
                reader.JumpToAddress(templateSceneObjectPtr);
                reader.ReadX(ref templateSceneObject, true);
                nameCopy = templateSceneObject.sceneObject.name;

                if (animationClipPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(animationClipPtr);
                    reader.ReadX(ref animationClip, true);
                }

                if (textureMetadataPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(textureMetadataPtr);
                    reader.ReadX(ref textureMetadata, true);
                }

                if (skeletalAnimatorPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(skeletalAnimatorPtr);
                    reader.ReadX(ref skeletalAnimator, true);
                }

                // 1518 objects without a transform
                // They appear to use animation, so the matrix is null
                // They do have a "normal" transform, though
                if (transformMatrix3x4Ptr.IsNotNullPointer)
                {
                    reader.JumpToAddress(transformMatrix3x4Ptr);
                    reader.ReadX(ref transformMatrix3x4, true);
                }

                // Assert pointer and the like
                ValidateReferences();
            }
            // After deserializing sub-structures, return to end position
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                // Get pointers from refered instances
                templateSceneObjectPtr = templateSceneObject.GetPointer();
                animationClipPtr = animationClip.GetPointer();
                textureMetadataPtr = textureMetadata.GetPointer();
                skeletalAnimatorPtr = skeletalAnimator.GetPointer();
                transformMatrix3x4Ptr = transformMatrix3x4.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(lodFar);
                writer.WriteX(lodNear);
                writer.WriteX(templateSceneObjectPtr);
                writer.WriteX(transform);
                writer.WriteX(zero_0x2C);
                writer.WriteX(animationClipPtr);
                writer.WriteX(textureMetadataPtr);
                writer.WriteX(skeletalAnimatorPtr);
                writer.WriteX(transformMatrix3x4Ptr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // This pointer CANNOT be null and must refer to an object.
            Assert.IsTrue(templateSceneObject != null);
            Assert.IsTrue(templateSceneObjectPtr.IsNotNullPointer);
            Assert.ReferencePointer(templateSceneObject, templateSceneObjectPtr);
            // This should always exist
            Assert.IsTrue(transform != null);

            // Optional data
            Assert.ReferencePointer(animationClip, animationClipPtr);
            Assert.ReferencePointer(textureMetadata, textureMetadataPtr);
            Assert.ReferencePointer(skeletalAnimator, skeletalAnimatorPtr);
            Assert.ReferencePointer(transformMatrix3x4, transformMatrix3x4Ptr);

            // Constants 
            Assert.IsTrue(zero_0x2C == 0);
        }

        public override string ToString()
        {
            return
                $"{nameof(SceneObjectDynamic)}(" +
                $"LOD A: {lodNear}, " +
                $"LOD B: {lodFar}, " +
                $"{transform} " +
                $"Name: {nameCopy}" +
                $")";
        }

    }
}