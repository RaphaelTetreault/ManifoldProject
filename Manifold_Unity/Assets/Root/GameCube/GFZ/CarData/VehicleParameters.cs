using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ.CarData
{
    // Structure
    // https://github.com/yoshifan/fzerogx-docs/blob/master/addresses/base_machine_stat_blocks.md

    public enum CarDataFlags0x48_U8 : byte
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        UNK_FLAG_2 = 1 << 2,
        UNK_FLAG_3 = 1 << 3,
        UNK_FLAG_4 = 1 << 4,
        UNK_FLAG_5 = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
    }

    public enum CarDataFlags0x49_U8 : byte
    {
        UNK_FLAG_0 = 1 << 0,
        UNK_FLAG_1 = 1 << 1,
        UNK_FLAG_2 = 1 << 2,
        UNK_FLAG_3 = 1 << 3,
        UNK_FLAG_4 = 1 << 4,
        UNK_FLAG_5 = 1 << 5,
        UNK_FLAG_6 = 1 << 6,
        UNK_FLAG_7 = 1 << 7,
    }

    [Serializable]
    public struct VehicleParameters : IBinarySerializable, IBinaryAddressable
    {
        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        #region MEMBERS

        [Space]
        [Tooltip("Runtime variable")]
        public uint namePointer;
        public float weight;
        public float acceleration;
        public float maxSpeed;
        public float grip1;
        public float grip3;
        public float turnTension;
        public float driftAcceleration;
        public float turnMovement;
        public float strafeTurn;
        public float strafe;
        public float turnReaction;
        public float grip2;
        public float boostStrength;
        public float boostDuration;
        public float turnDeceleration;
        public float drag;
        public float body;
        [HexFlags(numDigits:2)]
        public CarDataFlags0x48_U8 unk_0x48;
        [HexFlags(numDigits: 2)]
        public CarDataFlags0x49_U8 unk_0x49;
        public ushort zero_0x4A;
        public float cameraReorientation;
        public float cameraRepositioning;
        public Vector3 tiltFrontRight;
        public Vector3 tiltFrontLeft;
        public Vector3 tiltBackRight;
        public Vector3 tiltBackLeft;
        public Vector3 wallCollisionFrontRight;
        public Vector3 wallCollisionFrontLeft;
        public Vector3 wallCollisionBackRight;
        public Vector3 wallCollisionBackLeft;

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

        #region MEMBERS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref namePointer);
            reader.ReadX(ref weight);
            reader.ReadX(ref acceleration);
            reader.ReadX(ref maxSpeed);
            reader.ReadX(ref grip1);
            reader.ReadX(ref grip3);
            reader.ReadX(ref turnTension);
            reader.ReadX(ref driftAcceleration);
            reader.ReadX(ref turnMovement);
            reader.ReadX(ref strafeTurn);
            reader.ReadX(ref strafe);
            reader.ReadX(ref turnReaction);
            reader.ReadX(ref grip2);
            reader.ReadX(ref boostStrength);
            reader.ReadX(ref boostDuration);
            reader.ReadX(ref turnDeceleration);
            reader.ReadX(ref drag);
            reader.ReadX(ref body);
            reader.ReadX(ref unk_0x48);
            reader.ReadX(ref unk_0x49);
            reader.ReadX(ref zero_0x4A);
            reader.ReadX(ref cameraReorientation);
            reader.ReadX(ref cameraRepositioning);
            reader.ReadX(ref tiltFrontRight);
            reader.ReadX(ref tiltFrontLeft);
            reader.ReadX(ref tiltBackRight);
            reader.ReadX(ref tiltBackLeft);
            reader.ReadX(ref wallCollisionFrontRight);
            reader.ReadX(ref wallCollisionFrontLeft);
            reader.ReadX(ref wallCollisionBackRight);
            reader.ReadX(ref wallCollisionBackLeft);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(namePointer);
            writer.WriteX(weight);
            writer.WriteX(acceleration);
            writer.WriteX(maxSpeed);
            writer.WriteX(grip1);
            writer.WriteX(grip3);
            writer.WriteX(turnTension);
            writer.WriteX(driftAcceleration);
            writer.WriteX(turnMovement);
            writer.WriteX(strafeTurn);
            writer.WriteX(strafe);
            writer.WriteX(turnReaction);
            writer.WriteX(grip2);
            writer.WriteX(boostStrength);
            writer.WriteX(boostDuration);
            writer.WriteX(turnDeceleration);
            writer.WriteX(drag);
            writer.WriteX(body);
            writer.WriteX(unk_0x48);
            writer.WriteX(unk_0x49);
            writer.WriteX(zero_0x4A);
            writer.WriteX(cameraReorientation);
            writer.WriteX(cameraRepositioning);
            writer.WriteX(tiltFrontRight);
            writer.WriteX(tiltFrontLeft);
            writer.WriteX(tiltBackRight);
            writer.WriteX(tiltBackLeft);
            writer.WriteX(wallCollisionFrontRight);
            writer.WriteX(wallCollisionFrontLeft);
            writer.WriteX(wallCollisionBackRight);
            writer.WriteX(wallCollisionBackLeft);
        }

        #endregion
    }
}