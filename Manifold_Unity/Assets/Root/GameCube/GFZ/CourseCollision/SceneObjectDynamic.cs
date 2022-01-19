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
        public Pointer animationPtr;
        public Pointer textureMetadataPtr;
        public Pointer skeletalAnimatorPtr;
        public Pointer transformPtr;
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
                reader.ReadX(ref animationPtr);
                reader.ReadX(ref textureMetadataPtr);
                reader.ReadX(ref skeletalAnimatorPtr);
                reader.ReadX(ref transformPtr);
            }
            this.RecordEndAddress(reader);
            {
                //
                reader.JumpToAddress(templateSceneObjectPtr);
                reader.ReadX(ref templateSceneObject, true);
                nameCopy = templateSceneObject.sceneObject.name;

                if (animationPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(animationPtr);
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
                if (transformPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(transformPtr);
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
                animationPtr = animationClip.GetPointer();
                textureMetadataPtr = textureMetadata.GetPointer();
                skeletalAnimatorPtr = skeletalAnimator.GetPointer();
                transformPtr = transformMatrix3x4.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(lodFar);
                writer.WriteX(lodNear);
                writer.WriteX(templateSceneObjectPtr);
                writer.WriteX(transform);
                writer.WriteX(zero_0x2C);
                writer.WriteX(animationPtr);
                writer.WriteX(textureMetadataPtr);
                writer.WriteX(skeletalAnimatorPtr);
                writer.WriteX(transformPtr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // This pointer CANNOT be null and must refer to an object.
            Assert.PointerReferenceValid(templateSceneObject, templateSceneObjectPtr);
            //Assert.IsTrue(instanceReferencePtr.IsNotNullPointer);
            //Assert.IsTrue(instanceReference != null);

            // Assert pointers only if type is not null
            if (animationClip != null)
                Assert.IsTrue(animationPtr.IsNotNullPointer);
            if (textureMetadata != null)
                Assert.IsTrue(textureMetadataPtr.IsNotNullPointer);
            if (skeletalAnimator != null)
                Assert.IsTrue(skeletalAnimatorPtr.IsNotNullPointer);
            if (transformMatrix3x4 != null)
                Assert.IsTrue(transformPtr.IsNotNullPointer);

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