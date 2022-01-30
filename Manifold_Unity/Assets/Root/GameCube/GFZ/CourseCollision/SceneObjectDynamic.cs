using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Represents a complex scene object that can have various properties.
    /// </summary>
    [Serializable]
    public class SceneObjectDynamic :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public int unk0x00; // rendering? 
        public int unk0x04; // rendering?-- 2x 16bit values? Start/end?
        public Pointer sceneObjectPtr;
        public TransformPRXS transformPRXS = new TransformPRXS();
        public int zero_0x2C; // null ptr?
        public Pointer animationClipPtr;
        public Pointer textureScrollPtr;
        public Pointer skeletalAnimatorPtr;
        public Pointer transformMatrix3x4Ptr;
        // FIELDS (deserialized from pointers)
        public SceneObjectDefinition sceneObject;
        public AnimationClip animationClip;
        public TextureScroll textureScroll;
        public SkeletalAnimator skeletalAnimator;
        public TransformMatrix3x4 transformMatrix3x4;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public string Name => sceneObject.Name;

        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk0x00);
                reader.ReadX(ref unk0x04);
                reader.ReadX(ref sceneObjectPtr);
                reader.ReadX(ref transformPRXS, true);
                reader.ReadX(ref zero_0x2C);
                reader.ReadX(ref animationClipPtr);
                reader.ReadX(ref textureScrollPtr);
                reader.ReadX(ref skeletalAnimatorPtr);
                reader.ReadX(ref transformMatrix3x4Ptr);
            }
            this.RecordEndAddress(reader);
            {
                //
                reader.JumpToAddress(sceneObjectPtr);
                reader.ReadX(ref sceneObject, true);

                if (animationClipPtr.IsNotNull)
                {
                    reader.JumpToAddress(animationClipPtr);
                    reader.ReadX(ref animationClip, true);
                }

                if (textureScrollPtr.IsNotNull)
                {
                    reader.JumpToAddress(textureScrollPtr);
                    reader.ReadX(ref textureScroll, true);
                }

                if (skeletalAnimatorPtr.IsNotNull)
                {
                    reader.JumpToAddress(skeletalAnimatorPtr);
                    reader.ReadX(ref skeletalAnimator, true);
                }

                // 1518 objects without a transform
                // They appear to use animation, so the matrix is null
                // They do have a "normal" transform, though
                if (transformMatrix3x4Ptr.IsNotNull)
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
                sceneObjectPtr = sceneObject.GetPointer();
                animationClipPtr = animationClip.GetPointer();
                textureScrollPtr = textureScroll.GetPointer();
                skeletalAnimatorPtr = skeletalAnimator.GetPointer();
                transformMatrix3x4Ptr = transformMatrix3x4.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk0x00);
                writer.WriteX(unk0x04);
                writer.WriteX(sceneObjectPtr);
                writer.WriteX(transformPRXS);
                writer.WriteX(zero_0x2C);
                writer.WriteX(animationClipPtr);
                writer.WriteX(textureScrollPtr);
                writer.WriteX(skeletalAnimatorPtr);
                writer.WriteX(transformMatrix3x4Ptr);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // This pointer CANNOT be null and must refer to an object.
            Assert.IsTrue(sceneObject != null);
            Assert.IsTrue(sceneObjectPtr.IsNotNull);
            Assert.ReferencePointer(sceneObject, sceneObjectPtr);
            // This should always exist
            Assert.IsTrue(transformPRXS != null);

            // Optional data
            Assert.ReferencePointer(animationClip, animationClipPtr);
            Assert.ReferencePointer(textureScroll, textureScrollPtr);
            Assert.ReferencePointer(skeletalAnimator, skeletalAnimatorPtr);
            Assert.ReferencePointer(transformMatrix3x4, transformMatrix3x4Ptr);

            // Constants 
            Assert.IsTrue(zero_0x2C == 0);
        }

        public override string ToString()
        {
            return
                $"{nameof(SceneObjectDynamic)}(" +
                $"{nameof(unk0x04)}: {unk0x04:x8}, " +
                $"{nameof(unk0x00)}: {unk0x00:x8}, " +
                $"{transformPRXS} " +
                $"{nameof(Name)}: {Name}" +
                $")";
        }

    }
}