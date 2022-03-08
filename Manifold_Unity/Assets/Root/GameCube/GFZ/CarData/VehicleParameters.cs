using Manifold.IO;
using System;
using System.IO;
using Unity.Mathematics;

namespace GameCube.GFZ.CarData
{
    // Structure
    // https://github.com/yoshifan/fzerogx-docs/blob/master/addresses/base_machine_stat_blocks.md

    [Serializable]
    public struct VehicleParameters :
        IBinarySerializable,
        IBinaryAddressable
    {
        // FIELDS
        private Pointer namePtr;
        private float weight;
        private float acceleration;
        private float maxSpeed;
        private float grip1;
        private float grip3;
        private float turnTension;
        private float driftAcceleration;
        private float turnMovement;
        private float strafeTurn;
        private float strafe;
        private float turnReaction;
        private float grip2;
        private float boostStrength;
        private float boostDuration;
        private float turnDeceleration;
        private float drag;
        private float body;
        private CarDataFlags0x48 unk_0x48;
        private CarDataFlags0x49 unk_0x49;
        private ushort zero_0x4A;
        private float cameraReorientation;
        private float cameraRepositioning;
        private float3 tiltFrontRight;
        private float3 tiltFrontLeft;
        private float3 tiltBackRight;
        private float3 tiltBackLeft;
        private float3 wallCollisionFrontRight;
        private float3 wallCollisionFrontLeft;
        private float3 wallCollisionBackRight;
        private float3 wallCollisionBackLeft;
        // REFERENCES
        private ShiftJisCString name;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public Pointer NamePtr { get => namePtr; set => namePtr = value; }
        public float Weight { get => weight; set => weight = value; }
        public float Acceleration { get => acceleration; set => acceleration = value; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        public float Grip1 { get => grip1; set => grip1 = value; }
        public float Grip3 { get => grip3; set => grip3 = value; }
        public float TurnTension { get => turnTension; set => turnTension = value; }
        public float DriftAcceleration { get => driftAcceleration; set => driftAcceleration = value; }
        public float TurnMovement { get => turnMovement; set => turnMovement = value; }
        public float StrafeTurn { get => strafeTurn; set => strafeTurn = value; }
        public float Strafe { get => strafe; set => strafe = value; }
        public float TurnReaction { get => turnReaction; set => turnReaction = value; }
        public float Grip2 { get => grip2; set => grip2 = value; }
        public float BoostStrength { get => boostStrength; set => boostStrength = value; }
        public float BoostDuration { get => boostDuration; set => boostDuration = value; }
        public float TurnDeceleration { get => turnDeceleration; set => turnDeceleration = value; }
        public float Drag { get => drag; set => drag = value; }
        public float Body { get => body; set => body = value; }
        public CarDataFlags0x48 Unk_0x48 { get => unk_0x48; set => unk_0x48 = value; }
        public CarDataFlags0x49 Unk_0x49 { get => unk_0x49; set => unk_0x49 = value; }
        public ushort Zero_0x4A { get => zero_0x4A; set => zero_0x4A = value; }
        public float CameraReorientation { get => cameraReorientation; set => cameraReorientation = value; }
        public float CameraRepositioning { get => cameraRepositioning; set => cameraRepositioning = value; }
        public float3 TiltFrontRight { get => tiltFrontRight; set => tiltFrontRight = value; }
        public float3 TiltFrontLeft { get => tiltFrontLeft; set => tiltFrontLeft = value; }
        public float3 TiltBackRight { get => tiltBackRight; set => tiltBackRight = value; }
        public float3 TiltBackLeft { get => tiltBackLeft; set => tiltBackLeft = value; }
        public float3 WallCollisionFrontRight { get => wallCollisionFrontRight; set => wallCollisionFrontRight = value; }
        public float3 WallCollisionFrontLeft { get => wallCollisionFrontLeft; set => wallCollisionFrontLeft = value; }
        public float3 WallCollisionBackRight { get => wallCollisionBackRight; set => wallCollisionBackRight = value; }
        public float3 WallCollisionBackLeft { get => wallCollisionBackLeft; set => wallCollisionBackLeft = value; }
        public ShiftJisCString Name { get => name; set => name = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref namePtr);
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
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(namePtr);
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
            this.RecordEndAddress(writer);
        }

    }
}