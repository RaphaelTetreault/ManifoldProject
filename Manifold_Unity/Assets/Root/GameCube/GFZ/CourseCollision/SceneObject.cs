﻿using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class SceneObject : IBinarySerializable, IBinaryAddressableRange
    {

        #region MEMBERS


        // metadata
        [SerializeField]
        private AddressRange addressRange;
        /// <summary>
        /// Object's name from table sub-structure
        /// </summary>
        public string name;

        // structure
        public LevelOfDetailRadius lodFar;
        public LevelOfDetailRadius lodNear;
        public Pointer collisionBindingPtr;
        public Transform transform;
        public int zero_0x2C;
        public Pointer animationPtr;
        public Pointer unkPtr_0x34;
        public Pointer skeletalAnimatorPtr;
        public Pointer transformPtr;

        // sub-structures
        public ColliderObject colliderBinding;
        public AnimationClip animation;
        public UnknownSceneObjectData unk1;
        public SkeletalAnimator skeletalAnimator;
        public TransformMatrix3x4 transformMatrix3x4 = new TransformMatrix3x4();


        #endregion

        #region PROPERTIES


        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        #endregion

        #region METHODS


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref lodFar, false);
                reader.ReadX(ref lodNear, false);
                reader.ReadX(ref collisionBindingPtr);
                reader.ReadX(ref transform, true);
                reader.ReadX(ref zero_0x2C);
                reader.ReadX(ref animationPtr);
                reader.ReadX(ref unkPtr_0x34);
                reader.ReadX(ref skeletalAnimatorPtr);
                reader.ReadX(ref transformPtr);
            }
            this.RecordEndAddress(reader);
            {
                // Assert constants
                Assert.IsTrue(zero_0x2C == 0);
                Assert.IsTrue(collisionBindingPtr.IsNotNullPointer);

                //
                reader.JumpToAddress(collisionBindingPtr);
                reader.ReadX(ref colliderBinding, true);
                name = colliderBinding.objectAttributes.name;

                if (animationPtr.IsNotNullPointer)
                {
                    reader.JumpToAddress(animationPtr);
                    reader.ReadX(ref animation, true);
                }

                if (unkPtr_0x34.IsNotNullPointer)
                {
                    reader.JumpToAddress(unkPtr_0x34);
                    reader.ReadX(ref unk1, true);
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
            }
            // After deserializing sub-structures, return to end position
            reader.BaseStream.Seek(addressRange.endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(lodFar);
            writer.WriteX(lodNear);
            writer.WriteX(collisionBindingPtr);
            writer.WriteX(transform);
            writer.WriteX(zero_0x2C);
            writer.WriteX(animationPtr);
            writer.WriteX(unkPtr_0x34);
            writer.WriteX(skeletalAnimatorPtr);
            writer.WriteX(transformPtr);

            // Write values pointed at, update ptrs above
            throw new NotImplementedException();
        }


        #endregion

    }
}