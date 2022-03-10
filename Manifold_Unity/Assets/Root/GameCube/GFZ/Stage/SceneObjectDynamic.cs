using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Represents a complex scene object that can have various properties.
    /// </summary>
    [Serializable]
    public sealed class SceneObjectDynamic :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // FIELDS
        private int unk0x00; // rendering? 
        private int unk0x04; // LOD Flags? -1 = no LOD/no-disable, otherwise flags.
        private Pointer sceneObjectPtr;
        private TransformTRXS transformTRXS = new TransformTRXS();
        private int zero_0x2C; // null ptr?
        private Pointer animationClipPtr;
        private Pointer textureScrollPtr;
        private Pointer skeletalAnimatorPtr;
        private Pointer transformMatrix3x4Ptr;
        // FIELDS (deserialized from pointers)
        private SceneObject sceneObject;
        private AnimationClip animationClip;
        private TextureScroll textureScroll;
        private SkeletalAnimator skeletalAnimator;
        private TransformMatrix3x4 transformMatrix3x4;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public string Name => SceneObject.Name;

        public int Unk0x00 { get => unk0x00; set => unk0x00 = value; }
        public int Unk0x04 { get => unk0x04; set => unk0x04 = value; }
        public Pointer SceneObjectPtr { get => sceneObjectPtr; set => sceneObjectPtr = value; }
        public TransformTRXS TransformTRXS { get => transformTRXS; set => transformTRXS = value; }
        public Pointer AnimationClipPtr { get => animationClipPtr; set => animationClipPtr = value; }
        public Pointer TextureScrollPtr { get => textureScrollPtr; set => textureScrollPtr = value; }
        public Pointer SkeletalAnimatorPtr { get => skeletalAnimatorPtr; set => skeletalAnimatorPtr = value; }
        public Pointer TransformMatrix3x4Ptr { get => transformMatrix3x4Ptr; set => transformMatrix3x4Ptr = value; }
        public SceneObject SceneObject { get => sceneObject; set => sceneObject = value; }
        public AnimationClip AnimationClip { get => animationClip; set => animationClip = value; }
        public TextureScroll TextureScroll { get => textureScroll; set => textureScroll = value; }
        public SkeletalAnimator SkeletalAnimator { get => skeletalAnimator; set => skeletalAnimator = value; }
        public TransformMatrix3x4 TransformMatrix3x4 { get => transformMatrix3x4; set => transformMatrix3x4 = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref unk0x00);
                reader.ReadX(ref unk0x04);
                reader.ReadX(ref sceneObjectPtr);
                reader.ReadX(ref transformTRXS);
                reader.ReadX(ref zero_0x2C);
                reader.ReadX(ref animationClipPtr);
                reader.ReadX(ref textureScrollPtr);
                reader.ReadX(ref skeletalAnimatorPtr);
                reader.ReadX(ref transformMatrix3x4Ptr);
            }
            this.RecordEndAddress(reader);
            {
                //
                reader.JumpToAddress(SceneObjectPtr);
                reader.ReadX(ref sceneObject);

                if (AnimationClipPtr.IsNotNull)
                {
                    reader.JumpToAddress(AnimationClipPtr);
                    reader.ReadX(ref animationClip);
                }

                if (TextureScrollPtr.IsNotNull)
                {
                    reader.JumpToAddress(TextureScrollPtr);
                    reader.ReadX(ref textureScroll);
                }

                if (SkeletalAnimatorPtr.IsNotNull)
                {
                    reader.JumpToAddress(SkeletalAnimatorPtr);
                    reader.ReadX(ref skeletalAnimator);
                }

                // 1518 objects without a transform
                // They appear to use animation, so the matrix is null
                // They do have a "normal" transform, though
                if (TransformMatrix3x4Ptr.IsNotNull)
                {
                    reader.JumpToAddress(TransformMatrix3x4Ptr);
                    reader.ReadX(ref transformMatrix3x4);
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
                writer.WriteX(transformTRXS);
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
            Assert.IsTrue(SceneObject != null);
            Assert.IsTrue(SceneObjectPtr.IsNotNull);
            Assert.ReferencePointer(SceneObject, SceneObjectPtr);
            // This should always exist
            Assert.IsTrue(TransformTRXS != null);

            // Optional data
            Assert.ReferencePointer(AnimationClip, AnimationClipPtr);
            Assert.ReferencePointer(TextureScroll, TextureScrollPtr);
            Assert.ReferencePointer(SkeletalAnimator, SkeletalAnimatorPtr);
            Assert.ReferencePointer(TransformMatrix3x4, TransformMatrix3x4Ptr);

            // Constants 
            Assert.IsTrue(zero_0x2C == 0);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, nameof(SceneObjectDynamic));
            indentLevel++;
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Name)}: {Name}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Unk0x00)}: {Unk0x00}");
            builder.AppendLineIndented(indent, indentLevel, $"{nameof(Unk0x04)}: {Unk0x04}");
            builder.AppendLineIndented(indent, indentLevel, transformTRXS);
            builder.AppendLineIndented(indent, indentLevel, sceneObject);
            builder.AppendLineIndented(indent, indentLevel, animationClip);
            builder.AppendLineIndented(indent, indentLevel, textureScroll);
            builder.AppendLineIndented(indent, indentLevel, skeletalAnimator);
            builder.AppendLineIndented(indent, indentLevel, transformMatrix3x4);
        }

        public string PrintSingleLine()
        {
            return $"{nameof(SceneObjectDynamic)}({Name})";
        }

        public override string ToString() => PrintSingleLine();

    }
}