﻿using GameCube.GFZX01.CourseCollision.Animation;
using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZX01.CourseCollision
{
    [Serializable]
    public class AnimationClip : IBinarySerializable, IBinaryAddressable
    {
        // 6
        public const int kSizeCurvesPtrs = 6 + 5;
        const int kSizeZero_0x08 = 0x10;

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public float unk_0x00;
        public float unk_0x04;
        public byte[] zero_0x08;
        public EnumFlags32 unk_layer_0x18;
        /// <summary>
        /// idx: 0,1,2: scale
        /// idx: 3,4,5: rotation
        /// idx: 6,7,8: position
        /// idx: 9: unused?
        /// idx: 10: light
        /// </summary>
        public AnimationCurve[] animCurves;

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
                reader.ReadX(ref zero_0x08, kSizeZero_0x08);
                reader.ReadX(ref unk_layer_0x18);
                reader.ReadX(ref animCurves, kSizeCurvesPtrs, true);
            }
            endAddress = reader.BaseStream.Position;
            {
                foreach (var zero in zero_0x08)
                    Assert.IsTrue(zero == 0);
            }
            // No jumping required
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(unk_0x00);
            writer.WriteX(unk_0x04);
            writer.WriteX(zero_0x08, false);
            writer.WriteX(unk_layer_0x18);
            writer.WriteX(animCurves, false);

            // TODO: Ensure the ptr addresses are correct
            //throw new NotImplementedException();
        }

        #endregion

    }
}