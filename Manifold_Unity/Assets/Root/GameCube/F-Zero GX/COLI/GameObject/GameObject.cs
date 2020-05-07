using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GameCube.FZeroGX.GMA;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class GameObject : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public int unk_0x00;
        public int unk_0x04;
        public int collisionBindingAbsPtr;
        public Vector3 collisionPosition;
        public int unk_0x18;
        public int unk_0x1C;
        public Vector3 collisionScale;
        public int zero_0x2C; // Abs or Rel?
        public uint animationAbsPtr; // Abs or Rel?
        public uint unkPtr_0x34; // Abs or Rel?
        public uint unkPtr_0x38; // Abs or Rel?
        public uint transformPtr; // Abs or Rel?

        public CollisionBinding collisionBinding;
        public Animation animation;
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
                reader.ReadX(ref collisionPosition);
                reader.ReadX(ref unk_0x18);
                reader.ReadX(ref unk_0x1C);
                reader.ReadX(ref collisionScale);
                reader.ReadX(ref zero_0x2C);
                reader.ReadX(ref animationAbsPtr);
                reader.ReadX(ref unkPtr_0x34);
                reader.ReadX(ref unkPtr_0x38);
                reader.ReadX(ref transformPtr);
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

                if (transformPtr > 0)
                {
                    reader.BaseStream.Seek(transformPtr, SeekOrigin.Begin);
                    reader.ReadX(ref transform, true);
                }
                else
                {
                    Debug.Log($"{name} at 0x{endAddress:X8}");
                }
            }
            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(collisionBindingAbsPtr);
            writer.WriteX(collisionPosition);
            writer.WriteX(unk_0x18);
            writer.WriteX(unk_0x1C);
            writer.WriteX(collisionScale);
            writer.WriteX(zero_0x2C);
            writer.WriteX(animationAbsPtr);
            writer.WriteX(unkPtr_0x34);
            writer.WriteX(unkPtr_0x38);
            writer.WriteX(transformPtr);

            // Write values pointed at by, update ptrs above
            throw new NotImplementedException();
        }

        #endregion

    }
}