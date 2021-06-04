using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AnimationCurve :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public KeyableAttribute[] keyableAttributes;


        // CONSTRUCTORS
        public AnimationCurve()
        {
            keyableAttributes = new KeyableAttribute[0];
        }
        public AnimationCurve(int numKeyables)
        {
            keyableAttributes = new KeyableAttribute[numKeyables];
        }


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public int Length => keyableAttributes.Length;


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref keyableAttributes, keyableAttributes.Length, true);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                foreach (var keyable in keyableAttributes)
                    writer.WriteX(keyable);
            }
            this.RecordEndAddress(writer);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            if (keyableAttributes.Length == 0)
            {
                return new AddressRange();
            }
            else
            {
                Serialize(writer);
                return addressRange;
            }
        }

        /// <summary>
        /// Helper function. Get ArrayPointer for data as stored in data while using
        /// the wrapper class.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        /// <returns>Array pointer with length = keyables, pointer = written address in stream.</returns>
        public ArrayPointer SerializeAsArrayReference(BinaryWriter writer)
        {
            if (Length > 0)
            {
                var pointer = SerializeWithReference(writer).GetPointer();
                var arrayPointer = new ArrayPointer(Length, pointer.address);
                return arrayPointer;
            }
            else
            {
                return new ArrayPointer();
            }
        }

    }
}
