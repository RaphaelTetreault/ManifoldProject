﻿using Manifold.IO;
using System.IO;
using System;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// This structure appears to be a Maya (4.?) KeyableAttribute
    /// </summary>
    [Serializable]
    public class KeyableAttribute :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FEILDS
        public InterpolationMode easeMode;
        public float time;
        public float value;
        public float zTangentIn;
        public float zTangentOut;


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
                reader.ReadX(ref easeMode);
                reader.ReadX(ref time);
                reader.ReadX(ref value);
                reader.ReadX(ref zTangentIn);
                reader.ReadX(ref zTangentOut);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(easeMode);
                writer.WriteX(time);
                writer.WriteX(value);
                writer.WriteX(zTangentIn);
                writer.WriteX(zTangentOut);
            }
            this.RecordEndAddress(writer);
        }

        public override string ToString()
        {
            return
                $"{nameof(KeyableAttribute)}" +
                $"({nameof(easeMode)}: {easeMode}," +
                $" {nameof(time)}: {time:0.000}," +
                $" {nameof(value)}: {value:0.000}," +
                $" {nameof(zTangentIn)}: {zTangentIn:0.00}," +
                $" {nameof(zTangentOut)}: {zTangentOut:0.00})";
        }

    }
}