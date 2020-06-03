using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZX01.CourseCollision
{
    [Serializable]
    public class GameObject : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public EnumFlags32 unk_0x00;
        public EnumFlags32 unk_0x04;
        public int collisionBindingAbsPtr;
        public Vector3 position;
        public EnumFlags16 unk_0x18;
        public EnumFlags16 unk_0x1A;
        public EnumFlags16 unk_0x1C;
        public EnumFlags16 unk_0x1E;
        public Vector3 scale;
        /// <summary>
        /// 2020/05/12 Raph: Confirmed 0
        /// </summary>
        public int zero_0x2C;
        public uint animationAbsPtr;
        public uint unkPtr_0x34; // Abs or Rel?
        public uint unkPtr_0x38; // Abs or Rel?
        public uint transformAbsPtr;

        public CollisionBinding collisionBinding;
        public AnimationClip animation;
        public ObjectTable_Unk1 unk1;
        public ObjectTable_Unk2 unk2;
        public Transform transform;

        public string name;

        #endregion

        #region PROPERTIES

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;
            {
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref unk_0x04);
                reader.ReadX(ref collisionBindingAbsPtr);
                reader.ReadX(ref position);
                reader.ReadX(ref unk_0x18);
                reader.ReadX(ref unk_0x1A);
                reader.ReadX(ref unk_0x1C);
                reader.ReadX(ref unk_0x1E);
                reader.ReadX(ref scale);
                reader.ReadX(ref zero_0x2C);
                reader.ReadX(ref animationAbsPtr);
                reader.ReadX(ref unkPtr_0x34);
                reader.ReadX(ref unkPtr_0x38);
                reader.ReadX(ref transformAbsPtr);
            }
            endAddress = reader.BaseStream.Position;
            {
                Assert.IsTrue(zero_0x2C == 0);

                Assert.IsTrue(collisionBindingAbsPtr > 0);
                reader.BaseStream.Seek(collisionBindingAbsPtr, SeekOrigin.Begin);
                reader.ReadX(ref collisionBinding, true);
                name = collisionBinding.referenceBinding.name;

                if (animationAbsPtr > 0)
                {
                    reader.BaseStream.Seek(animationAbsPtr, SeekOrigin.Begin);
                    reader.ReadX(ref animation, true);
                }

                if (unkPtr_0x34 > 0)
                {
                    reader.BaseStream.Seek(unkPtr_0x34, SeekOrigin.Begin);
                    reader.ReadX(ref unk1, true);
                }

                if (unkPtr_0x38 > 0)
                {
                    reader.BaseStream.Seek(unkPtr_0x38, SeekOrigin.Begin);
                    reader.ReadX(ref unk2, true);
                }

                if (transformAbsPtr > 0)
                {
                    reader.BaseStream.Seek(transformAbsPtr, SeekOrigin.Begin);
                    reader.ReadX(ref transform, true);
                }
                else
                {
                    // 1518 objects without a transform
                    // They appear to use "Collision Position" but
                    // they don't have collision, they have animations.'
                    var matrix = new Matrix4x4();
                    matrix.SetTRS(position, Quaternion.identity, scale);

                    transform = new Transform()
                    {
                        matrix = matrix,
                    };
                }
            }
            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(collisionBindingAbsPtr);
            writer.WriteX(position);
            writer.WriteX(unk_0x18);
            writer.WriteX(unk_0x1A);
            writer.WriteX(unk_0x1C);
            writer.WriteX(unk_0x1E);
            writer.WriteX(scale);
            writer.WriteX(zero_0x2C);
            writer.WriteX(animationAbsPtr);
            writer.WriteX(unkPtr_0x34);
            writer.WriteX(unkPtr_0x38);
            writer.WriteX(transformAbsPtr);

            // Write values pointed at by, update ptrs above
            throw new NotImplementedException();
        }

        #endregion

    }
}